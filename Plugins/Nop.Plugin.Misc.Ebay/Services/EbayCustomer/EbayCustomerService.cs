using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EbaySoapServiceClient;
using Nop.Core.Domain.Logging;
using Newtonsoft.Json;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.Ebay.Services.EbayCustomer
{
    public class EbayCustomerService : IEbayCustomerService
    {
        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly Nop.Services.Logging.ILogger _logger;

        public EbayCustomerService(IAddressService addressService, ICustomerService customerService, ICountryService countryService, IStateProvinceService stateProvinceService, ILogger logger)
        {
            _addressService = addressService;
            _customerService = customerService;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _logger = logger;
        }

        public Customer GetCustomer(UserType user, AddressType address, AddressType billingAddress)
        {
           
            if (address.Phone == "Invalid Request")
                address.Phone = "";
            if (user.Email == "Invalid Request")
                user.Email = "";

            if (string.IsNullOrEmpty(address.LastName) && !string.IsNullOrEmpty(address.Name))
            {
                int i = 0;
                string[] array = address.Name.Split(' ');
                foreach (string item in array)
                {
                    if (i > 0)
                    {
                        AddressType addressType = address;
                        addressType.LastName = addressType.LastName + item + " ";
                    }
                    i++;
                }
            }
            if (billingAddress == null)
                billingAddress = address;

            var billAddress = BuildAddress(billingAddress, user.Email);
            var shipAddress = BuildAddress(address, user.Email);

            var customer = _customerService.GetCustomerByEmail(user.Email);
            if (customer == null)
            {
                customer = CreateCustomerEntry(user, billAddress);
                if (customer != null)
                    AddCustomerRole(customer);

                AddCustomerBillingAndShippingAddress(billAddress, shipAddress, customer);
            }

            return customer;
           
        }

        public void AddCustomerBillingAndShippingAddress(Address addressBilling, Address shippingAddress, Customer customer)
        {
            var customerSavedAddress = _customerService.GetAddressesByCustomerId(customer.Id);
            var billing = addressBilling;
            var existingAddress = customerSavedAddress.Where(
                x => x.FirstName.Equals(billing.FirstName)
                     && x.LastName.Equals(billing.LastName)
                     && x.PhoneNumber.Equals(billing.PhoneNumber)
                     && x.Email.Equals(billing.Email)
                     && x.Address1.Equals(billing.Address1)
                     && x.City.Equals(billing.City)
                     && x.Address2.Equals(billing.Address2)
                     && x.ZipPostalCode.Equals(billing.ZipPostalCode)
            ).ToList();

            if (!existingAddress.Any())
            {
                _addressService.InsertAddress(addressBilling);
                _customerService.InsertCustomerAddress(customer, addressBilling);
                customer.BillingAddressId = addressBilling.Id;
            }
            else
                customer.BillingAddressId = existingAddress.First().Id;

            customerSavedAddress = _customerService.GetAddressesByCustomerId(customer.Id);
            var shipping = shippingAddress;
            var existingShippingAddress = customerSavedAddress.Where(
                x => x.FirstName.Equals(shipping.FirstName)
                     && x.LastName.Equals(shipping.LastName)
                     && x.PhoneNumber.Equals(shipping.PhoneNumber)
                     && x.Email.Equals(shipping.Email)
                     && x.Address1.Equals(shipping.Address1)
                     && x.Address2.Equals(shipping.Address2)
                     && x.City.Equals(shipping.City)
                     && x.ZipPostalCode.Equals(shipping.ZipPostalCode)
            ).ToList();

            if (!existingShippingAddress.Any())
            {
                _addressService.InsertAddress(shippingAddress);
                _customerService.InsertCustomerAddress(customer, shippingAddress);
                customer.ShippingAddressId = shippingAddress.Id;
            }
            else
                customer.ShippingAddressId = existingShippingAddress.First().Id;
            
            _customerService.UpdateCustomer(customer);

        }

        public Address BuildAddress(AddressType address, string email)
        {
            var countryId2 = 0;
            var stateProvinceId2 = 0;
            var country2 = _countryService.GetCountryByTwoLetterIsoCode(address.Country.ToString());
            if (country2 != null)
            {
                countryId2 = country2.Id;
                var stateProvinces2 = _stateProvinceService.GetStateProvincesByCountryId(country2.Id, 0, false);
                var state2 = stateProvinces2.FirstOrDefault(x => x.Abbreviation == address.StateOrProvince);
                if (state2 != null) stateProvinceId2 = state2.Id;
            }

            var addressShipping = new Address
            {
                Address1 = string.IsNullOrEmpty(address.Street) ? "" :address.Street,
                Address2 = string.IsNullOrEmpty(address.Street1 ) ? "" :address.Street1,
                City =string.IsNullOrEmpty(address.CityName) ? "" :address.CityName,
                CreatedOnUtc = DateTime.Now,
                Email = email,
                FirstName = string.IsNullOrEmpty(address.FirstName) ? "" :address.FirstName,
                LastName =  string.IsNullOrEmpty(address.LastName) ? "" :address.LastName,
                PhoneNumber = string.IsNullOrEmpty(address.Phone) ? "" :address.Phone,
                ZipPostalCode =string.IsNullOrEmpty(address.PostalCode) ? "" : address.PostalCode,
                Company = string.IsNullOrEmpty(address.CompanyName) ? "" :address.CompanyName
            };
            if (countryId2 > 0) addressShipping.CountryId = countryId2;
            addressShipping.StateProvinceId = null;
            if (stateProvinceId2 != 0) addressShipping.StateProvinceId = stateProvinceId2;
            return addressShipping;
        }
        public void AddCustomerRole(Customer customer)
        {
            if (customer != null && customer.Id > 0)
            {
                var registerRole = CreateEbayCustomerRoleIfDoesNotExist();
                var val2 = new CustomerCustomerRoleMapping {CustomerId = customer.Id, CustomerRoleId = registerRole.Id};
                _customerService.AddCustomerRoleMapping(val2);
            }
        }

        public CustomerRole CreateEbayCustomerRoleIfDoesNotExist()
        {
            var ebayRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.RegisteredRoleName);
            if (ebayRole == null)
            {
                var customerRole = new CustomerRole
                {
                    Active = true,
                    Name = "Ebay Customer",
                    SystemName = NopCustomerDefaults.RegisteredRoleName,
                    IsSystemRole = true,
                    FreeShipping = false,
                    TaxExempt = false
                };
                
                _customerService.InsertCustomerRole(customerRole);
            }

            return ebayRole;
        }

        public Customer CreateCustomerEntry(UserType user, Address address)
        {
            var email = string.Empty;
            var userName = $"{address.FirstName}{address.LastName}";

            if (!string.IsNullOrEmpty(user.Email))
                userName = email = user.Email;
            else if (!string.IsNullOrEmpty(user.BillingEmail))
                userName = email = user.Email;

            if (string.IsNullOrEmpty(email))
                email = address.Email;

            if (string.IsNullOrEmpty(email)) // still null ?
                email = $"{Guid.NewGuid()}@ebaycustomer.com";

            if (string.IsNullOrEmpty(userName)) // still null ?
                userName = email;

            var customer = _customerService.GetCustomerByUsername(userName);
            if (customer == null)
            {
                _logger.Information("Going to Create New User in CreateCustomerEntry(UserType user, Address address) execution.");
                
                customer = BuildCustomer(email, userName);
                _customerService.InsertCustomer(customer);

                _logger.Information($"Customer with email {email} and username {userName} is created successfully.");

                return customer;
            }

            return customer;
        }

        private Customer BuildCustomer(string email, string userName)
        {
            _logger.Information("Building Customer Object for Ebay Customer.");

            Customer customer;
            customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                CreatedOnUtc = DateTime.UtcNow,
                AdminComment = "Account From eBay",
                LastActivityDateUtc = DateTime.UtcNow,
                VendorId = 0,
                IsTaxExempt = false,
                IsSystemAccount = false,
                Active = false,   // D-Channel dont want to see Ebay Customers. All they want to see is the users who are registered as clients.
                Email = email,
                Username = userName
            };

            _logger.Information($"Customer Object after Calling BuildCustomer(string email, string userName): {JsonConvert.SerializeObject(customer)}");

            return customer;
        }
    }
}

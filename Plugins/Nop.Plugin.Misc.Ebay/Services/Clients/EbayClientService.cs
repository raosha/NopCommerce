using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data;
using Nop.Plugin.Misc.Ebay.Domains;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.Ebay.Services.Clients
{
    public class ClientService : IClientService
    {
        private readonly IRepository<EbayClient> _ebayClientRepository;
        private readonly IRepository<EbayConfiguration> _ebayConfigurationRepository;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        public ClientService(IRepository<EbayClient> ebayClientRepository, IRepository<EbayConfiguration> ebayConfigurationRepository, ILogger logger, ICustomerService customerService)
        {
            _ebayClientRepository = ebayClientRepository;
            _ebayConfigurationRepository = ebayConfigurationRepository;
            _logger = logger;
            _customerService = customerService;
        }
        public List<EbayClientViewModel> GetList()
        {
            var table = _ebayClientRepository.Table.ToList();
            var modelToReturn = new List<EbayClientViewModel>();

            foreach (var item in table)
            {
                modelToReturn.Add(item.ToModel<EbayClientViewModel>());
            }
            return modelToReturn;
        }

        public bool AddClient(EbayClientViewModel model)
        {
            var modelToInsert = model.ToEntity<EbayClient>();

            var activeConfig = _ebayConfigurationRepository.Table.Where(x => x.IsActive);

            if (activeConfig != null && activeConfig.Any())
                modelToInsert.ConfigurationId = activeConfig.FirstOrDefault().Id;
            else
            {
                _logger.Information($"No Active configuration found to save the client against. Client >>> {JsonConvert.SerializeObject(model)}");
                return false;
            }

            if (_ebayClientRepository.Table.Any(x => x.UserName == model.UserName))
            {
                _logger.Information($"Duplicate Client {model.UserName}. Client with same user name already exists.");
                return false;
            }

            _ebayClientRepository.Insert(modelToInsert);

            _logger.Information($"New Client {model.UserName} is created and system will start importing their ebay orders.");


            _logger.Information($"Going to create user for {model.UserName}.");
            // Also Create user/customer for this ebay client
            CreateCustomerEntry(model.UserName);

            return true;
        }

        public void UpdateClient(EbayClientViewModel model)
        {
            var convertedModel = model.ToEntity<EbayClient>();
            _ebayClientRepository.Update(convertedModel);
        }

        public void DeleteClient(int id)
        {
            var modelToDelete = _ebayClientRepository.Table.First(x => x.Id == id);
            _ebayClientRepository.Delete(modelToDelete);
        }

        public void DeleteClients(List<int> ids)
        {
            foreach (var id in ids)
            {
                DeleteClient(id);
            }
        }

        public EbayClientViewModel GetById(int id)
        {
            var table = _ebayClientRepository.Table.First(x => x.Id == id);
            var modelToReturn = table.ToModel<EbayClientViewModel>();
            return modelToReturn;
        }
        public List<EbayClientViewModel> GetAllActiveClients()
        {
            var table = _ebayClientRepository.Table.Where(x => x.IsActive);
            var modelToReturn = new List<EbayClientViewModel>();

            foreach (var item in table)
            {
                modelToReturn.Add(item.ToModel<EbayClientViewModel>());
            }
            return modelToReturn;

        }

        public EbaySearchModel PrepareSearchModel()
        {
            return new EbaySearchModel();
        }

        public PagedList<EbayClientViewModel> GetEbayClients(int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var table = _ebayClientRepository.Table;
            var modelToReturn = new List<EbayClientViewModel>();

            foreach (var item in table)
            {
                modelToReturn.Add(item.ToModel<EbayClientViewModel>());
            }

            var pagedResult = new PagedList<EbayClientViewModel>(modelToReturn, pageIndex, pageSize);
            return pagedResult;
        }
        private PagedList<EbayClient> PerformSearch(EbaySearchModel searchModel)
        {
            var query = _ebayClientRepository.Table;
            var result = new PagedList<EbayClient>(query, searchModel.Page - 1, pageSize: searchModel.PageSize);
            return result;
        }

        public EbayClientListModel PrepareEbayClientListModel(EbaySearchModel searchModel)
        {
            var queryResult = PerformSearch(searchModel);
            var model = new EbayClientListModel().PrepareToGrid(searchModel, queryResult, () =>
            {
                //fill in model values from the entity
                return queryResult.Select(config =>
                {
                    var manufacturerModel = config.ToModel<EbayClientViewModel>();
                    return manufacturerModel;
                });
            });

            return model;
        }

        public EbayClientViewModel PrepareClientViewModel(int id = 0)
        {
            return id != 0 ? GetById(id) : new EbayClientViewModel();
        }

        private void CreateCustomerEntry(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                _logger.Information("Cannot create user because user name is empty.");
                return;
            }
            
            var customer = _customerService.GetCustomerByUsername(userName);

            if (customer == null)
                customer = _customerService.GetCustomerByEmail($"{userName}@ebayClient.com");

            try
            {
                if (customer == null)
                {
                    customer = BuildCustomer(userName);
                    
                    _logger.Information($"New User to be created: {JsonConvert.SerializeObject(customer)}");
                    
                    _customerService.InsertCustomer(customer);
                    
                    _logger.Information($"New User is created for Client {userName}.");

                    AddCustomerRole(customer);
                }
            }
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, $"ailed to create customer for user name {userName}: {ex.Message}", $"{JsonConvert.SerializeObject(ex)}");
            }
        }

        private static Customer BuildCustomer(string userName)
        {
            Customer customer;
            customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                CreatedOnUtc = DateTime.UtcNow,
                AdminComment =
                    "Account For Ebay Seller/Client From eBay. This Account is purely for Invoicing Purpose.",
                LastActivityDateUtc = DateTime.UtcNow,
                VendorId = 0,
                IsTaxExempt = false,
                Active = true,
                Email = $"{userName}@ebayClient.com",
                RegisteredInStoreId = 1, // Default it to very first store.
                Username = userName,
                SystemName = userName,
            };
            return customer;
        }

        private void AddCustomerRole(Customer customer)
        {
            _logger.Information($"Adding 'Registered' role for user {customer.Username}");
            var guestRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.RegisteredRoleName);
            var roleMapping = new CustomerCustomerRoleMapping();
            roleMapping.CustomerId = customer.Id;
            roleMapping.CustomerRoleId = guestRole.Id;
            _customerService.AddCustomerRoleMapping(roleMapping);
        }
    }
}

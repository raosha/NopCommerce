using EbaySoapServiceClient;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Misc.Ebay.Services.EbayCustomer
{
    public interface IEbayCustomerService
    {
        Customer GetCustomer(UserType user , AddressType address, AddressType billingAddress);
    }
}
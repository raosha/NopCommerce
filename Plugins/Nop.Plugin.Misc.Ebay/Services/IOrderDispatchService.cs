using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Misc.Ebay.Services
{
    public interface IOrderDispatchService
    {
        Task DispatchOrders();
        bool AddDispatchableOrdersRecord(IList<string> orderIds, string dispatchRequestedBy);
    }
}


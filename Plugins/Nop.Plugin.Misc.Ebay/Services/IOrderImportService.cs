using System.Threading.Tasks;
using Nop.Plugin.Misc.Ebay.Models.EbayRequests;

namespace Nop.Plugin.Misc.Ebay.Services
{
    public interface IOrderImportService
    {
        Task<bool> ImportOrder();
        Task<bool> ImportTestOrders();
        Task<bool> ImportForClient(int clientId);
    }
}
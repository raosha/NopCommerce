using Nop.Plugin.Misc.Ebay.Models;
using Nop.Plugin.Misc.Ebay.Models.CoreExtension;
using Nop.Web.Areas.Admin.Models.Orders;

namespace Nop.Plugin.Misc.Ebay.Factory
{
    public interface IOrderSearchResultFactory
    {
        CustomOrderListModel PrepareOrderListModel(EbayOrdersSearchModel searchModel);
        OrderSearchModel PrepareOrderSearchModel(EbayOrdersSearchModel searchModel);
    }
}
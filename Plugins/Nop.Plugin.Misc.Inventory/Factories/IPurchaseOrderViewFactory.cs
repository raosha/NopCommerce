namespace Nop.Plugin.Misc.Inventory.Factories
{
    using Nop.Plugin.Misc.Inventory.Models;

    public interface IPurchaseOrderViewFactory
    {
        PurchaseOrderListModel PrepareList(PurchaseOrderSearchModel searchModel);
    }
}
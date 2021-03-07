using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Misc.Inventory.Models
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Nop.Web.Framework.Models;

    public interface IPurchaseOrderListModel
    {
        /// <summary>
        /// Perform additional actions for binding the model
        /// </summary>
        /// <param name="bindingContext">Model binding context</param>
        /// <remarks>Developers can override this method in custom partial classes in order to add some custom model binding</remarks>
        void BindModel(ModelBindingContext bindingContext);
    }

    public partial class PurchaseOrderListModel : BasePagedListModel<PurchaseOrderViewModel>, IPurchaseOrderListModel
    {
    }
}

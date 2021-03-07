using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Inventory.Factories;
using Nop.Plugin.Misc.Inventory.Models;
using Nop.Plugin.Misc.Inventory.Services;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.Inventory.Controllers
{
    public class CreatePurchaseOrderController : BasePluginController
    {
        private readonly IPermissionService _permissionService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IPurchaseOrderFactory _purchaseOrderFactory;

        public CreatePurchaseOrderController(IPermissionService permissionService, IPurchaseOrderService purchaseOrderService, IPurchaseOrderFactory purchaseOrderFactory)
        {
            _permissionService = permissionService;
            _purchaseOrderService = purchaseOrderService;
            _purchaseOrderFactory = purchaseOrderFactory;
        }
        
        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Create(int id = 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedDataTablesJson();

            var model = _purchaseOrderFactory.PrepareViewModel(id);
            return View(model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult CreateOrderLine(int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedDataTablesJson();

            var model = new PurchaseOrderLineViewModel();
            return PartialView(model);
        }
    }
}

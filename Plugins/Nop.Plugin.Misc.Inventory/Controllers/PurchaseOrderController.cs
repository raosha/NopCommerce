using Microsoft.AspNetCore.Http;
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
    using System.Threading.Tasks;

    public class PurchaseOrderController : BasePluginController
    {
        private readonly IPermissionService _permissionService;
        private readonly IPurchaseOrderFactory _purchaseOrderFactory;
        private readonly IPurchaseOrderViewFactory _purchaseOrderViewFactory;
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(IPermissionService permissionService, IPurchaseOrderFactory purchaseOrderFactory, IPurchaseOrderViewFactory purchaseOrderViewFactory, IPurchaseOrderService purchaseOrderService)
        {
            _permissionService = permissionService;
            _purchaseOrderFactory = purchaseOrderFactory;
            _purchaseOrderViewFactory = purchaseOrderViewFactory;
            _purchaseOrderService = purchaseOrderService;
        }
       
        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();
            var model = _purchaseOrderFactory.PrepareSearchModel();

            return View(model);
        }
        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> List(PurchaseOrderSearchModel searchModel)
        {
           if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedDataTablesJson();

           var model = _purchaseOrderViewFactory.PrepareList(searchModel);
           
           return Json(model);
        }
        //[AuthorizeAdmin]
        //[Area(AreaNames.Admin)]
        //public virtual IActionResult Edit(int id)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
        //        return AccessDeniedView();

        //    //try to get an order with the specified id
        //    var purchaseOrder = _purchaseOrderService.GetById(id);
        //    if (purchaseOrder == null)
        //        return RedirectToAction("Create");

        //    //prepare model
        //    var model = _purchaseOrderFactory.PrepareViewModel(id,false);

        //    return View(model);
        //}

        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Create(int id= 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedDataTablesJson();

            var model = _purchaseOrderFactory.PrepareViewModel(id, id == 0);
            return View(model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Create(PurchaseOrderViewModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedDataTablesJson();
            if (model.Id > 0)
            {
                // load PO View Model.
                //var existingModel = _purchaseOrderFactory.PrepareViewModel(model.Id, model.Id == 0);
                
                _purchaseOrderFactory.UpdatePurchaseOrder(model);
            }
            else
            {
                _purchaseOrderFactory.AddPurchaseOrder(model);
            }
            

            return RedirectToAction("List");
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Create1(FormCollection model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedDataTablesJson();
            
            return RedirectToAction();
        }


        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult PurchaseOrderLine(int vendorId, int lineNumber)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedDataTablesJson();

            var model = _purchaseOrderFactory.PrepareProductsForVendors(vendorId);
            model.LineNumber = lineNumber;
            return PartialView(model);
        }
    }
}

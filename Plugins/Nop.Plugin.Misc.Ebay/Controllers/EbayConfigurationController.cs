using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Plugin.Misc.Ebay.Services;
using Nop.Plugin.Misc.Ebay.Services.Configurations;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.Ebay.Controllers
{
    public class EbayConfigurationController : BasePluginController
    {
        private readonly IPermissionService _permissionService;
        private readonly IConfigurationService _ebayConfigurationService;
        public EbayConfigurationController( IPermissionService permissionService, IConfigurationService ebayConfigurationService)
        {
            _permissionService = permissionService;
            _ebayConfigurationService = ebayConfigurationService;
        }
        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult List()
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
           
            var model = _ebayConfigurationService.PrepareSearchModel();

            return View(model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult List(EbaySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedDataTablesJson();

            var model = _ebayConfigurationService.PrepareConfigurationListModel(searchModel);
            
            var restult =  Json(model);
            return restult;
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            _ebayConfigurationService.DeleteConfiguration(selectedIds.ToList());
            return Json(new { Result = true });
        }

        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Create(int id=0)
        {
            var model = _ebayConfigurationService.PrepareConfigurationViewModel(id);
            return View(model);
        }
        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Create(EbayConfigurationViewModel model)
        {
            _ebayConfigurationService.AddConfiguration(model);
            return View(model);
        }
    }
}

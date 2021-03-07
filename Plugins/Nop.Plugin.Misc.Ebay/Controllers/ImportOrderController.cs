using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Ebay.Services;
using Nop.Plugin.Misc.Ebay.Services.Clients;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.Ebay.Controllers
{
    public class ImportOrderController : BasePluginController
    {
        private readonly IPermissionService _permissionService;
        private readonly IOrderImportService _orderImportService;
        private readonly INotificationService _notificationService;
        public ImportOrderController(IPermissionService permissionService, IClientService ebayClientService, IOrderImportService orderImportService, INotificationService notificationService)
        {
            _permissionService = permissionService;
            _orderImportService = orderImportService;
            _notificationService = notificationService;
        }

        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            return Ok("Import Ebay Orders.");
        }

        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> ImportEbayOrders()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                var orderImported =  await _orderImportService.ImportOrder();

                if (!orderImported)
                    _notificationService.ErrorNotification("Failed to Import Orders.");
                else
                    _notificationService.ErrorNotification("Successfully Imported Ebay Orders.");

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest($"Failed to Import Ebay Orders. Reason:  {e.Message}");
            }
        }
    }
}

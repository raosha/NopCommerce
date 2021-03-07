using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Plugin.Misc.Ebay.Services.Clients;
using Nop.Plugin.Misc.Ebay.Services.EbayExternal;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.Ebay.Controllers
{

    public class EbayClientsController : BasePluginController
    {
        private readonly IPermissionService _permissionService;
        private readonly IClientService _ebayClientService;
        private readonly IExternalEbayAuthService _externalEbayAuthService;
        private readonly INotificationService _notificationService;
        public EbayClientsController(IPermissionService permissionService, IClientService ebayClientService, IExternalEbayAuthService externalEbayAuthService, INotificationService notificationService)
        {
            _permissionService = permissionService;
            _ebayClientService = ebayClientService;
            _externalEbayAuthService = externalEbayAuthService;
            _notificationService = notificationService;
        }

        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            var model = _ebayClientService.PrepareSearchModel();
            return View(model);
        }

       
        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> List(EbaySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedDataTablesJson();

            var model = _ebayClientService.PrepareEbayClientListModel(searchModel);
            return Json(model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            _ebayClientService.DeleteClients(selectedIds.ToList());
            return Json(new { Result = true });
        }

        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            var signinUrl = await _externalEbayAuthService.GetAuthSignInUrl();
            return Redirect(signinUrl);
        }
        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Create(EbayClientViewModel model)
        {
            _ebayClientService.AddClient(model);
            return View(model);
        }

        [HttpGet]
        [Area(AreaNames.Admin)]
        [AllowAnonymous]
        public async Task<IActionResult> EbayCallBackForClient(string ebaytkn, string tknexp, string username)
        {
            if (string.IsNullOrEmpty(ebaytkn) || ebaytkn.Length < 20)
                ebaytkn= await _externalEbayAuthService.FetchToken();
         
            var clientAdded =  _ebayClientService.AddClient(new EbayClientViewModel()
            {
                IsActive = true,
                Comments = "Imported Via Ebay",
                Token = ebaytkn,
                UserName = username,
                LastImportTime = DateTime.UtcNow.AddDays(-10), // Go 10 Days.
                TokenExpiresOn = DateTime.UtcNow.AddDays(365)
            });

           if (!clientAdded)
               _notificationService.ErrorNotification("Cannot add new Client. Please make sure that client with same name does not already exists.");
           else
               _notificationService.ErrorNotification($"Successfully integrated client {username}.");
            
           return RedirectToAction("List");            
        }

    }
}

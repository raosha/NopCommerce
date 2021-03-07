using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nop.Plugin.Misc.Ebay.Helpers;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Plugin.Misc.Ebay.Services;
using Nop.Plugin.Misc.Ebay.Services.Clients;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using NUglify.Helpers;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.Misc.Ebay.Controllers
{
    public class DeliveryLabelController : BasePluginController
    {
        private readonly IPermissionService _permissionService;
        private readonly IDeliveryLabelService _deliveryLabelService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        public DeliveryLabelController(IPermissionService permissionService, IDeliveryLabelService deliveryLabelService, ILogger logger, INotificationService notificationService)
        {
            _permissionService = permissionService;
            _deliveryLabelService = deliveryLabelService;
            _logger = logger;
            _notificationService = notificationService;
        }

        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            var model = _deliveryLabelService.PrepareSearchModel();
            return View(model);
        }


        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> List(FileUploadViewModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedDataTablesJson();

            var model = _deliveryLabelService.PrepareDeliveryLabelListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            _deliveryLabelService.DeleteDeliveryLabels(selectedIds.ToList());
            return Json(new { Result = true });
        }

        [HttpGet]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Create()
        {
            var model = _deliveryLabelService.PrepareFileUploadViewModel();
            return View(model);
        }
        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Create(FileUploadViewModel uploadModel)
        {
            var formFileContent =
                await FileHelpers.ProcessFormFile<FileUploadViewModel>(uploadModel.FormFile,_logger);

            if (formFileContent.Length == 0)
                return BadRequest("Invalid File Uploaded.");

            var model = new DeliveryLabelViewModel
            {
                Content = formFileContent,
                Title = uploadModel.Title,
                Name = uploadModel.FormFile.FileName,
                UploadOn = DateTime.UtcNow,
                Size = uploadModel.FormFile.Length,
                MimeType = uploadModel.FormFile.ContentType,
                Base64Contents = Convert.ToBase64String(formFileContent)
            };

            var isAdded = _deliveryLabelService.InsertDeliveryLabel(model);

            if (!isAdded)
            {
                _notificationService.ErrorNotification(
                    "Cannot add delivery label. Please make sure that delivery label with title name does not already exists.");

                return View(uploadModel);
            }

            return RedirectToAction("List");
        }
    }
}

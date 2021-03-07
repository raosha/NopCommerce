using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Tasks;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Tasks;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.Ebay.Infrastructure
{
    public class EbayPlugin : BasePlugin, IMiscPlugin, IPlugin, IAdminMenuPlugin
    {
        private static string pn = "eBay Integration";

        private readonly IProductService _productService;

        private readonly ISettingService _settingService;

        private readonly IScheduleTaskService _scheduleTaskService;

        private readonly IWorkContext _workContext;

        private readonly AdminAreaSettings _adminAreaSettings;

        private readonly ILocalizationService _localizationService;

        private readonly ICategoryService _categoryService;

        private readonly IPictureService _pictureService;

        private readonly ILogger _logger;

        private readonly ICustomerService _customerService;

        public static bool PaidVersion = true;

        private readonly IStoreContext _storeContext;

        private readonly IWebHelper _webHelper;

        private readonly IOrderService _orderService;

     
        public EbayPlugin(ISettingService settingService, ILocalizationService localizationService, AdminAreaSettings adminAreaSettings, IWorkContext workContext, IScheduleTaskService scheduleTaskService, IProductService productService, ILogger logger, ICustomerService customerService, IStoreContext storeContext, IWebHelper webHelper, IOrderService orderService, ICategoryService categoryService, IPictureService pictureService)
        {
            _settingService = settingService;
            _localizationService = localizationService;
            _adminAreaSettings = adminAreaSettings;
            _workContext = workContext;
            _scheduleTaskService = scheduleTaskService;
            _productService = productService;
            _logger = logger;
            _customerService = customerService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _orderService = orderService;
            _categoryService = categoryService;
            _pictureService = pictureService;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/EbayConfiguration/Configure";
        }
        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var myPluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Ebay");
            if (myPluginNode == null)
            {
                myPluginNode = new SiteMapNode()
                {
                    SystemName = "Ebay",
                    Title = "Ebay",
                    Visible = true,
                    IconClass = "fa-gear"
                };
                rootNode.ChildNodes.Add(myPluginNode);
            }
            var configManuItem = new SiteMapNode()
            {
                SystemName = "EbayConfigurations",
                Title = "Ebay Configurations",
                ControllerName = "EbayConfiguration",
                ActionName = "List",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };
            var clientsManuItem = new SiteMapNode()
            {
                SystemName = "EbayClients",
                Title = "Ebay Clients",
                ControllerName = "EbayClients",
                ActionName = "List",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };
            var ordersManuItem = new SiteMapNode()
            {
                SystemName = "EbayOrders",
                Title = "Ebay Orders",
                ControllerName = "EbayOrder",
                ActionName = "List",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            var deliveryLabelManuItem = new SiteMapNode()
            {
                SystemName = "DeliveryLabels",
                Title = "Delivery Labels",
                ControllerName = "DeliveryLabel",
                ActionName = "List",
                Visible = true,
                RouteValues = new RouteValueDictionary() {{"area", "Admin"}},
            };

            myPluginNode.ChildNodes.Add(configManuItem);
            myPluginNode.ChildNodes.Add(clientsManuItem);
            myPluginNode.ChildNodes.Add(ordersManuItem);
            myPluginNode.ChildNodes.Add(deliveryLabelManuItem);
        }

        public void InstallStringResources()
        {
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.SiteCode", "eBay Site Code ");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.EndPoint", "Endpoint");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.SignInUrl", "SignInUrl");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.Version", "Version");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.CertId", "CertId");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.AppId", "AppId");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.DevId", "DevId");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.IsSandbox", "IsSandbox");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.IsActive", "IsActive");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.Token", "Token");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.RuName", "RuName");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.Comments", "Comments");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.UserName", "User Name");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.LastImportTime", "LastImportTime");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.TokenExpiresOn", "Token Expiry");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.ImportOrders", "Import Orders");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.DispatchOrders", "Dispatch Orders");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.Dispatch.Selected", "Dispatch Selected");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.Dispatch.All", "Dispatch All");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.PrintDeliveryLabel", "Print Delivery Label(s)");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.PrintDeliveryLabel.Selected", "Print Delivery Label (Selected)");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.PrintDeliveryLabel.All", "Print Delivery Label (All)");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.NoClientSelected", "No Client has been selected to import the orders for.");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.FailedImport", "Failed to Import Order. Please contact system administrator.");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.FailedDispatch", "Failed to dispatch Orders. Please contact system administrator.");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.DeliveryLabel.Title", "Label Title");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.DeliveryLabel.Name", "File Name");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.DeliveryLabel.Size", "File Size (Bytes)");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.DeliveryLabel.UploadOn", "Upload Date");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.DeliveryLabel.FormFile", "File");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.DeliveryLabel.File", "Upload File");
            _localizationService.AddOrUpdateLocaleResource("Admin.Orders.Fields.Title", "Title");
            _localizationService.AddOrUpdateLocaleResource("Admin.Orders.Fields.Sku", "Sku");
            _localizationService.AddOrUpdateLocaleResource("Admin.Orders.Fields.SellerName", "Seller Name");
            _localizationService.AddOrUpdateLocaleResource("Admin.Orders.Fields.PostCode", "Post Code");
            _localizationService.AddOrUpdateLocaleResource("Admin.Orders.Fields.ExtendedOrderId", "Extended Order Id");
            _localizationService.AddOrUpdateLocaleResource("Admin.Ebay.DeleteOrders", "Delete Selected Order(s)");
            _localizationService.AddOrUpdateLocaleResource("Admin.Orders.List.CustomOrderIds", "Ebay Extended Order Id(s)");
    }

        public override void Install()
        {
            InstallStringResources();
            InstallScheduleTasks();
        }

        private Core.Domain.Tasks.ScheduleTask FindScheduledTask(string name)
        {
            return _scheduleTaskService.GetTaskByType(name);
        }
        protected virtual void InstallScheduleTasks()
        {
            var orderImportTask =
                new Core.Domain.Tasks.ScheduleTask
                {
                    Name = "Import Orders",
                    Seconds = 120,
                    Type = "Nop.Plugin.Misc.Ebay.ScheduleTask.OrderImportScheduleTask, Nop.Plugin.Misc.Ebay",
                    Enabled = true,
                    StopOnError = false
            };

            var orderDispatchTask =
                new Core.Domain.Tasks.ScheduleTask
                {
                    Name = "Dispatch Orders",
                    Seconds = 6000, // Every 10 Minutes
                    Type = "Nop.Plugin.Misc.Ebay.ScheduleTask.OrderDispatchScheduledTask, Nop.Plugin.Misc.Ebay",
                    Enabled = true,
                    StopOnError = false
                };

            var existingTask =
                FindScheduledTask("Nop.Plugin.Misc.Ebay.ScheduleTask.OrderImportScheduleTask, Nop.Plugin.Misc.Ebay");
            if (existingTask == null)
                _scheduleTaskService.InsertTask(orderImportTask);

            existingTask =
                FindScheduledTask("Nop.Plugin.Misc.Ebay.ScheduleTask.OrderDispatchScheduledTask, Nop.Plugin.Misc.Ebay");
            if (existingTask == null)
                _scheduleTaskService.InsertTask(orderDispatchTask);
        }

        public override void Uninstall()
        {
           this.Uninstall();
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone == AdminWidgetZones.CategoryDetailsBlock)
            {
                return "eBayCategoryViewComponent";
            }
            if (widgetZone == AdminWidgetZones.ProductDetailsBlock)
            {
                return "eBayProductViewComponent";
            }
            return "";
        }
    }
}

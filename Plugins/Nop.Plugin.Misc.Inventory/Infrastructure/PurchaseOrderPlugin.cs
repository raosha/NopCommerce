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

namespace Nop.Plugin.Misc.Inventory.Infrastructure
{
    public class PurchaseOrderPlugin : BasePlugin, IMiscPlugin, IPlugin, IAdminMenuPlugin
    {



        private readonly IScheduleTaskService _scheduleTaskService;


        private readonly ILocalizationService _localizationService;


        public static bool PaidVersion = true;

        private readonly IStoreContext _storeContext;

        private readonly IWebHelper _webHelper;

        private readonly IOrderService _orderService;

     

        public PurchaseOrderPlugin(ISettingService settingService, ILocalizationService localizationService, AdminAreaSettings adminAreaSettings, IWorkContext workContext, IScheduleTaskService scheduleTaskService, IProductService productService, ILogger logger, ICustomerService customerService, IStoreContext storeContext, IWebHelper webHelper, IOrderService orderService, ICategoryService categoryService, IPictureService pictureService)
        {
            _localizationService = localizationService;
            _scheduleTaskService = scheduleTaskService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _orderService = orderService;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PurchaseOrder/Configure";
        }
        public void ManageSiteMap(SiteMapNode rootNode)
        {

            var myPluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Inventory");
            if (myPluginNode == null)
            {
                myPluginNode = new SiteMapNode()
                {
                    SystemName = "Inventory",
                    Title = "Inventory",
                    Visible = true,
                    IconClass = "fa-gear"
                };
                rootNode.ChildNodes.Add(myPluginNode);
            }
            var menuItem = new SiteMapNode()
            {
                SystemName = "PurchaseOrders",
                Title = "Purchase Orders",
                ControllerName = "PurchaseOrder",
                ActionName = "List",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };
            myPluginNode.ChildNodes.Add(menuItem);
        }

        public void InstallStringResources()
        {
            _localizationService.AddOrUpdateLocaleResource("Admin.Inventory.List.SearchByVendor", "Select Vendor");
            _localizationService.AddOrUpdateLocaleResource("Admin.Inventory.List.WarehouseSearch", "Vendor WareHouse");
            _localizationService.AddOrUpdateLocaleResource("Admin.Inventory.List.AvailablePurchaseOrderPaymentStatuses", "Payment Status");
            _localizationService.AddOrUpdateLocaleResource("Admin.Inventory.List.AvailablePurchaseOrderStatuses", "Order Status");
            _localizationService.AddOrUpdateLocaleResource("Admin.Inventory.List.WarehouseSearch", "Vendor WareHouse");
            _localizationService.AddOrUpdateLocaleResource("Admin.Inventory.PurchaseOrder.Name", "Order Name");
            _localizationService.AddOrUpdateLocaleResource("Admin.Inventory.PurchaseOrder.OrderDate", "Order Date");
            _localizationService.AddOrUpdateLocaleResource("Admin.Inventory.PurchaseOrder.DeliveryDate", "Delivery Date");
            _localizationService.AddOrUpdateLocaleResource("Admin.Inventory.PurchaseOrder.Total", "Total");
            _localizationService.AddOrUpdateLocaleResource("Admin.Inventory.PurchaseOrder.OrderReference", "PO Reference");
            

        }

        public override void Install()
        {
            InstallStringResources();
        }

        private ScheduleTask FindScheduledTask(string name)
        {
            return _scheduleTaskService.GetTaskByType(name);
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Inventory.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {

            endpointRouteBuilder.MapAreaControllerRoute(
                name: "Nop.Plugin.Inventory",
                areaName: "Admin",
                pattern: "Admin/Inventory/{controller=PurchaseOrder}/{action=List}/{id?}");

            endpointRouteBuilder.MapAreaControllerRoute(
                name: "Nop.Plugin.PurchaseOrder",
                areaName: "Admin",
                pattern: "Admin/{controller=PurchaseOrder}/{action=List}/{id?}");
        }

        public int Priority => 0;
    }
}


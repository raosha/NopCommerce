using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Ebay.Infrastructure
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
                name: "Nop.Plugin.Misc.Ebay",
                areaName: "Admin",
                pattern: "Admin/{controller=EbayConfiguration}/{action=List}/{id?}");

            endpointRouteBuilder.MapAreaControllerRoute(
                name: "Nop.Plugin.Misc.Ebay",
                areaName: "Admin",
                pattern: "Admin/{controller=EbayClients}/{action=List}/{id?}");

            endpointRouteBuilder.MapAreaControllerRoute(
                name: "Nop.Plugin.Misc.Ebay",
                areaName: "Admin",
                pattern: "Admin/{controller=EbayOrder}/{action=List}/{id?}");

            endpointRouteBuilder.MapAreaControllerRoute(
                 name: "Nop.Plugin.Misc.Ebay",
                 areaName: "Admin",
                 pattern: "Admin/{controller=EbayOrder}/{action=List}/{id?}");

            endpointRouteBuilder.MapAreaControllerRoute(
                name: "Nop.Plugin.Misc.Ebay",
                areaName: "Admin",
                pattern: "Admin/{controller=EbayOrder}/{action=AddressEdit}/{addressId}/{orderId?}");
            
            endpointRouteBuilder.MapAreaControllerRoute(
                name: "Nop.Plugin.Misc.Ebay",
                areaName: "Admin",
                pattern: "Admin/{controller=DeliveryLabel}/{action=List}/{id?}");
        }

        public int Priority => 0;
    }
}


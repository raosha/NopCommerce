using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Nop.Plugin.Misc.Inventory.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            viewLocations = new[] { $"/Plugins/Nop.Plugin.Misc.Inventory/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);

            return viewLocations;
        }
    }
}

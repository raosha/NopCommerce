using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Misc.Inventory.Factories;
using Nop.Plugin.Misc.Inventory.Services;

namespace Nop.Plugin.Misc.Inventory.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<PurchaseOrderService>().As<IPurchaseOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseOrderLineService>().As<IPurchaseOrderLineService>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseOrderLineService>().As<IPurchaseOrderLineService>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseOrderFactory>().As<IPurchaseOrderFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseOrderViewFactory>().As<IPurchaseOrderViewFactory>().InstancePerLifetimeScope();
            //builder.RegisterType(new eBayAPIInterfaceClient(eBayAPIInterfaceClient.EndpointConfiguration.eBayAPI));

        }
        public int Order => -1;
    }
}

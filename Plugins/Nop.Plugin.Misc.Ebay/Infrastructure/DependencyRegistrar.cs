using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Misc.Ebay.Factory;
using Nop.Plugin.Misc.Ebay.Services;
using Nop.Plugin.Misc.Ebay.Services.Clients;
using Nop.Plugin.Misc.Ebay.Services.Configurations;
using Nop.Plugin.Misc.Ebay.Services.EbayCustomer;
using Nop.Plugin.Misc.Ebay.Services.EbayExternal;
using Nop.Plugin.Misc.Ebay.Services.EbayOrders;
using Nop.Plugin.Misc.Ebay.Services.Print;

namespace Nop.Plugin.Misc.Ebay.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderImportService>().As<IOrderImportService>().InstancePerLifetimeScope();
            builder.RegisterType<EbayCustomerService>().As<IEbayCustomerService>().InstancePerLifetimeScope();
            builder.RegisterType<EbayOrderItemService>().As<IEbayOrderItemService>().InstancePerLifetimeScope();
            builder.RegisterType<EbayOrderService>().As<IEbayOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<ClientService>().As<IClientService>().InstancePerLifetimeScope();
            builder.RegisterType<ExternalEbayOrderService>().As<IExternalEbayOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<ExternalEbayAuthService>().As<IExternalEbayAuthService>().InstancePerLifetimeScope();
            builder.RegisterType<EbayCustomItemService>().As<IEbayCustomItemService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderDispatchService>().As<IOrderDispatchService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderSearchResultFactory>().As<IOrderSearchResultFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DeliveryLabelService>().As<IDeliveryLabelService>().InstancePerLifetimeScope();
            builder.RegisterType<FileUploadService>().As<IFileUploadService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerPrintService>().As<ICustomerPrintService>().InstancePerLifetimeScope();

        }

        public int Order => -1;
    }
}

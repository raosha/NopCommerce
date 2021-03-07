using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EbaySoapServiceClient;
using Newtonsoft.Json;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Plugin.Misc.Ebay.Models.EbayRequests;
using Nop.Plugin.Misc.Ebay.Services.Clients;
using Nop.Plugin.Misc.Ebay.Services.Configurations;
using Nop.Plugin.Misc.Ebay.Services.EbayCustomer;
using Nop.Plugin.Misc.Ebay.Services.EbayExternal;
using Nop.Plugin.Misc.Ebay.Services.EbayOrders;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.Ebay.Services
{
    public class OrderImportService : IOrderImportService
    {
        private readonly IEbayCustomItemService _ebayCustomItemService;
        private readonly ILogger _logger;
        private readonly IConfigurationService _configurationService;
        private readonly IClientService _ebayClientService;
        private readonly IExternalEbayOrderService _ebayWebService;
        private readonly IEbayCustomerService _customerService;
        private readonly IEbayOrderService _ebayOrderService;

        public OrderImportService(IEbayCustomerService customerService, IExternalEbayOrderService ebayWebService, IClientService ebayClientService,
            IConfigurationService configurationService, ILogger logger, IEbayCustomItemService ebayCustomItemService, IEbayOrderService ebayOrderService)
        {
            _customerService = customerService;
            _ebayWebService = ebayWebService;
            _ebayClientService = ebayClientService;
            _configurationService = configurationService;
            _logger = logger;
            _ebayCustomItemService = ebayCustomItemService;
            _ebayOrderService = ebayOrderService;
        }

        public async Task<bool> ImportOrder()
        {
            var activeConfiguration = _configurationService.GetActiveConfiguration();
            var allClientsData = _ebayClientService.GetAllActiveClients();
            var logMessage = "Importing schedule task is called to import orders";
            _logger.InsertLog(logLevel: LogLevel.Information, logMessage);

            foreach (var client in allClientsData)
                await SingleClientImport(activeConfiguration, client);

            return true;
        }
        public async Task<bool> ImportForClient(int clientId)
        {
            var activeConfiguration = _configurationService.GetActiveConfiguration();
            var client = _ebayClientService.GetAllActiveClients().Where(x=>x.Id == clientId);
            if (client.Any())
                await SingleClientImport(activeConfiguration, client.First());

            return true;
        }
        private async Task SingleClientImport(EbayConfigurationViewModel activeConfiguration, EbayClientViewModel client)
        {
            var requestModel = BuildRequestModel(activeConfiguration, client);
            var logMessage = "Importing order start for client " + client.UserName;
            var logMessageFull = "sending message with " + activeConfiguration.ToString() + " with client information " + client.ToString();
            _logger.InsertLog(logLevel: LogLevel.Information, logMessage, logMessageFull);

            var orderStopWatch = new Stopwatch();
            orderStopWatch.Start();

            var clientOrders = await _ebayWebService.GetOrdersRequest(requestModel);

            if (clientOrders.Ack != 0)
            {
                var result = string.Empty;
                if (clientOrders.Errors != null && clientOrders.Errors.Any())
                {
                    var errors = clientOrders.Errors;
                    foreach (var error in errors)
                        result += "error code -- " + error.ErrorCode
                            + " -- message -- " + error.ShortMessage + "--";
                }

                _logger.InsertLog(LogLevel.Information,
                    "Ack from ebay is : " + clientOrders.Ack, result);
            }

            // Sometimes we have orders alongside errors/warnings.
            if (clientOrders.OrderArray != null && clientOrders.OrderArray.Any())
            {
                logMessage = "Importing took  " + orderStopWatch.ElapsedMilliseconds;
                _logger.InsertLog(logLevel: LogLevel.Information, logMessage);
                await StartImport(clientOrders, client, requestModel);
                UpdateLastImportTimeForClient(client);
            }
        }

        private void UpdateLastImportTimeForClient(EbayClientViewModel client)
        {
            // Update the Client's Last Import Time.
            client.LastImportTime = DateTime.UtcNow;
            _ebayClientService.UpdateClient(client);
        }

        private async Task StartImport(GetOrdersResponseType clientOrders, EbayClientViewModel client, EbayOrderRequestModel requestModel)
        {
            var logMessage = "Total order to insert for " + clientOrders.OrderArray.Count();
            _logger.InsertLog(logLevel: LogLevel.Information, logMessage);
            
            var ordersJson = JsonConvert.SerializeObject(clientOrders.OrderArray.ToList().Select(x => x.TransactionArray[0]));
            Console.WriteLine(ordersJson);

            foreach (var order in clientOrders.OrderArray.ToList())
                try
                {
                    if (MakeSureOrderNotAlreadyImported(order.OrderID) && order.TransactionArray.Count() > 0)
                    {
                        var customer = _customerService.GetCustomer(order.TransactionArray[0].Buyer,
                            order.ShippingAddress,
                            order.TransactionArray[0].Buyer.RegistrationAddress);
                        await _ebayOrderService.CreateOrder(order, customer, client, requestModel);
                    }
                }
                catch (Exception ex)
                {
                    _logger.InsertLog(LogLevel.Error, $"Import order: {ex.Message}", $"{JsonConvert.SerializeObject(ex)}");
                }

        }

        public async Task<bool> ImportTestOrders()
        {
            if(File.Exists("file.json"))
            {
                var orders = JsonConvert.DeserializeObject<GetOrdersResponseType>(File.ReadAllText("file.json"));
                var activeConfiguration = _configurationService.GetActiveConfiguration();
                var allClientsData = _ebayClientService.GetAllActiveClients();
                var requestModel = BuildRequestModel(activeConfiguration, allClientsData.First());
                await StartImport(orders,new EbayClientViewModel(), requestModel);
            }

            return true;
        }

        private bool MakeSureOrderNotAlreadyImported(string ebayOrderId)
        {
            var ebayOrder = _ebayCustomItemService.GetOrderRecordByEbay(ebayOrderId);
            if (ebayOrder == null)
            {
                return true;
            }
            var logMessage = "Ebay order id : " + ebayOrderId + " already exist in system with nop id : " + ebayOrder.Id;
            _logger.InsertLog(logLevel: LogLevel.Information, logMessage);
            return false;
        }

        private EbayOrderRequestModel BuildRequestModel(EbayConfigurationViewModel model, EbayClientViewModel client)
        {
            var requestModel =
                new EbayOrderRequestModel
                {
                    AppId = model.AppId,
                    AuthCert = model.CertId,
                    DevId = model.DevId,
                    SiteId = model.SiteCode,
                    DateFrom = client.LastImportTime,
                    AuthToken = client.Token,
                    Version = model.Version,
                    SiteCode = model.SiteCode,
                    DateTo = DateTime.UtcNow  // Just add the date to filter because this is the time when user is trying to import.
                };
            return requestModel;
        }

    }
}

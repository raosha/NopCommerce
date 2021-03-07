using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Data;
using Nop.Plugin.Misc.Ebay.Domains;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;

namespace Nop.Plugin.Misc.Ebay.Services
{
    public class EbayDispatchableOrdersService : IEbayDispatchableOrdersService
    {
        private readonly IRepository<EbayDispatchableOrders> _ebayDispatchableOrdersRepository;
        private readonly ILogger _logger;

        public EbayDispatchableOrdersService(IRepository<EbayDispatchableOrders> ebayDispatchableOrdersRepository, ILogger logger)
        {
            _ebayDispatchableOrdersRepository = ebayDispatchableOrdersRepository;
            _logger = logger;
        }

        public void InsertDispatchableOrders(List<EbayDispatchableOrdersViewModel> ordersToDispatch)
        {
            if (ordersToDispatch.Any())
            {
                foreach (var orderToDispatch in ordersToDispatch)
                {
                    var dispatchableOrder = orderToDispatch.ToEntity<EbayDispatchableOrders>();
                    _ebayDispatchableOrdersRepository.Insert(dispatchableOrder);
                }
            }
        }

        public void UpdateDispatchableOrdersStatus(List<EbayDispatchableOrdersViewModel> dispatchableOrders)
        {
            if (dispatchableOrders.Any())
            {
                foreach (var orderToDispatch in dispatchableOrders)
                {
                    var dispatchableOrder = orderToDispatch.ToEntity<EbayDispatchableOrders>();
                    _ebayDispatchableOrdersRepository.Update(dispatchableOrder);
                }
            }
        }
    }
}

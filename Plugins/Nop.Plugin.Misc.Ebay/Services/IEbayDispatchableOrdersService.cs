using System;
using System.Collections.Generic;
using System.Text;
using Nop.Plugin.Misc.Ebay.Models;

namespace Nop.Plugin.Misc.Ebay.Services
{
    public interface IEbayDispatchableOrdersService
    {
        void InsertDispatchableOrders(List<EbayDispatchableOrdersViewModel> model);
        void UpdateDispatchableOrdersStatus(List<EbayDispatchableOrdersViewModel> model);
    }
}

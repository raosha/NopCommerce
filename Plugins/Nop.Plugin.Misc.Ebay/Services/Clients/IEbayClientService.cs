using System.Collections.Generic;
using Nop.Core;
using Nop.Plugin.Misc.Ebay.Models;

namespace Nop.Plugin.Misc.Ebay.Services.Clients
{
    public interface IClientService
    {
        List<EbayClientViewModel> GetList();
        bool AddClient(EbayClientViewModel model);
        void UpdateClient(EbayClientViewModel model);
        void DeleteClient(int id);
        void DeleteClients(List<int> ids);
        EbayClientViewModel GetById(int id);
        List<EbayClientViewModel> GetAllActiveClients();
        EbaySearchModel PrepareSearchModel();
        PagedList<EbayClientViewModel> GetEbayClients(int pageIndex = 0, int pageSize = int.MaxValue);
        EbayClientListModel PrepareEbayClientListModel(EbaySearchModel searchModel);
        EbayClientViewModel PrepareClientViewModel(int id = 0);
    }
}

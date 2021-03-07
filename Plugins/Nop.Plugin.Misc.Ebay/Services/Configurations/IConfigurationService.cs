using System.Collections.Generic;
using Nop.Core;
using Nop.Plugin.Misc.Ebay.Domains;
using Nop.Plugin.Misc.Ebay.Models;

namespace Nop.Plugin.Misc.Ebay.Services.Configurations
{
    public interface IConfigurationService
    {
        List<EbayConfigurationViewModel> GetList();
        void AddConfiguration(EbayConfigurationViewModel model);
        void UpdateConfiguration(EbayConfigurationViewModel model);
        void DeleteConfiguration(int id);
        void DeleteConfiguration(List<int> ids);
        EbayConfigurationViewModel GetById(int id);
        EbayConfigurationViewModel GetActiveConfiguration();
        EbaySearchModel PrepareSearchModel();
        PagedList<EbayConfigurationViewModel> GetEbayConfigurations(int pageIndex = 0, int pageSize = int.MaxValue);
        EbayConfigurationListModel PrepareConfigurationListModel(EbaySearchModel searchModel);
        EbayConfigurationViewModel PrepareConfigurationViewModel(int id = 0);
        EbayConfiguration GetActiveConfig();
    }
}
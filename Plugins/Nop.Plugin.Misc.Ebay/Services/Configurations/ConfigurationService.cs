using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.Ebay.Domains;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.Ebay.Services.Configurations
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IRepository<EbayConfiguration> _ebayConfigurationRepository;

        public ConfigurationService(IRepository<EbayConfiguration> ebayConfigurationRepository)
        {
            _ebayConfigurationRepository = ebayConfigurationRepository;
        }
        public List<EbayConfigurationViewModel> GetList()
        {
            var table = _ebayConfigurationRepository.Table.ToList();
            var modelToReturn = new List<EbayConfigurationViewModel>();

            foreach (var item in table)
            {
                modelToReturn.Add(item.ToModel<EbayConfigurationViewModel>());
            }
            return modelToReturn;
        }
        public void AddConfiguration(EbayConfigurationViewModel model)
        {
            if (model.Id != 0)
            {
                UpdateConfiguration(model);
            }
            else
            {
                var modelToInsert = model.ToEntity<EbayConfiguration>();
                _ebayConfigurationRepository.Insert(modelToInsert);    
            }
            
        }
        public void UpdateConfiguration(EbayConfigurationViewModel model)
        {
             var convertedModel = model.ToEntity<EbayConfiguration>();
             _ebayConfigurationRepository.Update(convertedModel);
        }
        public void DeleteConfiguration(int id)
        {
            var modelToDelete = _ebayConfigurationRepository.Table.First(x => x.Id == id);
            _ebayConfigurationRepository.Delete(modelToDelete);
        }
        public void DeleteConfiguration(List<int> ids)
        {
            foreach (var id in ids)
            {
                DeleteConfiguration(id);
            }
        }
        public EbayConfigurationViewModel GetById(int id)
        {
            var table =  _ebayConfigurationRepository.Table.First(x=>x.Id == id);
            var modelToReturn = table.ToModel<EbayConfigurationViewModel>();
            return modelToReturn;
        }
        public EbayConfigurationViewModel GetActiveConfiguration()
        {
            var table = _ebayConfigurationRepository.Table.Where(x=>x.IsActive);
            return table.Any() ? table.First().ToModel<EbayConfigurationViewModel>() : new EbayConfigurationViewModel();
        }
        public EbayConfiguration GetActiveConfig()
        {
            var table = _ebayConfigurationRepository.Table.Where(x=>x.IsActive);
            return table.Any() ? table.First() : new EbayConfiguration();
        }


        public EbaySearchModel PrepareSearchModel()
        {
            return new EbaySearchModel();
        }

        public PagedList<EbayConfigurationViewModel> GetEbayConfigurations(int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var table = _ebayConfigurationRepository.Table;
            var modelToReturn = new List<EbayConfigurationViewModel>();
          
            var pagedResult = new PagedList<EbayConfigurationViewModel>(modelToReturn, pageIndex, pageSize);
            return pagedResult;
        }

        private PagedList<EbayConfiguration> PerformSearch(EbaySearchModel searchModel)
        {
            var query = _ebayConfigurationRepository.Table;
            var result = new PagedList<EbayConfiguration>(query, searchModel.Page - 1, pageSize: searchModel.PageSize);
            return result;
        }

        public EbayConfigurationListModel PrepareConfigurationListModel(EbaySearchModel searchModel)
        {

            var queryResult = PerformSearch(searchModel);

            var model = new EbayConfigurationListModel().PrepareToGrid(searchModel, queryResult, () =>
            {
                //fill in model values from the entity
                return queryResult.Select(config =>
                {
                    var manufacturerModel = config.ToModel<EbayConfigurationViewModel>();
                    return manufacturerModel;
                });
            });
            return model;
        }
        public EbayConfigurationViewModel PrepareConfigurationViewModel(int id=0)
        {
            return id != 0 ? GetById(id) : new EbayConfigurationViewModel();
        }
    }
}

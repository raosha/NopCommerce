using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Misc.Ebay.Domains;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.Ebay.Services
{
    public class DeliveryLabelService : IDeliveryLabelService
    {
        private readonly IRepository<DeliveryLabel> _deliveryLabelRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger _logger;
        
        public DeliveryLabelService(IRepository<DeliveryLabel> deliveryLabelRepository, ILogger logger, IFileUploadService fileUploadService)
        {
            _deliveryLabelRepository = deliveryLabelRepository;
            _logger = logger;
            _fileUploadService = fileUploadService;
        }

        public bool DeleteDeliveryLabel(int labelId)
        {
            try
            {
                var modelToDelete = _deliveryLabelRepository.Table.First(x => x.Id == labelId);
                _deliveryLabelRepository.Delete(modelToDelete);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to delete delivery label with Id: {labelId}. Error: {e.Message}", null, null);
            }

            return false;
        }

        public bool DeleteDeliveryLabels(List<int> labelIds)
        {
            try
            {
                foreach (var labelId in labelIds)
                {
                    DeleteDeliveryLabel(labelId);
                }
                return true;
            }
            catch (Exception e)
            {

                _logger.Error($"Failed to delete all delivery label with Ids: ({string.Join(",", labelIds.Select(n => n.ToString()).ToArray())}). Error: {e.Message}", null, null);
            }

            return false;
        }

        public IPagedList<DeliveryLabelViewModel> GetAllDeliveryLables(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                var table = _deliveryLabelRepository.Table;
                var modelToReturn = new List<DeliveryLabelViewModel>();

                foreach (var item in table)
                {
                    var viewModelItem = item.ToModel<DeliveryLabelViewModel>();
                    viewModelItem.Url = _fileUploadService.GetDeliveryLabelUrl(item.Name, item.Content);
                    modelToReturn.Add(viewModelItem);
                }

                var pagedResult = new PagedList<DeliveryLabelViewModel>(modelToReturn, pageIndex, pageSize);
                return pagedResult;
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to get all delivery labels . Error: {e.Message}", e, null);
            }

            return null;
        }

        public List<DeliveryLabelViewModel> GetList()
        {
            var table = _deliveryLabelRepository.Table.ToList();
            var modelToReturn = new List<DeliveryLabelViewModel>();

            foreach (var item in table)
            {
                var model = item.ToModel<DeliveryLabelViewModel>();
                model.Url = _fileUploadService.GetDeliveryLabelUrl(model.Name,model.Content);
                modelToReturn.Add(model);
            }
            return modelToReturn;
        }

        public EbaySearchModel PrepareSearchModel()
        {
            return new EbaySearchModel();
        }

        public DeliveryLabelListModel PrepareDeliveryLabelListModel(FileUploadViewModel searchModel)
        {
            var queryResult = PerformSearch(searchModel);
            var model = new DeliveryLabelListModel().PrepareToGrid(searchModel, queryResult, () =>
            {
                //fill in model values from the entity
                return queryResult.Select(config =>
                {
                    var dlModel = config.ToModel<DeliveryLabelViewModel>();
                    if (string.IsNullOrEmpty(dlModel.Base64Contents))
                        dlModel.Base64Contents = Convert.ToBase64String(dlModel.Content);

                    return dlModel;
                });
            });

            return model;
        }

        public DeliveryLabelViewModel PrepareDeliveryLabelViewModel(int id = 0)
        {
            return id != 0 ? GetDeliveryLabelById(id) : new DeliveryLabelViewModel();
        }
        public FileUploadViewModel PrepareFileUploadViewModel()
        {
            return new FileUploadViewModel();
        }
        private PagedList<DeliveryLabel> PerformSearch(FileUploadViewModel searchModel)
        {
            var query = _deliveryLabelRepository.Table;
            var result = new PagedList<DeliveryLabel>(query, searchModel.Page - 1, pageSize: searchModel.PageSize);
            return result;
        }

        public DeliveryLabelViewModel GetDeliveryLabelById(int labelId)
        {
            var row = _deliveryLabelRepository.Table.First(x => x.Id == labelId);
            var modelToReturn = row.ToModel<DeliveryLabelViewModel>();
            modelToReturn.Url = _fileUploadService.GetDeliveryLabelUrl(row.Name,row.Content);
            return modelToReturn;
        }

        public bool InsertDeliveryLabel(DeliveryLabelViewModel deliveryLabel)
        {
            try
            {
                if (_deliveryLabelRepository.Table.Any(x => x.Title == deliveryLabel.Title))
                {
                    _logger.Information($"Duplicate title {deliveryLabel}. Delivery label with same title already exists.");
                    return false;
                }

                var modelToInsert = deliveryLabel.ToEntity<DeliveryLabel>();
                _deliveryLabelRepository.Insert(modelToInsert);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to insert delivery label : {JsonConvert.SerializeObject(deliveryLabel)}. Error: {e.Message}", e, null);
            }

            return false;
        }

        public bool UpdateDeliveryLabel(DeliveryLabelViewModel deliveryLabel)
        {
            try
            {
                if (_deliveryLabelRepository.Table.Any(x => x.Title == deliveryLabel.Title))
                {
                    _logger.Information($"Duplicate title {deliveryLabel}. Delivery label with same title already exists.");
                    return false;
                }

                var modelToInsert = deliveryLabel.ToEntity<DeliveryLabel>();
                _deliveryLabelRepository.Update(modelToInsert);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to update delivery label : {JsonConvert.SerializeObject(deliveryLabel)}. Error: {e.Message}", e, null);
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Plugin.Misc.Ebay.Domains;
using Nop.Plugin.Misc.Ebay.Models;

namespace Nop.Plugin.Misc.Ebay.Services
{
    public interface IDeliveryLabelService
    {
        DeliveryLabelViewModel GetDeliveryLabelById(int labelId);
        bool InsertDeliveryLabel(DeliveryLabelViewModel deliveryLabel);
        bool UpdateDeliveryLabel(DeliveryLabelViewModel deliveryLabel);
        bool DeleteDeliveryLabel(int labelId);
        bool DeleteDeliveryLabels(List<int> labelIds); 
        List<DeliveryLabelViewModel> GetList();
        IPagedList<DeliveryLabelViewModel> GetAllDeliveryLables(int pageIndex = 0, int pageSize = int.MaxValue);
        EbaySearchModel PrepareSearchModel();
        DeliveryLabelListModel PrepareDeliveryLabelListModel(FileUploadViewModel searchModel);
        DeliveryLabelViewModel PrepareDeliveryLabelViewModel(int id = 0);
        FileUploadViewModel PrepareFileUploadViewModel();
    }
}

using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Misc.Inventory.Domains;
using Nop.Plugin.Misc.Inventory.Models;

namespace Nop.Plugin.Misc.Inventory.AutoMapper
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor

        public MapperConfiguration()
        {
            CreateMap<PurchaseOrderLine, PurchaseOrderLineViewModel>().ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderViewModel>().ReverseMap();
            CreateMap<PurchaseOrderNote, PurchaseOrderNoteViewModel>().ReverseMap();
        }
        #endregion
        #region Properties
        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 1;

        #endregion
    }
}

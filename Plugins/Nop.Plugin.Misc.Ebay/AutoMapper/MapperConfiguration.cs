using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Misc.Ebay.Domains;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Plugin.Misc.Ebay.Models.EbayRequests;

namespace Nop.Plugin.Misc.Ebay.AutoMapper
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor

        public MapperConfiguration()
        {
            CreateMap<EbayConfiguration, EbayConfigurationViewModel>().ReverseMap();
            CreateMap<EbayConfiguration, EbaySignInRequestModel>().ReverseMap();
            CreateMap<DeliveryLabel, DeliveryLabelViewModel>().ReverseMap();
            CreateMap<EbayClient, EbayClientViewModel>().ReverseMap();
            CreateMap<EbayDispatchableOrders, EbayDispatchableOrdersViewModel>().ReverseMap();
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

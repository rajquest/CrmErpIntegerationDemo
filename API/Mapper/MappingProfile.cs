using AutoMapper;
using API.Models.InforErp;

namespace API.Mapper
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<ItemLotLocation, ItemLotLocationSerialNumbers>();
            CreateMap<JobOrderItem, JobVariance>();
            CreateMap<InvoiceListingItem, InvoicedOrder>(); 
            CreateMap<TaxJurisdictionDetails, InvoicedOrder>();
        }
    }
}

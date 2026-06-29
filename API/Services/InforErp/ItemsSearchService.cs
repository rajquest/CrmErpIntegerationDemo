using API.Interfaces;
using API.Models.InforErp;

namespace API.Services.InforErp
{
    public class ItemsSearchService : IItemsSearchService
    {
        private readonly IInforErpService _erpInforService;

        public ItemsSearchService(IInforErpService erpInforService)
        {
            _erpInforService = erpInforService;
        }

        public async Task<SLItem[]> GetItems(string? filter, int rowCount, string bearerToken)
        {
            var tableName = "SLItems";
            var attributesList =
                "Item,Description,UM,CostType,UnitCost,UWsPrice,Revision,ProductCode,LotSize,AbcCode,CostMethod,DerQtyOnHand";
            var items = await _erpInforService
                .GetIdoTableDataAsync<SLItem>(tableName,
                    attributesList,
                    filter,
                    rowCount,
                    bearerToken);
            return items;
        }
    }
}

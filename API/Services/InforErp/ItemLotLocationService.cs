using AutoMapper;
using API.Common;
using API.Interfaces;
using API.Models.InforErp;

namespace API.Services.InforErp
{
    public class ItemLotLocationService : IItemLotLocationService
    {
        private readonly IInforErpService _erpInforService;
        private readonly IMapper _mapper;

        public ItemLotLocationService(IInforErpService erpInforService, IMapper mapper)
        {
            _mapper = mapper;
            _erpInforService = erpInforService;
        }

        public async Task<ItemLotLocation[]> GetItemLotLocations(string? filter, 
            int rowCount, 
            string bearerToken)
        {
            var tableName = "SLLotLocs";
            var attributesList =
                "Item,ItemDescription,LotRevision,QtyOnHand,ItemUM,UnitCost,Loc,LocationDescription,WBLotExpDate,Whse,WhseName,ItemSerialTracked,Lot,WBLotCreateDate";
            var items = await _erpInforService
                .GetIdoTableDataAsync<ItemLotLocation>(tableName,
                    attributesList,
                    filter,
                    rowCount,
                    bearerToken);
            return items;
        }

        public async Task<SerialNumberItem[]> GetSerialNumber(string? filter, int rowCount, string bearerToken)
        {
            var tableName = "SLSerials";
            var attributesList =
                "Item,Stat,SerNum,Loc,Lot,ExpDate,Whse";
            var items = await _erpInforService
                .GetIdoTableDataAsync<SerialNumberItem>(tableName,
                    attributesList,
                    filter,
                    rowCount,
                    bearerToken);
            return items;
        }

        /// Builds a flattened list of lot rows where serial-tracked lots
        /// are expanded into multiple rows grouped by expiry date.
        /// Each expiry group becomes a separate row with aggregated serial numbers.
        public ItemLotLocationSerialNumbers[] BuildLotSerialView(List<ItemLotLocation> lots,
            List<SerialNumberItem> serials)
        {
            var result = new List<ItemLotLocationSerialNumbers>();
            
            foreach (var item in lots)
            {
                // Base mapped lot row (used when serial tracking is disabled)
                var baseLotRow = _mapper.Map<ItemLotLocationSerialNumbers>(item);
                // If serial tracking is off, return the lot as-is
                if (String.IsNullOrEmpty(item.ItemSerialTracked) || item.ItemSerialTracked == "0")
                {
                    result.Add(baseLotRow);
                    continue;
                }

                // Group serial numbers for this lot by expiry date
                var serialGroupsByExpiry = GetSerialGroupsByExpiry(item, serials);

                foreach (var expiryDateItem in serialGroupsByExpiry)
                {
                    // Create a new row per expiry group
                    var lotRowForExpiry = _mapper.Map<ItemLotLocationSerialNumbers>(item);
                    var distinctSerialNumbers = expiryDateItem.SerialNumbers.ToList();
                    lotRowForExpiry.SerialNumbers = StringUtils.ToCommaSeparated(distinctSerialNumbers);
                    lotRowForExpiry.ExpiryDates = expiryDateItem.ExpiryDate;
                    lotRowForExpiry.SerialNbrExpiryDate = DateTime.TryParse(expiryDateItem.ExpiryDate, out var parsedDate)
                        ? parsedDate
                        : null;
                    result.Add(lotRowForExpiry);    
                }
            }

            return result.ToArray();
        }

        /// Filters serial records for a specific Item/Lot/Location and 
        /// groups the matching serial numbers by Expiry Date.
        /// Each group contains distinct serial numbers that share the same expiry.
        private List<SerialGroup> GetSerialGroupsByExpiry(
            ItemLotLocation itemLot,
            IEnumerable<SerialNumberItem> serials)
        {
            return serials
                .Where(x =>
                    x.Item == itemLot.Item &&
                    x.Lot == itemLot.Lot &&
                    x.Loc == itemLot.Loc &&
                    !string.IsNullOrWhiteSpace(x.SerNum))
                .GroupBy(x => StringUtils.FormatToShortDate(x.ExpDate))
                .Select(g => new SerialGroup
                {
                    ExpiryDate = g.Key,
                    SerialNumbers = g.Select(x => x.SerNum).Distinct().ToList()
                })
                .ToList();
        }
    }
}

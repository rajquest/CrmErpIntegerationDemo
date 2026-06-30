using Microsoft.AspNetCore.Mvc;
using API.Filters;
using API.Interfaces;
using API.Models.InforErp;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : Controller
    {
        private readonly IItemLotLocationService _itemLotLocationService;
        private readonly IInforErpService _erpService;

        public InventoryController(IItemLotLocationService itemLotLocationService, IInforErpService erpService)
        {
            _itemLotLocationService = itemLotLocationService;
            _erpService = erpService;
        }

        [HttpPost("GetItemLotLocation")]
        [RequiresBearerToken]
        public async Task<ActionResult<ItemLotLocation[]>> GetIdoItemLotLocationTableData([FromQuery] int rowCount,
            [FromQuery] string? filter,
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            [FromBody] string bearerToken)
        {
            var items = await _itemLotLocationService.GetItemLotLocations(filter, rowCount, bearerToken);
            return Ok(items);
        }

        [HttpPost("GetSerialNumbers")]
        [RequiresBearerToken]
        public async Task<ActionResult<SerialNumberItem[]>> GetIdoItemSerialNumbers([FromQuery] int rowCount,
            [FromQuery] string? filter,
            [FromBody] string bearerToken)
        {
            var items = await _itemLotLocationService.GetSerialNumber(filter, rowCount, bearerToken);
            return Ok(items);
        }

        [HttpPost("GetItemLotSerialNumbers")]
        [RequiresBearerToken]
        public async Task<ActionResult<ItemLotLocationSerialNumbers[]>> GetItemLotSerialNumbers([FromQuery] int rowCount,
            [FromQuery] string? filter,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] bool? hasExpiryDateConflict,
            [FromBody] string bearerToken)
        {
            var items = await _itemLotLocationService.GetItemLotLocations(filter, 0, bearerToken);
            var serialNumbers = await _itemLotLocationService.GetSerialNumber("", 0, bearerToken);

            var lotSerialNbrs = _itemLotLocationService.BuildLotSerialView(items.ToList(), serialNumbers.ToList());

            if (hasExpiryDateConflict != null && hasExpiryDateConflict.GetValueOrDefault())
            {
                return lotSerialNbrs
                    .Where(x => x.LotExpiryDate.HasValue && !string.IsNullOrWhiteSpace(x.ExpiryDates))
                    .ToArray();
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                var start = startDate.Value.Date;
                var end = endDate.Value.Date;

                lotSerialNbrs = lotSerialNbrs
                    .Where(x =>
                        (x.LotExpiryDate.HasValue &&
                         x.LotExpiryDate.Value.Date >= start &&
                         x.LotExpiryDate.Value.Date <= end)
                        ||
                        (x.SerialNbrExpiryDate.HasValue &&
                         x.SerialNbrExpiryDate.Value.Date >= start &&
                         x.SerialNbrExpiryDate.Value.Date <= end))
                    .ToArray();
            }

            return Ok(lotSerialNbrs);
        }

        [HttpPost("GetInventoryLots")]
        [RequiresBearerToken]
        public async Task<ActionResult<InventoryLot[]>> GetInventoryLots(
            [FromQuery] int rowCount,
            [FromQuery] string? filter,
            [FromBody] string bearerToken)
        {
            var items = await _erpService.GetIdoTableDataAsync<InventoryLot>(
                "SLLots",
                "Item,ItemDescription,Revision,ExpDate,DerQtyOnHand,DerExtUnitCost,ItemU_M,Lot",
                filter, rowCount, bearerToken);
            return Ok(items);
        }
    }
}

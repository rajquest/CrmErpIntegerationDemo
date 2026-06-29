using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.Models.InforErp;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : Controller
    {
        private readonly IItemLotLocationService _itemLotLocationService;
        
        public InventoryController(IItemLotLocationService itemLotLocationService)
        {
            _itemLotLocationService = itemLotLocationService;
        }

        [HttpPost("GetItemLotLocation")]
        public async Task<ActionResult<ItemLotLocation[]>> GetIdoItemLotLocationTableData([FromQuery] int rowCount,
            [FromQuery] string? filter,
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            [FromBody] string bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Missing Bearer token. Please authenticate first."
                });
            }

            var items = await _itemLotLocationService
                .GetItemLotLocations(filter, rowCount, bearerToken);
            return Ok(items);
        }

        [HttpPost("GetSerialNumbers")]
        public async Task<ActionResult<SerialNumberItem[]>> GetIdoItemSerialNumbers([FromQuery] int rowCount,
            [FromQuery] string? filter,
            [FromBody] string bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Missing Bearer token. Please authenticate first."
                });
            }


            var items = await _itemLotLocationService
                .GetSerialNumber(filter, rowCount, bearerToken);
            return Ok(items);
        }

        [HttpPost("GetItemLotSerialNumbers")]
        public async Task<ActionResult<ItemLotLocationSerialNumbers[]>> GetItemLotSerialNumbers([FromQuery] int rowCount,
            [FromQuery] string? filter,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] bool? hasExpiryDateConflict,
            [FromBody] string bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Missing Bearer token. Please authenticate first."
                });
            }

            var items = await _itemLotLocationService
                .GetItemLotLocations(filter, 0, bearerToken);

            // Get all serial numbers once to join avoiding multiple api requests
            var serialNumbers = await _itemLotLocationService
                .GetSerialNumber("", 0, bearerToken);

            Console.WriteLine("items fetched count" + items.Length);
            Console.WriteLine("serial nbrs fetched count" + serialNumbers.Length);

            var lotSerialNbrs = _itemLotLocationService
                .BuildLotSerialView(items.ToList(), serialNumbers.ToList());

            if (hasExpiryDateConflict != null && hasExpiryDateConflict.GetValueOrDefault())
            {
                return lotSerialNbrs
                    .Where(x =>
                        x.LotExpiryDate.HasValue &&
                        !string.IsNullOrWhiteSpace(x.ExpiryDates))
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
                         x.SerialNbrExpiryDate.Value.Date <= end)
                    )
                    .ToArray(); 
            }

            return Ok(lotSerialNbrs);
        }
    }
}

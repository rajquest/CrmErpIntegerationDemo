using Microsoft.AspNetCore.Mvc;
using API.Filters;
using API.Interfaces;
using API.Models.DropdownValues;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DropdownController : ControllerBase
    {
        private readonly IDropdownService _dropdownService;

        public DropdownController(IDropdownService dropdownService)
        {
            _dropdownService = dropdownService;
        }

        [HttpPost("GetItems")]
        [RequiresBearerToken]
        public async Task<ActionResult<Items[]>> GetItems([FromBody] string bearerToken)
        {
            var items = await _dropdownService.GetItemList(bearerToken);
            var results = items?.Select(x => new Items { Item = x.Item, Description = x.Description }).ToList();
            return Ok(results);
        }

        [HttpPost("GetWarehouses")]
        [RequiresBearerToken]
        public async Task<ActionResult<Warehouse[]>> GetWarehouses([FromBody] string bearerToken)
        {
            return Ok(await _dropdownService.GetWarehouse(bearerToken));
        }

        [HttpPost("GetLocations")]
        [RequiresBearerToken]
        public async Task<ActionResult<Location[]>> GetLocations([FromBody] string bearerToken)
        {
            return Ok(await _dropdownService.GetLocations(bearerToken));
        }

        [HttpPost("GetOrderNumbers")]
        [RequiresBearerToken]
        public async Task<ActionResult<Order[]>> GetOrderNumbers([FromBody] string bearerToken)
        {
            return Ok(await _dropdownService.GetCustomerOrderNumbers(bearerToken));
        }
    }
}

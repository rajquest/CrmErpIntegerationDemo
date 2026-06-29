using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.Models.DropdownValues;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LookupController: Controller
    {
        private readonly ILookupService lookupService;
        public LookupController(ILookupService lookupService)
        {
            this.lookupService = lookupService;
        }

        [HttpPost("GetItems")]
        public async Task<ActionResult<Items[]>> GetItems([FromBody] string bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Missing Bearer token. Please authenticate first."
                });
            }

            var items = await lookupService.GetItemList(bearerToken);
            
            var results = items
                .Select(x => new Items
                {
                    Item = x.Item,
                    Description = x.Description
                })
                .ToList();
            return Ok(results);
        }

        [HttpPost("GetWarehouses")]
        public async Task<ActionResult<Items[]>> GetWarehouses([FromBody] string bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Missing Bearer token. Please authenticate first."
                });
            }

            var items = await lookupService.GetWarehouse(bearerToken);

            return Ok(items);
        }

        [HttpPost("GetLocations")]
        public async Task<ActionResult<Items[]>> GetLocations([FromBody] string bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Missing Bearer token. Please authenticate first."
                });
            }

            var items = await lookupService.GetLocations(bearerToken);

            return Ok(items);
        }

        [HttpPost("GetOrderNumbers")]
        public async Task<ActionResult<Order[]>> GetOrderNumbers([FromBody] string bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Missing Bearer token. Please authenticate first."
                });
            }

            var items = await lookupService.GetCustomerOrderNumbers(bearerToken);

            return Ok(items);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using API.Filters;
using API.Interfaces;
using API.Models.InforErp;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InforExplorerController : ControllerBase
    {
        private readonly IInforErpService _erpService;
        private readonly IItemsSearchService _itemsSearchService;

        public InforExplorerController(IInforErpService erpService, IItemsSearchService itemsSearchService)
        {
            _erpService = erpService;
            _itemsSearchService = itemsSearchService;
        }

        [HttpPost("GetConfigurations")]
        public async Task<ActionResult<string[]>> GetConfigurations(string bearerToken)
        {
            var items = await _erpService.GetConfigurationList(bearerToken);
            return Ok(items);
        }

        [HttpPost("GetIdoTableSchema")]
        [RequiresBearerToken]
        public async Task<ActionResult<string[]>> GetIdoTableSchema(
            [FromQuery] string tableName,
            [FromBody] string bearerToken)
        {
            var items = await _erpService.GetTableAttributesListAsync(tableName, bearerToken);
            return Ok(items);
        }

        [HttpPost("GetSLItems")]
        [RequiresBearerToken]
        public async Task<ActionResult<SLItem[]>> GetSLItems(
            [FromQuery] int rowCount,
            [FromQuery] string? filter,
            [FromBody] string bearerToken)
        {
            var items = await _itemsSearchService.GetItems(filter, rowCount, bearerToken);
            return Ok(items);
        }
    }
}

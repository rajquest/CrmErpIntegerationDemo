using Microsoft.AspNetCore.Mvc;
using API.Filters;
using API.Interfaces;
using API.Models.InforErp;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaxController : ControllerBase
    {
        private readonly ITaxLookupService _taxLookupService;

        public TaxController(ITaxLookupService taxLookupService)
        {
            _taxLookupService = taxLookupService;
        }

        [HttpPost("GetTaxCodes")]
        [RequiresBearerToken]
        public async Task<ActionResult<TaxCodeItem[]>> GetTaxCodes(
            [FromQuery] int rowCount,
            [FromQuery] string? filter,
            [FromBody] string bearerToken)
        {
            var items = await _taxLookupService.GetTaxCodesAsync(filter, rowCount, bearerToken);
            return Ok(items);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.Models.InforErp;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobVarianceController:Controller
    {
        private readonly IJobVarianceService _jobVarianceService;

        public JobVarianceController(IJobVarianceService jobVarianceService)
        {
            _jobVarianceService = jobVarianceService;
        }

        [HttpPost("GetCostVariance")]
        public async Task<ActionResult<JobVariance[]>> GetCostVariance([FromQuery] int rowCount,
            [FromQuery] string? filter, // <-- OPTIONAL filter string
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

            var items = await _jobVarianceService.ConstructJobVarianceReport(filter, rowCount, bearerToken);

            var filteredItems = rowCount>0 ? items.Take(rowCount): Array.Empty<JobVariance>(); 
            return Ok(filteredItems);
        }

    }
}

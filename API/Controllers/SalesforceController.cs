using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using API.Interfaces;
using API.Models.Salesforce;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesforceController : ControllerBase
    {
        private readonly ILogger<SalesforceController> _logger;
        private readonly ITokenService _tokenService;
        private readonly ISalesforceService _salesforceService;
        public SalesforceController(ILogger<SalesforceController> logger,
            ITokenService tokenService,
            ISalesforceService salesforceService)
        {
            _logger = logger;
            _tokenService = tokenService;
            _salesforceService = salesforceService;
        }

        [HttpGet("GetToken")]
        public async Task<ActionResult<string>> GetOAuthToken()
        {
            var token = await _tokenService.GetSalesforceOAuthTokenAsync();
            return Ok(token);
        }

        [HttpPost("GetAllsObjects")]
        public async Task<ActionResult<string[]>> GetsObjects([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Missing token");
            
            var sObjects = await _salesforceService.GetAllSObjectsAsync(token);
            return Ok(sObjects);
        }

        [HttpPost("GetTableColumnNames/{sObjectName}")]
        public async Task<ActionResult<List<SObjectFieldInfo>>> GetTableColumnNames(string sObjectName, 
            [FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Missing token");

            var sObjects = await _salesforceService.GetTableColumnNames(token, sObjectName);
            return Ok(sObjects);
        }

        [HttpPost("CareProgramEnrolleeProduct/{rowCount}")]
        public async Task<ActionResult<CareProgramEnrolleeProduct[]>> GetCareProgramEnrolleeProduct(int rowCount,
            [FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Missing token");
            var sObjects = await _salesforceService.GetCareProgramEnrolleeProductRowsAsync(token, rowCount);
            return Ok(sObjects);
        }

        [HttpPost("Accounts/{rowCount}")]
        public async Task<ActionResult<Account[]>> GetAccounts(int rowCount,
            [FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Missing token");
            var sObjects = await _salesforceService.GetAccountRowsAsync(token, rowCount);
            return Ok(sObjects);
        }
    }
}

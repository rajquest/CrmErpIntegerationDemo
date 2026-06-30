using Microsoft.AspNetCore.Mvc;
using API.Filters;
using API.Interfaces;
using API.Models.Salesforce;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesforceController : ControllerBase
    {
        private readonly ILogger<SalesforceController> _logger;
        private readonly ISalesforceTokenService _tokenService;
        private readonly ISalesforceService _salesforceService;

        public SalesforceController(ILogger<SalesforceController> logger,
            ISalesforceTokenService tokenService,
            ISalesforceService salesforceService)
        {
            _logger = logger;
            _tokenService = tokenService;
            _salesforceService = salesforceService;
        }

        [HttpGet("GetToken")]
        public async Task<ActionResult<string>> GetOAuthToken()
        {
            var token = await _tokenService.GetOAuthTokenAsync();
            return Ok(token);
        }

        [HttpPost("GetAllsObjects")]
        [RequiresBearerToken]
        public async Task<ActionResult<string[]>> GetsObjects([FromBody] string bearerToken)
        {
            var sObjects = await _salesforceService.GetAllSObjectsAsync(bearerToken);
            return Ok(sObjects);
        }

        [HttpPost("GetTableColumnNames/{sObjectName}")]
        [RequiresBearerToken]
        public async Task<ActionResult<List<SObjectFieldInfo>>> GetTableColumnNames(string sObjectName,
            [FromBody] string bearerToken)
        {
            var sObjects = await _salesforceService.GetTableColumnNames(bearerToken, sObjectName);
            return Ok(sObjects);
        }

        [HttpPost("CareProgramEnrolleeProduct/{rowCount}")]
        [RequiresBearerToken]
        public async Task<ActionResult<CareProgramEnrolleeProduct[]>> GetCareProgramEnrolleeProduct(int rowCount,
            [FromBody] string bearerToken)
        {
            var sObjects = await _salesforceService.GetCareProgramEnrolleeProductRowsAsync(bearerToken, rowCount);
            return Ok(sObjects);
        }

        [HttpPost("Accounts/{rowCount}")]
        [RequiresBearerToken]
        public async Task<ActionResult<Account[]>> GetAccounts(int rowCount,
            [FromBody] string bearerToken)
        {
            var sObjects = await _salesforceService.GetAccountRowsAsync(bearerToken, rowCount);
            return Ok(sObjects);
        }
    }
}

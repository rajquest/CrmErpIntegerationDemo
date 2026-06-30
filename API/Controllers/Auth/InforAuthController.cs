using Microsoft.AspNetCore.Mvc;
using API.Filters;
using API.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InforAuthController : ControllerBase
    {
        private readonly IInforTokenService _tokenService;

        public InforAuthController(IInforTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpGet("GetToken")]
        public async Task<ActionResult<string>> GetToken()
        {
            var token = await _tokenService.GetClientCredentialTokenAsync();
            return Ok(token);
        }

        [HttpPost("GetApiToken")]
        [RequiresBearerToken]
        public async Task<ActionResult<string>> GetApiToken([FromBody] string bearerToken)
        {
            var token = await _tokenService.GetErpApiTokenAsync(bearerToken);
            return Ok(token);
        }
    }
}

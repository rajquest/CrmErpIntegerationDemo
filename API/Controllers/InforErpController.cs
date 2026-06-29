using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.Models.InforErp;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InforErpController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly IInforErpService _erpInforService;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IItemsSearchService _itemsSearchService;
        private readonly ILookupService _lookupService;
        public InforErpController(ITokenService tokenService,
            IInforErpService erpInforService, 
            ICustomerOrderService customerOrderService,
            IItemsSearchService itemsSearchService,
            ILookupService lookupService)
        {
            _tokenService = tokenService;
            _erpInforService = erpInforService;
            _customerOrderService = customerOrderService;
            _itemsSearchService = itemsSearchService;
            _lookupService = lookupService;
        }

        [HttpGet("GetToken")]
        public async Task<ActionResult<string>> GetOAuthToken()
        {
            //var token = await _tokenService.GetInforAccessTokenAsync();
            var token = await _tokenService.GetInforAccessTokenClientCredentialAsync();
            return Ok(token);
        }

        [HttpPost("GetApiToken")]
        public async Task<ActionResult<string>> GetInforErpApiAccessTokenAsync([FromBody] string bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Missing Bearer token. Please authenticate first."
                });
            }
            var token = await _tokenService.GetInforErpApiAccessTokenAsync(bearerToken);
            return Ok(token);
        }

        [HttpPost("GetConfigurations")]
        public async Task<ActionResult<string>> GetConfigurations(string bearerToken)
        {
            var items = await _erpInforService.GetConfigurationList(bearerToken);
            return Ok(items);
        }

        [HttpPost("GetIdoTableSchema")]
        public async Task<ActionResult<string[]>> GetIdoTableSchema([FromQuery] string tableName,
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
            var items = await _erpInforService.GetTableAttributesListAsync(tableName, bearerToken);
            return Ok(items);
        }

        [HttpPost("GetSLItems")]
        public async Task<ActionResult<SLItem[]>> GetSLItems([FromQuery] int rowCount,
            [FromQuery] string? filter,   // <-- OPTIONAL filter string
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

            var items = await _itemsSearchService.GetItems(filter, rowCount, bearerToken);
            return Ok(items);
        }

        [HttpPost("GetCustomers")]
        public async Task<ActionResult<Customer[]>> GetCustomers([FromQuery] int rowCount,
            [FromQuery] string? filter,   // <-- OPTIONAL filter string
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

            var items = _lookupService.GetCustomerOrderNumbers(bearerToken);
            
            return Ok(items);
        }

        [HttpPost("GetCustomerOrders")]
        public async Task<ActionResult<CustomerOrderItem[]>> GetCustomerOrders([FromQuery] int rowCount,
            [FromQuery] string? filter,   // <-- OPTIONAL filter string
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
            
            var items = await _customerOrderService.GetCustomerOrders(rowCount,
                    filter,
                    bearerToken);

            return Ok(items);
        }

        [HttpPost("GetTaxCodes")]
        public async Task<ActionResult<TaxCodeItem[]>> TaxCodes([FromQuery] int rowCount,
            [FromQuery] string? filter,   // <-- OPTIONAL filter string
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

            var tableName = "SLTaxcodes";
            var attributesList =
                "TaxCodeType,TaxCode,TaxRate,TaxJur,TaxJurDescription,NxtLvlCode,NxtLvlDescription";
            var items = await _erpInforService
                .GetIdoTableDataAsync<TaxCodeItem>(tableName,
                    attributesList,
                    filter,
                    rowCount,
                    bearerToken);

            return Ok(items);
        }

        [HttpPost("GetInventoryLots")]
        public async Task<ActionResult<InventoryLot[]>> GetInventoryLotTableData([FromQuery] int rowCount,
            [FromQuery] string? filter,   // <-- OPTIONAL filter string
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

            var tableName = "SLLots";
            var attributesList =
                "Item,ItemDescription,Revision,ExpDate,DerQtyOnHand,DerExtUnitCost,ItemU_M,Lot";
            var items = await _erpInforService
                .GetIdoTableDataAsync<InventoryLot>(tableName,
                    attributesList,
                    filter,
                    rowCount,
                    bearerToken);
            return Ok(items);
        }
    }
}
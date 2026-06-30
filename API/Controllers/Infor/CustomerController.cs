using Microsoft.AspNetCore.Mvc;
using API.Filters;
using API.Interfaces;
using API.Models.InforErp;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;

        public CustomerController(ICustomerService customerService, IOrderService orderService)
        {
            _customerService = customerService;
            _orderService = orderService;
        }

        [HttpPost("GetCustomers")]
        [RequiresBearerToken]
        public async Task<ActionResult<Customer[]>> GetCustomers([FromBody] string bearerToken)
        {
            var items = await _customerService.GetCustomers(bearerToken);
            return Ok(items);
        }

        [HttpPost("GetCustomerOrders")]
        [RequiresBearerToken]
        public async Task<ActionResult<CustomerOrderItem[]>> GetCustomerOrders(
            [FromQuery] int rowCount,
            [FromQuery] string? filter,
            [FromBody] string bearerToken)
        {
            var items = await _orderService.GetCustomerOrders(rowCount, filter, bearerToken);
            return Ok(items);
        }
    }
}

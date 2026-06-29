using Microsoft.AspNetCore.Mvc;
using API.Common;
using API.Interfaces;
using API.Models.InforErp;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceOrderController: Controller
    {
        private readonly ICustomerOrderService _customerOrderService;

        public InvoiceOrderController(ICustomerOrderService customerOrderService)
        {
            _customerOrderService = customerOrderService;
        }

        [HttpPost("InvoicedOrders")]
        public async Task<ActionResult<InvoicedOrder[]>> InvoicedOrders([FromQuery] string filter, 
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

            var items = await _customerOrderService
                .GetInvoicedOrders(filter, bearerToken);

            return Ok(items);
        }

        [HttpPost("material-transactions")]
        public async Task<ActionResult<MaterialTransactionSummary[]>> GetMaterialTransactions([FromQuery] string filter,
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

            var transactions = await _customerOrderService
                .GetMaterialTransaction(filter, bearerToken);

            bool spansMultipleMonths = transactions
                .Where(t => t.TransactionDate.HasValue)
                .Select(t => new { t.TransactionDate.Value.Year, t.TransactionDate.Value.Month })
                .Distinct()
                .Count() > 1;

            IEnumerable<MaterialTransactionSummary> grouped;

            if (spansMultipleMonths)
            {
                grouped = transactions
                    .Where(t => t.TransactionDate.HasValue)
                    .GroupBy(t => new { t.Item, t.TransactionDate.Value.Year, t.TransactionDate.Value.Month })
                    .Select(g => new MaterialTransactionSummary
                    {
                        Item = g.Key.Item!,
                        MonthYear = $"{StringUtils.GetMonthName(g.Key.Month)} {g.Key.Year}",
                        ItemDescription = g.Select(x => x.ItemDescription)
                            .FirstOrDefault(d => !string.IsNullOrWhiteSpace(d)),
                        TotalQty = g.Sum(x => StringUtils.ToDecimal(x.Qty))
                    });
            }
            else
            {
                grouped = transactions
                    .GroupBy(t => t.Item)
                    .Select(g => new MaterialTransactionSummary
                    {
                        Item = g.Key!,
                        ItemDescription = g.Select(x => x.ItemDescription)
                            .FirstOrDefault(d => !string.IsNullOrWhiteSpace(d)),
                        TotalQty = g.Sum(x => StringUtils.ToDecimal(x.Qty))
                    });
            }

            return Ok(grouped.ToArray());
        }
    }
}

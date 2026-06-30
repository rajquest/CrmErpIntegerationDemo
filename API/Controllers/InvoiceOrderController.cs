using Microsoft.AspNetCore.Mvc;
using API.Common;
using API.Filters;
using API.Interfaces;
using API.Models.InforErp;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceOrderController : Controller
    {
        private readonly IInvoiceEnrichmentService _invoiceEnrichmentService;
        private readonly IOrderService _orderService;

        public InvoiceOrderController(IInvoiceEnrichmentService invoiceEnrichmentService, IOrderService orderService)
        {
            _invoiceEnrichmentService = invoiceEnrichmentService;
            _orderService = orderService;
        }

        [HttpPost("InvoicedOrders")]
        [RequiresBearerToken]
        public async Task<ActionResult<InvoicedOrder[]>> InvoicedOrders([FromQuery] string filter,
            [FromBody] string bearerToken)
        {
            var items = await _invoiceEnrichmentService.GetInvoicedOrders(filter, bearerToken);
            return Ok(items);
        }

        [HttpPost("material-transactions")]
        [RequiresBearerToken]
        public async Task<ActionResult<MaterialTransactionSummary[]>> GetMaterialTransactions([FromQuery] string filter,
            [FromBody] string bearerToken)
        {
            var transactions = await _orderService.GetMaterialTransaction(filter, bearerToken);

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

using AutoMapper;
using API.Common;
using API.Interfaces;
using API.Models.InforErp;

namespace API.Services.Infor
{
    public class InvoiceEnrichmentService : IInvoiceEnrichmentService
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly ITaxLookupService _taxLookupService;
        private readonly IMapper _mapper;

        public InvoiceEnrichmentService(
            IOrderService orderService,
            ICustomerService customerService,
            ITaxLookupService taxLookupService,
            IMapper mapper)
        {
            _orderService = orderService;
            _customerService = customerService;
            _taxLookupService = taxLookupService;
            _mapper = mapper;
        }

        public async Task<InvoicedOrder[]> GetInvoicedOrders(string filter, string bearerToken)
        {
            var invoiceOrders = new List<InvoicedOrder>();
            var invoices = await _orderService.GetInvoiceListing(filter, bearerToken);
            var allTaxCodes = await _taxLookupService.GetAllTaxCodes(bearerToken);

            foreach (var invoice in invoices)
            {
                var invoiceOrder = _mapper.Map<InvoicedOrder>(invoice);
                var orderFilter = $"CoNum='{invoiceOrder.DerCoNum}' and CoLine='1'";
                var orders = await _orderService.GetCustomerOrders(0, orderFilter, bearerToken);

                if (orders.Length > 0)
                {
                    var order = orders.First();
                    invoiceOrder.DerCustNum  = order.DerCustNum;
                    invoiceOrder.Stat        = order.Stat;
                    invoiceOrder.CoOrderDate = order.CoOrderDate;
                    invoiceOrder.ShipDate    = order.ShipDate;

                    invoiceOrder.CustomerName = await _customerService.GetCustomerName(order.DerCustNum, bearerToken);
                    var shipTo = await _customerService.GetCustomerShipTo(order.DerCustNum, null, bearerToken);
                    invoiceOrder.ShipToState   = shipTo?.State;
                    invoiceOrder.ShipToTaxCode = shipTo?.TaxCode1;

                    if (string.Equals(shipTo?.TaxCode1, "NT", StringComparison.OrdinalIgnoreCase))
                    {
                        invoiceOrder.Notes       = "EXEMPT";
                        invoiceOrder.ExemptAmount = invoiceOrder.InvhdrPrice;
                    }
                    else
                    {
                        var orderSalesTax = await _orderService.GetSalesTaxForOrder(order.CoNum, bearerToken);
                        invoiceOrder.TaxRateList      = LoadTaxJurisdictionDetails(invoiceOrder.ShipToTaxCode, allTaxCodes);
                        invoiceOrder.TotalSalesTaxRate = SalesTaxUtils.CalculateTotalTaxRate(invoiceOrder.TaxRateList);
                        invoiceOrder.Notes        = $"TAX - {shipTo?.State} {shipTo?.City}";
                        invoiceOrder.SalesTax     = orderSalesTax?.SalesTaxT;
                        invoiceOrder.TaxableAmount = invoice.DerExtendedPrice;
                    }
                }

                invoiceOrders.Add(invoiceOrder);
            }

            var addon = await GetNoOrderInvoicedOrders(filter, bearerToken);
            if (addon?.Any() == true)
                invoiceOrders.AddRange(addon);

            return invoiceOrders.ToArray();
        }

        public async Task<List<InvoicedOrder>> GetNoOrderInvoicedOrders(string filter, string bearerToken)
        {
            filter = filter.Replace("Item like 'CRDLA%' AND ", "");
            var invoiceOrders = new List<InvoicedOrder>();
            var invoices = await _orderService.GetInvoiceListing(filter, bearerToken);

            foreach (var invoice in invoices)
            {
                if (!string.IsNullOrWhiteSpace(invoice.DerCoNum)) continue;

                var invoiceOrder = _mapper.Map<InvoicedOrder>(invoice);
                invoiceOrder.DerCustNum   = invoice.InvhdrCustNum;
                invoiceOrder.CustomerName = invoice.CustaddrName;

                var shipTo = await _customerService.GetCustomerShipTo(invoice.InvhdrCustNum, null, bearerToken);
                invoiceOrder.ShipToState   = shipTo?.State;
                invoiceOrder.ShipToTaxCode = shipTo?.TaxCode1;

                if (shipTo?.TaxCode1?.Equals("NT", StringComparison.OrdinalIgnoreCase) == true)
                {
                    invoiceOrder.Notes        = "EXEMPT";
                    invoiceOrder.ExemptAmount = invoiceOrder.InvhdrPrice;
                }
                else
                {
                    invoiceOrder.TaxableAmount = invoiceOrder.InvhdrPrice;
                }

                invoiceOrders.Add(invoiceOrder);
            }

            return invoiceOrders;
        }

        private TaxJurisdictionDetails[] LoadTaxJurisdictionDetails(string taxCode, TaxCodeItem[] allTaxCodes)
        {
            var results = new List<TaxJurisdictionDetails>();
            AddTaxLevel(taxCode, allTaxCodes, results);
            return results.ToArray();
        }

        private void AddTaxLevel(string taxCode, TaxCodeItem[] allTaxCodes, List<TaxJurisdictionDetails> results)
        {
            if (string.IsNullOrWhiteSpace(taxCode) || allTaxCodes == null) return;
            var match = allTaxCodes.FirstOrDefault(x => x.TaxCode == taxCode);
            if (match == null) return;

            results.Add(new TaxJurisdictionDetails
            {
                TaxCode = match.TaxCode, TaxJur = match.TaxJur,
                Description = match.Description, TaxRate = match.TaxRate
            });

            if (!string.IsNullOrWhiteSpace(match.NxtLvlCode))
                AddTaxLevel(match.NxtLvlCode, allTaxCodes, results);
        }
    }
}

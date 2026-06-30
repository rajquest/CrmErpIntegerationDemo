using API.Interfaces;
using API.Models.InforErp;

namespace API.Services.Infor
{
    public class OrderService : IOrderService
    {
        private readonly IInforErpService _erpService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IInforErpService erpService, ILogger<OrderService> logger)
        {
            _erpService = erpService;
            _logger = logger;
        }

        public Task<CustomerOrderItem[]> GetCustomerOrders(int rowCount, string? filterCriteria, string bearerToken)
            => _erpService.GetIdoTableDataAsync<CustomerOrderItem>(
                "SLCoitems",
                "CoNum,DerCustNum,CoType,DerCustPo,CoOrderDate,QtyOrdered,Stat,ShipDate,CustSeq,CoPrice,Item,CoLine",
                filterCriteria, rowCount, bearerToken);

        public Task<InvoiceListingItem[]> GetInvoiceListing(string filterCriteria, string bearerToken)
            => _erpService.GetIdoTableDataAsync<InvoiceListingItem>(
                "SLInvitemalls",
                "CoLine,DerInvNum,DerCoNum,InvhdrCustNum,CustaddrName,CoNum,DerCustPo,Item,DerDescription,QtyInvoiced,InvhdrMiscCharges,InvhdrPrepaidAmt,InvhdrFreight,InvhdrPrice,InvhdrInvDate,DerExtendedPrice,Price",
                filterCriteria, 0, bearerToken);

        public Task<InvoiceListingItem[]> GetInvoiceListingForOrder(string orderNumber, string bearerToken)
            => _erpService.GetIdoTableDataAsync<InvoiceListingItem>(
                "SLInvitemalls",
                "DerInvNum,DerCoNum,CoNum,Item,DerDescription,QtyInvoiced,InvhdrMiscCharges,InvhdrPrepaidAmt,InvhdrFreight,InvhdrPrice,InvhdrInvDate",
                $"CoNum='{orderNumber}'", 0, bearerToken);

        public Task<MaterialTransactionItem[]> GetMaterialTransaction(string filter, string bearerToken)
            => _erpService.GetIdoTableDataAsync<MaterialTransactionItem>(
                "SLMatlTrans", "TransDate,TransType,RefType,Item,ItemDescription,Lot,Qty",
                filter, 0, bearerToken);

        public async Task<SalesTaxItem?> GetSalesTaxForOrder(string orderNumber, string bearerToken)
        {
            try
            {
                var items = await _erpService.GetIdoTableDataAsync<SalesTaxItem>(
                    "SLCos", "CoNum,SalesTaxT", $"CoNum='{orderNumber}'", 0, bearerToken);
                return items?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch sales tax for order {OrderNumber}", orderNumber);
                return null;
            }
        }
    }
}

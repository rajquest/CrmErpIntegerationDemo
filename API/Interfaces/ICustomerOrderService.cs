using API.Models.InforErp;

namespace API.Interfaces
{
    public interface ICustomerOrderService
    {
        Task<CustomerOrderItem[]> GetCustomerOrders(int rowCount,
            string filterCriteria,
            string bearerToken);
        Task<InvoiceListingItem[]> GetInvoiceListingForOrder(string orderNumber, string bearerToken);
        Task<InvoiceListingItem[]> GetInvoiceListing(string filterCriteria, string bearerToken);
        Task<CustomerShipToRecord?> GetCustomerShipTo(string customerNumber, string? customerSeq, string bearerToken);
        Task<InvoicedOrder[]> GetInvoicedOrders(string filter, string bearerToken);
        Task<SalesTaxItem?> GetSalesTaxForOrder(string orderNumber, string bearerToken);
        Task<MaterialTransactionItem[]> GetMaterialTransaction(string filter, string bearerToken);
    }
}

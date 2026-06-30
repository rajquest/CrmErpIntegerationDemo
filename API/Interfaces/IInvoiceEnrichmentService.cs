using API.Models.InforErp;

namespace API.Interfaces
{
    public interface IInvoiceEnrichmentService
    {
        Task<InvoicedOrder[]> GetInvoicedOrders(string filter, string bearerToken);
        Task<List<InvoicedOrder>> GetNoOrderInvoicedOrders(string filter, string bearerToken);
    }
}

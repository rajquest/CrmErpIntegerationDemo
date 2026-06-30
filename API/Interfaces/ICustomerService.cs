using API.Models.InforErp;

namespace API.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer[]> GetCustomers(string bearerToken);
        Task<string> GetCustomerName(string customerNumber, string bearerToken);
        Task<CustomerShipToRecord?> GetCustomerShipTo(string customerNumber, string? customerSeq, string bearerToken);
    }
}

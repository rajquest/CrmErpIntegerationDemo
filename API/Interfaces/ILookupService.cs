using API.Models.DropdownValues;
using API.Models.InforErp;

namespace API.Interfaces
{
    public interface ILookupService
    {
        Task<SLItem[]?> GetItemList(string bearerToken);
        Task<Warehouse[]?> GetWarehouse(string bearerToken);
        Task<Location[]?> GetLocations(string bearerToken);
        Task<Order[]> GetCustomerOrderNumbers(string bearerToken);
        Task<string> GetCustomerName(string customerNumber, string bearerToken);
        Task<Customer[]> GetCustomers(string bearerToken);
    }
}

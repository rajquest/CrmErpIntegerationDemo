using API.Models.DropdownValues;
using API.Models.InforErp;

namespace API.Interfaces
{
    public interface IDropdownService
    {
        Task<SLItem[]?> GetItemList(string bearerToken);
        Task<Warehouse[]?> GetWarehouse(string bearerToken);
        Task<Location[]?> GetLocations(string bearerToken);
        Task<Order[]> GetCustomerOrderNumbers(string bearerToken);
    }
}

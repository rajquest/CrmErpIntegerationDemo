using API.Models.InforErp;

namespace API.Interfaces
{
    public interface IItemLotLocationService
    {
        ItemLotLocationSerialNumbers[] BuildLotSerialView(List<ItemLotLocation> lots,
            List<SerialNumberItem> serials);

        Task<ItemLotLocation[]> GetItemLotLocations(string? filter,
            int rowCount, string bearerToken);
        Task<SerialNumberItem[]> GetSerialNumber(string? filter, int rowCount, string bearerToken);
    }
}

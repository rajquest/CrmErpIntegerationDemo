using API.Models.InforErp;

namespace API.Interfaces
{
    public interface IItemsSearchService
    {
        Task<SLItem[]> GetItems(string? filter, int rowCount, string bearerToken);
    }
}

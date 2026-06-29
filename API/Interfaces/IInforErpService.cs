using API.Models.InforErp;

namespace API.Interfaces
{
    public interface IInforErpService
    {
        Task<string[]> GetConfigurationList(string bearerToken);
        Task<string[]> GetTableAttributesListAsync(string tableName, string bearerToken);
        
        Task<TItem[]> GetIdoTableDataAsync<TItem>(
            string idoName,
            string properties,
            string? filters,
            int rowCount,
            string bearerToken);
    }
}

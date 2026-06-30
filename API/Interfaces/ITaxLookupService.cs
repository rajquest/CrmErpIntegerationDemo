using API.Models.InforErp;

namespace API.Interfaces
{
    public interface ITaxLookupService
    {
        Task<TaxCodeItem[]> GetAllTaxCodes(string bearerToken);
        Task<TaxCodeItem[]> GetTaxCodesAsync(string? filter, int rowCount, string bearerToken);
    }
}

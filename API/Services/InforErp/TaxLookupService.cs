using API.Interfaces;
using API.Models.InforErp;

namespace API.Services.InforErp
{
    public class TaxLookupService : ITaxLookupService
    {
        private readonly IInforErpService _erpInforService;
        private readonly ILogger<TaxLookupService> _logger;

        public TaxLookupService(IInforErpService erpInforService, ILogger<TaxLookupService> logger)
        {
            _erpInforService = erpInforService;
            _logger = logger;
        }

        public async Task<TaxCodeItem[]> GetAllTaxCodes(string bearerToken)
        {
            try
            {
                return await _erpInforService.GetIdoTableDataAsync<TaxCodeItem>(
                    "SLTaxcodes",
                    "TaxCodeType,TaxCode,TaxRate,TaxJur,Description,NxtLvlCode,NxtLvlDescription",
                    null, 0, bearerToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch all tax codes");
                return Array.Empty<TaxCodeItem>();
            }
        }

        public async Task<TaxCodeItem[]> GetTaxCodesAsync(string? filter, int rowCount, string bearerToken)
        {
            try
            {
                return await _erpInforService.GetIdoTableDataAsync<TaxCodeItem>(
                    "SLTaxcodes",
                    "TaxCodeType,TaxCode,TaxRate,TaxJur,TaxJurDescription,NxtLvlCode,NxtLvlDescription",
                    filter, rowCount, bearerToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch tax codes");
                return Array.Empty<TaxCodeItem>();
            }
        }

        private TaxCodeItem? GetTaxCodeFromCache(string searchValue, TaxCodeItem[] allTaxCodes)
        {
            if (string.IsNullOrWhiteSpace(searchValue) || allTaxCodes == null)
                return null;

            return allTaxCodes
                .FirstOrDefault(t => string.Equals(t.TaxCode, searchValue, StringComparison.OrdinalIgnoreCase));
        }
    }
}

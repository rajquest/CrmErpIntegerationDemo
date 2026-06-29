using API.Interfaces;
using API.Models.InforErp;

namespace API.Services.InforErp
{
    public class TaxLookupService :ITaxLookupService
    {
        private readonly IInforErpService _erpInforService;
               
        public TaxLookupService(IInforErpService erpInforService)
        {
            _erpInforService = erpInforService;        
        }

        private async Task<TaxCodeItem?> GetTaxCode(string searchValue, string bearerToken)
        {
            try
            {
                var filterCriteria = $"TaxCode='{searchValue}'";

                var tableName = "SLTaxcodes";
                var attributesList = "TaxCodeType,TaxCode,TaxRate,TaxJur,TaxJurDescription,NxtLvlCode,NxtLvlDescription";
                var items = await _erpInforService
                    .GetIdoTableDataAsync<TaxCodeItem>(tableName,
                        attributesList,
                        filterCriteria,
                        0,
                        bearerToken);

                return items?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return null;
            }
        }

        private TaxCodeItem? GetTaxCodeFromCache(string searchValue, TaxCodeItem[] allTaxCodes)
        {
            if (string.IsNullOrWhiteSpace(searchValue) || allTaxCodes == null)
                return null;

            return allTaxCodes
                .FirstOrDefault(t =>
                    string.Equals(t.TaxCode, searchValue, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<TaxCodeItem[]> GetAllTaxCodes(string bearerToken)
        {
            try
            {
                var tableName = "SLTaxcodes";
                var attributesList = "TaxCodeType,TaxCode,TaxRate,TaxJur,Description,NxtLvlCode,NxtLvlDescription";
                var items = await _erpInforService
                    .GetIdoTableDataAsync<TaxCodeItem>(tableName,
                        attributesList,
                        null,
                        0,
                        bearerToken);

                return items;
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return null;
            }
        }
    }
}

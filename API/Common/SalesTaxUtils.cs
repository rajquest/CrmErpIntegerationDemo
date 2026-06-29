using API.Models.InforErp;

namespace API.Common
{
    public static class SalesTaxUtils
    {
        public static decimal CalculatePercentageAmount(decimal? baseAmount, decimal? percentage)
        {
            var amount = baseAmount ?? 0m;
            var percent = percentage ?? 0m;

            return (amount * percent) / 100m;
        }

        public static decimal CalculateTotalTaxRate(TaxJurisdictionDetails[] details)
        {
            if (details == null || details.Length == 0)
                return 0m;

            return details
                .Where(d => !string.IsNullOrWhiteSpace(d.TaxRate))
                .Sum(d => decimal.TryParse(d.TaxRate, out var rate) ? rate : 0m);
        }
    }
}

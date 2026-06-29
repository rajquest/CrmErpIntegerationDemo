using System.Globalization;

namespace API.Common
{
    public static class StringUtils
    {
        public static string ToCommaSeparated(IEnumerable<string> values)
        {
            if (values == null)
                return string.Empty;

            return string.Join(", ",
                values.Where(v => !string.IsNullOrWhiteSpace(v)));
        }

        public static string FormatToShortDate(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            if (DateTime.TryParseExact(
                    input,
                    "yyyyMMdd HH:mm:ss.fff",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
            {
                return date.ToString("MM/dd/yyyy");
            }

            return string.Empty;
        }
        public static string GetMonthName(int month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");

            return CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month);
        }
        public static DateTime? ParseToDateTime(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (DateTime.TryParseExact(
                    input,
                    "yyyyMMdd HH:mm:ss.fff",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
            {
                return date;
            }

            return null;
        }

        public static decimal ToDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0m;

            return decimal.TryParse(
                value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var result)
                ? result
                : 0m;
        }

    }
}

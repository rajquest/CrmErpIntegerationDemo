using API.Interfaces;
using System.Text.Json;
using API.Common;
using API.Models.InforErp;

namespace API.Services.InforErp
{
    public class InforErpService : IInforErpService
    {
        private readonly IAppSettingsManager _appSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseApiUrl;

        public InforErpService(IAppSettingsManager appSettings, IHttpClientFactory httpClientFactory)
        {
            _baseApiUrl = $"{appSettings.InforErp.ApiBaseUrl}{appSettings.InforErp.IdoBasePath}";
            _appSettings = appSettings;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string[]> GetConfigurationList(string baseApiToken)
        {
            var url = $"{_baseApiUrl}/configurations";
            using var request = InforErpHelper.CreateGetRequest(url, baseApiToken, _appSettings.InforErp.X_Infor_MongooseConfig);

            var client = _httpClientFactory.CreateClient("InforApi");
            using var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API call failed: {response.StatusCode} - {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            return InforErpHelper.getConfigurationsList(json);
        }

        public async Task<string[]> GetTableAttributesListAsync(string tableName, string bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
                throw new ArgumentException("Bearer token cannot be null or empty.", nameof(bearerToken));

            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));

            var url = $"{_baseApiUrl}/info/{tableName}";
            using var request = InforErpHelper.CreateGetRequest(url, bearerToken, _appSettings.InforErp.X_Infor_MongooseConfig);

            var client = _httpClientFactory.CreateClient("InforApi");
            using var response = await client.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to fetch table attributes from Infor ERP: {response.StatusCode} - {json}");

            return InforErpHelper.GetTableColumnNames(json);
        }

        public async Task<TItem[]> GetIdoTableDataAsync<TItem>(string idoName,
            string properties,
            string? filters,
            int rowCount,
            string bearerToken)
        {
            var url = $"{_baseApiUrl}/load/{idoName}?properties={properties}&recordCap={rowCount}";

            if (!string.IsNullOrWhiteSpace(filters))
                url += $"&filter={Uri.EscapeDataString(filters)}";

            using var request = InforErpHelper.CreateGetRequest(url, bearerToken, _appSettings.InforErp.X_Infor_MongooseConfig);

            var client = _httpClientFactory.CreateClient("InforApi");
            using var response = await client.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to fetch data: {response.StatusCode} - {json}");

            var wrapper = JsonSerializer.Deserialize<IdoResponse<TItem>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return wrapper?.Items?.ToArray() ?? Array.Empty<TItem>();
        }
    }
}

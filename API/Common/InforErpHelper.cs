using API.Models.InforErp;
using System.Net.Http.Headers;
using System.Text.Json;

namespace API.Common
{
    public static class InforErpHelper
    {
        public static HttpRequestMessage CreateGetRequest(string url,
            string bearerToken,
            string mongooseConfig)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(mongooseConfig))
            {
                request.Headers.Add("X-Infor-MongooseConfig", mongooseConfig);
            }

            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            return request;
        }
        public static string[] getConfigurationsList(string json)
        {
            var result = JsonSerializer.Deserialize<InforConfigurationResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (result != null && result.Success)
                return result.Configurations ?? Array.Empty<string>();

            return Array.Empty<string>();
        }
        public static string[] GetTableColumnNames(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return Array.Empty<string>();

            var result = JsonSerializer.Deserialize<TableSchemaResponse>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null || !result.Success || result.Properties == null)
                return Array.Empty<string>();

            return result.Properties
                .Select(p => p.Name ?? string.Empty)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }

    }
}

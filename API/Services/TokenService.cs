using API.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IAppSettingsManager _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public TokenService(IAppSettingsManager settings, IHttpClientFactory httpClientFactory)
        {
            _settings = settings;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetInforAccessTokenAsync()
        {
            var inforSettings = _settings.InforErp;
            var http = _httpClientFactory.CreateClient();
            var form = new Dictionary<string, string>
            {
                ["grant_type"] = inforSettings.GrantType ?? "password",
                ["username"] = inforSettings.ApiUsername,
                ["password"] = inforSettings.ApiPassword,
                ["client_id"] = inforSettings.ClientId,
                ["client_secret"] = inforSettings.ClientSecret
            };

            using var content = new FormUrlEncodedContent(form);
            using var response = await http.PostAsync(_settings.InforErp.TokenUrl, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            return json?["access_token"]?.ToString() ?? throw new Exception("No access_token returned from Infor ION");
        }

        public async Task<string> GetInforAccessTokenClientCredentialAsync()
        {
            var http = _httpClientFactory.CreateClient("InforApi");

            var tokenUrl = _settings.InforErp.TokenUrl;
            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _settings.InforErp.ClientId,
                ["client_secret"] = _settings.InforErp.ClientSecret
            };

            using var content = new FormUrlEncodedContent(form);

            using var response = await http.PostAsync(tokenUrl, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            return json.GetProperty("access_token").GetString()
                   ?? throw new Exception("No access_token returned from Infor SSO");
        }

        public async Task<string> GetInforErpApiAccessTokenAsync(string bearerToken)
        {
            try
            {
                var inforSettings = _settings.InforErp;
                var http = _httpClientFactory.CreateClient("InforApi");

                var url = $"{inforSettings.ApiBaseUrl}{inforSettings.IdoBasePath}/token/{inforSettings.X_Infor_MongooseConfig}/{inforSettings.ApiUsername}/{inforSettings.ApiPassword}";

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(bearerToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                // this can TIME OUT → wrap SendAsync in try/catch
                var response = await http.SendAsync(request);

                // this also can throw
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                try
                {
                    using var doc = JsonDocument.Parse(content);
                    return doc.RootElement.TryGetProperty("Token", out var tokenProp)
                        ? tokenProp.GetString() ?? string.Empty
                        : string.Empty;
                }
                catch
                {
                    return content.Trim();
                }
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }

        public async Task<string> GetSalesforceOAuthTokenAsync()
        {
            var salesforceSettings = _settings.Salesforce;
            var http = _httpClientFactory.CreateClient();

            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = salesforceSettings.ClientId,
                ["client_secret"] = salesforceSettings.ClientSecret
            };

            var resp = await http.PostAsync($"{salesforceSettings.BaseUrl}/services/oauth2/token",
                new FormUrlEncodedContent(formData));
            resp.EnsureSuccessStatusCode();

            var token = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            return token!["access_token"];
        }

    }
}

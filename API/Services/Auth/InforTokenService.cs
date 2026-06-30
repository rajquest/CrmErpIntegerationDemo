using API.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace API.Services.Auth
{
    public class InforTokenService : IInforTokenService
    {
        private readonly IAppSettingsManager _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public InforTokenService(IAppSettingsManager settings, IHttpClientFactory httpClientFactory)
        {
            _settings = settings;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var s = _settings.InforErp;
            var form = new Dictionary<string, string>
            {
                ["grant_type"] = s.GrantType ?? "password",
                ["username"]   = s.ApiUsername,
                ["password"]   = s.ApiPassword,
                ["client_id"]  = s.ClientId,
                ["client_secret"] = s.ClientSecret
            };
            return await PostOAuthTokenAsync(form);
        }

        public async Task<string> GetClientCredentialTokenAsync()
        {
            var s = _settings.InforErp;
            var form = new Dictionary<string, string>
            {
                ["grant_type"]    = "client_credentials",
                ["client_id"]     = s.ClientId,
                ["client_secret"] = s.ClientSecret
            };
            return await PostOAuthTokenAsync(form, "InforApi");
        }

        public async Task<string> GetErpApiTokenAsync(string bearerToken)
        {
            try
            {
                var s = _settings.InforErp;
                var http = _httpClientFactory.CreateClient("InforApi");
                var url = $"{s.ApiBaseUrl}{s.IdoBasePath}/token/{s.X_Infor_MongooseConfig}/{s.ApiUsername}/{s.ApiPassword}";

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(bearerToken))
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

                var response = await http.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    using var doc = JsonDocument.Parse(content);
                    return doc.RootElement.TryGetProperty("Token", out var tokenProp)
                        ? tokenProp.GetString() ?? string.Empty
                        : string.Empty;
                }
                catch { return content.Trim(); }
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }

        private async Task<string> PostOAuthTokenAsync(Dictionary<string, string> form, string clientName = "")
        {
            var http = string.IsNullOrEmpty(clientName)
                ? _httpClientFactory.CreateClient()
                : _httpClientFactory.CreateClient(clientName);

            using var content = new FormUrlEncodedContent(form);
            using var response = await http.PostAsync(_settings.InforErp.TokenUrl, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("access_token").GetString()
                   ?? throw new Exception("No access_token returned from Infor SSO");
        }
    }
}

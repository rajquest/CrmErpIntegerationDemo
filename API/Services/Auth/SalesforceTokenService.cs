using API.Interfaces;

namespace API.Services.Auth
{
    public class SalesforceTokenService : ISalesforceTokenService
    {
        private readonly IAppSettingsManager _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public SalesforceTokenService(IAppSettingsManager settings, IHttpClientFactory httpClientFactory)
        {
            _settings = settings;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetOAuthTokenAsync()
        {
            var sf = _settings.Salesforce;
            var http = _httpClientFactory.CreateClient();
            var form = new Dictionary<string, string>
            {
                ["grant_type"]    = "client_credentials",
                ["client_id"]     = sf.ClientId,
                ["client_secret"] = sf.ClientSecret
            };

            using var content = new FormUrlEncodedContent(form);
            var resp = await http.PostAsync($"{sf.BaseUrl}/services/oauth2/token", content);
            resp.EnsureSuccessStatusCode();

            var token = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            return token!["access_token"];
        }
    }
}

namespace API.Models.Salesforce
{
    public class SalesforceSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string ApiVersion { get; set; } = "v60.0";
    }
}

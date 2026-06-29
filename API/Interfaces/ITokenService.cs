namespace API.Interfaces
{
    public interface ITokenService
    {
        Task<string> GetSalesforceOAuthTokenAsync();
        Task<string> GetInforAccessTokenClientCredentialAsync();
        Task<string> GetInforAccessTokenAsync();
        Task<string> GetInforErpApiAccessTokenAsync(string bearerToken);
    }
}

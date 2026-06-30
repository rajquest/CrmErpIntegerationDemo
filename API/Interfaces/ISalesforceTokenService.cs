namespace API.Interfaces
{
    public interface ISalesforceTokenService
    {
        Task<string> GetOAuthTokenAsync();
    }
}

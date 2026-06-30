using API.Models.InforErp;

namespace API.Interfaces
{
    public interface IInforTokenService
    {
        Task<string> GetAccessTokenAsync();
        Task<string> GetClientCredentialTokenAsync();
        Task<string> GetErpApiTokenAsync(string bearerToken);
    }
}

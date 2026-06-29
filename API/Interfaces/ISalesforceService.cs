using API.Models.Salesforce;

namespace API.Interfaces
{
    public interface ISalesforceService
    {
        Task<string[]> GetAllSObjectsAsync(string token);
        Task<List<SObjectFieldInfo>> GetTableColumnNames(string token, string sObjectName);
        Task<List<CareProgramEnrolleeProduct>> GetCareProgramEnrolleeProductRowsAsync(string token, int limit = 10);
        Task<List<Account>> GetAccountRowsAsync(string token, int limit = 10);
    }
}

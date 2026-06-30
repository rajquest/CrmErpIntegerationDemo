using NetCoreForce.Client;
using API.Interfaces;
using API.Models.Salesforce;

namespace API.Services
{
    public class SalesforceService : ISalesforceService
    {
        private readonly IAppSettingsManager _settings;

        public SalesforceService(IAppSettingsManager settings)
        {
            _settings = settings;
        }

        public async Task<string[]> GetAllSObjectsAsync(string token)
        {
            var client = CreateClient(token);
            var describe = await client.DescribeGlobal();
            return describe.SObjects.Select(s => s.Name).ToArray();
        }

        public async Task<List<SObjectFieldInfo>> GetTableColumnNames(string token, string sObjectName)
        {
            if (string.IsNullOrWhiteSpace(sObjectName))
                throw new ArgumentException("sObject name is required.", nameof(sObjectName));

            var client = CreateClient(token);
            var describe = await client.GetObjectDescribe(sObjectName);

            return describe.Fields.Select(f => new SObjectFieldInfo
            {
                Name = f.Name,
                Label = f.Label,
                Type = f.Type,
                Length = f.Length,
                Updateable = f.Updateable,
                Nillable = f.Nillable
            }).ToList();
        }

        public async Task<List<CareProgramEnrolleeProduct>> GetCareProgramEnrolleeProductRowsAsync(string token, int limit = 10)
        {
            var sObjectName = "CareProgramEnrolleeProduct";
            var client = CreateClient(token);
            var sObject = await client.GetObjectDescribe(sObjectName);

            var fieldNames = sObject.Fields
                .Where(f => !f.Name.EndsWith("__r"))
                .Select(f => f.Name);

            var soql = $"SELECT {string.Join(",", fieldNames)} FROM {sObjectName} LIMIT {limit}";
            var results = new List<CareProgramEnrolleeProduct>();

            await foreach (var record in client.QueryAsync<CareProgramEnrolleeProduct>(soql))
                results.Add(record);

            return results;
        }

        public async Task<List<Account>> GetAccountRowsAsync(string token, int limit = 10)
        {
            var sObjectName = "Account";
            var client = CreateClient(token);
            var sObject = await client.GetObjectDescribe(sObjectName);

            var fieldNames = sObject.Fields
                .Where(f => !f.Name.EndsWith("__r") && !f.Name.Contains("Address"))
                .Select(f => f.Name);

            var soql = $"SELECT {string.Join(",", fieldNames)} FROM {sObjectName} LIMIT {limit}";
            var results = new List<Account>();

            await foreach (var record in client.QueryAsync<Account>(soql))
                results.Add(record);

            return results;
        }

        private ForceClient CreateClient(string token)
            => new ForceClient(_settings.Salesforce.BaseUrl, _settings.Salesforce.ApiVersion, token);
    }
}

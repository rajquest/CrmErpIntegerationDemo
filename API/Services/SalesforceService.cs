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
            var client = new ForceClient(_settings.Salesforce.BaseUrl, _settings.Salesforce.ApiVersion, token);
            var describe = await client.DescribeGlobal();
            var sObjectNames = describe.SObjects.Select(s => s.Name).ToArray();
            return sObjectNames;
        }
        
        public async Task<List<SObjectFieldInfo>> GetTableColumnNames(string token, string sObjectName)
        {
            var client = new ForceClient(_settings.Salesforce.BaseUrl, _settings.Salesforce.ApiVersion, token);
            if (string.IsNullOrWhiteSpace(sObjectName))
                throw new ArgumentException("sObject name is required.", nameof(sObjectName));

            // Describe the object (full metadata)
            var describe = await client.GetObjectDescribe(sObjectName);

            // Extract column list
            var fields = describe.Fields.Select(f => new SObjectFieldInfo
            {
                Name = f.Name,
                Label = f.Label,
                Type = f.Type,
                Length = f.Length,
                Updateable = f.Updateable,
                Nillable = f.Nillable
            }).ToList();
            return fields;
        }

        public async Task<List<CareProgramEnrolleeProduct>> GetCareProgramEnrolleeProductRowsAsync(string token, int limit = 10)
        {
            var sObjectName = "CareProgramEnrolleeProduct";

            // Create Salesforce client
            var client = new ForceClient(_settings.Salesforce.BaseUrl, _settings.Salesforce.ApiVersion, token);

            // Describe the object to get all fields
            var sObject = await client.GetObjectDescribe(sObjectName);

            // Build field list (skip deprecated or relationship fields)
            var fieldNames = sObject.Fields
                .Where(f => !f.Name.EndsWith("__r"))
                .Select(f => f.Name);

            // Build SOQL
            var soql = $"SELECT {string.Join(",", fieldNames)} FROM {sObjectName} LIMIT {limit}";

            // Execute query
            var asyncEnumerable = client.QueryAsync<CareProgramEnrolleeProduct>(soql);
            var results = new List<CareProgramEnrolleeProduct>();

            await foreach (var record in asyncEnumerable)
            {
                // Serialize each record to JSON
                results.Add(record);
            }

            return results;
        }

        public async Task<List<Account>> GetAccountRowsAsync(string token, int limit = 10)
        {
            var sObjectName = "Account";

            // Create Salesforce client
            var client = new ForceClient(_settings.Salesforce.BaseUrl, _settings.Salesforce.ApiVersion, token);

            // Describe the object to get all fields
            var sObject = await client.GetObjectDescribe(sObjectName);

            // Build field list (skip deprecated or relationship fields)
            var fieldNames = sObject.Fields
                .Where(f => !f.Name.EndsWith("__r") && !f.Name.Contains("Address"))
                .Select(f => f.Name);

            // Build SOQL
            var soql = $"SELECT {string.Join(",", fieldNames)} FROM {sObjectName} LIMIT {limit}";

            // Execute query
            var asyncEnumerable = client.QueryAsync<Account>(soql);
            var results = new List<Account>();

            await foreach (var record in asyncEnumerable)
            {
                // Serialize each record to JSON
                results.Add(record);
            }

            return results;
        }

    }
}

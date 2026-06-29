namespace API.Models.Salesforce
{
    public class SObjectMetadataResult
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string KeyPrefix { get; set; }
        public bool Custom { get; set; }
        public string UrlDetail { get; set; }
        public string UrlDescribe { get; set; }
        public string UrlLayout { get; set; }

        public List<SObjectFieldInfo> Fields { get; set; }
    }
}

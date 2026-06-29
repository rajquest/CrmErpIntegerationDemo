namespace API.Models.Salesforce
{
    public class SObjectFieldInfo
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public int? Length { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public bool Updateable { get; set; }
        public bool Nillable { get; set; }
        public bool Custom { get; set; }
        public bool RestrictedPicklist { get; set; }
        public List<string> PicklistValues { get; set; }
    }
}

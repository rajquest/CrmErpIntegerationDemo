namespace API.Models.InforErp
{
    public class TableSchemaResponse
    {
        public string? Message { get; set; }
        public bool Success { get; set; }
        public List<string>? Keys { get; set; }
        public List<TableProperty>? Properties { get; set; }
        public List<SubCollection>? SubCollections { get; set; }
    }

    public class TableProperty
    {
        public string? BooleanFalseValue { get; set; }
        public string? BooleanTrueValue { get; set; }
        public string? CaseFormat { get; set; }
        public string? ClrTypeName { get; set; }
        public string? ColumnDataType { get; set; }
        public string? DataType { get; set; }
        public string? DateFormat { get; set; }
        public int DecimalPos { get; set; }
        public string? DefaultIMECharset { get; set; }
        public string? DefaultValue { get; set; }
        public string? DomainIDOName { get; set; }
        public string? DomainListProperties { get; set; }
        public string? DomainProperty { get; set; }
        public string? InputMask { get; set; }
        public bool IsItemWarnings { get; set; }
        public string? JustifyFormat { get; set; }
        public string? LabelStringID { get; set; }
        public int Length { get; set; }
        public string? Name { get; set; }
        public string? PropertyClass { get; set; }
        public bool RORecord { get; set; }
        public bool ReadOnly { get; set; }
        public bool Required { get; set; }
    }

    public class SubCollection
    {
        public string? IDOName { get; set; }
        public List<LinkBy>? LinkBy { get; set; }
        public string? Name { get; set; }
    }

    public class LinkBy
    {
        public string? Child { get; set; }
        public string? Parent { get; set; }
    }
}
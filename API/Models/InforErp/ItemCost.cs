namespace API.Models.InforErp
{
    public class ItemCost
    {
        public string Item { get; set; } = string.Empty;
        public string? AsmSetup { get; set; }
        public string? AsmRun { get; set; }
        public string? AsmMatl { get; set; }
        public string? AsmTool { get; set; }
        public string? AsmFixture { get; set; }
        public string? AsmOther { get; set; }
        public string? AsmFixed { get; set; }
        public string? AsmVar { get; set; }
        public string? AsmOutside { get; set; }
        public string? SubMatl { get; set; }
        public string? UnitCost { get; set; }
    }
}

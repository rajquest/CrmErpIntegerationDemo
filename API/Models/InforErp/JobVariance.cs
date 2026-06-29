using API.Common;

namespace API.Models.InforErp
{
    public class JobVariance
    {
        public string? DerJob { get; set; }
        public string? Item { get; set; }
        public string? ItemDescription { get; set; }
        public string? Stat { get; set; } // Status
        public string? Rework { get; set; } // 0 | 1
        public string? OrdRelease { get; set; }
        public string? QtyReleased { get; set; }
        public string? QtyComplete { get; set; }
        public string? QtyScrapped { get; set; }
        public string? JobDate { get; set; }
        public string? JschEndDate { get; set; }
        public string? AsmSetup { get; set; }
        public string? AsmRun { get; set; }
        public string? AsmMatl { get; set; }
        public string? AsmTool { get; set; }
        public string? AsmFixture { get; set; }
        public string? AsmOther { get; set; }
        public string? AsmFixed { get; set; }
        public string? AsmVar { get; set; }
        public string? AsmOutside { get; set; }
        public string? StdUnitCost { get; set; }
        public string? AsmMatlSubtotal { get; set; }

        //Field8: Run Cost = Run * Qty Released
        public decimal RunCost =>
            StringUtils.ToDecimal(AsmRun) * StringUtils.ToDecimal(QtyReleased);
        
        //Field9: Material Cost = (Material + Assy Matl Subtotal) * Qty Released
        public decimal MaterialCost =>
            (StringUtils.ToDecimal(AsmMatl) + StringUtils.ToDecimal(AsmMatlSubtotal))
            * StringUtils.ToDecimal(QtyReleased);
        //Field10: Overhead Cost = (Fixed + Variable) * Qty Released
        public decimal OverheadCost =>
            (StringUtils.ToDecimal(AsmFixed) + StringUtils.ToDecimal(AsmVar))
            * StringUtils.ToDecimal(QtyReleased);
        //Field11: Outside Cost = Outside * Qty Released
        public decimal OutsideCost =>
            StringUtils.ToDecimal(AsmOutside) * StringUtils.ToDecimal(QtyReleased);

        //Math: Field12: Sum all of these items together = Total Cost
        public decimal TotalCost =>
            RunCost + MaterialCost + OverheadCost + OutsideCost;
    }
}

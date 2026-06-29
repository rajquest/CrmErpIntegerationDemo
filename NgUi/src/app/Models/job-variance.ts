export interface JobVariance {
  derJob: string | null;
  item: string | null;
  itemDescription: string | null;
  stat: string | null;
  rework: string | null;
  ordRelease: string | null;
  qtyReleased: string | null;
  qtyComplete: string | null;
  qtyScrapped: string | null;
  jobDate: string | null;
  jschEndDate: string | null;
  asmSetup: string | null;
  asmRun: string | null;
  asmMatl: string | null;
  asmTool: string | null;
  asmFixture: string | null;
  asmOther: string | null;
  asmFixed: string | null;
  asmVar: string | null;
  asmOutside: string | null;
  stdUnitCost: string | null;
  asmMatlSubtotal: string | null;
  runCost: number;
  materialCost: number;
  overheadCost: number;
  outsideCost: number;
  totalCost: number;
}

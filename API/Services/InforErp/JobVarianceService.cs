using AutoMapper;
using API.Interfaces;
using API.Models.InforErp;

namespace API.Services.InforErp
{
    public class JobVarianceService: IJobVarianceService
    {
        private readonly IInforErpService _erpInforService;
        private readonly IMapper _mapper;

        public JobVarianceService(IInforErpService erpInforService, IMapper mapper)
        {
            _mapper = mapper;
            _erpInforService = erpInforService;
        }

        public async Task<JobVariance[]> ConstructJobVarianceReport(string? filter, int rowCount, string bearerToken)
        {
            var jobOrders = await GetJobOrder(filter, rowCount, bearerToken);
            var jobVarianceDataset = _mapper.Map<JobVariance[]>(jobOrders);

            if (jobVarianceDataset == null) return null;
            
            await MaptemCost(jobVarianceDataset, bearerToken);

            foreach (var job in jobVarianceDataset)
            {
                Console.WriteLine($"{job.Item} {job.ItemDescription} {job.AsmFixed} {job.StdUnitCost}");
            }
            return jobVarianceDataset;
        }

        //private async Task MapJobItemCost(JobVariance[] jobs,string bearerToken)
        //{
        //    var allJobItemsCost = await GetJobItemsCost(null, 0, bearerToken);
        //    foreach (var item in jobs)
        //    {
        //        var itemCost = allJobItemsCost.FirstOrDefault(x => x.Item == item.Item);

        //        item.DerItmAsmSubtotal = itemCost.DerItmAsmSubtotal;
        //        item.QtyOnHand = itemCost.QtyOnHand;
        //        item.WBDerUnitCost = itemCost.WBDerUnitCost;
        //    }
        //}

        private async Task MaptemCost(JobVariance[] jobs, string bearerToken)
        {
            var allItemsCost = await GetItemsCost(null, 0, bearerToken);
           
            foreach (var item in jobs)
            {
                var itemCost = allItemsCost.FirstOrDefault(x => x.Item == item.Item);

                item.AsmSetup = itemCost.AsmSetup;
                item.AsmRun = itemCost.AsmRun;
                item.AsmMatl = itemCost.AsmMatl;
                item.AsmTool = itemCost.AsmTool;
                item.AsmFixture = itemCost.AsmFixture;
                item.AsmOther = itemCost.AsmOther;
                item.AsmFixed = itemCost.AsmFixed;
                item.AsmVar = itemCost.AsmVar;
                item.AsmOutside = itemCost.AsmOutside;
                item.StdUnitCost = itemCost.UnitCost;
                item.AsmMatlSubtotal = itemCost.SubMatl;
            }
        }
        private async Task<JobOrderItem[]> GetJobOrder(string? filter, int rowCount, string bearerToken)
        {
            var tableName = "SLJobs";
            var attributesList =
                "DerJob,Item,ItemDescription,Stat,OrdRelease,QtyReleased,QtyComplete,QtyScrapped,JobDate,JschEndDate,Rework";
            var items = await _erpInforService
                .GetIdoTableDataAsync<JobOrderItem>(tableName,
                    attributesList,
                    filter,
                    rowCount,
                    bearerToken);
            return items;
        }

        private async Task<JobItemCost[]> GetJobItemsCost(string? filter, int rowCount, string bearerToken)
        {
            var tableName = "SLItemwhses";
            var attributesList =
                "Item,ItemDescription,DerItmAsmSubtotal,QtyOnHand,WBDerUnitCost";
            var items = await _erpInforService
                .GetIdoTableDataAsync<JobItemCost>(tableName,
                    attributesList,
                    filter,
                    rowCount,
                    bearerToken);
            return items;
        }
        private async Task<ItemCost[]> GetItemsCost(string? filter, int rowCount, string bearerToken)
        {
            //SubMatl - Assembly Material Cost
            // Unit Cost
            var tableName = "SLItems";
            var attributesList =
                "Item,AsmSetup,AsmRun,AsmMatl,AsmTool,AsmFixture,AsmOther,AsmVar,AsmOutside,AsmFixed,SubMatl,UnitCost";
            var items = await _erpInforService
                .GetIdoTableDataAsync<ItemCost>(tableName,
                    attributesList,
                    filter,
                    rowCount,
                    bearerToken);
            return items;
        }
    }
}

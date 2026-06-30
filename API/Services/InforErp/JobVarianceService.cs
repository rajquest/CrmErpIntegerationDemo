using AutoMapper;
using API.Interfaces;
using API.Models.InforErp;

namespace API.Services.InforErp
{
    public class JobVarianceService : IJobVarianceService
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

            return jobVarianceDataset;
        }

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
            return await _erpInforService.GetIdoTableDataAsync<JobOrderItem>(
                "SLJobs",
                "DerJob,Item,ItemDescription,Stat,OrdRelease,QtyReleased,QtyComplete,QtyScrapped,JobDate,JschEndDate,Rework",
                filter, rowCount, bearerToken);
        }

        private async Task<ItemCost[]> GetItemsCost(string? filter, int rowCount, string bearerToken)
        {
            return await _erpInforService.GetIdoTableDataAsync<ItemCost>(
                "SLItems",
                "Item,AsmSetup,AsmRun,AsmMatl,AsmTool,AsmFixture,AsmOther,AsmVar,AsmOutside,AsmFixed,SubMatl,UnitCost",
                filter, rowCount, bearerToken);
        }
    }
}

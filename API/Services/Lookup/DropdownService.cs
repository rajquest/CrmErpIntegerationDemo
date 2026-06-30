using API.Interfaces;
using API.Models.DropdownValues;
using API.Models.InforErp;

namespace API.Services.Lookup
{
    public class DropdownService : IDropdownService
    {
        private readonly IInforErpService _erpService;
        private readonly ILogger<DropdownService> _logger;

        public DropdownService(IInforErpService erpService, ILogger<DropdownService> logger)
        {
            _erpService = erpService;
            _logger = logger;
        }

        public async Task<SLItem[]?> GetItemList(string bearerToken)
        {
            try { return await _erpService.GetIdoTableDataAsync<SLItem>("SLItems", "Item,Description", null, 0, bearerToken); }
            catch (Exception ex) { _logger.LogError(ex, "Failed to fetch item list"); return null; }
        }

        public async Task<Warehouse[]?> GetWarehouse(string bearerToken)
        {
            try { return await _erpService.GetIdoTableDataAsync<Warehouse>("SLWhses", "Whse,Name", null, 0, bearerToken); }
            catch (Exception ex) { _logger.LogError(ex, "Failed to fetch warehouses"); return null; }
        }

        public async Task<Location[]?> GetLocations(string bearerToken)
        {
            try { return await _erpService.GetIdoTableDataAsync<Location>("SLLocations", "Loc,Description", null, 0, bearerToken); }
            catch (Exception ex) { _logger.LogError(ex, "Failed to fetch locations"); return null; }
        }

        public Task<Order[]> GetCustomerOrderNumbers(string bearerToken)
            => _erpService.GetIdoTableDataAsync<Order>("SLCoitems", "CoNum", "CoLine='1'", 0, bearerToken);
    }
}

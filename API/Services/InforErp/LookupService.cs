using API.Interfaces;
using API.Models.DropdownValues;
using API.Models.InforErp;

namespace API.Services.InforErp
{
    public class LookupService: ILookupService
    {
        private readonly IInforErpService _erpInforService;

        public LookupService(IInforErpService erpInforService)
        {
            _erpInforService = erpInforService;
        }
        public async Task<SLItem[]?> GetItemList(string bearerToken)
        {
            try
            {
                var tableName = "SLItems";
                var attributesList = "Item,Description";
                var items = await _erpInforService
                    .GetIdoTableDataAsync<SLItem>(tableName,
                        attributesList,
                        null,
                        0,
                        bearerToken);
                return items;
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return null;
            }
        }

        public async Task<Warehouse[]?> GetWarehouse(string bearerToken)
        {
            try
            {
                var tableName = "SLWhses";
                var attributesList = "Whse,Name";
                var items = await _erpInforService
                    .GetIdoTableDataAsync<Warehouse>(tableName,
                        attributesList,
                        null,
                        0,
                        bearerToken);
                return items;
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return null;
            }
        }

        public async Task<Location[]?> GetLocations(string bearerToken)
        {
            try
            {
                var tableName = "SLLocations";
                var attributesList = "Loc,Description";
                var items = await _erpInforService
                    .GetIdoTableDataAsync<Location>(tableName,
                        attributesList,
                        null,
                        0,
                        bearerToken);
                return items;
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return null;
            }
        }

        public async Task<Order[]> GetCustomerOrderNumbers(string bearerToken)
        {   
            var tableName = "SLCoitems";
            var attributesList = "CoNum";
            var items = await _erpInforService
                .GetIdoTableDataAsync<Order>(tableName,
                    attributesList,
                    "CoLine='1'",
                    0,
                    bearerToken);

            return items;
        }
        public async Task<Customer[]> GetCustomers(string bearerToken)
        {
            var tableName = "SLCustomers";
            var attributesList =
                "Name,CustNum,State,StateDescription,Addr_1,Zip,_ItemId,CustSeq";
            var items = await _erpInforService
                .GetIdoTableDataAsync<Customer>(tableName,
                    attributesList,
                    null,
                    0,
                    bearerToken);
            return items;
        }
        public async Task<string> GetCustomerName(string customerNumber, string bearerToken)
        {
            var tableName = "SLCustomers";
            var attributesList =
                "Name,CustNum,State,StateDescription,Addr_1,Zip,_ItemId,CustSeq";
            var filter = $"CustNum='{customerNumber}'";
            var items = await _erpInforService
                .GetIdoTableDataAsync<Customer>(tableName,
                    attributesList,
                    filter,
                    0,
                    bearerToken);
            return items?.FirstOrDefault()?.Name;
        }

    }
}

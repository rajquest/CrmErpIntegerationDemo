using API.Interfaces;
using API.Models.InforErp;

namespace API.Services.Infor
{
    public class CustomerService : ICustomerService
    {
        private readonly IInforErpService _erpService;
        private readonly ILogger<CustomerService> _logger;

        private const string TableName = "SLCustomers";
        private const string CustomerAttributes = "Name,CustNum,State,StateDescription,Addr_1,Zip,_ItemId,CustSeq";
        private const string ShipToAttributes = "CustNum,Name,Addr_1,State,Zip,ShipToEmail,Addr_2,Country,County,City,DerDefaultShipTo,TaxCode1,Stat";

        public CustomerService(IInforErpService erpService, ILogger<CustomerService> logger)
        {
            _erpService = erpService;
            _logger = logger;
        }

        public Task<Customer[]> GetCustomers(string bearerToken)
            => _erpService.GetIdoTableDataAsync<Customer>(TableName, CustomerAttributes, null, 0, bearerToken);

        public async Task<string> GetCustomerName(string customerNumber, string bearerToken)
            => (await _erpService.GetIdoTableDataAsync<Customer>(TableName, CustomerAttributes,
                    $"CustNum='{customerNumber}'", 0, bearerToken))
                .FirstOrDefault()?.Name ?? string.Empty;

        public async Task<CustomerShipToRecord?> GetCustomerShipTo(string customerNumber, string? customerSeq, string bearerToken)
        {
            var items = await _erpService.GetIdoTableDataAsync<CustomerShipToRecord>(
                TableName, ShipToAttributes,
                $"CustNum='{customerNumber}' and DerDefaultShipTo='1'", 0, bearerToken);

            var record = items?.FirstOrDefault();
            if (record != null) return record;

            items = await _erpService.GetIdoTableDataAsync<CustomerShipToRecord>(
                TableName, ShipToAttributes,
                $"CustNum='{customerNumber}'", 0, bearerToken);

            return items?.FirstOrDefault();
        }
    }
}

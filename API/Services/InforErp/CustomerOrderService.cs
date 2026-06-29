using AutoMapper;
using API.Common;
using API.Interfaces;
using API.Models.DropdownValues;
using API.Models.InforErp;

namespace API.Services.InforErp
{    
    public class CustomerOrderService:ICustomerOrderService
    {
        private readonly IInforErpService _erpInforService;
        private readonly IMapper _imapper;
        private readonly ILookupService _lookupService;
        private readonly ITaxLookupService _taxLookupService;
        
        public CustomerOrderService(IInforErpService erpInforService, 
            IMapper mapper, 
            ILookupService lookupService, 
            ITaxLookupService taxLookupService)
        {
            _erpInforService = erpInforService;
            _imapper = mapper;
            _lookupService = lookupService;
            _taxLookupService = taxLookupService;
        }
       
        public async Task<CustomerOrderItem[]> GetCustomerOrders(int rowCount,
            string filterCriteria, 
            string bearerToken)
        {   // CustSeq - ShipTo
            // CoNum - Order Num
            var tableName = "SLCoitems";
            var attributesList =
                "CoNum,DerCustNum,CoType,DerCustPo,CoOrderDate,QtyOrdered,Stat,ShipDate,CustSeq,CoPrice,Item,CoLine";
            var items = await _erpInforService
                .GetIdoTableDataAsync<CustomerOrderItem>(tableName,
                    attributesList,
                    filterCriteria,
                    rowCount,
                    bearerToken);

            return items;
        }

        public async Task<InvoiceListingItem[]> GetInvoiceListingForOrder(string orderNumber, string bearerToken)
        {   
            var tableName = "SLInvitemalls";
            var attributesList =
                "DerInvNum,DerCoNum,CoNum,Item,DerDescription,QtyInvoiced,InvhdrMiscCharges,InvhdrPrepaidAmt,InvhdrFreight,InvhdrPrice,InvhdrInvDate";
            var filterCriteria = $"CoNum='{orderNumber}'";
            var items = await _erpInforService
                .GetIdoTableDataAsync<InvoiceListingItem>(tableName,
                    attributesList,
                    filterCriteria,
                    0,
                    bearerToken);

            return items;
        }

        public async Task<MaterialTransactionItem[]> GetMaterialTransaction(string filter, string bearerToken)
        {
            var tableName = "SLMatlTrans";
            var attributesList = "TransDate,TransType,RefType,Item,ItemDescription,Lot,Qty";
            
            var items = await _erpInforService
                .GetIdoTableDataAsync<MaterialTransactionItem>(tableName,
                    attributesList,
                    filter,
                    0,
                    bearerToken);

            return items;
        }

        public async Task<InvoiceListingItem[]> GetInvoiceListing(string filterCriteria, string bearerToken)
        {
            var tableName = "SLInvitemalls";
            var attributesList =
                "CoLine,DerInvNum,DerCoNum,InvhdrCustNum,CustaddrName,CoNum,DerCustPo,Item,DerDescription,QtyInvoiced,InvhdrMiscCharges,InvhdrPrepaidAmt,InvhdrFreight,InvhdrPrice,InvhdrInvDate,DerExtendedPrice,Price";
            
            var items = await _erpInforService
                .GetIdoTableDataAsync<InvoiceListingItem>(tableName,
                    attributesList,
                    filterCriteria,
                    0,
                    bearerToken);

            return items;
        }

        public async Task<CustomerShipToRecord?> GetCustomerShipTo(string customerNumber, string? customerSeq, string bearerToken)
        {
            if (string.IsNullOrEmpty(customerSeq))
            {
                customerSeq = "0";
            }
            var filterCriteria = $"CustNum='{customerNumber}' and DerDefaultShipTo='1'";

            var tableName = "SLCustomers";
            var attributesList = "CustNum,Name,Addr_1,State,Zip,ShipToEmail,Addr_2,Country,County,City,DerDefaultShipTo,TaxCode1,Stat";
            var items = await _erpInforService
                .GetIdoTableDataAsync<CustomerShipToRecord>(tableName,
                    attributesList,
                    filterCriteria,
                    0,
                    bearerToken);

            var record = items?.FirstOrDefault();

            if (record != null)
                return record;

            var fallbackFilter = $"CustNum='{customerNumber}'";

            items = await _erpInforService
                .GetIdoTableDataAsync<CustomerShipToRecord>(tableName,
                    attributesList,
                    fallbackFilter,
                    0,
                    bearerToken);
            return items?.FirstOrDefault();
        }
        public async Task<SalesTaxItem?> GetSalesTaxForOrder(string orderNumber, string bearerToken)
        {
            try
            {
                var filterCriteria = $"CoNum='{orderNumber}'";

                var tableName = "SLCos";
                var attributesList = "CoNum,SalesTaxT";
                var items = await _erpInforService
                    .GetIdoTableDataAsync<SalesTaxItem>(tableName,
                        attributesList,
                        filterCriteria,
                        0,
                        bearerToken);

                return items?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return null;
            }
        }
        public async Task<InvoicedOrder[]> GetInvoicedOrders(string filter, string bearerToken)
        {
            var invoiceOrders = new List<InvoicedOrder>();
            var invoices = await GetInvoiceListing(filter, bearerToken);
            // Step 2: Get all Tax codes for customers
            var allTaxCodes = await _taxLookupService.GetAllTaxCodes(bearerToken);

            foreach (var invoice in invoices)
            {
                var invoiceOrder = _imapper.Map<InvoicedOrder>(invoice);
                Console.WriteLine(invoiceOrder.DerCoNum);
                
                var orderFilter = $"CoNum='{invoiceOrder.DerCoNum}' and CoLine='1'";
                var orders = await GetCustomerOrders(0, orderFilter, bearerToken);
                
                if (orders.Length > 0)
                {
                    var order = orders.FirstOrDefault();
                    invoiceOrder.DerCustNum = order.DerCustNum;
                    invoiceOrder.Stat = order.Stat;
                    invoiceOrder.CoOrderDate = order.CoOrderDate;
                    invoiceOrder.ShipDate = order.ShipDate;

                    var customerName = await _lookupService.GetCustomerName(order.DerCustNum, bearerToken);
                    invoiceOrder.CustomerName = customerName;
                    // Load ShipTo
                    var shipTo = await GetCustomerShipTo(order.DerCustNum, null,bearerToken);
                    invoiceOrder.ShipToState = shipTo.State;
                    invoiceOrder.ShipToTaxCode = shipTo.TaxCode1;
                    
                    if (string.Equals(shipTo?.TaxCode1, "NT", StringComparison.OrdinalIgnoreCase))
                    {
                        invoiceOrder.Notes = "EXEMPT";
                        invoiceOrder.ExemptAmount = invoiceOrder.InvhdrPrice;
                    }
                    else
                    {
                        var orderSalesTax = await GetSalesTaxForOrder(order.CoNum,bearerToken); 
                        // get sales taxes list recursively
                        invoiceOrder.TaxRateList = LoadTaxJurisdictionDetails(invoiceOrder.ShipToTaxCode, allTaxCodes);
                        invoiceOrder.TotalSalesTaxRate = SalesTaxUtils.CalculateTotalTaxRate(invoiceOrder.TaxRateList);
                        invoiceOrder.Notes = $"TAX - {shipTo.State} {shipTo.City}";
                        invoiceOrder.SalesTax = orderSalesTax?.SalesTaxT;
                        invoiceOrder.TaxableAmount = invoice.DerExtendedPrice;
                    }
                }

                invoiceOrders.Add(invoiceOrder);
            }

            var addon =  await GetNoOrderInvoicedOrders(filter, bearerToken);
            if (addon != null && addon.Any())
            {
                invoiceOrders.AddRange(addon);
            }
            return invoiceOrders.ToArray();
        }

        // Invoices with order numbers
        public async Task<List<InvoicedOrder>> GetNoOrderInvoicedOrders(string filter, string bearerToken)
        {
            filter = filter.Replace("Item like 'CRDLA%' AND ", "");
            var invoiceOrders = new List<InvoicedOrder>();
            var invoices = await GetInvoiceListing(filter, bearerToken);
            foreach (var invoice in invoices)
            {
                var invoiceOrder = _imapper.Map<InvoicedOrder>(invoice);
                Console.WriteLine(invoiceOrder.DerCoNum);
                // handle rows without order number 
                if (string.IsNullOrWhiteSpace(invoice.DerCoNum))
                {
                    await AddInvoiceRowWithoutOrderNumber(invoiceOrder, bearerToken);
                    invoiceOrders.Add(invoiceOrder);
                }
                else if (string.IsNullOrEmpty(invoice.DerInvNum) &&
                         string.IsNullOrWhiteSpace(invoice.DerCoNum))
                {
                    await AddInvoiceRowWithInvoiceNumber(invoiceOrder, bearerToken);
                    invoiceOrders.Add(invoiceOrder);
                }
            }

            return invoiceOrders;
        }

        private async Task AddInvoiceRowWithoutOrderNumber(InvoicedOrder invoiceOrder, string bearerToken)
        {
            invoiceOrder.DerCustNum = invoiceOrder.InvhdrCustNum;
            invoiceOrder.CustomerName = invoiceOrder.CustaddrName;

            var shipTo = await GetCustomerShipTo(invoiceOrder.InvhdrCustNum, null, bearerToken);
            invoiceOrder.ShipToState = shipTo?.State;
            invoiceOrder.ShipToTaxCode = shipTo?.TaxCode1;

            if (shipTo.TaxCode1.Equals("NT", StringComparison.OrdinalIgnoreCase))
            {
                invoiceOrder.Notes = "EXEMPT";
                invoiceOrder.ExemptAmount = invoiceOrder.InvhdrPrice;
            }
            else
            {
                invoiceOrder.TaxableAmount = invoiceOrder.InvhdrPrice;
            }
        }

        private async Task AddInvoiceRowWithInvoiceNumber(InvoicedOrder invoiceOrder, string bearerToken)
        {
            invoiceOrder.DerCustNum = invoiceOrder.InvhdrCustNum;
            invoiceOrder.CustomerName = invoiceOrder.CustaddrName;

            var shipTo = await GetCustomerShipTo(invoiceOrder.InvhdrCustNum, null, bearerToken);
            invoiceOrder.ShipToState = shipTo?.State;
            invoiceOrder.ShipToTaxCode = shipTo?.TaxCode1;

            if (shipTo.TaxCode1.Equals("NT", StringComparison.OrdinalIgnoreCase))
            {
                invoiceOrder.Notes = "EXEMPT";
                invoiceOrder.ExemptAmount = invoiceOrder.InvhdrPrice;
            }
            else
            {
                invoiceOrder.TaxableAmount = invoiceOrder.InvhdrPrice;
            }
        }

        public TaxJurisdictionDetails[] LoadTaxJurisdictionDetails(string taxCode,
            TaxCodeItem[] allTaxCodes)
        {
            var results = new List<TaxJurisdictionDetails>();
            AddTaxLevel(taxCode, allTaxCodes, results);
            return results.ToArray();
        }

        private void AddTaxLevel(string taxCode, 
            TaxCodeItem[] allTaxCodes,
            List<TaxJurisdictionDetails> results)
        {
            if (string.IsNullOrWhiteSpace(taxCode) || allTaxCodes == null)
                return;

            var match = allTaxCodes
                .FirstOrDefault(x => x.TaxCode == taxCode);

            if (match == null)
                return;

            results.Add(new TaxJurisdictionDetails
            {
                TaxCode = match.TaxCode,
                TaxJur = match.TaxJur,
                Description = match.Description,
                TaxRate = match.TaxRate
            });

            // base case handled automatically
            if (!string.IsNullOrWhiteSpace(match.NxtLvlCode))
            {
                AddTaxLevel(match.NxtLvlCode, allTaxCodes, results);
            }
        }
    }
}

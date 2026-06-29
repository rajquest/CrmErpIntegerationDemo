import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { InforApiService } from '../../Services/infor-api.service';
import { ExcelExportService } from '../../Services/excel-export.service';
import { DateService } from '../../Services/date.service';
import { InvoicedOrder } from '../../Models/invoice-order';
import { Utils } from '../../core/utils';

@Component({
  selector: 'app-invoice-orders',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSlideToggleModule,
    MatIconModule,
    MatDatepickerModule
  ],
  templateUrl: './invoice-orders.component.html',
  styleUrl: './invoice-orders.component.scss'
})
export class InvoiceOrdersComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  utils = Utils;
  dataSource = new MatTableDataSource<InvoicedOrder>([]);

  minifiedColumns: string[] = [
    'derInvNum', 'derCoNum', 'invhdrInvDate', 'invhdrPrice', 'derCustNum',
    'invhdrMiscCharges', 'invhdrFreight', 'shipToState', 'shipToTaxCode',
    'customerName', 'totalSalesTaxRate', 'salesTax', 'taxableAmount', 'exemptAmount', 'taxRateList'
  ];

  detailedColumns: string[] = [
    'derInvNum', 'derCoNum', 'invhdrInvDate', 'qtyInvoiced', 'derCustPo',
    'invhdrPrice', 'item', 'derDescription', 'coLine', 'derCustPo', 'qtyInvoiced',
    'coNum', 'derCustNum', 'coOrderDate', 'shipDate', 'shipToState', 'shipToTaxCode',
    'customerName', 'notes', 'totalSalesTaxRate', 'taxRateList', 'salesTax',
    'taxableAmount', 'exemptAmount', 'invhdrMiscCharges', 'invhdrPrepaidAmt', 'invhdrFreight', 'stat'
  ];

  displayedColumns: string[] = [];
  toggleText: string = 'Minified View';
  useMinified = true;
  startDate?: Date;
  endDate?: Date;
  isLoading = false;
  rowCount = 25;
  rowCountOptions = [10, 25, 50, 100];

  private minifiedColumnLabels: Record<string, string> = {
    derInvNum: 'Invoice #', derCoNum: 'Order Number', invhdrInvDate: 'Invoice Date',
    invhdrPrice: 'Invoice Amount', derCustNum: 'Customer Number', invhdrMiscCharges: 'Misc Charges',
    invhdrFreight: 'Freight', shipToState: 'Ship To State', customerName: 'Customer Name',
    totalSalesTaxRate: 'Sales Tax Rate', salesTax: 'Sales Tax', taxableAmount: 'Taxable Amount',
    exemptAmount: 'Exempt Amount', taxRateList: 'Tax Rates Detail'
  };

  constructor(
    private inforService: InforApiService,
    private excelService: ExcelExportService
  ) { }

  ngOnInit(): void {
    this.updateDisplayedColumns();
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
  }

  toggleView(): void {
    this.useMinified = !this.useMinified;
    this.updateDisplayedColumns();
  }

  updateDisplayedColumns(): void {
    this.displayedColumns = this.useMinified ? this.minifiedColumns : this.detailedColumns;
    this.toggleText = this.useMinified ? 'Minified View' : 'Detailed View';
  }

  exportDataset() {
    const exportData = this.dataSource.data.map(order => {
      const row: Record<string, any> = {};
      this.minifiedColumns.forEach(col => {
        const label = this.minifiedColumnLabels[col] ?? col;
        row[label] = col === 'taxRateList'
          ? (order.taxRateList ?? []).map((t: any) => `${t.taxCode}-${t.taxRate}`).join(',')
          : (order as any)[col];
      });
      return row;
    });
    this.excelService.exportToExcel(exportData, 'customer_order_invoice');
  }

  onSearch(): void {
    this.loadInvoices();
  }

  private loadInvoices(): void {
    this.isLoading = true;
    const filter = this.buildFilter();
    this.inforService.getInvoiceOrders(filter).subscribe({
      next: (data: InvoicedOrder[]) => {
        this.dataSource.data = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load invoices', err);
        this.isLoading = false;
      }
    });
  }

  private buildFilter(): string | undefined {
    const filters: string[] = [];
    const range = DateService.getValidDateRange(this.startDate, this.endDate);
    filters.push(`Item like 'CRDLA%'`);
    if (range.start && range.end) {
      filters.push(`InvhdrInvDate>='${range.start}' AND InvhdrInvDate<='${range.end}'`);
    }
    return filters.length > 0 ? filters.join(' AND ') : undefined;
  }
}

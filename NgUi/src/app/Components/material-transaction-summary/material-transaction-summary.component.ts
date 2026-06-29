import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { InforApiService } from '../../Services/infor-api.service';
import { ExcelExportService } from '../../Services/excel-export.service';
import { DateService } from '../../Services/date.service';
import { MaterialTransactionSummary } from '../../Models/material-transaction-summary';

@Component({
  selector: 'app-material-transaction-summary',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatSortModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatDatepickerModule
  ],
  templateUrl: './material-transaction-summary.component.html',
  styleUrl: './material-transaction-summary.component.scss'
})
export class MaterialTransactionSummaryComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = [];
  dataSource = new MatTableDataSource<MaterialTransactionSummary>();
  isLoading = false;
  startDate!: Date;
  endDate!: Date;
  transactionTypes: string[] = ['Customer Orders', 'Service Orders'];
  hasMonthGrouping = false;
  selectedTransactionType: string = '';

  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private inforService: InforApiService,
    private excelService: ExcelExportService
  ) { }

  ngOnInit(): void {
    this.hasMonthGrouping = false;
    this.setColumns();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  onTransactionTypeChange(value: string): void {
    this.selectedTransactionType = value;
  }

  setColumns() {
    this.displayedColumns = this.hasMonthGrouping
      ? ['monthYear', 'item', 'itemDescription', 'totalQty']
      : ['item', 'itemDescription', 'totalQty'];
  }

  onSearch(): void {
    this.isLoading = true;
    const filter = this.buildFilter();
    this.inforService.getMaterialTransactionSummary(filter).subscribe({
      next: (result: MaterialTransactionSummary[]) => {
        this.dataSource.data = result;
        this.hasMonthGrouping = result.some(x => !!x.monthYear);
        this.setColumns();
      },
      error: (err) => {
        console.error('Error loading material transactions', err);
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  exportDataset() {
    const data = this.dataSource.data;
    this.excelService.exportToExcel(data, 'material_transactions');
  }

  private buildFilter(): string | undefined {
    const filters: string[] = [];
    const range = DateService.getValidDateRange(this.startDate, this.endDate);
    filters.push('Lot IS NOT NULL');
    if (range.start && range.end) {
      filters.push(`TransDate>='${range.start}' AND TransDate<='${range.end}'`);
    }
    if (this.selectedTransactionType && this.selectedTransactionType.trim() !== '' && this.selectedTransactionType === 'Customer Orders') {
      filters.push(`TransType='S'`);
    }
    if (this.selectedTransactionType && this.selectedTransactionType.trim() !== '' && this.selectedTransactionType === 'Service Orders') {
      filters.push(`TransType='G' AND RefType='F'`);
    }
    return filters.length > 0 ? filters.join(' AND ') : undefined;
  }
}

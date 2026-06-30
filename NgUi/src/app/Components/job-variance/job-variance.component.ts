import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule, MatSelectChange } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { InforApiService } from '../../Services/infor-api.service';
import { DialogService } from '../../Services/dialog.service';
import { ExcelExportService } from '../../Services/excel-export.service';
import { JobVariance } from '../../Models/job-variance';

@Component({
  selector: 'app-job-variance',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSlideToggleModule,
    MatIconModule,
    MatDatepickerModule
  ],
  templateUrl: './job-variance.component.html',
  styleUrl: './job-variance.component.scss'
})
export class JobVarianceComponent implements OnInit {
  fullColumns: string[] = [
    'derJob', 'item', 'itemDescription', 'stat', 'rework', 'qtyReleased', 'qtyComplete', 'qtyScrapped',
    'jobDate', 'jschEndDate', 'asmSetup', 'asmRun', 'asmMatl', 'asmMatlSubtotal', 'asmTool',
    'asmFixture', 'asmOther', 'asmFixed', 'asmVar', 'asmOutside', 'stdUnitCost',
    'runCost', 'materialCost', 'overheadCost', 'outsideCost', 'totalCost'
  ];

  minifiedColumns: string[] = [
    'derJob', 'item', 'stat', 'rework', 'jobDate', 'jschEndDate', 'qtyReleased',
    'runCost', 'materialCost', 'overheadCost', 'outsideCost', 'totalCost'
  ];

  useMinified = true;
  displayedColumns: string[] = [];
  dataSource = new MatTableDataSource<JobVariance>([]);
  rowCountOptions = [10, 20, 50, 100, 500, 1000];
  rowCount = 10;
  isLoading = false;
  isRework = '';
  startDate: Date | null = null;
  endDate: Date | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private inforApiService: InforApiService,
    private dialogService: DialogService,
    private excelService: ExcelExportService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.updateDisplayedColumns();
    this.loadJobvariance();
  }

  updateDisplayedColumns() {
    this.displayedColumns = this.useMinified ? this.minifiedColumns : this.fullColumns;
  }

  onRowCountChange(count: number): void {
    this.rowCount = count;
    this.loadJobvariance();
  }

  onSearch() {
    this.loadJobvariance();
  }

  exportDataset() {
    const data = this.dataSource.data;
    this.excelService.exportToExcel(data, 'job_variance');
  }

  onReworkChange(event: MatSelectChange): void {
    this.isRework = event.value;
    this.loadJobvariance();
  }

  loadJobvariance() {
    const filter: string = this.buildFilter();
    this.isLoading = true;
    this.inforApiService.getJobVariance(this.rowCount, filter).subscribe({
      next: (data) => {
        this.dataSource.data = data;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load Job Variance:', err);
        this.isLoading = false;
        this.dialogService.showMessage('Failed to load Job Variance data.');
      }
    });
  }

  private buildFilter(): string {
    const conditions: string[] = [];
    conditions.push(`Stat='C'`);

    if (this.isRework !== undefined && this.isRework !== null && this.isRework !== '') {
      conditions.push(`Rework=${this.isRework}`);
    }

    if (this.startDate && this.endDate) {
      const formatDate = (d: Date): string => {
        const year = d.getFullYear();
        const month = (d.getMonth() + 1).toString().padStart(2, '0');
        const day = d.getDate().toString().padStart(2, '0');
        return `${year}-${month}-${day}`;
      };
      const endPlusOne = new Date(this.endDate);
      endPlusOne.setDate(endPlusOne.getDate() + 1);
      conditions.push(`JschEndDate >= '${formatDate(this.startDate)}'`);
      conditions.push(`JschEndDate < '${formatDate(endPlusOne)}'`);
    }

    return conditions.join(' and ');
  }

  parseCsiDate(value: string): Date | null {
    if (!value) return null;
    const datePart = value.substring(0, 8);
    const year = +datePart.substring(0, 4);
    const month = +datePart.substring(4, 6) - 1;
    const day = +datePart.substring(6, 8);
    return new Date(year, month, day);
  }
}

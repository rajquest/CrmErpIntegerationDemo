import { ChangeDetectorRef, Component, AfterViewInit, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SalesforceService } from '../../Services/salesforce.service';
import { SalesforceProductRecord } from '../../Models/salesforce-product-record';

@Component({
  selector: 'app-device-data',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './device-data.component.html',
  styleUrl: './device-data.component.scss'
})
export class DeviceDataComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = [
    'name', 'createdDate', 'status', 'first_Home_Use_Date__c',
    'four_Year_End_of_Useful_Life_Date__c', 'five_Year_End_of_Useful_Life_Date__c',
    'product_Code__c', 'serial_Number__c'
  ];
  dataSource = new MatTableDataSource<SalesforceProductRecord>([]);
  isLoading = false;
  rowCount = 10;
  pageSize = 10;
  pageSizeOptions = [10, 20, 50, 100];

  @ViewChild('prodPaginator') paginator!: MatPaginator;
  @ViewChild('prodSort') sort!: MatSort;

  constructor(
    private salesforceService: SalesforceService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadProducts(this.pageSize);
    this.dataSource.filterPredicate = (data: SalesforceProductRecord, filter: string) =>
      JSON.stringify(data).toLowerCase().includes(filter.trim().toLowerCase());
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  onPageSizeChange(size: number): void {
    this.rowCount = size;
    this.loadProducts(size);
  }

  loadProducts(rowCount: number): void {
    this.isLoading = true;
    this.salesforceService.getProductData(rowCount).subscribe({
      next: (records) => {
        this.dataSource.data = records;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  applyFilter(event: Event): void {
    const filter = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filter.trim().toLowerCase();
  }
}

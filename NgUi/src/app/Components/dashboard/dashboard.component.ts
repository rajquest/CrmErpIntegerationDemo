import { AfterViewInit, ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule, MatSelectChange } from '@angular/material/select';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatInputModule } from '@angular/material/input';
import { InforApiService } from '../../Services/infor-api.service';
import { DialogService } from '../../Services/dialog.service';
import { TokenService } from '../../Services/token.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatFormFieldModule,
    MatSelectModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatProgressSpinnerModule,
    MatInputModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit, AfterViewInit {
  configurations: string[] = [];
  isLoading = false;
  errorMessage = '';
  schemaErrorMessage = '';
  selectedType = '';
  displayedColumns: string[] = ['attribute'];
  dataSource = new MatTableDataSource<string>([]);

  idoTables = [
    { value: 'SLLots', label: 'Lots' },
    { value: 'SLItems', label: 'Items' },
    { value: 'SLTaxcodes', label: 'Tax Codes' },
    { value: 'SLCustomers', label: 'Customers' },
    { value: 'SLCoitems', label: 'Customer Orders' },
    { value: 'SLLotLocs', label: 'Item Lot Locations' },
    { value: 'SLShipTos', label: 'Customer Ship To' },
    { value: 'SLInvitemalls', label: 'Invoice Listing' },
    { value: 'SLSerials', label: 'Serial Numbers' },
    { value: 'SLJobs', label: 'Job Orders' },
    { value: 'SLItemwhses', label: 'Item Cost' },
    { value: 'SLWhses', label: 'Warehouse' },
    { value: 'SLLocations', label: 'Location' },
    { value: 'SLCos', label: 'Order Header' },
    { value: 'SLMatlTrans', label: 'Material Transaction' }
  ];

  @ViewChild(MatPaginator)
  set paginator(paginator: MatPaginator) {
    if (paginator) {
      this.dataSource.paginator = paginator;
    }
  }

  @ViewChild(MatSort)
  set sort(sort: MatSort) {
    if (sort) {
      this.dataSource.sort = sort;
      this.dataSource.sortingDataAccessor = (item, property) => item;
    }
  }

  constructor(
    private inforApi: InforApiService,
    private dialogService: DialogService,
    private tokenService: TokenService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.idoTables = this.idoTables.sort((a, b) => a.label.localeCompare(b.label));
    const isToken = this.tokenService.checkInforTokensAndPrompt();
    if (!isToken) {
      this.dialogService.showMessage('Missing Infor Token. Generate or refresh new tokens');
    }
  }

  ngAfterViewInit(): void {
    this.dataSource.filterPredicate = (data: string, filter: string) =>
      data.toLowerCase().includes(filter);
  }

  onTypeChange(event: MatSelectChange): void {
    this.selectedType = event.value;
    if (this.selectedType && this.selectedType !== '') {
      this.LoadTableSchema(this.selectedType);
    }
  }

  LoadTableSchema(tableName: string): void {
    this.isLoading = true;
    this.schemaErrorMessage = '';

    this.inforApi.getTableAttributesList(tableName).subscribe({
      next: (resp: string[]) => {
        this.dataSource.data = resp;
        this.isLoading = false;
      },
      error: (err) => {
        const message = err?.message || 'Failed to load table schema.';
        this.schemaErrorMessage = message;
        this.dataSource.data = [];
        this.isLoading = false;
        this.dialogService.showMessage(`Error: ${message}`);
      }
    });
  }

  applyFilter(event: Event): void {
    const value = (event.target as HTMLInputElement).value || '';
    this.dataSource.filter = value.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  loadConfigurations() {
    this.isLoading = true;
    this.errorMessage = '';
    this.inforApi.getConfigurations().subscribe({
      next: (configs) => {
        console.log(configs);
        this.configurations = configs;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.message || 'Failed to load configurations.';
        this.dialogService.showMessage(`Error: ${this.errorMessage}`);
      }
    });
  }
}

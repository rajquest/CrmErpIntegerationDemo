import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SalesforceService } from '../../Services/salesforce.service';
import { DialogService } from '../../Services/dialog.service';
import { TokenService } from '../../Services/token.service';
import { SObjectFieldInfo } from '../../Models/sobject-field-info';

@Component({
  selector: 'app-salesforce-tables',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './salesforce-tables.component.html',
  styleUrl: './salesforce-tables.component.scss'
})
export class SalesforceTablesComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['TableName'];
  dataSource = new MatTableDataSource<{ TableName: string }>([]);
  isLoading = false;
  selectedTable: string | null = null;
  fieldDataSource = new MatTableDataSource<SObjectFieldInfo>([]);
  fieldColumns: string[] = ['name', 'label', 'type', 'length', 'nillable', 'updateable'];

  @ViewChild('mainPaginator') paginator!: MatPaginator;
  @ViewChild('mainSort') sort!: MatSort;
  @ViewChild('fieldPaginator') fieldPaginator!: MatPaginator;
  @ViewChild('fieldSort') fieldSort!: MatSort;

  constructor(
    private salesforceService: SalesforceService,
    private dialogService: DialogService,
    private tokenService: TokenService
  ) { }

  ngOnInit(): void {
    const isToken = this.tokenService.checkSalesforceTokensAndPrompt();
    if (isToken) {
      this.loadItems();
      this.dataSource.filterPredicate = (data: { TableName: string }, filter: string) =>
        data.TableName.toLowerCase().includes(filter);
      this.fieldDataSource.filterPredicate = (data: SObjectFieldInfo, filter: string) => {
        const f = filter.trim().toLowerCase();
        return data.name.toLowerCase().includes(f) ||
          data.label.toLowerCase().includes(f) ||
          data.type.toLowerCase().includes(f);
      };
    } else {
      this.dialogService.showMessage('Missing Salesforce Token. Generate or refresh new tokens');
    }
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.fieldDataSource.paginator = this.fieldPaginator;
    this.fieldDataSource.sort = this.fieldSort;
  }

  loadItems(): void {
    this.isLoading = true;
    this.salesforceService.getObjectsList().subscribe({
      next: (data) => {
        this.dataSource.data = data.map(x => ({ TableName: x }));
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load items:', err);
        this.isLoading = false;
      }
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  applyFieldFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.fieldDataSource.filter = filterValue.trim().toLowerCase();
    if (this.fieldDataSource.paginator) {
      this.fieldDataSource.paginator.firstPage();
    }
  }

  onRowClick(tableName: string): void {
    this.salesforceService.getObjectsFieldInfo(tableName).subscribe({
      next: (fields) => {
        this.selectedTable = tableName;
        this.fieldDataSource = new MatTableDataSource<SObjectFieldInfo>(fields);
        this.fieldDataSource.paginator = this.fieldPaginator;
        this.fieldDataSource.sort = this.fieldSort;
      },
      error: (err) => {
        console.error('Error loading table columns:', err);
      }
    });
  }
}

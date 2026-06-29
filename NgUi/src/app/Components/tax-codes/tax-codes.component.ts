import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { InforApiService } from '../../Services/infor-api.service';
import { DialogService } from '../../Services/dialog.service';
import { TokenService } from '../../Services/token.service';
import { TaxCodeItem } from '../../Models/tax-codes';

@Component({
  selector: 'app-tax-codes',
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
  templateUrl: './tax-codes.component.html',
  styleUrl: './tax-codes.component.scss'
})
export class TaxCodesComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = [
    'TaxCode', 'TaxCodeType', 'TaxRate', 'TaxJur', 'TaxJurDescription', 'NxtLvlCode', 'NxtLvlDescription'
  ];
  dataSource = new MatTableDataSource<TaxCodeItem>([]);
  isLoading = false;
  rowCount = 10;
  rowCountOptions = [10, 20, 50, 100];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private inforService: InforApiService,
    private dialogService: DialogService,
    private tokenService: TokenService
  ) { }

  ngOnInit(): void {
    const hasToken = this.tokenService.checkInforTokensAndPrompt();
    if (hasToken) {
      this.loadTaxCodes();
    } else {
      this.dialogService.showMessage('Missing Infor Token. Generate or refresh new tokens');
    }
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.sortingDataAccessor = (item, property) => {
      const value = (item as any)[property];
      return typeof value === 'string' ? value.toLowerCase() : value;
    };
  }

  loadTaxCodes(): void {
    this.isLoading = true;
    this.inforService.getTaxCodes(0).subscribe({
      next: (response) => {
        this.dataSource.data = response ?? [];
        setTimeout(() => {
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        });
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load tax codes:', err);
        this.isLoading = false;
      }
    });
  }

  onRowCountChange(count: number): void {
    this.rowCount = count;
    this.loadTaxCodes();
  }

  applyFilter(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.dataSource.filter = value.trim().toLowerCase();
  }
}

import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { InventoryLot } from '../../Models/inventory-lot';
import { DialogService } from '../../Services/dialog.service';
import { TokenService } from '../../Services/token.service';
import { InforApiService } from '../../Services/infor-api.service';

@Component({
  selector: 'app-inventory-lot',
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
  templateUrl: './inventory-lot.component.html',
  styleUrl: './inventory-lot.component.scss'
})
export class InventoryLotComponent implements OnInit {
  displayedColumns: string[] = [
    'item', 'itemDescription', 'revision', 'expDate', 'derQtyOnHand', 'derExtUnitCost', 'itemU_M', 'lot'
  ];
  dataSource = new MatTableDataSource<InventoryLot>([]);
  isLoading = false;
  rowCount = 10;
  rowCountOptions = [10, 20, 50, 100];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private inforItemsService: InforApiService,
    private dialogService: DialogService,
    private tokenService: TokenService
  ) { }

  ngOnInit(): void {
    const hasToken = this.tokenService.checkInforTokensAndPrompt();
    if (hasToken) {
      this.loadInventoryLots();
    } else {
      this.dialogService.showMessage('Missing Infor Token. Generate or refresh new tokens');
    }
  }

  loadInventoryLots(): void {
    this.isLoading = true;
    this.inforItemsService.getInventoryLots(this.rowCount).subscribe({
      next: (items) => {
        this.dataSource.data = items;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load inventory lots:', err);
        this.isLoading = false;
      }
    });
  }

  onRowCountChange(count: number): void {
    this.rowCount = count;
    this.loadInventoryLots();
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value.trim().toLowerCase();
    this.dataSource.filter = filterValue;
  }
}

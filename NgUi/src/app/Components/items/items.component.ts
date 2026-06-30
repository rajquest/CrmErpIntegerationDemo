import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { InforApiService } from '../../Services/infor-api.service';
import { DialogService } from '../../Services/dialog.service';
import { TokenService } from '../../Services/token.service';
import { Item } from '../../Models/items';

@Component({
  selector: 'app-items',
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
  templateUrl: './items.component.html',
  styleUrl: './items.component.scss'
})
export class ItemsComponent implements OnInit {
  displayedColumns: string[] = ['Item', 'Description', 'UM', 'UnitCost', 'ProductCode', 'Revision', 'DerQtyOnHand'];
  dataSource = new MatTableDataSource<Item>([]);
  isLoading = false;
  rowCount = 10;
  rowCountOptions = [10, 20, 50, 100];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private inforItemsService: InforApiService,
    private dialogService: DialogService,
    private tokenService: TokenService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    const isToken = this.tokenService.checkInforTokensAndPrompt();
    if (isToken) {
      this.loadItems();
    } else {
      this.dialogService.showMessage('Missing Infor Token. Generate or refresh new tokens');
    }
  }

  loadItems(): void {
    this.isLoading = true;
    this.inforItemsService.getSLItems(this.rowCount).subscribe({
      next: (items) => {
        this.dataSource.data = items;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load items:', err);
        this.isLoading = false;
      }
    });
  }

  onRowCountChange(count: number): void {
    this.rowCount = count;
    this.loadItems();
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
}

import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormControl } from '@angular/forms';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatSelectModule, MatSelectChange } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialog } from '@angular/material/dialog';
import { Observable, BehaviorSubject, combineLatest } from 'rxjs';
import { finalize, map, startWith, switchMap } from 'rxjs/operators';
import { InforApiService } from '../../Services/infor-api.service';
import { DialogService } from '../../Services/dialog.service';
import { TokenService } from '../../Services/token.service';
import { LookupService } from '../../Services/lookup.service';
import { ExcelExportService } from '../../Services/excel-export.service';
import { DateService } from '../../Services/date.service';
import { ItemLotLocation } from '../../Models/item-lot-location';
import { ItemList } from '../../Models/item-list';
import { Warehouse } from '../../Models/warehouse';
import { DateRange } from '../../Models/date-range';
import { Utils } from '../../core/utils';
import { SerialNumbersDialogComponent } from '../serial-numbers-dialog/serial-numbers-dialog.component';
import { MessageDialogComponent } from '../message-dialog/message-dialog.component';

@Component({
  selector: 'app-item-lot-location',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatAutocompleteModule,
    MatSelectModule,
    MatCheckboxModule,
    MatIconModule,
    MatDatepickerModule
  ],
  templateUrl: './item-lot-location.component.html',
  styleUrl: './item-lot-location.component.scss'
})
export class ItemLotLocationComponent implements OnInit {
  Utils = Utils;

  displayedColumns: string[] = [
    'item', 'itemDescription', 'loc', 'locationDescription', 'qtyOnHand', 'itemUM',
    'whse', 'lot', 'lotRevision', 'wbLotExpDate', 'serialNumbers', 'expiryDates'
  ];

  dataSource = new MatTableDataSource<ItemLotLocation>([]);
  isLoading = false;
  rowCount = 10;
  rowCountOptions = [10, 20, 50, 100, 500, 1000];
  selectedItem: string | null = null;
  startDate: Date | null = null;
  endDate: Date | null = null;
  isSerialNbrTracked: string = '';
  selectedWarehouse: string | null = null;

  items$ = new BehaviorSubject<ItemList[]>([]);
  filteredItems$!: Observable<ItemList[]>;
  itemCtrl = new FormControl<ItemList | string>('');
  warehouses: Warehouse[] = [];
  isExpiryDateConflict: boolean = false;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private dialog: MatDialog,
    private lookupService: LookupService,
    private excelService: ExcelExportService,
    private inforApiService: InforApiService,
    private dialogService: DialogService,
    private tokenService: TokenService
  ) { }

  ngOnInit(): void {
    const isValidToken = this.tokenService.checkInforTokensAndPrompt();
    if (!isValidToken) {
      this.dialogService.showMessage('Invalid token');
    }
    this.setupFilterOptions();
    this.loadDropdownValues();
  }

  loadDropdownValues() {
    this.lookupService.getSLItemsList().pipe(
      switchMap(items => {
        this.items$.next(items);
        return this.lookupService.getWarehouses();
      })
    ).subscribe({
      next: warehouses => {
        this.warehouses = warehouses;
      },
      error: err => {
        console.error('Initialization failed:', err);
      }
    });
  }

  private setupFilterOptions(): void {
    this.itemCtrl.valueChanges.subscribe(value => {
      if (!value) {
        this.selectedItem = null;
      }
    });

    this.filteredItems$ = combineLatest([
      this.items$,
      this.itemCtrl.valueChanges.pipe(startWith(''))
    ]).pipe(
      map(([items, value]) => {
        const search = typeof value === 'string'
          ? value.toLowerCase()
          : value?.item?.toLowerCase() ?? '';
        if (!search) return items;
        return items.filter((i: ItemList) =>
          i.item.toLowerCase().includes(search) ||
          i.description.toLowerCase().includes(search)
        );
      })
    );
  }

  displayItem(item: ItemList | string | null): string {
    if (!item) return '';
    if (typeof item === 'string') return item;
    return `${item.item} - ${item.description}`;
  }

  onExpiryConflictChange(event: any) { }

  exportDataset() {
    const data = this.dataSource.data;
    this.excelService.exportToExcel(data, 'item_lot_serial_numbers');
  }

  onWarehouseChange(event: MatSelectChange): void {
    this.selectedWarehouse = event.value;
  }

  onItemSelected(item: ItemList): void {
    this.selectedItem = item.item;
  }

  onSerialChange(event: MatSelectChange): void {
    this.isSerialNbrTracked = event.value;
  }

  onItemClick(row: ItemLotLocation): void {
    this.dialog.open(SerialNumbersDialogComponent, {
      width: '800px',
      data: { selectedRow: row }
    });
  }

  onSearch() {
    this.loadLotLocations();
  }

  loadLotLocations(): void {
    this.isLoading = true;
    const filter = this.buildFilter();
    const range: DateRange = DateService.getValidDateRange(this.startDate, this.endDate);

    this.inforApiService.getItemLotLocationWithSerialNbrsMapping(0, filter, range.start, range.end, this.isExpiryDateConflict)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (items) => {
          this.dataSource.data = items;
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        },
        error: (err) => {
          console.error('Failed to load lot locations:', err);
        }
      });
  }

  private buildFilter(): string | undefined {
    const filters: string[] = [];

    if (this.selectedItem && this.selectedItem !== '') {
      filters.push(`Item='${this.selectedItem}'`);
    }
    if (this.isSerialNbrTracked && this.isSerialNbrTracked !== 'both') {
      const serialValue = this.isSerialNbrTracked === 'yes' ? '1' : '0';
      filters.push(`ItemSerialTracked='${serialValue}'`);
    }
    if (this.selectedWarehouse) {
      filters.push(`Whse='${this.selectedWarehouse}'`);
    }

    return filters.length > 0 ? filters.join(' AND ') : undefined;
  }

  openSerialDialog(msgPayload: string): void {
    this.dialog.open(MessageDialogComponent, {
      width: '600px',
      data: { message: msgPayload }
    });
  }
}

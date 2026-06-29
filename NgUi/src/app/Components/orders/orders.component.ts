import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
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
import { MatDatepickerModule } from '@angular/material/datepicker';
import { InforApiService } from '../../Services/infor-api.service';
import { DialogService } from '../../Services/dialog.service';
import { TokenService } from '../../Services/token.service';
import { LookupService } from '../../Services/lookup.service';
import { DateService } from '../../Services/date.service';
import { CustomerOrder } from '../../Models/customer-order';
import { DateRange } from '../../Models/date-range';
import { OrderNumber } from '../../Models/order-number';
import { Utils } from '../../core/utils';
import { BehaviorSubject, Observable, combineLatest, finalize, map, startWith } from 'rxjs';

@Component({
  selector: 'app-orders',
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
    MatDatepickerModule
  ],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss'
})
export class OrdersComponent implements OnInit, AfterViewInit {
  Utils = Utils;
  displayedColumns: string[] = [
    'coNum', 'derCustNum', 'coType', 'derCustPo', 'custSeq', 'item', 'qtyOrdered', 'coOrderDate', 'shipDate', 'stat', 'coPrice'
  ];
  dataSource = new MatTableDataSource<CustomerOrder>([]);
  isLoading = false;
  rowCount = 10;
  rowCountOptions = [10, 20, 50, 100, 500, 1000];

  startDate: Date | null = null;
  endDate: Date | null = null;

  orderCtrl = new FormControl<string | OrderNumber>('');
  orders$ = new BehaviorSubject<OrderNumber[]>([]);
  filteredOrders$!: Observable<OrderNumber[]>;
  selectedOrder: OrderNumber | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private inforItemsService: InforApiService,
    private dialogService: DialogService,
    private lookupService: LookupService,
    private tokenService: TokenService
  ) { }

  ngOnInit(): void {
    const isToken = this.tokenService.checkInforTokensAndPrompt();
    if (isToken) {
      this.setupFilterOptions();
      this.loadDropdownValues();
    } else {
      this.dialogService.showMessage('Missing Infor Token. Generate or refresh new tokens');
    }
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  loadDropdownValues() {
    this.lookupService.getOrderNumbers().subscribe({
      next: (orders) => {
        this.orders$.next(orders);
      },
      error: err => {
        console.error('Order load failed:', err);
      }
    });
  }

  private setupFilterOptions(): void {
    this.orderCtrl.valueChanges.subscribe(value => {
      if (!value) {
        this.selectedOrder = null;
      }
    });

    this.filteredOrders$ = combineLatest([
      this.orders$,
      this.orderCtrl.valueChanges.pipe(startWith(''))
    ]).pipe(
      map(([orders, value]) => {
        const search = typeof value === 'string'
          ? value.toLowerCase()
          : value?.coNum?.toLowerCase() ?? '';
        if (!search) return orders;
        return orders.filter((o: OrderNumber) => o.coNum.toLowerCase().includes(search));
      })
    );
  }

  onOrderSelected(order: OrderNumber) {
    this.selectedOrder = order;
  }

  displayOrder(order: OrderNumber | string | null): string {
    if (!order) return '';
    if (typeof order === 'string') return order;
    return order.coNum;
  }

  onSearch() {
    this.loadCustomerOrders();
  }

  loadCustomerOrders(): void {
    this.isLoading = true;
    const filter = this.buildFilter();
    this.inforItemsService.getCustomerOrders(0, filter)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (data) => {
          this.dataSource.data = data;
        },
        error: (err) => {
          console.error('Failed to load items:', err);
        }
      });
  }

  private buildFilter(): string | undefined {
    const filters: string[] = [];
    const range: DateRange = DateService.getValidDateRange(this.startDate, this.endDate);

    filters.push(`Item like 'CRDLA%'`);

    if (range.start && range.end) {
      filters.push(`CoOrderDate>='${range.start}' and CoOrderDate<='${range.end}'`);
    }

    if (this.selectedOrder) {
      filters.push(`CoNum='${this.selectedOrder.coNum}'`);
    }

    return filters.length > 0 ? filters.join(' AND ') : undefined;
  }
}

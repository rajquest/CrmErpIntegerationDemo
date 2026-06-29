import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./Components/get-token/get-token.component').then(m => m.GetTokenComponent) },
  { path: 'dashboard', loadComponent: () => import('./Components/dashboard/dashboard.component').then(m => m.DashboardComponent) },
  { path: 'getToken', loadComponent: () => import('./Components/get-token/get-token.component').then(m => m.GetTokenComponent) },
  { path: 'items', loadComponent: () => import('./Components/items/items.component').then(m => m.ItemsComponent) },
  { path: 'orders', loadComponent: () => import('./Components/orders/orders.component').then(m => m.OrdersComponent) },
  { path: 'inventoryLots', loadComponent: () => import('./Components/inventory-lot/inventory-lot.component').then(m => m.InventoryLotComponent) },
  { path: 'itemLocations', loadComponent: () => import('./Components/item-lot-location/item-lot-location.component').then(m => m.ItemLotLocationComponent) },
  { path: 'materialTransactions', loadComponent: () => import('./Components/material-transaction-summary/material-transaction-summary.component').then(m => m.MaterialTransactionSummaryComponent) },
  { path: 'taxCodes', loadComponent: () => import('./Components/tax-codes/tax-codes.component').then(m => m.TaxCodesComponent) },
  { path: 'jobVariance', loadComponent: () => import('./Components/job-variance/job-variance.component').then(m => m.JobVarianceComponent) },
  { path: 'invoicedOrders', loadComponent: () => import('./Components/invoice-orders/invoice-orders.component').then(m => m.InvoiceOrdersComponent) },
  { path: 'sfTables', loadComponent: () => import('./Components/salesforce-tables/salesforce-tables.component').then(m => m.SalesforceTablesComponent) },
  { path: 'sfDevices', loadComponent: () => import('./Components/device-data/device-data.component').then(m => m.DeviceDataComponent) }
];

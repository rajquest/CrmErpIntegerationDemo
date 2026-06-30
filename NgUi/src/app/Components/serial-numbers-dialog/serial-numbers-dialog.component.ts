import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { ItemLotLocation } from '../../Models/item-lot-location';
import { SerialNumberItem } from '../../Models/serial-number-item';
import { InforApiService } from '../../Services/infor-api.service';
import { Utils } from '../../core/utils';

@Component({
  selector: 'app-serial-numbers-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatTableModule, MatButtonModule],
  templateUrl: './serial-numbers-dialog.component.html'
})
export class SerialNumbersDialogComponent implements OnInit {
  Utils = Utils;
  selectedRow!: ItemLotLocation;
  displayedColumns: string[] = ['Item', 'SerNum', 'Stat', 'Loc', 'Lot', 'ExpDate', 'Whse'];
  dataSource = new MatTableDataSource<SerialNumberItem>();

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { selectedRow: ItemLotLocation },
    private dialogRef: MatDialogRef<SerialNumbersDialogComponent>,
    private erpService: InforApiService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.selectedRow = this.data.selectedRow;
    this.loadSerialNumbers();
  }

  loadSerialNumbers() {
    const item = this.selectedRow.item;
    const lot = this.selectedRow.lot;
    const filter = `Item='${item}' and Lot='${lot}'`;

    this.erpService.getSerialNumbers(0, filter).subscribe({
      next: result => {
        this.dataSource.data = result;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error('Failed to load serials', err);
      }
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}

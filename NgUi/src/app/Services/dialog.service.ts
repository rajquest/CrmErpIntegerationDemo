import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MessageDialogComponent } from '../Components/message-dialog/message-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class DialogService {
  constructor(private dialog: MatDialog) { }

  showMessage(message: string): void {
    this.dialog.open(MessageDialogComponent, {
      width: '400px',
      height: '300px',
      data: { message },
      panelClass: 'centered-dialog',
      disableClose: false,
      hasBackdrop: true
    });
  }
}

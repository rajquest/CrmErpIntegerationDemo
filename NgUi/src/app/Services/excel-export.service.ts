import { DatePipe } from '@angular/common';
import { Injectable } from '@angular/core';
import * as XLSX from 'xlsx';

@Injectable({
  providedIn: 'root',
})
export class ExcelExportService {
  constructor(private datePipe: DatePipe) { }

  exportToExcel<T>(data: T[], fileName: string, sheetName: string = 'Sheet1'): void {

    if (!data || data.length === 0) {
      console.warn('No data to export');
      return;
    }

    const formattedData = data.map(row => {
      const formattedRow: any = {};

      Object.keys(row as any).forEach(key => {
        let value: any = (row as any)[key];

        if (value === null || value === undefined) {
          formattedRow[key] = value;
          return;
        }

        if (!isNaN(value) && value !== '' && typeof value !== 'boolean') {
          const num = Number(value);
          formattedRow[key] = num % 1 === 0 ? num : Number(num.toFixed(2));
          return;
        }

        if (typeof value === 'string' && /^\d{8}\s\d{2}:\d{2}:\d{2}/.test(value)) {
          const datePart = value.substring(0, 8);
          const year = datePart.substring(0, 4);
          const month = datePart.substring(4, 6);
          const day = datePart.substring(6, 8);
          formattedRow[key] = `${month}/${day}/${year}`;
          return;
        }

        formattedRow[key] = value;
      });

      return formattedRow;
    });

    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(formattedData);
    const workbook: XLSX.WorkBook = XLSX.utils.book_new();

    XLSX.utils.book_append_sheet(workbook, worksheet, sheetName);

    const formattedDate = this.datePipe.transform(new Date(), 'yyyy_MM_dd')
      ?? Date.now().toString();

    XLSX.writeFile(workbook, `${fileName}_${formattedDate}.xlsx`);
  }
}

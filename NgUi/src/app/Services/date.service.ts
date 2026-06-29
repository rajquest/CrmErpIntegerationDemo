import { Injectable } from '@angular/core';
import { DateRange } from '../Models/date-range';

@Injectable({
  providedIn: 'root',
})
export class DateService {

  static getValidDateRange(startDate?: string | Date | null, endDate?: string | Date | null): DateRange {

    if (!startDate || !endDate) {
      return { start: '', end: '' };
    }

    const start = new Date(startDate);
    const end = new Date(endDate);

    if (isNaN(start.getTime()) || isNaN(end.getTime()) || start > end) {
      return { start: '', end: '' };
    }

    return {
      start: start.toISOString().split('T')[0],
      end: end.toISOString().split('T')[0]
    };
  }
}

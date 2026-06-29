export class Utils {
  static parseInforDate(value: string | null): Date | null {
    if (!value) return null;
    const iso = value.replace(/^(\d{4})(\d{2})(\d{2})\s/, '$1-$2-$3T');
    return new Date(iso);
  }

  static toInteger(value: string | number | null | undefined): number {
    if (value === null || value === undefined || value === '') return 0;
    const num = typeof value === 'string' ? parseFloat(value) : value;
    if (isNaN(num)) return 0;
    return Math.trunc(num);
  }
}

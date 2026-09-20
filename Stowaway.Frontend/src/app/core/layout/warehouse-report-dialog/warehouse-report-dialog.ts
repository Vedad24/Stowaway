import { Component, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule, MatCheckboxChange } from '@angular/material/checkbox';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatRadioModule } from '@angular/material/radio';

export interface WarehouseReportDialogData {
  warehouseName: string;
}

export type WarehouseReportType = 'containers' | 'items' | 'both';

export interface WarehouseReportDialogResult {
  type: WarehouseReportType;
  containerColumns: string[];
  itemColumns: string[];
}

export interface ReportColumnOption {
  key: string;
  label: string;
}

// Keys mirror Stowaway.Backend/Stowaway.Application/Abstractions/Reporting/WarehouseReportColumns.cs -
// keep in sync. "Name" isn't listed since it's always included in both tables.
export const CONTAINER_COLUMN_OPTIONS: ReportColumnOption[] = [
  { key: 'location', label: 'Location' },
  { key: 'type', label: 'Type' },
  { key: 'items', label: 'Items' },
  { key: 'containers', label: 'Containers' },
  { key: 'status', label: 'Status' },
];

export const ITEM_COLUMN_OPTIONS: ReportColumnOption[] = [
  { key: 'container', label: 'Container' },
  { key: 'quantity', label: 'Quantity' },
  { key: 'supplier', label: 'Supplier' },
  { key: 'tags', label: 'Tags' },
];

@Component({
  selector: 'app-warehouse-report-dialog',
  standalone: true,
  imports: [ReactiveFormsModule, MatDialogModule, MatRadioModule, MatCheckboxModule, MatButtonModule],
  templateUrl: './warehouse-report-dialog.html',
  styleUrl: './warehouse-report-dialog.css',
})
export class WarehouseReportDialog {
  readonly dialogRef = inject(MatDialogRef<WarehouseReportDialog>);
  readonly data = inject(MAT_DIALOG_DATA) as WarehouseReportDialogData;

  readonly reportType = new FormControl<WarehouseReportType>('both', { nonNullable: true });

  readonly containerColumnOptions = CONTAINER_COLUMN_OPTIONS;
  readonly itemColumnOptions = ITEM_COLUMN_OPTIONS;

  readonly selectedContainerColumns = signal(new Set(CONTAINER_COLUMN_OPTIONS.map(c => c.key)));
  readonly selectedItemColumns = signal(new Set(ITEM_COLUMN_OPTIONS.map(c => c.key)));

  toggleContainerColumn(key: string, event: MatCheckboxChange): void {
    this.selectedContainerColumns.update(current => {
      const next = new Set(current);
      event.checked ? next.add(key) : next.delete(key);
      return next;
    });
  }

  toggleItemColumn(key: string, event: MatCheckboxChange): void {
    this.selectedItemColumns.update(current => {
      const next = new Set(current);
      event.checked ? next.add(key) : next.delete(key);
      return next;
    });
  }

  canGenerate(): boolean {
    const needsContainerColumns = this.reportType.value !== 'items';
    const needsItemColumns = this.reportType.value !== 'containers';

    if (needsContainerColumns && this.selectedContainerColumns().size === 0) {
      return false;
    }
    if (needsItemColumns && this.selectedItemColumns().size === 0) {
      return false;
    }
    return true;
  }

  generate(): void {
    const result: WarehouseReportDialogResult = {
      type: this.reportType.value,
      containerColumns: [...this.selectedContainerColumns()],
      itemColumns: [...this.selectedItemColumns()],
    };
    this.dialogRef.close(result);
  }
}

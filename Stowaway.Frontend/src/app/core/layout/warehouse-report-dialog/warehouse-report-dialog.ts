import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatRadioModule } from '@angular/material/radio';

export interface WarehouseReportDialogData {
  warehouseName: string;
}

export type WarehouseReportType = 'containers' | 'items' | 'both';

@Component({
  selector: 'app-warehouse-report-dialog',
  standalone: true,
  imports: [FormsModule, MatDialogModule, MatRadioModule, MatButtonModule],
  templateUrl: './warehouse-report-dialog.html',
  styleUrl: './warehouse-report-dialog.css',
})
export class WarehouseReportDialog {
  readonly dialogRef = inject(MatDialogRef<WarehouseReportDialog>);
  readonly data = inject(MAT_DIALOG_DATA) as WarehouseReportDialogData;

  reportType = signal<WarehouseReportType>('both');

  generate(): void {
    this.dialogRef.close(this.reportType());
  }
}

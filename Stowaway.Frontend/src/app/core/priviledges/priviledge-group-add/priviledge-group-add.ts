import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { PriviledgeGroupService } from '../../../services/storage-identity/priviledge-group/priviledge-group-service';

@Component({
  selector: 'app-priviledge-group-add',
  standalone: true,
  imports: [FormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './priviledge-group-add.html',
  styleUrl: './priviledge-group-add.css',
})
export class PriviledgeGroupAdd {
  readonly dialogRef = inject(MatDialogRef<PriviledgeGroupAdd>);
  private readonly priviledgeGroupService = inject(PriviledgeGroupService);
  private readonly data = inject(MAT_DIALOG_DATA) as { warehouseId: number | null } | undefined;

  name = '';
  isSubmitting = false;

  submit(): void {
    if (!this.name.trim()) {
      return;
    }

    this.isSubmitting = true;

    const payload = {
      name: this.name.trim(),
      warehouseId: this.data?.warehouseId ?? 0,
      privilegeIds: [],
    };

    this.priviledgeGroupService.create(payload).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.dialogRef.close(true);
      },
      error: () => {
        this.isSubmitting = false;
      },
    });
  }
}

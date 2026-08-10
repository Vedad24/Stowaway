import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatRadioModule } from '@angular/material/radio';
import { ContainerApiService } from '../../../services/storage/container/container';
import { ContainerTreeService, ContainerOption } from '../../../services/storage/container/container-tree';
import { ItemApiService } from '../../../services/storage/item/item';
import { ListItemQuery } from '../../../services/storage/item/item.model';
import { extractErrorMessage } from '../../../models/http-error';

export interface ContainerDeleteDialogData {
  id: number;
  name: string;
  warehouseId: number;
}

type DeleteMode = 'deleteContents' | 'moveContents';

@Component({
  selector: 'app-container-delete',
  standalone: true,
  imports: [FormsModule, MatDialogModule, MatFormFieldModule, MatSelectModule, MatRadioModule, MatButtonModule],
  templateUrl: './container-delete.html',
  styleUrl: './container-delete.css',
})
export class ContainerDelete implements OnInit {
  readonly dialogRef = inject(MatDialogRef<ContainerDelete>);
  readonly data = inject(MAT_DIALOG_DATA) as ContainerDeleteDialogData;
  private readonly containerService = inject(ContainerApiService);
  private readonly containerTree = inject(ContainerTreeService);
  private readonly itemService = inject(ItemApiService);

  isLoading = signal(true);
  childContainerCount = signal(0);
  childItemCount = signal(0);
  readonly hasContents = computed(() => this.childContainerCount() > 0 || this.childItemCount() > 0);

  mode = signal<DeleteMode>('deleteContents');
  moveTargetId = signal<number | null>(null);
  moveOptions = signal<ContainerOption[]>([]);
  isLoadingMoveOptions = signal(false);

  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  readonly canConfirm = computed(() => {
    if (!this.hasContents()) {
      return true;
    }
    return this.mode() === 'deleteContents' || this.moveTargetId() != null;
  });

  ngOnInit(): void {
    const itemQuery = new ListItemQuery();
    itemQuery.containerId = this.data.id;

    Promise.all([
      firstValueFrom(this.containerService.list({ warehouseId: this.data.warehouseId, parentContainerId: this.data.id })),
      firstValueFrom(this.itemService.list(itemQuery)),
    ]).then(([childContainers, itemResponse]) => {
      this.childContainerCount.set(childContainers.length);
      this.childItemCount.set((itemResponse.items ?? []).length);
      this.isLoading.set(false);
    }).catch(() => {
      this.isLoading.set(false);
      this.errorMessage.set('Unable to check the container contents.');
    });
  }

  selectMode(mode: DeleteMode): void {
    this.mode.set(mode);
    if (mode !== 'moveContents' || this.moveOptions().length > 0 || this.isLoadingMoveOptions()) {
      return;
    }

    this.isLoadingMoveOptions.set(true);
    this.containerTree.loadOptions(this.data.warehouseId, this.data.id).then(options => {
      this.moveOptions.set(options);
      this.isLoadingMoveOptions.set(false);
      if (this.moveTargetId() == null && options.length) {
        this.moveTargetId.set(options[0].id);
      }
    }).catch(() => {
      this.isLoadingMoveOptions.set(false);
      this.errorMessage.set('Unable to load other containers.');
    });
  }

  confirm(): void {
    if (!this.canConfirm()) {
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const request = !this.hasContents()
      ? undefined
      : this.mode() === 'deleteContents'
        ? { deleteContents: true }
        : { moveContentsToContainerId: this.moveTargetId()! };

    this.containerService.delete(this.data.id, request).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(extractErrorMessage(err, 'Unable to delete the container. Please try again.'));
      },
    });
  }
}

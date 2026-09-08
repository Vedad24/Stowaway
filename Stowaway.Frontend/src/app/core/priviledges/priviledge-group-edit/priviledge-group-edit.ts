import { CommonModule, Location } from '@angular/common';
import { ChangeDetectorRef, Component, inject, OnInit, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { PriviledgeGroupService } from '../../../services/storage-identity/priviledge-group/priviledge-group-service';
import { PriviledgesService } from '../../../services/storage-identity/priviledges/priviledges-service';
import { ListPriviledgeGroupQueryDto, UpdatePriviledgeGroupCommand } from '../../../services/storage-identity/priviledge-group/priviledge-group-service.models';
import { ListPriviledgesQueryDto } from '../../../services/storage-identity/priviledges/priviledges-service.models';
import { PriviledgeGroupAdd } from '../priviledge-group-add/priviledge-group-add';
import { ConfirmDialog } from '../../../shared/confirm-dialog/confirm-dialog';
import { ActivatedRoute } from '@angular/router';
import { WarehouseApiService } from '../../../services/storage/warehouse/warehouse';

interface PrivilegeToggleOption {
  id: number;
  name: string;
  enabled: boolean;
}

@Component({
  selector: 'app-priviledge-group-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatListModule, MatCardModule, MatButtonModule, MatIconModule, MatTooltipModule, MatSlideToggleModule, MatDividerModule, MatFormFieldModule, MatInputModule],
  templateUrl: './priviledge-group-edit.html',
  styleUrl: './priviledge-group-edit.css',
})
export class PriviledgeGroupEdit implements OnInit {
  private readonly priviledgeGroupService = inject(PriviledgeGroupService);
  private readonly priviledgesService = inject(PriviledgesService);
  private readonly warehouseApiService = inject(WarehouseApiService);
  private readonly dialog = inject(MatDialog);
  private readonly route = inject(ActivatedRoute);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly location = inject(Location);

  groups: ListPriviledgeGroupQueryDto[] = [];
  privileges: ListPriviledgesQueryDto[] = [];

  selectedGroupId: number | null = null;
  selectedPrivileges: PrivilegeToggleOption[] = [];
  isSaving = signal(false);

  warehouseName = new FormControl({ value: '', disabled: true }, { nonNullable: true });
  isEditingName = false;
  isSavingName = signal(false);

  ngOnInit(): void {
    this.loadGroups();
    this.loadPrivileges();
    this.loadWarehouseName();
  }

  goBack(): void {
    this.location.back();
  }

  loadWarehouseName(): void {
    const warehouseId = this.warehouseId;
    if (!warehouseId) {
      return;
    }
    this.warehouseApiService.getById(warehouseId).subscribe({
      next: (response) => {
        this.warehouseName.setValue(response.name);
        this.cdr.detectChanges();
      },
    });
  }

  toggleNameEdit(): void {
    if (!this.isEditingName) {
      this.isEditingName = true;
      this.warehouseName.enable();
      return;
    }
    this.saveWarehouseName();
  }

  saveWarehouseName(): void {
    const warehouseId = this.warehouseId;
    if (!warehouseId) {
      return;
    }

    this.isSavingName.set(true);
    this.warehouseApiService.updateName(warehouseId, { name: this.warehouseName.value }).subscribe({
      next: (response) => {
        this.warehouseName.setValue(response.name);
        this.warehouseName.disable();
        this.isEditingName = false;
        this.isSavingName.set(false);
      },
      error: () => {
        this.isSavingName.set(false);
      },
    });
  }

  get warehouseId(): number | null {
    const warehouseId = this.route.snapshot.paramMap.get('warehouseId');
    return warehouseId ? Number(warehouseId) : null;
  }

  get selectedGroup(): ListPriviledgeGroupQueryDto | undefined {
    return this.groups.find((group) => group.id === this.selectedGroupId);
  }

  loadGroups(): void {
    const warehouseId = this.warehouseId;
    this.priviledgeGroupService.list(warehouseId ?? 0).subscribe({
      next: (response) => {
        this.groups = response ?? [];
        if (!this.selectedGroupId && this.groups.length) {
          this.selectGroup(this.groups[0]);
        }
        this.cdr.detectChanges();
      },
    });
  }

  loadPrivileges(): void {
    this.priviledgesService.list().subscribe({
      next: (response) => {
        this.privileges = response ?? [];
        if (this.selectedGroup) {
          this.syncSelectedPrivileges();
        }
      },
    });
  }

  selectGroup(group: ListPriviledgeGroupQueryDto): void {
    this.selectedGroupId = group.id;
    this.syncSelectedPrivileges();
  }

  syncSelectedPrivileges(): void {
    const selectedGroup = this.selectedGroup;
    this.selectedPrivileges = (this.privileges ?? []).map((privilege) => ({
      id: privilege.id,
      name: privilege.name,
      enabled: !!selectedGroup && selectedGroup.priviledgeIds.includes(privilege.id),
    }));
  }

  togglePrivilege(privilegeId: number, checked: boolean): void {
    const privilege = this.selectedPrivileges.find((item) => item.id === privilegeId);
    if (privilege) {
      privilege.enabled = checked;
    }
  }

  openAddDialog(): void {
    const dialogRef = this.dialog.open(PriviledgeGroupAdd, {
      width: '420px',
      disableClose: false,
      data: { warehouseId: this.warehouseId },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        
        this.loadGroups();
      }
    });
  }

  deleteGroup(group: ListPriviledgeGroupQueryDto, event: Event): void {
    event.stopPropagation();

    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '380px',
      data: {
        title: 'Delete privilege group',
        message: `Are you sure you want to delete "${group.name}"? This cannot be undone.`,
        confirmLabel: 'Delete',
        danger: true,
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.priviledgeGroupService.delete(group.id).subscribe({
        next: () => {
          if (this.selectedGroupId === group.id) {
            this.selectedGroupId = null;
          }
          this.loadGroups();
        },
      });
    });
  }

  save(): void {
    if (!this.selectedGroup) {
      return;
    }

    this.isSaving.set(true);

    const payload = {
      priviledgeId: this.selectedGroup.id,
      name: this.selectedGroup.name,
      warehouseId: this.selectedGroup.warehouseId,
      priviledgeIds: this.selectedPrivileges.filter((item) => item.enabled).map((item) => item.id),
    } as UpdatePriviledgeGroupCommand;

    this.priviledgeGroupService.update(this.selectedGroup.id, payload).subscribe({
      next: (response) => {
        this.isSaving.set(false);

      },
      error: () => {
        this.isSaving.set(false);
        
      },
    });
  }
}

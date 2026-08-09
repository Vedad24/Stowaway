import { Component, inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { PriviledgeGroupService } from '../../../../services/storage-identity/priviledge-group/priviledge-group-service';
import { ListPriviledgeGroupQueryDto } from '../../../../services/storage-identity/priviledge-group/priviledge-group-service.models';
import { WarehouseApiService } from '../../../../services/storage/warehouse/warehouse';
import { UserService } from '../../../../services/identity/user/user-service';
import { AutocompleteComponent, IOptionsInfo } from '../../../../shared/autocomplete-component/autocomplete-component';

export interface EditWarehouseUserPriviledgeGroupData {
  warehouseId: number;
  userId: number;
}

@Component({
  selector: 'app-edit-warehouse-user-priviledge-group',
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatDialogModule, AutocompleteComponent],
  templateUrl: './edit-warehouse-user-priviledge-group.html',
  styleUrl: './edit-warehouse-user-priviledge-group.css',
})
export class EditWarehouseUserPriviledgeGroup {
  private priviledgeGroupService = inject(PriviledgeGroupService);
  private warehouseService = inject(WarehouseApiService);
  private userService = inject(UserService);
  private dialogRef = inject(MatDialogRef<EditWarehouseUserPriviledgeGroup>);
  data = inject<EditWarehouseUserPriviledgeGroupData>(MAT_DIALOG_DATA);

  warehouseName = signal('');
  userName = signal('');
  priviledgeGroups = signal<ListPriviledgeGroupQueryDto[]>([]);
  priviledgeGroupInfo: IOptionsInfo = { displayName: 'name' };
  priviledgeGroupControl = new FormControl<ListPriviledgeGroupQueryDto | null>(null);

  constructor() {
    this.priviledgeGroupService.list(this.data.warehouseId).subscribe(
      (response) => {
        this.priviledgeGroups.set(response);
      }
    );

    this.warehouseService.getById(this.data.warehouseId).subscribe(
      (response) => {
        this.warehouseName.set(response.name);
      }
    );

    this.userService.get(this.data.userId).subscribe(
      (response) => {
        this.userName.set(`${response.firstName} ${response.lastName}`);
      }
    );
  }

  save(): void {
    const priviledgeGroupId = this.priviledgeGroupControl.value?.id;
    if (!priviledgeGroupId) {
      return;
    }

    this.priviledgeGroupService.createUpdateWarehouseUser({
      userId: this.data.userId,
      warehouseId: this.data.warehouseId,
      priviledgeGroupId,
    }).subscribe(
      (response) => {
        this.dialogRef.close(response);
      }
    );
  }

  cancel(): void {
    this.dialogRef.close();
  }
}

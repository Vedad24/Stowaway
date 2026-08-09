import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { WarehouseApiService } from '../../../services/storage/warehouse/warehouse';
import { ListWarehouseQuery, ListWarehouseQueryDto } from '../../../services/storage/warehouse/warehouse.model';
import { PriviledgesService } from '../../../services/storage-identity/priviledges/priviledges-service';
import { UserService } from '../../../services/identity/user/user-service';
import { GetUserById, GetUserByIdOrMailDto, RoleName } from '../../../services/identity/user/user-service.models';
import { BaseListPagedComponent } from '../../base-classes/base-list-paged-component';
import { PaginationTable, TableColumnDef } from '../../../shared/pagination-table/pagination-table';
import { EditWarehouseUserPriviledgeGroup } from './edit-warehouse-user-priviledge-group/edit-warehouse-user-priviledge-group';

@Component({
  selector: 'app-warehouse-user-management',
  imports: [PaginationTable],
  templateUrl: './warehouse-user-management.html',
  styleUrl: './warehouse-user-management.css',
})
/*
  FinalResult of this component: Put(UserId + WarehouseId + PriviledgeGroupId)
  UserId -> FromRoute
  WarehouseId -> From ListButtonClick
  PriviledgeGroupId -> From AutoCompleteComponent
*/
export class WarehouseUserManagement
  extends BaseListPagedComponent<ListWarehouseQueryDto, ListWarehouseQuery>
  implements OnInit
{
  private activatedRoute = inject(ActivatedRoute);
  private warehouseService = inject(WarehouseApiService);
  private priviledgesService = inject(PriviledgesService);
  private userService = inject(UserService);
  private cdr = inject(ChangeDetectorRef);
  private dialog = inject(MatDialog);
  //UserId from url
  user !: GetUserByIdOrMailDto;
  readonly RoleName = RoleName;

  //Column definitions for the warehouse table
  columnDef: TableColumnDef<ListWarehouseQueryDto>[] =
  [
    {
      columnDef: 'name',
      header: 'Name',
      cell: (row: ListWarehouseQueryDto) => `${row.name}`,
    },
    {
      columnDef: 'city',
      header: 'City',
      cell: (row: ListWarehouseQueryDto) => `${row.city}`,
    },
    {
      columnDef: 'address',
      header: 'Address',
      cell: (row: ListWarehouseQueryDto) => `${row.address}`,
    },
    {
      columnDef: 'capacity',
      header: 'Capacity',
      cell: (row: ListWarehouseQueryDto) => `${row.capacity}`,
    },
    {
      columnDef: 'isEnabled',
      header: 'Enabled',
      cell: (row: ListWarehouseQueryDto) => row.isEnabled ? 'Yes' : 'No',
    },
    {
      columnDef: 'btnEditGroup',
      header: 'Edit group',
      type: 'action',
      buttons:
      [{
        type: 'text',
        label: 'Edit group',
        color: '',
        action: (row: ListWarehouseQueryDto) => this.onEditGroup(row)
      }]
    },
  ]

  onEditGroup(warehouse: ListWarehouseQueryDto) {
    this.dialog.open(EditWarehouseUserPriviledgeGroup, {
      width: '480px',
      data: { warehouseId: warehouse.id, userId: this.user.id },
    });
  }

  constructor() {
    super();
    this.request = new ListWarehouseQuery();
    this.request.paging = { page: 1, pageSize: 10 };

    const userId = this.activatedRoute.snapshot.paramMap.get("userId");
    console.log(userId);
    console.log(Number(userId));
    this.userService.get(Number(userId)).subscribe(
      (response) =>
      {
        this.user = response;
        this.cdr.detectChanges();
      }
    )
  }

  ngOnInit() {
    this.loadPagedData();
  }

  protected override loadPagedData(): void {
    this.startLoading();

    this.warehouseService.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err.message);
        this.stopLoading();
        this.cdr.detectChanges();
      }
    });
  }
}

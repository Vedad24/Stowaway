import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { PaginationTable, TableColumnDef } from '../../../shared/pagination-table/pagination-table';
import { CreateUserCommand, ListUserQuery, ListUserQueryDto, RoleName, UpdateUserCommand } from '../../../services/identity/user/user-service.models';
import { UserService } from '../../../services/identity/user/user-service';
import { EmployeeAddEdit } from './employee-add-edit/employee-add-edit';
import { ConfirmDialog } from '../../../shared/confirm-dialog/confirm-dialog';
import { BaseListPagedComponent } from '../../base-classes/base-list-paged-component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-employee-management',
  imports: [PaginationTable, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './employee-management.html',
  styleUrl: './employee-management.css',
})
export class EmployeeManagement extends BaseListPagedComponent<ListUserQueryDto, ListUserQuery> implements OnInit {

  //Services
  private userService = inject(UserService);
  private dialog = inject(MatDialog);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);

  //Search field
  searchField = '';

  //Column definitions for the table
  /*
    First Name
    Last Name
    Email
    Role
    <Edit> button
  */
  columnDef : TableColumnDef<ListUserQueryDto>[] =
  [
    {
      columnDef: 'firstName',
      header: 'First Name',
      cell: (row: ListUserQueryDto) => `${row.firstName}`,
    },
    {
      columnDef: 'lastName',
      header: 'Last Name',
      cell: (row: ListUserQueryDto) => `${row.lastName}`,
    },
    {
      columnDef: 'email',
      header: 'Email',
      cell: (row: ListUserQueryDto) => `${row.email}`,
    },
    {
      columnDef: 'role',
      header: 'Role',
      cell: (row: ListUserQueryDto) => `${RoleName[row.roleId]}`
    },
    {
      columnDef: 'btnEdit',
      header: 'Edit',
      type: 'action',
      buttons:
      [{
        type: 'icon',
        label: 'Edit',
        icon: 'edit',
        color: '',
        action: (row: ListUserQueryDto) => this.onEdit(row)
      },
      {
        type: 'icon',
        label: 'Warehouses',
        icon: 'engineering',
        color: '',
        action: (row: ListUserQueryDto) => this.onWarehouseManage(row)
      }
    ]
    },
    {columnDef: 'btnDelete',
      header: 'Delete',
      type: 'action',
      buttons:
      [
        {
        type: 'icon',
        label: 'Delete',
        icon: 'delete',
        color: '',
        action: (row: ListUserQueryDto) => this.onDelete(row)
      }
      ]
    }
  ]
  onDelete(row : ListUserQueryDto)
  {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '380px',
      data: {
        title: 'Delete employee',
        message: `Are you sure you want to delete "${row.firstName} ${row.lastName}"? This cannot be undone.`,
        confirmLabel: 'Delete',
        danger: true,
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.userService.delete(row.id).subscribe(
        {
          next: () =>
          {
            this.loadPagedData();
          },
          error: () => {console.error("There was an error deleteing the user");}
        }
        )
    });
  }
  onWarehouseManage(row : ListUserQueryDto)
  {
    const id = row.id;
    this.router.navigate(['/employee-management/warehouse-user-manage', id]);
  }

  constructor() {
    super();
    this.request = new ListUserQuery();
    this.request.paging = { page: 1, pageSize: 10 };
  }

  ngOnInit(): void {
    this.loadPagedData();
  }

  protected override loadPagedData(): void {
    this.startLoading();

    this.userService.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error loading data', error);
        this.stopLoading();
        this.cdr.detectChanges();
      }
    });
  }

  onEdit(row: ListUserQueryDto) : void{
    //open dialog with existing employee data
    const dialogRef = this.dialog.open(EmployeeAddEdit, {
      width: '480px',
      data: row,
    });
    //wait for when closed with payload(or false)
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        //Send data to backend to update user
        this.updateEmployee(row.id, result);
      }
    });
  }

  onSearch(): void {
    this.request.search = this.searchField || null;
    this.paging.page = 1;
    this.loadPagedData();
  }

  onAddEmployee(): void {
    //open dialog
    const dialogRef = this.dialog.open(EmployeeAddEdit, {
      width: '480px',
    });
    //wait for when closed with payload(or false)
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        //Send data to backend to create user(idK)
        this.addEmployee(result);
      }
    });
  }

  private addEmployee(resultString: string) {
    const result = JSON.parse(resultString);
    const createPayload : CreateUserCommand =
    {
      email: result.email,
      firstName: result.firstName,
      lastName: result.lastName,
      password: result.password,
      role: {id: Number(result.role.key)}
    }
    this.userService.create(createPayload).subscribe(
      {
        next: () =>
        {
          this.loadPagedData();
        }
      }
      )
  }

  private updateEmployee(id: number, resultString: string) {
    const result = JSON.parse(resultString);
    const updatePayload : UpdateUserCommand =
    {
      id: id,
      email: result.email,
      password: result.password ? result.password : null,
      firstName: result.firstName,
      lastName: result.lastName,
      role: result.role ? {id: Number(result.role.key)} : null,
      isEnabled: null
    }
    this.userService.update(updatePayload).subscribe(
      {
        next: () =>
        {
          this.loadPagedData();
        }
      }
      )
  }

}

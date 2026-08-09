import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { PaginationTable, TableColumnDef } from '../../../shared/pagination-table/pagination-table';
import { CreateUserCommand, ListUserQuery, ListUserQueryDto, RoleName } from '../../../services/identity/user/user-service.models';
import { UserService } from '../../../services/identity/user/user-service';
import { EmployeeAddEdit } from './employee-add-edit/employee-add-edit';
import { BaseListPagedComponent } from '../../base-classes/base-list-paged-component';

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
      }]
    }
  ]

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
    console.error("Not implemented", row.email);
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

  private addEmployee(resultString: any) {
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
        next: (response) =>
        {
          console.log("User created with id:", response);
          this.loadPagedData();
        }
      }
      )
  }

}

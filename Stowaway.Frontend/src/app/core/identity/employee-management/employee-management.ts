import { AfterViewInit, Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { PaginationTable, TableColumnDef } from '../../../shared/pagination-table/pagination-table';
import { CreateUserCommand, ListUserQuery, ListUserQueryDto, ListUserQueryResponse, RoleName } from '../../../services/identity/user/user-service.models';
import { UserService } from '../../../services/identity/user/user-service';
import { PageRequest } from '../../../models/paging/page-request';
import { MatPaginator } from '@angular/material/paginator';
import { EmployeeAddEdit } from './employee-add-edit/employee-add-edit';

@Component({
  selector: 'app-employee-management',
  imports: [PaginationTable, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './employee-management.html',
  styleUrl: './employee-management.css',
})
export class EmployeeManagement implements AfterViewInit {
  
  //Services
  userService = inject(UserService);
  dialog = inject(MatDialog);
  //Raw user data from backend
  rawData = signal<ListUserQueryDto[]>([]); 
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
  onEdit(row: ListUserQueryDto) : void{
    console.error("Not implemented", row.email);
  }
  //Search field
  searchField : string = "";
  //Paginator
  @ViewChild(PaginationTable) paginationTable !: PaginationTable<ListUserQueryDto>
  
  ngAfterViewInit(): void {
    this.loadData();
  }

  loadData(): void {
    //Get data from backend
    //Prepare payload
    const pageNum = this.paginationTable.paginator.pageIndex + 1;
    const pageSize = this.paginationTable.paginator.pageSize;
    const payload : ListUserQuery = {
      search: this.searchField,
      roleId: null,
      paging: new PageRequest()
    }
    this.userService.list(payload).subscribe(
      {
      next: (response) =>
      {
        this.rawData.set(response.items);
      },
      error: (error) =>
      {
        console.error("Error loading data", error);
      }
    }
    )
  }

  onSearch(): void {
    this.paginationTable.paginator.pageIndex = 0;
    this.loadData();
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
        this.AddEmployee(result);  
      }
    });
  }
  AddEmployee(resultString: any) {
    const result = JSON.parse(resultString);
    console.log("form result:", result);
    console.log("role from form", result["role"]);
    const createPayload : CreateUserCommand =
    {
      email: result.email,
      firstName: result.firstName,
      lastName: result.lastName,
      password: result.password,
      role: {id: Number(result.role.key)}
    }
    console.log("create payload:", createPayload);  
    this.userService.create(createPayload).subscribe(
      {
        next: (response) =>
        {
          console.log("User created with id:", response);
          this.loadData();
        }
      }
      )
  }

}


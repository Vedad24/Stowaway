import { AfterViewInit, Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import { PaginationTable, TableColumnDef } from '../../../shared/pagination-table/pagination-table';
import { ListUserQuery, ListUserQueryDto, ListUserQueryResponse, RoleName } from '../../../services/identity/user/user-service.models';
import { UserService } from '../../../services/identity/user/user-service';
import { PageRequest } from '../../../models/paging/page-request';
import { MatPaginator } from '@angular/material/paginator';

@Component({
  selector: 'app-employee-management',
  imports: [PaginationTable],
  templateUrl: './employee-management.html',
  styleUrl: './employee-management.css',
})
export class EmployeeManagement implements AfterViewInit {
  
  //Services
  userService = inject(UserService);
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
    }
  ]
  //Search field
  searchField : string = "";
  //Paginator
  @ViewChild(PaginationTable) paginationTable !: PaginationTable<ListUserQueryDto>
  
  ngAfterViewInit(): void {
    //Get data from backend
    //Prepare payload
    const pageNum = this.paginationTable.paginator.pageIndex + 1;
    const pageSize = this.paginationTable.paginator.pageSize;
    const payload : ListUserQuery = {
      search: this.searchField,
      roleId: null,
      paging: new PageRequest(pageNum, pageSize)
    }
    this.userService.list(payload).subscribe(
      (response) => 
      {
        this.rawData.set(response.items);
      }
    )
  }

}

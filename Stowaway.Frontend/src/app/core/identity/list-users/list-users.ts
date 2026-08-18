import { Component, inject, ViewChild } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { UserService } from '../../../services/identity/user/user-service';
import { catchError, delay, observable, Observable, of, ReplaySubject, tap } from 'rxjs';
import { PageRequest } from '../../../models/paging/page-request';
import { ListUserQueryDto, ListUserQueryResponse } from '../../../services/identity/user/user-service.models';
import { observeNotification } from 'rxjs/internal/Notification';
import { DataSource } from '@angular/cdk/table';
import { CollectionViewer } from '@angular/cdk/collections';
import { PaginationTable } from "../../../shared/pagination-table/pagination-table";
import { error } from 'console';
@Component({
  selector: 'app-list-users',
  imports: [MatTableModule, MatPaginatorModule],
  templateUrl: './list-users.html',
  styleUrl: './list-users.css',
  standalone: true
})
export class ListUsers {
  userService = inject(UserService);
  columns = [
    {
      columnDef: 'id',
      header: 'ID',
      cell: (user: ListUserQueryDto) => `${user.id}`,
    },
    {
      columnDef: 'email',
      header: 'Email',
      cell: (user: ListUserQueryDto) => `${user.email}`,
    },
    {
      columnDef: 'firstName',
      header: 'First Name',
      cell: (user: ListUserQueryDto) => `${user.firstName}`
    },
    {
      columnDef: 'lastName',
      header: 'Last Name',
      cell: (user: ListUserQueryDto) => `${user.lastName}`
    },
  ]
  userColumns = this.columns.map(c => c.columnDef);
  users = new MatTableDataSource<ListUserQueryDto>();
  @ViewChild(MatPaginator) paginator !: MatPaginator;

  ngOnInit() {
      this.refreshUsers();  
  }

  ngAfterViewInit() {
    this.users.paginator = this.paginator;
    
  }

  refreshUsers()
  {
    this.userService.list({search:null, roleId:null, paging: new PageRequest()})
      .pipe(
        tap( (response : ListUserQueryResponse) => {
          delay(0);
        this.users.data = response.items;
        }),
      )
      .subscribe({next: (response) => {},error: error => {}});
  }

}



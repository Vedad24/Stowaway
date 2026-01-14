import { Component, inject, ViewChild } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { UserService } from '../../../services/identity/user/user-service';
import { catchError, observable, Observable, of, ReplaySubject, tap } from 'rxjs';
import { PageRequest } from '../../../models/paging/page-request';
import { ListUserQueryDto, ListUserQueryResponse } from '../../../services/identity/user/user-service.models';
import { observeNotification } from 'rxjs/internal/Notification';
import { DataSource } from '@angular/cdk/table';
import { CollectionViewer } from '@angular/cdk/collections';
@Component({
  selector: 'app-list-users',
  imports: [MatTableModule, MatPaginatorModule],
  templateUrl: './list-users.html',
  styleUrl: './list-users.css',
  standalone: true
})
export class ListUsers {
  userService = inject(UserService)
  
  usersCustomDataSource = new UserDataSource([]);
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
    console.log("Refreshing users...");
    this.userService.list({search:null, roleId:null, paging: new PageRequest()})
      .pipe(
        tap( (response : ListUserQueryResponse) => {
        this.users.data = response.items;
        }),
        catchError( err => { 
          throw new Error('Failed to load users: ' + err.message);
        })
      )
      .subscribe();
  }


}

class UserDataSource extends DataSource<ListUserQueryDto> {
  override disconnect(collectionViewer: CollectionViewer): void {
    // No-op
  }
  private _dataStream = new ReplaySubject<ListUserQueryDto[]>();

  constructor(initialData : ListUserQueryDto[]) {
    super();
    this.setData(initialData)
  }

  connect(): Observable<ListUserQueryDto[]> {
    return this._dataStream;
  }

  setData(initialData: ListUserQueryDto[]) {
    this._dataStream.next([]);
    this._dataStream.next(initialData);
  }
}



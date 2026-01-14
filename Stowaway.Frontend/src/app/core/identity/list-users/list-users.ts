import { Component, inject } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { UserService } from '../../../services/identity/user/user-service';
import { catchError, tap } from 'rxjs';
import { PageRequest } from '../../../models/paging/page-request';
import { ListUserQueryDto } from '../../../services/identity/user/user-service.models';
@Component({
  selector: 'app-list-users',
  imports: [MatTableModule],
  templateUrl: './list-users.html',
  styleUrl: './list-users.css',
  standalone: true
})
export class ListUsers {
  userService = inject(UserService)
  users : ListUserQueryDto[] = [];
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
  ngOnInit() {
    
    this.userService.list({search:null, roleId:null, paging: new PageRequest()})
    .pipe(
      tap( (response) => {
        this.users = response.items;
        this.displayUsers();
      }),
      catchError( err => { 
        throw new Error('Failed to load users: ' + err.message);
      })
    )
    .subscribe();
  }
  displayUsers()
  {
    console.log(this.users);
  }
}


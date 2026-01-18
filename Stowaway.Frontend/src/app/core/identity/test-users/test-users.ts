import { ChangeDetectorRef, Component, inject, ViewChild } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, FormBuilder} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule, MatFormField } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { CreateUserCommand, ListUserQueryDto, ListUserQueryResponse, Role, UpdateUserCommand } from '../../../services/identity/user/user-service.models';
import { ListSupplierQuery } from '../../../services/storage/supplier/supplier.model';
import { HttpClient } from '@angular/common/http';
import { UserService } from '../../../services/identity/user/user-service';
import { delay, tap } from 'rxjs';
import { ListUsers } from "../list-users/list-users";
import { AutocompleteComponent, IOptionsInfo } from '../../../shared/autocomplete-component/autocomplete-component';
import { environment } from '../../../../enviroments/enivroment';

@Component({
  selector: 'app-test-users',
  imports: [MatButtonModule, FormsModule, MatInputModule, ReactiveFormsModule, MatFormField, MatCheckboxModule, ListUsers, AutocompleteComponent],
  templateUrl: './test-users.html',
  styleUrl: './test-users.css',
})
export class TestUsers {

  htpp = inject(HttpClient);
  userService = inject(UserService);
  //last resort for NG0100
  cd = inject(ChangeDetectorRef);

  @ViewChild(ListUsers) listUsersComponent !: ListUsers;


  roles: any[] = [];  
  rolesDisplayInfo : IOptionsInfo = {displayName : "roleName"};

  ngAfterViewInit()
  {
    this.htpp.get<any[]>(`${environment.apiUrl}/Roles`, {})
    .subscribe({next :(response) =>
    {
      setTimeout(() => { 
        this.roles = response;
        this.cd.detectChanges();
      });
    },
    error: err => console.log("Error loading roles?")
  });
  }

  // ngOnInit()
  // {
  //   this.htpp.get<any[]>(`${environment.apiUrl}/Roles`, {})
  //   .pipe(
  //     tap((response) => 
  //     {
  //       delay(0);
  //       this.roles = response;
  //     })
  //   )
  //   .subscribe();
    
  // }
  
  deleteUser() {
    console.log("Deleting user with ID: " + this.userForm.controls.id.value);
    this.userService.delete(this.userForm.controls.id.value!).pipe(
      tap((isDeleted) => {
        console.log("User deleted? " + isDeleted);
        this.listUsersComponent.refreshUsers();
      })
    ).subscribe();
  }
  updateUser() {
    const command : UpdateUserCommand= {
      id: this.userForm.controls.id.value!,
      email: this.userForm.controls.email.value!,
      firstName: this.userForm.controls.firstName.value!,
      lastName: this.userForm.controls.lastName.value!,
      role : this.userForm.controls.role.value!,
      isEnabled : this.userForm.controls.isEnabled.value!
    }
    this.userService.update(command).pipe(
      tap( () => {
        console.log("Updated user with ID: " + command.id);
        this.listUsersComponent.refreshUsers();
      })
    ).subscribe();
  }
  createUser() {
    const command : CreateUserCommand = {
      email: this.userForm.controls.email.value!,
      firstName: this.userForm.controls.firstName.value!,
      lastName: this.userForm.controls.lastName.value!,
      password : this.userForm.controls.password.value!
    }
    this.userService.create( command ).pipe(
      tap( (newUserId) => {
        console.log("Created user with ID: " + newUserId);
        this.listUsersComponent.refreshUsers();
      })
    ).subscribe();
  }
  
  userForm = new FormGroup({
    id : new FormControl<number | null>(0),
    email : new FormControl<string | null>('nullMail'),
    password : new FormControl<string>('null'),
    firstName : new FormControl<string | null>('nullName'),
    lastName : new FormControl<string | null>('nullLastName'),
    role : new FormControl<Role | null>(null),
    isEnabled : new FormControl<boolean>(true),
  });

}

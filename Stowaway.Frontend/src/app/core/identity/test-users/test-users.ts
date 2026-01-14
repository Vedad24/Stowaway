import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, FormBuilder} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule, MatFormField } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { CreateUserCommand, ListUserQueryDto, ListUserQueryResponse, Role } from '../../../services/identity/user/user-service.models';
import { ListSupplierQuery } from '../../../services/storage/supplier/supplier.model';
import { HttpClient } from '@angular/common/http';
import { UserService } from '../../../services/identity/user/user-service';
import { tap } from 'rxjs';
import { ListUsers } from "../list-users/list-users";

@Component({
  selector: 'app-test-users',
  imports: [MatButtonModule, FormsModule, MatInputModule, ReactiveFormsModule, MatFormField, MatCheckboxModule, ListUsers],
  templateUrl: './test-users.html',
  styleUrl: './test-users.css',
})
export class TestUsers {

  htpp = inject(HttpClient);
  userService = inject(UserService);

  users: any
  
  deleteUser() {
    console.log("Deleting user with ID: " + this.userForm.controls.id.value);
    this.userService.delete(this.userForm.controls.id.value!).pipe(
      tap((isDeleted) => {
        console.log("User deleted? " + isDeleted);
      })
    ).subscribe();
  }
  updateUser() {
  throw new Error('Method not implemented.');
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

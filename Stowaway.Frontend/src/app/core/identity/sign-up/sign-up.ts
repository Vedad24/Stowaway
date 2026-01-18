import { Component, Inject, inject } from '@angular/core';
import { UserService } from '../../../services/identity/user/user-service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateUserCommand, RoleName } from '../../../services/identity/user/user-service.models';
import { MatInputModule } from "@angular/material/input";
import {MatStepperModule} from "@angular/material/stepper"
import { MatButtonModule } from '@angular/material/button';
import { catchError, tap } from 'rxjs';
import { Router } from '@angular/router';
import {MatIconModule} from '@angular/material/icon'
@Component({
  selector: 'app-sign-up',
  imports: [MatInputModule, MatStepperModule, ReactiveFormsModule, MatButtonModule],
  templateUrl: './sign-up.html',
  styleUrl: './sign-up.css',
})
export class SignUp {
  router = inject(Router);
  userService = inject(UserService);
  fb = inject(FormBuilder);

   emailForm: FormGroup = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
    profileForm: FormGroup = this.fb.group({
        firstName: ['', Validators.required],
        lastName: ['', Validators.required]
      });
  passwordForm: FormGroup = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8)]]
    });

  createUser()
  {
    const payload : CreateUserCommand = 
    {
        email: this.emailForm.value.email!,
        firstName: this.profileForm.value.firstName!,
        lastName: this.profileForm.value.lastName!,
        password: this.passwordForm.value.password!,
    }
    //console.log(payload);
    this.userService.create(payload)
    .pipe(
      tap((response) => 
      {
        console.log("Created user with ID", response);
        this.userService.setData({email: payload.email, password: payload.password});
        this.router.navigate(['/login']);
      }),
      catchError( (err) => {
        throw new Error(err);
      })
    )
    .subscribe();
  }
}

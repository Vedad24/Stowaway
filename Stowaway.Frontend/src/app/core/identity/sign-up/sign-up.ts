import { Component, inject } from '@angular/core';
import { UserService } from '../../../services/identity/user/user-service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateUserCommand } from '../../../services/identity/user/user-service.models';
import { MatInputModule } from '@angular/material/input';
import { MatStepperModule } from '@angular/material/stepper';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { catchError, of, tap } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sign-up',
  imports: [MatInputModule, MatStepperModule, ReactiveFormsModule, MatButtonModule, MatCardModule, MatSnackBarModule],
  templateUrl: './sign-up.html',
  styleUrl: './sign-up.css',
})
export class SignUp {
  router = inject(Router);
  userService = inject(UserService);
  fb = inject(FormBuilder);
  snackBar = inject(MatSnackBar);

  emailForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
  });

  profileForm: FormGroup = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
  });

  passwordForm: FormGroup = this.fb.group({
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  createUser() {
    const payload: CreateUserCommand = {
      email: this.emailForm.value.email!,
      firstName: this.profileForm.value.firstName!,
      lastName: this.profileForm.value.lastName!,
      password: this.passwordForm.value.password!,
    };

    this.userService.create(payload)
      .pipe(
        tap((response) => {
          this.userService.setData({ email: payload.email, password: payload.password });
          this.router.navigate(['/login']);
        }),
        catchError((err) => {
          this.showSignUpError();
          return of(null);
        })
      )
      .subscribe();
  }

  showSignUpError(): void {
    this.snackBar.open('Sign up failed. Please try again.', 'Dismiss', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: ['invalid-credentials-snackbar'],
    });
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}

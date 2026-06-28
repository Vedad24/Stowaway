import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../services/identity/auth/auth-service';
import { CurrentUserService } from '../../services/identity/auth/current-user-service';
import { catchError, of, tap } from 'rxjs';
import { Router } from '@angular/router';
import { UserService } from '../../services/identity/user/user-service';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, MatInputModule, MatButtonModule, MatCardModule, MatIconModule, MatSnackBarModule],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
})
export class LoginPage {
  authService = inject(AuthService);
  currentUserService = inject(CurrentUserService);
  router = inject(Router);
  userService = inject(UserService);
  snackBar = inject(MatSnackBar);

  hidePassword = true;

  ngOnInit() {
    const data = this.userService.signUpData;
    if (data.email != null && data.password != null) {
      this.loginForm.patchValue({
        email: data.email,
        password: data.password,
      });
      this.userService.clearData();
    }
  }

  tryLogin() {
    if (!this.loginForm.valid) {
      return;
    }

    this.authService.login(this.loginForm.value.email!, this.loginForm.value.password!)
      .pipe(
        tap((success) => {
          if (success) {
            this.router.navigate(['main']);
            return;
          }

          this.showInvalidCredentials();
        }),
        catchError(() => {
          this.showInvalidCredentials();
          return of(false);
        })
      )
      .subscribe();
  }

  showInvalidCredentials(): void {
    this.snackBar.open('Invalid Credentials', 'Dismiss', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: ['invalid-credentials-snackbar'],
    });
  }

  togglePasswordVisibility(): void {
    this.hidePassword = !this.hidePassword;
  }

  goToSignUp(): void {
    this.router.navigate(['/sign-up']);
  }

  loginForm = new FormGroup({
    email: new FormControl('admin@market.local', [Validators.email, Validators.required]),
    password: new FormControl('Admin123!', Validators.required),
  });
}

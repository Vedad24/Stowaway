import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { catchError, tap, throwError } from 'rxjs';
import { CurrentUserService } from '../../../services/identity/auth/current-user-service'; 
import { UserService } from '../../../services/identity/user/user-service';
import { GetUserByIdOrMailDto, UpdateUserCommand, Role, RoleName } from '../../../services/identity/user/user-service.models';

@Component({
  selector: 'app-user-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-settings.html',
  styleUrls: ['./user-settings.css'],
})
export class UserSettings implements OnInit {
  currentUserService = inject(CurrentUserService);
  userService = inject(UserService);
  router = inject(Router);
  fb = inject(FormBuilder);

  form = this.fb.group({
    email: [''],
    firstName: [''],
    lastName: [''],
    role : ['']
  });

  user?: GetUserByIdOrMailDto;
  isSubmitting = false;
  message = '';
  errorMessage = '';

  ngOnInit(): void {
    
    if (!this.currentUserService.userEmail) {
      this.errorMessage = 'Unable to load current user information.';
      return;
    }

    this.userService.getByMail(this.currentUserService.userEmail).subscribe({
      next: (response) => {
        this.user = response;
        this.form.patchValue({
          email: response.email,
          firstName: response.firstName,
          lastName: response.lastName,
          role: RoleName[response.role.id] ?? 'Unknown',
        });
      },
      error: () => {
        this.errorMessage = 'Failed to load your profile. Please try again later.';
      },
    });
  }

  save(): void {
    this.errorMessage = '';
    this.message = '';

    if (this.form.invalid || !this.user) {
      this.errorMessage = 'Please complete the form before saving.';
      return;
    }

    this.isSubmitting = true;
    const payload: UpdateUserCommand = {
      id: this.user.id,
      email: this.form.value.email ?? null,
      firstName: this.form.value.firstName ?? null,
      lastName: this.form.value.lastName ?? null,
      role: this.user.role ?? null,
      isEnabled: this.user.isEnabled ?? null,
    };

    this.userService
      .update(payload)
      .pipe(
        tap(() => {
          this.message = 'Your settings have been saved successfully.';
        }),
        catchError((error) => {
          this.errorMessage = 'Unable to save settings right now.';
          return throwError(() => error);
        })
      )
      .subscribe({
        complete: () => {
          this.isSubmitting = false;
        },
      });
  }

  cancel(): void {
    this.router.navigate(['/main']);
  }
}

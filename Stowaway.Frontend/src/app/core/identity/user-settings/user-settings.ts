import { Component, inject, OnInit, signal } from '@angular/core';
import { ResolveStart, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { catchError, tap, throwError } from 'rxjs';
import { CurrentUserService } from '../../../services/identity/auth/current-user-service'; 
import { UserService } from '../../../services/identity/user/user-service';
import { GetUserByIdOrMailDto, UpdateUserCommand, Role, RoleName } from '../../../services/identity/user/user-service.models';
import { resolveSoa } from 'node:dns';

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
    email: [{value: '', disabled: true}],
    firstName: [''],
    lastName: [''],
    role : [{value: '', disabled : true}],
  });

  user?: GetUserByIdOrMailDto;
  isSubmitting = signal(false);
  message = '';
  errorMessage = '';

  ngOnInit(): void {
    
    if (!this.currentUserService.userId) {
      this.errorMessage = 'Unable to load current user information.';
      return;
    }
    this.loadUserData();
  }
  loadUserData() : void {
    this.userService.get(this.currentUserService.userId).subscribe({
      next: (response) => {
        this.user = response;
        console.log('User data loaded:', response);
        console.log("Reponse role id", response.role.id);
        console.log("role", RoleName[response.role.id]);
        this.form.patchValue({
          email: response.email,
          firstName: response.firstName,
          lastName: response.lastName,
          role: RoleName[response.role.id]
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

    this.isSubmitting.set(true);
    const payload: UpdateUserCommand = {
      id: this.currentUserService.userId,
      email: this.form.value.email ?? null,
      firstName: this.form.value.firstName ?? null,
      lastName: this.form.value.lastName ?? null,
      role: this.user.role ?? null,
      isEnabled: this.user.isEnabled ?? null,
    };
    console.log("Update user payload", payload);
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
          this.isSubmitting.set(false);
        },
      });
  }

  cancel(): void {
    this.router.navigate(['/main']);
  }
}



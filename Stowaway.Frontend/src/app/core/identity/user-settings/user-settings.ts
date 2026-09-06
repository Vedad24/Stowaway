import { Component, inject, OnInit, signal } from '@angular/core';
import { ResolveStart, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { catchError, tap, throwError } from 'rxjs';
import { CurrentUserService } from '../../../services/identity/auth/current-user-service'; 
import { UserService } from '../../../services/identity/user/user-service';
import { GetSelfDto, UpdateSelfCommand, RoleName } from '../../../services/identity/user/user-service.models';


@Component({
  selector: 'app-user-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatButtonModule, MatIconModule, MatTooltipModule, RouterLink],
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

  user?: GetSelfDto;
  isSubmitting = signal(false);
  message = '';
  errorMessage = '';

  ngOnInit(): void {
    this.loadUserData();
  }
  loadUserData() : void {
    this.userService.getSelf().subscribe({
      next: (response) => {
        this.user = response;
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
    const payload: UpdateSelfCommand = {
      email: this.form.value.email ?? null,
      firstName: this.form.value.firstName ?? null,
      lastName: this.form.value.lastName ?? null,
    };
    this.userService
      .updateSelf(payload)
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
    this.router.navigate(['/choose-module']);
  }
}



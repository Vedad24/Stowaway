import { inject, Injectable } from '@angular/core';
import { CurrentUserService } from './current-user-service';
import { HttpClient } from '@angular/common/http';
import { API_CONFIG } from '../../../core/config/api-config';
import { catchError, map, Observable, of, switchMap, tap } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class AuthService {


  backendApi = inject(HttpClient)
  currentUserService : CurrentUserService = inject(CurrentUserService);
  private readonly config = inject(API_CONFIG);
  login(email: string, password: string) : Observable<boolean>{

    return this.backendApi.post(`${this.config.baseUrl}/${this.config.auth.login}`, {
      email: email,
      password: password,
      fingerprint: ''
    })
    .pipe(
      switchMap(() => this.currentUserService.loadCurrentUser()),

      map( () => true ),

      catchError( err => {
        return of (false);
      }),
    );
  }

  isLoggedIn(): import("@angular/router").MaybeAsync<import("@angular/router").GuardResult> {
    return this.currentUserService.currentUser !== null;
  }

  refresh(): Observable<boolean> {
    return this.backendApi.post(`${this.config.baseUrl}/${this.config.auth.refresh}`, {})
      .pipe(
        map(() => true),
        catchError(() => of(false)),
      );
  }

  logout(): Observable<boolean> {
    return this.backendApi.post(`${this.config.baseUrl}/${this.config.auth.logout}`, {})
    .pipe(
      tap(() => this.currentUserService.clearUser()),
      map(() => true),
      catchError(() => {
        this.currentUserService.clearUser();
        return of(false);
      }),
    );
  }
}

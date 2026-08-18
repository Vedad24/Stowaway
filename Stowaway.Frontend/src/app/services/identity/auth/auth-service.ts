import { inject, Injectable } from '@angular/core';
import { CurrentUserService } from './current-user-service'; 
import { HttpClient, HttpResponse } from '@angular/common/http';
import { environment } from '../../../../enviroments/enivroment'
import { ApiEndpoints } from '../../../shared/constants/api-endpoints';
import { LoginCommandDto } from './auth-service.models';
import { catchError, map, Observable, of, tap } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  
  
  backendApi = inject(HttpClient)
  currentUserService : CurrentUserService = inject(CurrentUserService);
  backendUrl = environment.apiUrl;
  login(email: string, password: string) : Observable<boolean>{
    
    return this.backendApi.post(`${this.backendUrl}/${ApiEndpoints.Auth}/login`, {
      email: email,
      password: password,
      fingerprint: ''
    })
    .pipe(
      
      tap( (response) => {
        this.currentUserService.initializeUser(response as LoginCommandDto, email);
      }),

      map( () => true ),
      
      catchError( err => {
        return of (false);
      }),
    );
  }

  isLoggedIn(): import("@angular/router").MaybeAsync<import("@angular/router").GuardResult> {
    return this.currentUserService.currentUser !== null;
  }
}
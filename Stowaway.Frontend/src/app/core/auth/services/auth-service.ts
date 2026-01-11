import { inject, Injectable } from '@angular/core';
import { CurrentUserService } from './current-user-service';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { backendUrl } from '../../../app.config';
import { LoginCommandDto } from '../models/login-model';
import { catchError, map, Observable, of, tap } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  
  backendApi = inject(HttpClient)


  currentUser : CurrentUserService = inject(CurrentUserService);
  
  login(email: string, password: string) : Observable<boolean>{
    
    return this.backendApi.post(`${backendUrl}/api/auth/login`, {
      email: email,
      password: password,
      fingerprint: ''
    })
    .pipe(
      
      tap( (response) => {
        console.log('authLogin -> tap ->', response);
        this.currentUser.initializeUser(response as LoginCommandDto, email);
      }),

      map( () => true ),
      
      catchError( err => {
        return of (false);
      }),
    );
    
  }
}
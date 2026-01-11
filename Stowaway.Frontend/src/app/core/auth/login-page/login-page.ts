import { Component, inject } from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms'
import {MatInputModule} from '@angular/material/input'
import { MatAnchor, MatButtonModule } from "@angular/material/button";
import { AuthService } from '../services/auth-service';
import { CurrentUserService } from '../services/current-user-service';
import { catchError, tap } from 'rxjs';
import { Router } from '@angular/router';
import { fileURLToPath } from 'url';
@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, MatInputModule, MatAnchor, MatButtonModule],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
})
export class LoginPage {
  
  authService = inject(AuthService);
  currentUserService = inject(CurrentUserService);
  router = inject(Router);
  
  tryLogin() {

    try{

      this.authService.login(this.loginForm.value.email!, this.loginForm.value.password!)
      .pipe(
        
        catchError( err => { 
          throw new Error('Login request failed: ' + err.message);
        }),

        tap((success) => {
          if(success){
            this.router.navigate(['']); 
          }
        })
      )
    }
    catch(error){
      console.error(error);
    }
  }
  
  loginForm = new FormGroup(
    {
      email : new FormControl('admin@market.local',Validators.email),
      password : new FormControl('Admin123!', Validators.required)
    }
  )
  
}

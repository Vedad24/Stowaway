import { Component, inject } from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms'
import {MatInputModule} from '@angular/material/input'
import { MatAnchor, MatButtonModule } from "@angular/material/button";
import { AuthService } from '../services/auth-service';
import { CurrentUserService } from '../services/current-user-service';
import { catchError, tap } from 'rxjs';
import { Router } from '@angular/router';
import { fileURLToPath } from 'url';
import { UserService } from '../../../services/identity/user/user-service';
import { MatIcon } from "@angular/material/icon";
@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, MatInputModule, MatAnchor, MatButtonModule, MatIcon],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
})
export class LoginPage {
  
  authService = inject(AuthService);
  currentUserService = inject(CurrentUserService);
  router = inject(Router);
  userService = inject(UserService);

  hidePassword = true;
  ngOnInit()
  {
    const data = this.userService.signUpData;
    if(data)
    {
      console.log("Gotten data", data);
      this.loginForm.patchValue(
        {
          email: data.email,
          password : data.password
        }
      )
      this.userService.clearData();
    }
  }
  tryLogin() {

    try{

      this.authService.login(this.loginForm.value.email!, this.loginForm.value.password!)
      .pipe(
        
        catchError( err => { 
          throw new Error('Login request failed: ' + err.message);
        }),

        tap((success) => {
          if(success){
            this.router.navigate(['main']); 
          }
        })
      ).subscribe();
    }
    catch(error){
      console.error(error);
    }
  }
  
  loginForm = new FormGroup(
    {
      email : new FormControl('admin@market.local',[Validators.email, Validators.required]),
      password : new FormControl('Admin123!', Validators.required)
    }
  )
  
}

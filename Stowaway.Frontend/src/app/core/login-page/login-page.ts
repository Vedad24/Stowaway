import { Component } from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms'
import {MatInputModule} from '@angular/material/input'
import { MatAnchor, MatButtonModule } from "@angular/material/button";
@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, MatInputModule, MatAnchor, MatButtonModule],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
})
export class LoginPage {
  tryLogin() {
    throw new Error('Method not implemented.');
  }
  loginForm = new FormGroup(
    {
      email : new FormControl('',Validators.email),
      password : new FormControl('', Validators.required)
    }
  )
  
}

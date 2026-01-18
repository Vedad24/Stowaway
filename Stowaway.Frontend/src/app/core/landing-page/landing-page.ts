import { Component, inject } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';
import { LoginPage } from '../auth/login-page/login-page';
import {MatButtonModule} from '@angular/material/button'
@Component({
  selector: 'app-landing-page',
  imports: [MatButtonModule],
  templateUrl: './landing-page.html',
  styleUrl: './landing-page.css',
})
export class LandingPage {
  goToSignUp() {
    this.router.navigate(['/sign-up'])
  }
  router = inject(Router)
  goToLogin()
  {
    this.router.navigate(['/login']);
  }
}

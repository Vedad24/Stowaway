import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';

@Component({
  selector: 'app-landing-page',
  imports: [MatButtonModule, MatCardModule, MatToolbarModule],
  templateUrl: './landing-page.html',
  styleUrl: './landing-page.css',
})
export class LandingPage {
  readonly router = inject(Router);

  goToSignUp(): void {
    this.router.navigate(['/sign-up']);
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}

import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { CurrentUserService } from '../../services/identity/auth/current-user-service';
import { AuthService } from '../../services/identity/auth/auth-service';
import { ThemeToggle } from '../../shared/theme-toggle/theme-toggle';
import { Permissions } from '../../shared/constants/permissions';

@Component({
  selector: 'app-choose-module',
  imports: [MatButtonModule, MatCardModule, MatIconModule, ThemeToggle],
  templateUrl: './choose-module.html',
  styleUrl: './choose-module.css',
})
export class ChooseModule {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  readonly currentUser = inject(CurrentUserService);

  get canViewOrders(): boolean {
    return this.currentUser.isAdmin || this.currentUser.permissions.includes(Permissions.OrderRead);
  }

  openWarehouseManagement(): void {
    this.router.navigate(['/main']);
  }

  openWorkerManagement(): void {
    this.router.navigate(['/employee-management']);
  }

  openOrders(): void {
    this.router.navigate(['/orders']);
  }

  openDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  openUserSettings(): void {
    this.router.navigate(['/user/settings']);
  }

  logout(): void {
    this.authService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }
}

import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { ThemeToggle } from '../../shared/theme-toggle/theme-toggle';
import { DashboardApiService } from '../../services/dashboard/dashboard';
import { GetGlobalStatsQueryDto } from '../../services/dashboard/dashboard.model';

@Component({
  selector: 'app-dashboard',
  imports: [MatButtonModule, MatCardModule, MatIconModule, ThemeToggle],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  private readonly router = inject(Router);
  private readonly dashboardApiService = inject(DashboardApiService);

  readonly stats = signal<GetGlobalStatsQueryDto | null>(null);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  constructor() {
    this.dashboardApiService.getGlobalStats().subscribe({
      next: (result) => {
        this.stats.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Failed to load dashboard data.');
        this.loading.set(false);
      },
    });
  }

  back(): void {
    this.router.navigate(['/choose-module']);
  }
}

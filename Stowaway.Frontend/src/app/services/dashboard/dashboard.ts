import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../core/config/api-config';
import { GetGlobalStatsQueryDto } from './dashboard.model';

@Injectable({
  providedIn: 'root',
})
export class DashboardApiService {
  private readonly config = inject(API_CONFIG);
  private get baseUrl() { return this.config.baseUrl; }
  private http = inject(HttpClient);

  getGlobalStats(): Observable<GetGlobalStatsQueryDto> {
    return this.http.get<GetGlobalStatsQueryDto>(`${this.baseUrl}/${this.config.dashboard.globalStats}`);
  }
}

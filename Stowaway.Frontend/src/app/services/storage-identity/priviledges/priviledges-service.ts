import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import {
  ListPriviledgesQueryDto,
} from './priviledges-service.models';
import { API_CONFIG } from '../../../core/config/api-config';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PriviledgesService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(API_CONFIG);

  public list(): Observable<ListPriviledgesQueryDto[]> {
    return this.http.get<ListPriviledgesQueryDto[]>(`${this.config.baseUrl}/${this.config.storageIdentity.privileges}`);
  }

  priviledgeData: {
    name?: string;
    description?: string;
  } = {};

  setData(data: { name: string; description: string }) {
    this.priviledgeData = data;
  }

  clearData() {
    this.priviledgeData = {};
  }
}

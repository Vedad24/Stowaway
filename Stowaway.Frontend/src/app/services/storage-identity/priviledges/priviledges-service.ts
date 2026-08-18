import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import {
  ListPriviledgesQueryDto,
} from './priviledges-service.models';
import { environment } from '../../../../enviroments/enivroment';
import { ApiEndpoints } from '../../../shared/constants/api-endpoints';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PriviledgesService {
  private readonly http = inject(HttpClient);
  private readonly priviledgeURL = `${environment.apiUrl}/${ApiEndpoints.StorageIdentityPrivileges}`;

  public list(): Observable<ListPriviledgesQueryDto[]> {
    return this.http.get<ListPriviledgesQueryDto[]>(this.priviledgeURL);
  }

  priviledgeData: {
    name?: string;
    description?: string;
  } = {};

  setData(data: { name: string; description: string }) {
    this.priviledgeData = data;
    console.log('data is set', this.priviledgeData);
  }

  clearData() {
    this.priviledgeData = {};
  }
}

import { Injectable, inject } from '@angular/core';
import { Observable, onErrorResumeNextWith } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../enviroments/enivroment';
import { ListNamesContainerQueryResponse, ListNamesContainersQuery } from './container.model';

@Injectable({
  providedIn: 'root',
})
export class ContainerApiService {
  private readonly baseUrl = `${environment.apiUrl}/Container`;
  private http = inject(HttpClient);

  list(request? : ListNamesContainersQuery): Observable<ListNamesContainerQueryResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListNamesContainerQueryResponse>(this.baseUrl, { params });
  }
}

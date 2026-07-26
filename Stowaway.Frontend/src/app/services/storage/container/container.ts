import { Injectable, inject } from '@angular/core';
import { Observable, onErrorResumeNextWith } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../enviroments/enivroment';
import { ListContainersQueryResponse, ListContainersQuery } from './container.model';

@Injectable({
  providedIn: 'root',
})
export class ContainerApiService {
  private readonly baseUrl = `${environment.apiUrl}/Container`;
  private http = inject(HttpClient);

  list(request? : ListContainersQuery): Observable<ListContainersQueryResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListContainersQueryResponse>(this.baseUrl, { params });
  }
}

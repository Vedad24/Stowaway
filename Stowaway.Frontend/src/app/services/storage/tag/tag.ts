import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../enviroments/enivroment';
import { TagDto, CreateTagCommand } from './tag.model';

@Injectable({
  providedIn: 'root',
})
export class TagApiService {
  private readonly baseUrl = `${environment.apiUrl}/Tag`;
  private http = inject(HttpClient);

  list(): Observable<TagDto[]> {
    return this.http.get<TagDto[]>(this.baseUrl);
  }

  create(payload: CreateTagCommand): Observable<TagDto> {
    return this.http.post<TagDto>(this.baseUrl, payload);
  }
}

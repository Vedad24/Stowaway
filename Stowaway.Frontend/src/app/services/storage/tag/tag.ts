import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { API_CONFIG } from '../../../core/config/api-config';
import { TagDto, CreateTagCommand } from './tag.model';

@Injectable({
  providedIn: 'root',
})
export class TagApiService {
  private readonly config = inject(API_CONFIG);
  private http = inject(HttpClient);

  list(): Observable<TagDto[]> {
    return this.http.get<TagDto[]>(`${this.config.baseUrl}/${this.config.tag.list}`);
  }

  create(payload: CreateTagCommand): Observable<TagDto> {
    return this.http.post<TagDto>(`${this.config.baseUrl}/${this.config.tag.create}`, payload);
  }
}

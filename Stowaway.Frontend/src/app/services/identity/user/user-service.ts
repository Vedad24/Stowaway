import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CreateUserCommand, GetUserById, GetUserByIdDto, ListUserQuery, ListUserQueryResponse, UpdateUserCommand } from './user-service.models';
import { environment } from '../../../../enviroments/enivroment';
import { Observable } from 'rxjs';
import { ListItemQueryResponse } from '../../storage/item/item.model';
import { buildHttpParams } from '../../../models/build-http-params';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  http = inject(HttpClient);
  userURL = `${environment.apiUrl}/User`;
  
  public get( id: number) : Observable<GetUserByIdDto>{
    return this.http.get<GetUserByIdDto>(`${this.userURL}/${id}`);
  }

  public list(payload : ListUserQuery | null) : Observable<ListUserQueryResponse>{
    
    const params = payload ? buildHttpParams(payload as any) : undefined;
    return this.http.get<ListUserQueryResponse>(this.userURL, { params });
  } 

  public create(payload : CreateUserCommand) : Observable<number>{
    return this.http.post<number>(this.userURL, payload);
  }

  public update(payload : UpdateUserCommand) : Observable<void>{
    return this.http.put<void>(`${this.userURL}`, payload);
  }

  public delete(id : number) : Observable<boolean>{
    return this.http.delete<boolean>(`${this.userURL}/${id}`);
  }

}

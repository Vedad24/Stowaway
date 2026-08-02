import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CreateUserCommand, GetSelfDto, GetUserById, GetUserByIdOrMailDto, ListUserQuery, ListUserQueryResponse, UpdateSelfCommand, UpdateSelfCommandDto, UpdateUserCommand } from './user-service.models';
import { environment } from '../../../../enviroments/enivroment';
import { Observable } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  http = inject(HttpClient);
  userURL = `${environment.apiUrl}/User`;
  
  public get( id: number) : Observable<GetUserByIdOrMailDto>{
    return this.http.get<GetUserByIdOrMailDto>(`${this.userURL}/${id}`);
  }
  public getByMail( email: string) : Observable<GetUserByIdOrMailDto>{
    return this.http.get<GetUserByIdOrMailDto>(`${this.userURL}/mail/${email}`);
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

  public getSelf() : Observable<GetSelfDto>{
    return this.http.get<GetSelfDto>(`${this.userURL}/me`);
  }

  public updateSelf(payload : UpdateSelfCommand) : Observable<UpdateSelfCommandDto>{
    return this.http.put<UpdateSelfCommandDto>(`${this.userURL}/me`, payload);
  }

  public deleteSelf() : Observable<void>{
    return this.http.delete<void>(`${this.userURL}/me`);
  }

  signUpData : {
    email? : string,
    password? : string 
  } = {};

  setData(data : {email:string; password : string})
  {

    this.signUpData = data;
    console.log("data is set", this.signUpData);
  }
  clearData()
  {
    this.signUpData = {};
  }
}

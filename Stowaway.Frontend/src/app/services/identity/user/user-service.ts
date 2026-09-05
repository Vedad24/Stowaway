import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CreateUserCommand, GetEmployeeFormQueryDto, GetSelfDto, GetUserById, GetUserByIdOrMailDto, ListUserQuery, ListUserQueryResponse, UpdateSelfCommand, UpdateSelfCommandDto, UpdateUserCommand } from './user-service.models';
import { API_CONFIG } from '../../../core/config/api-config';
import { Observable } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';
import { buildUrl } from '../../../models/build-url';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  http = inject(HttpClient);
  private readonly config = inject(API_CONFIG);
  private get baseUrl() { return this.config.baseUrl; }

  public get( id: number) : Observable<GetUserByIdOrMailDto>{
    return this.http.get<GetUserByIdOrMailDto>(`${this.baseUrl}/${buildUrl(this.config.user.byId, { id })}`);
  }
  public getByMail( email: string) : Observable<GetUserByIdOrMailDto>{
    return this.http.get<GetUserByIdOrMailDto>(`${this.baseUrl}/${buildUrl(this.config.user.byMail, { email })}`);
  }

  public list(payload : ListUserQuery | null) : Observable<ListUserQueryResponse>{

    const params = payload ? buildHttpParams(payload) : undefined;
    return this.http.get<ListUserQueryResponse>(`${this.baseUrl}/${this.config.user.list}`, { params });
  }

  public create(payload : CreateUserCommand) : Observable<number>{
    return this.http.post<number>(`${this.baseUrl}/${this.config.user.create}`, payload);
  }

  public update(payload : UpdateUserCommand) : Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${this.config.user.update}`, payload);
  }

  public delete(id : number) : Observable<boolean>{
    return this.http.delete<boolean>(`${this.baseUrl}/${buildUrl(this.config.user.delete, { id })}`);
  }

  public getSelf() : Observable<GetSelfDto>{
    return this.http.get<GetSelfDto>(`${this.baseUrl}/${this.config.user.self}`);
  }

  public updateSelf(payload : UpdateSelfCommand) : Observable<UpdateSelfCommandDto>{
    return this.http.put<UpdateSelfCommandDto>(`${this.baseUrl}/${this.config.user.self}`, payload);
  }

  public deleteSelf() : Observable<void>{
    return this.http.delete<void>(`${this.baseUrl}/${this.config.user.self}`);
  }

  public getEmployeeForm() : Observable<GetEmployeeFormQueryDto>{
    return this.http.get<GetEmployeeFormQueryDto>(`${this.baseUrl}/${this.config.user.employeeForm}`);
  }

  signUpData : {
    email? : string,
    password? : string 
  } = {};

  setData(data : {email:string; password : string})
  {

    this.signUpData = data;
  }
  clearData()
  {
    this.signUpData = {};
  }
}

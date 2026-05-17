import { Injectable } from '@angular/core';
import { CurrentUserDto, LoginCommandDto } from './auth-service.models';


@Injectable({
  providedIn: 'root',
})
export class CurrentUserService {
  private _currentUser : CurrentUserDto | null = null;
  public get currentUser() : CurrentUserDto | null {  this.getUserFromStorage(); return this._currentUser; }
  get user (): CurrentUserDto | null {
    return this._currentUser;
  }

  initializeUser(response : LoginCommandDto, email : string)
  {
    this._currentUser = {
      email : email,
      roleId : 1, //please for the love of God change this later
      accessToken : response.accessToken
    }
    localStorage.setItem('currentUserStorage', JSON.stringify(this._currentUser));
  }

  getUserFromStorage()
  {
    this._currentUser = JSON.parse(localStorage.getItem('currentUserStorage')!);
  }
  
}


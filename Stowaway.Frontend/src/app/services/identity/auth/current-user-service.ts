import { Injectable } from '@angular/core';
import { CurrentUserDto, JwtUserPayload, LoginCommandDto } from './auth-service.models';
import { jwtDecode, JwtPayload } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class CurrentUserService {
  private _currentUser : CurrentUserDto | null = null;
  public get currentUser() : CurrentUserDto | null {  return this._currentUser; }
  get user (): CurrentUserDto | null {
    return this._currentUser;
  }

  initializeUser(response : LoginCommandDto, email : string)
  {
    this._currentUser = {
      roleId : 1, //please for the love of God change this later
      accessToken : response.accessToken
    }
    localStorage.setItem('currentUserStorage', JSON.stringify(this._currentUser));
  }

  getUserFromStorage()
  {
    this._currentUser = JSON.parse(localStorage.getItem('currentUserStorage')!);
  }

  public get userEmail() : string 
  {
    
    const decodedJwt = jwtDecode<JwtUserPayload>(this._currentUser!.accessToken);
    console.log(decodedJwt);
    return decodedJwt.email;
  }
  
}


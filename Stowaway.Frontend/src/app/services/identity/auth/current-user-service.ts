import { Injectable } from '@angular/core';
import { CurrentUserDto, JwtUserPayload, LoginCommandDto } from './auth-service.models';
import { jwtDecode, JwtPayload } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class CurrentUserService {
  private _currentUser : CurrentUserDto | null = null;
  public get currentUser() : CurrentUserDto | null { 
    if(this._currentUser === null )
      this.getUserFromStorage();
    return this._currentUser; }
  

  initializeUser(response : LoginCommandDto, email : string)
  {
    //console.log("LoginDTO response: ", response);
    this._currentUser = {
      roleId : 1, //please for the love of God change this later
      accessToken : response.accessToken
    }
    //console.log("Current user", this._currentUser);
    localStorage.setItem('currentUserStorage', JSON.stringify(this._currentUser));
  }

  getUserFromStorage()
  {
    this._currentUser = JSON.parse(localStorage.getItem('currentUserStorage')!);
    //console.log("Current user after storage read:", this._currentUser);
  }
  private get decodedJwt() : JwtUserPayload
  {
      const jwt = jwtDecode<JwtUserPayload>(this.currentUser!.accessToken);
      //console.log("JWT decode: ", jwt);
      return jwt;
  }
  public get userEmail() : string 
  {
    return this.decodedJwt.email;
  }

  public get userId() : number
  {
    const id = this.decodedJwt.nameid;
    //console.log(id);
    //console.log(this.userEmail);
    return Number(id);
    
  }


  
}


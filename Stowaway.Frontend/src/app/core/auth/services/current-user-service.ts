import { Injectable } from '@angular/core';
import { LoginCommandDto } from '../models/login-model';
interface CurrentUserDto
{
    email: string;
    roleId : number;
    accessToken : string;
}

@Injectable({
  providedIn: 'root',
})
export class CurrentUserService {
  _currentUser : CurrentUserDto | null = null;
  
  get user (): CurrentUserDto | null {
    return this._currentUser;
  }

  initializeUser(response : LoginCommandDto, email : string)
  {
    //console.log("Reponse as login command dto:", response)
    this._currentUser = {
      email : email,
      roleId : 1, //please for the love of God change this later
      accessToken : response.accessToken
    }
    // console.log('Initialized current user:');
    // console.log(this._currentUser);
    localStorage.setItem('currentUserStorage', JSON.stringify(this._currentUser));
  }

  getUserFromStorage()
  {
    this._currentUser = JSON.parse(localStorage.getItem('currentUserStorage')!);
  }
  
}


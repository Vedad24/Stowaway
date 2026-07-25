import { inject, Injectable } from '@angular/core';
import { CurrentUserDto, JwtUserPayload, LoginCommandDto } from './auth-service.models';
import { jwtDecode, JwtPayload } from 'jwt-decode';
import { LocalStorageService } from '../../local-storage-service';

@Injectable({
  providedIn: 'root',
})
export class CurrentUserService {
  private _currentUser : CurrentUserDto | null = null;
  private localStorageService = inject(LocalStorageService);

  public get currentUser() : CurrentUserDto | null { 
    if(this._currentUser === null )
      this.getUserFromStorage();
    return this._currentUser; }
  

  initializeUser(response : LoginCommandDto, email : string)
  {
    const decoded = jwtDecode<JwtUserPayload>(response.accessToken);
    const roleClaim =
      decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
      decoded.role;
    console.log("LoginDTO response: ", roleClaim);
    this._currentUser = {
      roleId : roleClaim != null ? roleClaim == "Admin" ? 1 : 0 : 0,
      accessToken: response.accessToken
    }
    //console.log("Current user", this._currentUser);
    this.localStorageService.setItem('currentUserStorage', JSON.stringify(this._currentUser));
  }

  getUserFromStorage()
  {
    this._currentUser = JSON.parse(this.localStorageService.getItem('currentUserStorage')!);
    //console.log("Current user after storage read:", this._currentUser);
  }
  private get decodedJwt() : JwtUserPayload | null
  {
      if(this.currentUser === null)
        return null;
      const jwt = jwtDecode<JwtUserPayload>(this.currentUser!.accessToken);
      //console.log("JWT decode: ", jwt);
      return jwt;
  }
  public get userEmail() : string
  {
    return this.decodedJwt? this.decodedJwt.email : "";
  }

  public get userId() : number
  {
    if(this.decodedJwt === null)
      return -1;
    const id = this.decodedJwt.nameid;
    //console.log(id);
    //console.log(this.userEmail);
    return Number(id);
    
  }

  public get roleId(): number {
    return this.currentUser?.roleId ?? -1;
  }

  public get permissions(): string[] {
    const raw = this.decodedJwt?.permission;
    if (raw == null) {
      return [];
    }
    return Array.isArray(raw) ? raw : [raw];
  }

  public get isManager(): boolean {
    if (this.roleId === 1) {
      return true;
    }
    const workerManageCodes = [
      'WarehouseUsers.Manage',
    ];
    return this.permissions.some((p) => workerManageCodes.includes(p));
  }

}


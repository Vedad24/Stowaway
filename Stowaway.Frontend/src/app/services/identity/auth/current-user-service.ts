import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { CurrentUserDto, GetSelfResponseDto } from './auth-service.models';
import { API_CONFIG } from '../../../core/config/api-config';
import { RoleName } from '../user/user-service.models';
@Injectable({
  providedIn: 'root',
})
export class CurrentUserService {
  private _currentUser : CurrentUserDto | null = null;
  private backendApi = inject(HttpClient);
  private readonly config = inject(API_CONFIG);

  public get currentUser() : CurrentUserDto | null {
    return this._currentUser;
  }

  // Hydrates the in-memory user state from the server, since the access token is an
  // httpOnly cookie now and can't be decoded client-side. Call on app bootstrap and after login.
  loadCurrentUser(): Observable<CurrentUserDto | null> {
    return this.backendApi.get<GetSelfResponseDto>(`${this.config.baseUrl}/${this.config.user.self}`)
      .pipe(
        map((response) => {
          const user: CurrentUserDto = {
            userId: response.userId,
            email: response.email,
            firstName: response.firstName,
            lastName: response.lastName,
            roleId: response.role.id,
            permissions: response.permissions,
          };
          this._currentUser = user;
          return user;
        }),
        catchError(() => {
          this._currentUser = null;
          return of(null);
        }),
      );
  }

  clearUser()
  {
    this._currentUser = null;
  }

  public get userEmail() : string
  {
    return this._currentUser?.email ?? "";
  }

  public get userId() : number
  {
    return this._currentUser?.userId ?? -1;
  }

  public get roleId(): number {
    return this._currentUser?.roleId ?? -1;
  }

  public get permissions(): string[] {
    return this._currentUser?.permissions ?? [];
  }

  public get isAdmin(): boolean {
    return this.roleId === RoleName.Admin;
  }

  public get isManager(): boolean {
    if (this.roleId === RoleName.Manager) {
      return true;
    }
    const workerManageCodes = [
      'WarehouseUsers.Manage',
    ];
    return this.permissions.some((p) => workerManageCodes.includes(p));
  }

}

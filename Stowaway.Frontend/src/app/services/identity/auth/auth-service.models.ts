export interface LoginCommand
{
    email : string;
    password : string;
    fingerprint : string;
}

export interface LoginCommandDto
{
    accessToken : string;

    refreshToken : string;

    expiresAtUtc : Date

}

export interface CurrentUserDto
{
    roleId : number;
    accessToken : string;
}

export interface JwtUserPayload
{
    sub: string;
    nameid: string;
    email: string;
    permission?: string[];
    ver: string;
    iat: number;
    jti: string;
    aud: string;
    role?: string;
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string;
}
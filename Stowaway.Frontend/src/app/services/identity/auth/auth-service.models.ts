export interface LoginCommand
{
    email : string;
    password : string;
    fingerprint : string;
}

export interface CurrentUserDto
{
    userId : number;
    email : string;
    firstName : string;
    lastName : string;
    roleId : number;
    permissions : string[];
}

// Shape returned by GET /User/me (GetSelfQueryDto) - used to hydrate CurrentUserDto,
// since the access token is now an httpOnly cookie and can no longer be decoded client-side.
export interface GetSelfResponseDto
{
    userId : number;
    email : string;
    firstName : string;
    lastName : string;
    role : { id : number };
    permissions : string[];
}

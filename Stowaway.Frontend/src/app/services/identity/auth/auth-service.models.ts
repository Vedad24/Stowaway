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
    email: string;
    roleId : number;
    accessToken : string;
}
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

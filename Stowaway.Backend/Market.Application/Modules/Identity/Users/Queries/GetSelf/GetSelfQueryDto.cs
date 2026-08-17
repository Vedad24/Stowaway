namespace Stowaway.Application.Modules.Identity.Users.Queries.GetSelf
{
    public class GetSelfQueryDto
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required GetSelfQueryDtoRoleDto Role { get; set; }
    }

    public class GetSelfQueryDtoRoleDto
    {
        public int Id { get; set; }
    }
}

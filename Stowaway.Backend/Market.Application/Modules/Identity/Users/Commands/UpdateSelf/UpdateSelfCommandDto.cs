namespace Stowaway.Application.Modules.Identity.Users.Commands.UpdateSelf
{
    public class UpdateSelfCommandDto
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}

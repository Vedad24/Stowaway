namespace Stowaway.Application.Modules.Identity.Users.Commands.UpdateSelf
{
    public class UpdateSelfCommand : IRequest<UpdateSelfCommandDto>
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

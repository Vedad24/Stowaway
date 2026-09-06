namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.CreateUpdate
{
    public class CreateUpdateWarehouseUserCommandValidator : AbstractValidator<CreateUpdateWarehouseUserCommand>
    {
        public CreateUpdateWarehouseUserCommandValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.WarehouseId).GreaterThan(0);
            RuleFor(x => x.PriviledgeGroupId).GreaterThan(0);
        }
    }
}

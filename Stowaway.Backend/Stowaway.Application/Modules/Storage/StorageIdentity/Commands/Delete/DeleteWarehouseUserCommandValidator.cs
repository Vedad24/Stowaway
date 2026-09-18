namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Delete
{
    public class DeleteWarehouseUserCommandValidator : AbstractValidator<DeleteWarehouseUserCommand>
    {
        public DeleteWarehouseUserCommandValidator()
        {
            RuleFor(x => x.WarehouseId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.PriviledgeGroupId).GreaterThan(0);
        }
    }
}

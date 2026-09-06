namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Update
{
    public sealed class UpdatePriviledgeGroupCommandValidator : AbstractValidator<UpdatePriviledgeGroupCommand>
    {
        public UpdatePriviledgeGroupCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.WarehouseId)
                .GreaterThan(0);

            RuleFor(x => x.PriviledgeIds)
                .NotNull();
        }
    }
}

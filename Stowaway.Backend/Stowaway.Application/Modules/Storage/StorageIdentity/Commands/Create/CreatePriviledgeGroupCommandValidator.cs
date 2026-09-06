namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Create
{
    public sealed class CreatePriviledgeGroupCommandValidator : AbstractValidator<CreatePriviledgeGroupCommand>
    {
        public CreatePriviledgeGroupCommandValidator()
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

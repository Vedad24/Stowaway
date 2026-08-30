namespace Market.Application.Modules.Sales.Payment.Commands.Update
{
    public sealed class UpdateStripePaymentCommandValidator : AbstractValidator<UpdateStripePaymentCommand>
    {
        public UpdateStripePaymentCommandValidator()
        {
            RuleFor(x => x.EventType)
                .NotEmpty();

            RuleFor(x => x.EventId)
                .NotEmpty();

            RuleFor(x => x.EventData)
                .NotNull();
        }
    }
}

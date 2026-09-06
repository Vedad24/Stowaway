
namespace Stowaway.Application.Modules.Sales.Order.Commands.Delete
{
    public class DeleteOrderCommandHandler(IAppDbContext db) : IRequestHandler<DeleteOrderCommand, bool>
    {
        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var toRemove = db.Orders.FirstOrDefault(o => o.Id == request.Id);
            if (toRemove == null)
                return false;
            db.Orders.Remove(toRemove);
            await db.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
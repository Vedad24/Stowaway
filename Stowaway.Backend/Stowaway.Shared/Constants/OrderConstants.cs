namespace Stowaway.Shared.Constants
{
    public static class OrderConstants
    {
        // The app doesn't implement discounts and never will, but Discount is a persisted
        // column on OrderItemEntity - keep it at 0 here rather than dropping the column/migration.
        public const decimal DefaultDiscount = 0m;
    }
}

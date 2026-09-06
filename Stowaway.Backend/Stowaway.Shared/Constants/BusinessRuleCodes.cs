namespace Stowaway.Shared.Constants
{
    // Dot-namespaced, descriptive business-rule codes for StowawayBusinessRuleException, one
    // constant per distinct situation - matches the convention already used by Container/Item
    // modules (e.g. "canvas-position.incomplete") instead of terse ad-hoc literals like "P-C-S".
    public static class BusinessRuleCodes
    {
        public const string CartNotOwner = "cart.not-owner";

        public const string OrderNotOwner = "order.not-owner";
        public const string OrderEmpty = "order.empty";
    }
}

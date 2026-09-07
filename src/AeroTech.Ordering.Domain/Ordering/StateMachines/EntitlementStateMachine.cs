using AeroTech.Ordering.Domain.SharedKernel.Enums;

namespace AeroTech.Ordering.Domain.Ordering.StateMachines
{
    public static class EntitlementStateMachine
    {
        private static readonly Dictionary<EntitlementCommercialStatus, EntitlementCommercialStatus[]> Allowed = new()
        {
            [EntitlementCommercialStatus.Pending] =
            [
                EntitlementCommercialStatus.Active,
                EntitlementCommercialStatus.Cancelled
            ],
            [EntitlementCommercialStatus.Active] =
            [
                EntitlementCommercialStatus.Cancelled,
                EntitlementCommercialStatus.Replaced
            ],
            [EntitlementCommercialStatus.Cancelled] = [],
            [EntitlementCommercialStatus.Replaced] = []
        };

        public static bool IsTerminal(EntitlementCommercialStatus status) =>
            status is EntitlementCommercialStatus.Cancelled or EntitlementCommercialStatus.Replaced;

        public static bool CanTransition(EntitlementCommercialStatus from, EntitlementCommercialStatus to) =>
            Allowed.TryGetValue(from, out var targets) && targets.Contains(to);
    }
}

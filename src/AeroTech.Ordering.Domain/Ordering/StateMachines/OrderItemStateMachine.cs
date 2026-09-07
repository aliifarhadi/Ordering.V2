using AeroTech.Ordering.Domain.SharedKernel.Enums;

namespace AeroTech.Ordering.Domain.Ordering.StateMachines
{
    public static class OrderItemStateMachine
    {
        private static readonly Dictionary<OrderItemCommercialStatus, OrderItemCommercialStatus[]> Allowed = new()
        {
            [OrderItemCommercialStatus.Pending] =
            [
                OrderItemCommercialStatus.Confirmed,
                OrderItemCommercialStatus.Cancelled
            ],
            [OrderItemCommercialStatus.Confirmed] =
            [
                OrderItemCommercialStatus.Cancelled,
                OrderItemCommercialStatus.Replaced,
                OrderItemCommercialStatus.PartiallyChanged
            ],
            [OrderItemCommercialStatus.PartiallyChanged] =
            [
                OrderItemCommercialStatus.PartiallyChanged,
                OrderItemCommercialStatus.Cancelled,
                OrderItemCommercialStatus.Replaced
            ],
            [OrderItemCommercialStatus.Cancelled] = [],
            [OrderItemCommercialStatus.Replaced] = []
        };

        public static bool IsTerminal(OrderItemCommercialStatus status) =>
            status is OrderItemCommercialStatus.Cancelled or OrderItemCommercialStatus.Replaced;

        public static bool CanTransition(OrderItemCommercialStatus from, OrderItemCommercialStatus to)
        {
            if (from == to)
                return from is OrderItemCommercialStatus.PartiallyChanged;

            return Allowed.TryGetValue(from, out var targets) && targets.Contains(to);
        }
    }
}

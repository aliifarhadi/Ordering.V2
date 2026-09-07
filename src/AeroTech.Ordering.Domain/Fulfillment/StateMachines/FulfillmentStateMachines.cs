using AeroTech.Ordering.Domain.SharedKernel.Enums;

namespace AeroTech.Ordering.Domain.Fulfillment.StateMachines
{
    public static class TicketCouponStateMachine
    {
        private static readonly Dictionary<TicketCouponStatus, TicketCouponStatus[]> Allowed = new()
        {
            [TicketCouponStatus.Open] =
            [
                TicketCouponStatus.Controlled,
                TicketCouponStatus.Void,
                TicketCouponStatus.Exchanged,
                TicketCouponStatus.Refunded
            ],
            [TicketCouponStatus.Controlled] =
            [
                TicketCouponStatus.Open,
                TicketCouponStatus.CheckedIn,
                TicketCouponStatus.NoShow
            ],
            [TicketCouponStatus.CheckedIn] =
            [
                TicketCouponStatus.Controlled,
                TicketCouponStatus.Boarded
            ],
            [TicketCouponStatus.Boarded] =
            [
                TicketCouponStatus.Controlled,
                TicketCouponStatus.Flown
            ],
            [TicketCouponStatus.Flown] = [TicketCouponStatus.Refunded],
            [TicketCouponStatus.NoShow] = [TicketCouponStatus.Refunded],
            [TicketCouponStatus.Void] = [],
            [TicketCouponStatus.Exchanged] = [],
            [TicketCouponStatus.Refunded] = []
        };

        public static bool CanTransition(TicketCouponStatus from, TicketCouponStatus to) =>
            Allowed.TryGetValue(from, out var targets) && targets.Contains(to);

        // FUL-004: coupons under control require control authority to service.
        public static bool RequiresControlAuthority(TicketCouponStatus status) =>
            status is TicketCouponStatus.Controlled or TicketCouponStatus.CheckedIn or TicketCouponStatus.Boarded;

        public static bool IsTerminal(TicketCouponStatus status) =>
            status is TicketCouponStatus.Void or TicketCouponStatus.Exchanged or TicketCouponStatus.Refunded;
    }

    public static class EmdCouponStateMachine
    {
        private static readonly Dictionary<EmdCouponStatus, EmdCouponStatus[]> Allowed = new()
        {
            [EmdCouponStatus.Open] =
            [
                EmdCouponStatus.Consumed,
                EmdCouponStatus.Void,
                EmdCouponStatus.Exchanged,
                EmdCouponStatus.Refunded
            ],
            [EmdCouponStatus.Consumed] = [EmdCouponStatus.Refunded],
            [EmdCouponStatus.Void] = [],
            [EmdCouponStatus.Exchanged] = [],
            [EmdCouponStatus.Refunded] = []
        };

        public static bool CanTransition(EmdCouponStatus from, EmdCouponStatus to) =>
            Allowed.TryGetValue(from, out var targets) && targets.Contains(to);

        public static bool IsTerminal(EmdCouponStatus status) =>
            status is EmdCouponStatus.Void or EmdCouponStatus.Exchanged or EmdCouponStatus.Refunded;
    }

    public static class SupplierReservationStateMachine
    {
        private static readonly Dictionary<SupplierReservationStatus, SupplierReservationStatus[]> Allowed = new()
        {
            [SupplierReservationStatus.Pending] =
            [
                SupplierReservationStatus.Confirmed,
                SupplierReservationStatus.Rejected
            ],
            [SupplierReservationStatus.Confirmed] =
            [
                SupplierReservationStatus.Cancelled,
                SupplierReservationStatus.Completed
            ],
            [SupplierReservationStatus.Cancelled] = [],
            [SupplierReservationStatus.Rejected] = [],
            [SupplierReservationStatus.Completed] = []
        };

        public static bool CanTransition(SupplierReservationStatus from, SupplierReservationStatus to) =>
            Allowed.TryGetValue(from, out var targets) && targets.Contains(to);
    }
}

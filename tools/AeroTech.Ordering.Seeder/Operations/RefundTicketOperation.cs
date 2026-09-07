using AeroTech.Ordering.Domain.Fulfillment.Aggregates;
using AeroTech.Ordering.Domain.Ordering.Aggregates;
using AeroTech.Ordering.Domain.Ordering.Entities;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using AeroTech.Ordering.Seeder.Scenarios;
using NodaTime;

namespace AeroTech.Ordering.Seeder.Operations
{
    /// <summary>
    /// Refunds one traveller's ticket together with the ancillaries issued to that traveller,
    /// following the CancellationProcess/RefundProcess ordering from the spec: acquire the lock,
    /// withdraw the fulfilment documents, then record the commercial cancellation, then release.
    ///
    /// Ordering does not own refund money. No amount is written here — the financial credit is
    /// Receivables' to produce, and this service only records the resulting commercial change.
    /// </summary>
    internal sealed class RefundTicketOperation
    {
        private readonly Instant _now;
        private readonly IEventIdFactory _eventIds;

        public RefundTicketOperation(Instant now, IEventIdFactory eventIds)
        {
            _now = now;
            _eventIds = eventIds;
        }

        public RefundResult Execute(
            Order order,
            ElectronicTicket ticket,
            ElectronicMiscDocument? document,
            bool includeAncillaries)
        {
            var actor = new ActorContext(
                ActorType.AirlineAgent,
                "seeder-refund",
                order.ServicingAuthority.OwnerSellerId,
                order.ServicingAuthority.OwnerOfficeId,
                true);

            // Coupons under external control (checked in, boarded) cannot be serviced, and a flown
            // coupon is not refundable through this path. Fail before touching anything.
            var blocked = ticket.Coupons
                .Where(coupon => coupon.Status is not TicketCouponStatus.Open)
                .ToArray();

            if (blocked.Length > 0)
            {
                var detail = string.Join(", ", blocked.Select(coupon => $"#{coupon.CouponNumber}={coupon.Status}"));
                throw new InvalidOperationException(
                    $"Ticket {ticket.TicketNumber} has coupons that are not Open: {detail}. Refund aborted.");
            }

            var entitlementIds = ticket.Coupons.Select(coupon => coupon.EntitlementId).ToList();
            var emdEntitlementIds = new List<EntitlementId>();

            if (includeAncillaries && document is not null)
            {
                var blockedEmd = document.Coupons
                    .Where(coupon => coupon.Status is not EmdCouponStatus.Open)
                    .ToArray();

                if (blockedEmd.Length > 0)
                {
                    var detail = string.Join(", ", blockedEmd.Select(coupon => $"#{coupon.CouponNumber}={coupon.Status}"));
                    throw new InvalidOperationException(
                        $"EMD {document.EmdNumber} has coupons that are not Open: {detail}. Refund aborted.");
                }

                emdEntitlementIds.AddRange(document.Coupons.Select(coupon => coupon.EntitlementId));
                entitlementIds.AddRange(emdEntitlementIds);
            }

            // 1. Acquire the Refund lock over the affected commercial scope.
            var workflowId = WorkflowInstanceId.New();
            var lockId = order.AcquireProcessingLock(
                workflowId,
                ProcessingLockType.Refund,
                _now.Plus(Duration.FromMinutes(30)),
                actor,
                _now,
                _eventIds.Next(),
                entitlementRefs: entitlementIds);

            // 2. Withdraw the fulfilment documents first: value must not be returned while the
            //    coupons are still live.
            ticket.RefundCoupons(
                ticket.Coupons.Select(coupon => coupon.Id),
                _now,
                _eventIds.Next());

            if (includeAncillaries && document is not null)
            {
                document.RefundCoupons(
                    document.Coupons.Select(coupon => coupon.Id),
                    _now,
                    _eventIds.Next());
            }

            // 3. Record the commercial consequence: the entitlements are no longer valid.
            order.ApplyCancellation(entitlementIds, actor, _now, _eventIds.Next());

            // 4. Release the lock.
            order.ReleaseProcessingLock(lockId, actor, _now, _eventIds.Next());

            order.CheckInvariants();

            var affectedItems = order.Items
                .Where(item => item.Entitlements.Any(entitlement => entitlementIds.Contains(entitlement.Id)))
                .ToArray();

            return new RefundResult(
                ticket,
                includeAncillaries ? document : null,
                entitlementIds,
                affectedItems);
        }
    }

    internal sealed record RefundResult(
        ElectronicTicket Ticket,
        ElectronicMiscDocument? Document,
        IReadOnlyCollection<EntitlementId> CancelledEntitlementIds,
        IReadOnlyCollection<OrderItem> AffectedItems);
}

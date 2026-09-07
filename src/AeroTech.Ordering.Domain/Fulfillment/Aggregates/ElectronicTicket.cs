using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.Fulfillment.Entities;
using AeroTech.Ordering.Domain.Fulfillment.Events;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.Fulfillment.Aggregates
{
    public sealed class ElectronicTicket : AggregateRoot<ElectronicTicketId>
    {
        private readonly List<TicketCoupon> _coupons = [];

        private ElectronicTicket()
        {
        }

        private ElectronicTicket(
            ElectronicTicketId id,
            string ticketNumber,
            OrderId orderId,
            TravelerId travelerId,
            CarrierCode issuingCarrier,
            Instant issuedAt)
        {
            Id = id;
            TicketNumber = ticketNumber;
            OrderId = orderId;
            TravelerId = travelerId;
            IssuingCarrier = issuingCarrier;
            IssuedAt = issuedAt;
            Status = ElectronicTicketStatus.Issued;
            AggregateVersion = 1;
        }

        public string TicketNumber { get; private set; } = null!;

        // FUL-001: correlation ids only. No navigation into the Ordering schema.
        public OrderId OrderId { get; private set; }

        public TravelerId TravelerId { get; private set; }

        public CarrierCode IssuingCarrier { get; private set; }

        public Instant IssuedAt { get; private set; }

        public ElectronicTicketStatus Status { get; private set; }

        public long AggregateVersion { get; private set; }

        public IReadOnlyCollection<TicketCoupon> Coupons => _coupons.AsReadOnly();

        public static ElectronicTicket Issue(
            ElectronicTicketId id,
            string ticketNumber,
            OrderId orderId,
            TravelerId travelerId,
            CarrierCode issuingCarrier,
            Instant issuedAt,
            IEnumerable<TicketCouponDraft> coupons,
            string eventId)
        {
            if (string.IsNullOrWhiteSpace(ticketNumber))
                throw ExceptionFactory.DocumentNumberIsInvalid(ticketNumber);

            var ticket = new ElectronicTicket(id, ticketNumber, orderId, travelerId, issuingCarrier, issuedAt);

            var couponNumber = 1;
            foreach (var draft in coupons)
            {
                var number = draft.CouponNumber ?? couponNumber;

                // FUL-003: no duplicate coupon numbers on one document.
                if (ticket._coupons.Any(coupon => coupon.CouponNumber == number))
                    throw ExceptionFactory.DuplicateCouponNumber(number);

                // FUL-002: every coupon references one item, one entitlement and one segment.
                ticket._coupons.Add(TicketCoupon.Create(
                    id,
                    number,
                    draft.OrderItemId,
                    draft.EntitlementId,
                    draft.JourneySegmentId,
                    draft.CouponId));

                couponNumber = number + 1;
            }

            if (ticket._coupons.Count == 0)
                throw ExceptionFactory.TicketRequiresAtLeastOneCoupon();

            ticket.Causes(new ElectronicTicketIssued(
                eventId,
                id.ToString(),
                issuedAt.ToDateTimeOffset(),
                ticket.AggregateVersion,
                id,
                ticketNumber,
                orderId,
                travelerId));

            return ticket;
        }

        public void TransferCouponControl(TicketCouponId couponId, string controlHolder, Instant occurredAt, string eventId)
        {
            var coupon = RequireCoupon(couponId);
            coupon.TransferControl(controlHolder, occurredAt);

            BumpVersion();
            Causes(new TicketCouponControlTransferred(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                couponId,
                controlHolder));
        }

        public void ReleaseCouponControl(TicketCouponId couponId, Instant occurredAt, string eventId)
        {
            RequireCoupon(couponId).ReleaseControl();

            BumpVersion();
            Causes(new TicketCouponControlReleased(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                couponId));
        }

        public void ApplyCouponCheckIn(TicketCouponId couponId, Instant occurredAt, string eventId) =>
            ApplyCouponStatus(couponId, TicketCouponStatus.CheckedIn, occurredAt, eventId);

        public void ApplyCouponBoarded(TicketCouponId couponId, Instant occurredAt, string eventId) =>
            ApplyCouponStatus(couponId, TicketCouponStatus.Boarded, occurredAt, eventId);

        public void ApplyCouponOffloaded(TicketCouponId couponId, Instant occurredAt, string eventId) =>
            ApplyCouponStatus(couponId, TicketCouponStatus.Controlled, occurredAt, eventId);

        public void ApplyCouponFlown(TicketCouponId couponId, Instant occurredAt, string eventId) =>
            ApplyCouponStatus(couponId, TicketCouponStatus.Flown, occurredAt, eventId);

        public void ApplyCouponNoShow(TicketCouponId couponId, Instant occurredAt, string eventId) =>
            ApplyCouponStatus(couponId, TicketCouponStatus.NoShow, occurredAt, eventId);

        public void VoidEligibleCoupons(IEnumerable<TicketCouponId> couponIds, Instant occurredAt, string eventId)
        {
            var ids = couponIds.Distinct().ToArray();

            foreach (var couponId in ids)
                RequireCoupon(couponId).Void();

            RecomputeStatus();

            BumpVersion();
            Causes(new TicketCouponsVoided(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                ids));
        }

        public void ExchangeCoupons(IEnumerable<TicketCouponId> couponIds, Instant occurredAt, string eventId)
        {
            var ids = couponIds.Distinct().ToArray();

            foreach (var couponId in ids)
                RequireCoupon(couponId).Exchange();

            RecomputeStatus();

            BumpVersion();
            Causes(new TicketCouponsExchanged(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                ids));
        }

        public void RefundCoupons(IEnumerable<TicketCouponId> couponIds, Instant occurredAt, string eventId)
        {
            var ids = couponIds.Distinct().ToArray();

            foreach (var couponId in ids)
                RequireCoupon(couponId).Refund();

            RecomputeStatus();

            BumpVersion();
            Causes(new TicketCouponsRefunded(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                ids));
        }

        /// <summary>Elevated-authority correction. The caller must have verified authority and recorded an audit reason.</summary>
        public void CorrectCouponStatus(
            TicketCouponId couponId,
            TicketCouponStatus status,
            string reason,
            Instant occurredAt,
            string eventId)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw ExceptionFactory.CancellationRequiresReason();

            RequireCoupon(couponId).CorrectStatus(status);
            RecomputeStatus();

            BumpVersion();
            Causes(new TicketCouponStatusCorrected(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                couponId,
                status,
                reason));
        }

        private void ApplyCouponStatus(TicketCouponId couponId, TicketCouponStatus status, Instant occurredAt, string eventId)
        {
            RequireCoupon(couponId).TransitionTo(status);
            RecomputeStatus();

            BumpVersion();
            Causes(new TicketCouponStatusChanged(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                couponId,
                status));
        }

        private void RecomputeStatus()
        {
            if (_coupons.All(coupon => coupon.Status is TicketCouponStatus.Void))
                Status = ElectronicTicketStatus.Void;
            else if (_coupons.All(coupon => coupon.Status is TicketCouponStatus.Exchanged))
                Status = ElectronicTicketStatus.Exchanged;
            else if (_coupons.All(coupon => coupon.IsTerminal || coupon.Status is TicketCouponStatus.Flown))
                Status = ElectronicTicketStatus.Closed;
            else
                Status = ElectronicTicketStatus.Issued;
        }

        private void BumpVersion() => AggregateVersion++;

        private TicketCoupon RequireCoupon(TicketCouponId couponId) =>
            _coupons.SingleOrDefault(coupon => coupon.Id == couponId)
            ?? throw ExceptionFactory.TicketCouponNotFound(couponId);
    }

    public sealed record TicketCouponDraft(
        OrderItemId OrderItemId,
        EntitlementId EntitlementId,
        JourneySegmentId JourneySegmentId,
        int? CouponNumber = null,
        TicketCouponId? CouponId = null);
}

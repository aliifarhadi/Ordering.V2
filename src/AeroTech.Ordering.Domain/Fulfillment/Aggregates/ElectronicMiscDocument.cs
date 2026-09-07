using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.Fulfillment.Entities;
using AeroTech.Ordering.Domain.Fulfillment.Events;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.Fulfillment.Aggregates
{
    public sealed class ElectronicMiscDocument : AggregateRoot<ElectronicMiscDocumentId>
    {
        private readonly List<EmdCoupon> _coupons = [];

        private ElectronicMiscDocument()
        {
        }

        private ElectronicMiscDocument(
            ElectronicMiscDocumentId id,
            string emdNumber,
            OrderId orderId,
            TravelerId travelerId,
            EmdType type,
            Instant issuedAt)
        {
            Id = id;
            EmdNumber = emdNumber;
            OrderId = orderId;
            TravelerId = travelerId;
            Type = type;
            IssuedAt = issuedAt;
            Status = ElectronicMiscDocumentStatus.Issued;
            AggregateVersion = 1;
        }

        public string EmdNumber { get; private set; } = null!;

        public OrderId OrderId { get; private set; }

        public TravelerId TravelerId { get; private set; }

        public EmdType Type { get; private set; }

        public Instant IssuedAt { get; private set; }

        public ElectronicMiscDocumentStatus Status { get; private set; }

        public long AggregateVersion { get; private set; }

        public IReadOnlyCollection<EmdCoupon> Coupons => _coupons.AsReadOnly();

        public static ElectronicMiscDocument Issue(
            ElectronicMiscDocumentId id,
            string emdNumber,
            OrderId orderId,
            TravelerId travelerId,
            EmdType type,
            Instant issuedAt,
            IEnumerable<EmdCouponDraft> coupons,
            string eventId)
        {
            if (string.IsNullOrWhiteSpace(emdNumber))
                throw ExceptionFactory.DocumentNumberIsInvalid(emdNumber);

            var document = new ElectronicMiscDocument(id, emdNumber, orderId, travelerId, type, issuedAt);

            var couponNumber = 1;
            foreach (var draft in coupons)
            {
                var number = draft.CouponNumber ?? couponNumber;

                if (document._coupons.Any(coupon => coupon.CouponNumber == number))
                    throw ExceptionFactory.DuplicateCouponNumber(number);

                // FUL-020: every EMD coupon references an order item and an entitlement.
                document._coupons.Add(EmdCoupon.Create(
                    id,
                    number,
                    draft.OrderItemId,
                    draft.EntitlementId,
                    draft.JourneySegmentId,
                    draft.CouponId));

                couponNumber = number + 1;
            }

            if (document._coupons.Count == 0)
                throw ExceptionFactory.EmdRequiresAtLeastOneCoupon();

            document.Causes(new ElectronicMiscDocumentIssued(
                eventId,
                id.ToString(),
                issuedAt.ToDateTimeOffset(),
                document.AggregateVersion,
                id,
                emdNumber,
                orderId,
                travelerId));

            return document;
        }

        public void ConsumeCoupon(EmdCouponId couponId, Instant occurredAt, string eventId) =>
            ApplyCouponStatus(couponId, EmdCouponStatus.Consumed, occurredAt, eventId);

        public void VoidCoupons(IEnumerable<EmdCouponId> couponIds, Instant occurredAt, string eventId)
        {
            foreach (var couponId in couponIds.Distinct())
                ApplyCouponStatus(couponId, EmdCouponStatus.Void, occurredAt, eventId);
        }

        public void ExchangeCoupons(IEnumerable<EmdCouponId> couponIds, Instant occurredAt, string eventId)
        {
            foreach (var couponId in couponIds.Distinct())
                ApplyCouponStatus(couponId, EmdCouponStatus.Exchanged, occurredAt, eventId);
        }

        public void RefundCoupons(IEnumerable<EmdCouponId> couponIds, Instant occurredAt, string eventId)
        {
            foreach (var couponId in couponIds.Distinct())
                ApplyCouponStatus(couponId, EmdCouponStatus.Refunded, occurredAt, eventId);
        }

        public void CorrectCouponStatus(
            EmdCouponId couponId,
            EmdCouponStatus status,
            string reason,
            Instant occurredAt,
            string eventId)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw ExceptionFactory.CancellationRequiresReason();

            RequireCoupon(couponId).CorrectStatus(status);
            RecomputeStatus();

            BumpVersion();
            Causes(new EmdCouponStatusChanged(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                couponId,
                status));
        }

        private void ApplyCouponStatus(EmdCouponId couponId, EmdCouponStatus status, Instant occurredAt, string eventId)
        {
            RequireCoupon(couponId).TransitionTo(status);
            RecomputeStatus();

            BumpVersion();
            Causes(new EmdCouponStatusChanged(
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
            if (_coupons.All(coupon => coupon.Status is EmdCouponStatus.Void))
                Status = ElectronicMiscDocumentStatus.Void;
            else if (_coupons.All(coupon => coupon.Status is EmdCouponStatus.Exchanged))
                Status = ElectronicMiscDocumentStatus.Exchanged;
            else if (_coupons.All(coupon => coupon.IsTerminal || coupon.Status is EmdCouponStatus.Consumed))
                Status = ElectronicMiscDocumentStatus.Closed;
            else
                Status = ElectronicMiscDocumentStatus.Issued;
        }

        private void BumpVersion() => AggregateVersion++;

        private EmdCoupon RequireCoupon(EmdCouponId couponId) =>
            _coupons.SingleOrDefault(coupon => coupon.Id == couponId)
            ?? throw ExceptionFactory.EmdCouponNotFound(couponId);
    }

    public sealed record EmdCouponDraft(
        OrderItemId OrderItemId,
        EntitlementId EntitlementId,
        JourneySegmentId? JourneySegmentId = null,
        int? CouponNumber = null,
        EmdCouponId? CouponId = null);
}

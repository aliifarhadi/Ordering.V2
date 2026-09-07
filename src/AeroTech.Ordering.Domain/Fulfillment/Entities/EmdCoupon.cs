using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.Fulfillment.StateMachines;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;

namespace AeroTech.Ordering.Domain.Fulfillment.Entities
{
    public sealed class EmdCoupon : Entity<EmdCouponId>
    {
        private EmdCoupon()
        {
        }

        private EmdCoupon(
            EmdCouponId id,
            ElectronicMiscDocumentId documentId,
            int couponNumber,
            OrderItemId orderItemId,
            EntitlementId entitlementId,
            JourneySegmentId? journeySegmentId,
            EmdCouponStatus status)
        {
            Id = id;
            ElectronicMiscDocumentId = documentId;
            CouponNumber = couponNumber;
            OrderItemId = orderItemId;
            EntitlementId = entitlementId;
            JourneySegmentId = journeySegmentId;
            Status = status;
        }

        public ElectronicMiscDocumentId ElectronicMiscDocumentId { get; private set; }

        public int CouponNumber { get; private set; }

        public OrderItemId OrderItemId { get; private set; }

        public EntitlementId EntitlementId { get; private set; }

        public JourneySegmentId? JourneySegmentId { get; private set; }

        public EmdCouponStatus Status { get; private set; }

        public bool IsTerminal => EmdCouponStateMachine.IsTerminal(Status);

        internal static EmdCoupon Create(
            ElectronicMiscDocumentId documentId,
            int couponNumber,
            OrderItemId orderItemId,
            EntitlementId entitlementId,
            JourneySegmentId? journeySegmentId = null,
            EmdCouponId? couponId = null) =>
            new(
                couponId ?? EmdCouponId.New(),
                documentId,
                couponNumber,
                orderItemId,
                entitlementId,
                journeySegmentId,
                EmdCouponStatus.Open);

        internal void TransitionTo(EmdCouponStatus target)
        {
            if (!EmdCouponStateMachine.CanTransition(Status, target))
                throw ExceptionFactory.EmdCouponCannotTransition(Id, Status, target);

            Status = target;
        }

        internal void CorrectStatus(EmdCouponStatus target) => Status = target;
    }
}

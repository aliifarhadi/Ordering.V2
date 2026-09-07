using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.Fulfillment.StateMachines;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.Fulfillment.Entities
{
    public sealed class TicketCoupon : Entity<TicketCouponId>
    {
        private TicketCoupon()
        {
        }

        private TicketCoupon(
            TicketCouponId id,
            ElectronicTicketId electronicTicketId,
            int couponNumber,
            OrderItemId orderItemId,
            EntitlementId entitlementId,
            JourneySegmentId journeySegmentId,
            TicketCouponStatus status)
        {
            Id = id;
            ElectronicTicketId = electronicTicketId;
            CouponNumber = couponNumber;
            OrderItemId = orderItemId;
            EntitlementId = entitlementId;
            JourneySegmentId = journeySegmentId;
            Status = status;
        }

        public ElectronicTicketId ElectronicTicketId { get; private set; }

        public int CouponNumber { get; private set; }

        public OrderItemId OrderItemId { get; private set; }

        public EntitlementId EntitlementId { get; private set; }

        public JourneySegmentId JourneySegmentId { get; private set; }

        public TicketCouponStatus Status { get; private set; }

        public string? ControlHolder { get; private set; }

        public Instant? ControlAcquiredAt { get; private set; }

        public bool IsTerminal => TicketCouponStateMachine.IsTerminal(Status);

        internal static TicketCoupon Create(
            ElectronicTicketId electronicTicketId,
            int couponNumber,
            OrderItemId orderItemId,
            EntitlementId entitlementId,
            JourneySegmentId journeySegmentId,
            TicketCouponId? couponId = null) =>
            new(
                couponId ?? TicketCouponId.New(),
                electronicTicketId,
                couponNumber,
                orderItemId,
                entitlementId,
                journeySegmentId,
                TicketCouponStatus.Open);

        internal void TransitionTo(TicketCouponStatus target)
        {
            if (!TicketCouponStateMachine.CanTransition(Status, target))
                throw ExceptionFactory.TicketCouponCannotTransition(Id, Status, target);

            Status = target;

            if (target is not TicketCouponStatus.Controlled)
            {
                ControlHolder = null;
                ControlAcquiredAt = null;
            }
        }

        internal void TransferControl(string controlHolder, Instant acquiredAt)
        {
            if (string.IsNullOrWhiteSpace(controlHolder))
                throw ExceptionFactory.ControlHolderIsRequired();

            if (Status is TicketCouponStatus.Open)
                TransitionTo(TicketCouponStatus.Controlled);

            ControlHolder = controlHolder;
            ControlAcquiredAt = acquiredAt;
        }

        internal void ReleaseControl()
        {
            if (Status is TicketCouponStatus.Controlled)
                TransitionTo(TicketCouponStatus.Open);

            ControlHolder = null;
            ControlAcquiredAt = null;
        }

        internal void Void()
        {
            // FUL-005: a flown coupon can never be voided.
            if (Status is TicketCouponStatus.Flown)
                throw ExceptionFactory.FlownCouponCannotBeVoided(Id);

            EnsureServiceable();
            TransitionTo(TicketCouponStatus.Void);
        }

        internal void Exchange()
        {
            EnsureServiceable();
            TransitionTo(TicketCouponStatus.Exchanged);
        }

        internal void Refund() => TransitionTo(TicketCouponStatus.Refunded);

        internal void CorrectStatus(TicketCouponStatus target)
        {
            Status = target;
            if (target is not TicketCouponStatus.Controlled)
            {
                ControlHolder = null;
                ControlAcquiredAt = null;
            }
        }

        private void EnsureServiceable()
        {
            if (TicketCouponStateMachine.RequiresControlAuthority(Status))
                throw ExceptionFactory.TicketCouponRequiresControlAuthority(Id, Status);
        }
    }
}

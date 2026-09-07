using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.Ordering.StateMachines;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class TimeLimit : Entity<TimeLimitId>
    {
        private readonly List<TimeLimitItemRef> _orderItemRefs = [];
        private readonly List<TimeLimitEntitlementRef> _entitlementRefs = [];
        private readonly List<TimeLimitTravelerRef> _travelerRefs = [];

        private TimeLimit()
        {
        }

        private TimeLimit(
            TimeLimitId id,
            OrderId orderId,
            TimeLimitType type,
            Instant dueAt,
            string? policyRef)
        {
            Id = id;
            OrderId = orderId;
            Type = type;
            DueAt = dueAt;
            Status = TimeLimitStatus.Active;
            PolicyRef = policyRef;
            ExtensionCount = 0;
        }

        public OrderId OrderId { get; private set; }

        public TimeLimitType Type { get; private set; }

        public Instant DueAt { get; private set; }

        public TimeLimitStatus Status { get; private set; }

        public string? PolicyRef { get; private set; }

        public int ExtensionCount { get; private set; }

        public IReadOnlyCollection<TimeLimitItemRef> OrderItemRefs => _orderItemRefs.AsReadOnly();

        public IReadOnlyCollection<TimeLimitEntitlementRef> EntitlementRefs => _entitlementRefs.AsReadOnly();

        public IReadOnlyCollection<TimeLimitTravelerRef> TravelerRefs => _travelerRefs.AsReadOnly();

        public IEnumerable<OrderItemId> OrderItemIds => _orderItemRefs.Select(reference => reference.OrderItemId);

        public IEnumerable<EntitlementId> EntitlementIds => _entitlementRefs.Select(reference => reference.EntitlementId);

        public IEnumerable<TravelerId> TravelerIds => _travelerRefs.Select(reference => reference.TravelerId);

        public bool IsActive => Status is TimeLimitStatus.Active;

        // INV-080: a time limit with no explicit scope applies to the whole order.
        public bool IsOrderWide =>
            _orderItemRefs.Count == 0 && _entitlementRefs.Count == 0 && _travelerRefs.Count == 0;

        internal static TimeLimit Create(
            TimeLimitId id,
            OrderId orderId,
            TimeLimitType type,
            Instant dueAt,
            Instant now,
            string? policyRef = null,
            IEnumerable<OrderItemId>? orderItemRefs = null,
            IEnumerable<EntitlementId>? entitlementRefs = null,
            IEnumerable<TravelerId>? travelerRefs = null)
        {
            if (dueAt <= now)
                throw ExceptionFactory.TimeLimitMustBeInTheFuture();

            var timeLimit = new TimeLimit(id, orderId, type, dueAt, policyRef);

            if (orderItemRefs is not null)
                timeLimit._orderItemRefs.AddRange(orderItemRefs.Distinct().Select(itemId => new TimeLimitItemRef(id, itemId)));
            if (entitlementRefs is not null)
                timeLimit._entitlementRefs.AddRange(entitlementRefs.Distinct().Select(entitlementId => new TimeLimitEntitlementRef(id, entitlementId)));
            if (travelerRefs is not null)
                timeLimit._travelerRefs.AddRange(travelerRefs.Distinct().Select(travelerId => new TimeLimitTravelerRef(id, travelerId)));

            return timeLimit;
        }

        internal void Extend(Instant newDueAt, int maximumExtensions)
        {
            EnsureActive();

            if (newDueAt <= DueAt)
                throw ExceptionFactory.TimeLimitExtensionMustBeLater();

            if (ExtensionCount >= maximumExtensions)
                throw ExceptionFactory.TimeLimitExtensionsExhausted(Id);

            DueAt = newDueAt;
            ExtensionCount++;
        }

        internal void MarkMet() => TransitionTo(TimeLimitStatus.Met);

        internal void Expire() => TransitionTo(TimeLimitStatus.Expired);

        internal void Cancel() => TransitionTo(TimeLimitStatus.Cancelled);

        internal bool CoversItem(OrderItemId orderItemId) =>
            IsOrderWide || OrderItemIds.Contains(orderItemId);

        internal bool CoversEntitlement(EntitlementId entitlementId) =>
            IsOrderWide || EntitlementIds.Contains(entitlementId);

        private void TransitionTo(TimeLimitStatus target)
        {
            // INV-081: an expired, cancelled or met time limit cannot change again.
            if (TimeLimitStateMachine.IsTerminal(Status))
                throw ExceptionFactory.TimeLimitIsTerminal(Id, Status);

            if (!TimeLimitStateMachine.CanTransition(Status, target))
                throw ExceptionFactory.TimeLimitNotActive(Id);

            Status = target;
        }

        private void EnsureActive()
        {
            if (!IsActive)
                throw ExceptionFactory.TimeLimitNotActive(Id);
        }
    }

    public sealed record TimeLimitItemRef(TimeLimitId TimeLimitId, OrderItemId OrderItemId);

    public sealed record TimeLimitEntitlementRef(TimeLimitId TimeLimitId, EntitlementId EntitlementId);

    public sealed record TimeLimitTravelerRef(TimeLimitId TimeLimitId, TravelerId TravelerId);
}

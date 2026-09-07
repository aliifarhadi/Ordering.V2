using AeroTech.Framework.Core.Domain.Events;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;

namespace AeroTech.Ordering.Domain.Ordering.Events
{
    public abstract record OrderDomainEvent(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    public sealed record OrderCreated(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        string Reference,
        OrderId RootOrderId,
        OrderId? ParentOrderId,
        string SellerId,
        string ChannelCode)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TravelerAdded(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        TravelerId TravelerId,
        TravelerType TravelerType)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TravelerUpdated(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        TravelerId TravelerId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TravelerRemoved(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        TravelerId TravelerId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record ContactAdded(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        ContactId ContactId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record ContactUpdated(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        ContactId ContactId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record ContactRemoved(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        ContactId ContactId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record JourneyAdded(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        JourneyId JourneyId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record JourneySegmentAdded(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        JourneyId JourneyId,
        JourneySegmentId JourneySegmentId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record JourneySegmentScheduleChanged(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        JourneySegmentId JourneySegmentId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record JourneySegmentRemoved(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        JourneySegmentId JourneySegmentId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record OrderItemAdded(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        OrderItemId OrderItemId,
        ProductType ProductType)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record OrderItemsConfirmed(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        IReadOnlyCollection<OrderItemId> OrderItemIds)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record EntitlementsCancelled(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        IReadOnlyCollection<EntitlementId> EntitlementIds)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record OrderItemsReplaced(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        IReadOnlyCollection<OrderItemId> PredecessorItemIds,
        IReadOnlyCollection<OrderItemId> SuccessorItemIds)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record OrderItemPartiallyChanged(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        OrderItemId OrderItemId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record ServicingDelegationGranted(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        ServicingDelegationId DelegationId,
        string DelegatePartyRef)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record ServicingDelegationRevoked(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        ServicingDelegationId DelegationId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record ExternalReferenceAdded(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        ExternalReferenceId ExternalReferenceId,
        string System,
        string Type,
        string Value)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record ExternalReferenceRemoved(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        ExternalReferenceId ExternalReferenceId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TimeLimitAdded(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        TimeLimitId TimeLimitId,
        TimeLimitType TimeLimitType)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TimeLimitExtended(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        TimeLimitId TimeLimitId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TimeLimitMet(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        TimeLimitId TimeLimitId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TimeLimitExpired(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        TimeLimitId TimeLimitId,
        TimeLimitType TimeLimitType)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TimeLimitCancelled(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        TimeLimitId TimeLimitId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record ProcessingLockAcquired(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        ProcessingLockId ProcessingLockId,
        ProcessingLockType LockType)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record ProcessingLockReleased(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId,
        ProcessingLockId ProcessingLockId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record OrderSplitCompleted(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId SourceOrderId,
        OrderId TargetOrderId,
        IReadOnlyCollection<TravelerId> MovedTravelerIds)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record OrderClosed(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        OrderId OrderId)
        : OrderDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);
}

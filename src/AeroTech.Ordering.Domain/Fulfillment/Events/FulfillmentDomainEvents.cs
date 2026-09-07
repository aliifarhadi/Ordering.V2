using AeroTech.Framework.Core.Domain.Events;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;

namespace AeroTech.Ordering.Domain.Fulfillment.Events
{
    public abstract record FulfillmentDomainEvent(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    public sealed record ElectronicTicketIssued(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        ElectronicTicketId TicketId,
        string TicketNumber,
        OrderId OrderId,
        TravelerId TravelerId)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TicketCouponControlTransferred(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        ElectronicTicketId TicketId,
        TicketCouponId CouponId,
        string ControlHolder)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TicketCouponControlReleased(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        ElectronicTicketId TicketId,
        TicketCouponId CouponId)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TicketCouponStatusChanged(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        ElectronicTicketId TicketId,
        TicketCouponId CouponId,
        TicketCouponStatus Status)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TicketCouponsVoided(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        ElectronicTicketId TicketId,
        IReadOnlyCollection<TicketCouponId> CouponIds)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TicketCouponsExchanged(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        ElectronicTicketId TicketId,
        IReadOnlyCollection<TicketCouponId> CouponIds)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TicketCouponsRefunded(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        ElectronicTicketId TicketId,
        IReadOnlyCollection<TicketCouponId> CouponIds)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record TicketCouponStatusCorrected(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        ElectronicTicketId TicketId,
        TicketCouponId CouponId,
        TicketCouponStatus Status,
        string Reason)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record ElectronicMiscDocumentIssued(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        ElectronicMiscDocumentId DocumentId,
        string EmdNumber,
        OrderId OrderId,
        TravelerId TravelerId)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record EmdCouponStatusChanged(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        ElectronicMiscDocumentId DocumentId,
        EmdCouponId CouponId,
        EmdCouponStatus Status)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record SupplierReservationCreated(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        SupplierReservationId ReservationId,
        OrderId OrderId,
        string SupplierId)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record SupplierReservationConfirmed(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        SupplierReservationId ReservationId,
        string? ExternalConfirmationNumber)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record SupplierReservationRejected(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        SupplierReservationId ReservationId,
        string? Reason)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record SupplierReservationCancelled(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        SupplierReservationId ReservationId)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record SupplierReservationCompleted(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        SupplierReservationId ReservationId)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);

    public sealed record DocumentNumberAllocated(
        string EventId,
        string AggregateId,
        DateTimeOffset TimeOfOccurrence,
        long AggregateVersion,
        DocumentStockId DocumentStockId,
        string DocumentNumber)
        : FulfillmentDomainEvent(EventId, AggregateId, TimeOfOccurrence, AggregateVersion);
}

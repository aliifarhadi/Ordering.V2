using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.Fulfillment.Entities;
using AeroTech.Ordering.Domain.Fulfillment.Events;
using AeroTech.Ordering.Domain.Fulfillment.StateMachines;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.Fulfillment.Aggregates
{
    public sealed class SupplierReservation : AggregateRoot<SupplierReservationId>
    {
        private readonly List<FulfillmentUnit> _fulfillmentUnits = [];

        private SupplierReservation()
        {
        }

        private SupplierReservation(
            SupplierReservationId id,
            OrderId orderId,
            string supplierId,
            ProductType productType,
            Instant createdAt)
        {
            Id = id;
            OrderId = orderId;
            SupplierId = supplierId;
            ProductType = productType;
            CreatedAt = createdAt;
            Status = SupplierReservationStatus.Pending;
            AggregateVersion = 1;
        }

        public OrderId OrderId { get; private set; }

        public string SupplierId { get; private set; } = null!;

        public ProductType ProductType { get; private set; }

        public string? ExternalConfirmationNumber { get; private set; }

        public SupplierReservationStatus Status { get; private set; }

        public Instant CreatedAt { get; private set; }

        public Instant? ConfirmedAt { get; private set; }

        public Instant? CancelledAt { get; private set; }

        public string? FailureReason { get; private set; }

        public long AggregateVersion { get; private set; }

        public IReadOnlyCollection<FulfillmentUnit> FulfillmentUnits => _fulfillmentUnits.AsReadOnly();

        public static SupplierReservation CreatePending(
            SupplierReservationId id,
            OrderId orderId,
            string supplierId,
            ProductType productType,
            Instant createdAt,
            IEnumerable<FulfillmentUnitDraft> units,
            string eventId)
        {
            // FUL-040: a reservation references exactly one external supplier.
            if (string.IsNullOrWhiteSpace(supplierId))
                throw ExceptionFactory.IdentifierIsRequired(nameof(supplierId));

            var reservation = new SupplierReservation(id, orderId, supplierId, productType, createdAt);

            foreach (var draft in units)
            {
                // FUL-041 and FUL-003: one active unit per entitlement.
                if (reservation._fulfillmentUnits.Any(unit => unit.EntitlementId == draft.EntitlementId && unit.IsActive))
                    throw ExceptionFactory.DuplicateActiveFulfillmentUnit(draft.EntitlementId);

                var unit = FulfillmentUnit.Create(
                    draft.OrderItemId,
                    draft.EntitlementId,
                    draft.UnitType,
                    FulfillmentUnitStatus.Pending,
                    draft.TravelerId,
                    draft.JourneySegmentId,
                    supplierId,
                    draft.ExternalReference,
                    draft.FulfillmentUnitId);

                unit.AttachToReservation(id);
                reservation._fulfillmentUnits.Add(unit);
            }

            if (reservation._fulfillmentUnits.Count == 0)
                throw ExceptionFactory.ReservationRequiresFulfillmentUnit(id);

            reservation.Causes(new SupplierReservationCreated(
                eventId,
                id.ToString(),
                createdAt.ToDateTimeOffset(),
                reservation.AggregateVersion,
                id,
                orderId,
                supplierId));

            return reservation;
        }

        public void Confirm(
            string? externalConfirmationNumber,
            bool supplierIssuesConfirmation,
            Instant occurredAt,
            string eventId)
        {
            EnsureCanTransitionTo(SupplierReservationStatus.Confirmed);

            // FUL-042: a confirmed reservation needs an external reference unless provider policy issues none.
            if (supplierIssuesConfirmation && string.IsNullOrWhiteSpace(externalConfirmationNumber))
                throw ExceptionFactory.ConfirmedReservationRequiresConfirmationNumber(Id);

            Status = SupplierReservationStatus.Confirmed;
            ExternalConfirmationNumber = externalConfirmationNumber;
            ConfirmedAt = occurredAt;

            foreach (var unit in _fulfillmentUnits.Where(unit => unit.Status is FulfillmentUnitStatus.Pending))
                unit.MarkActive();

            BumpVersion();
            Causes(new SupplierReservationConfirmed(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                externalConfirmationNumber));
        }

        public void Reject(string? reason, Instant occurredAt, string eventId)
        {
            EnsureCanTransitionTo(SupplierReservationStatus.Rejected);

            Status = SupplierReservationStatus.Rejected;
            FailureReason = reason;

            foreach (var unit in _fulfillmentUnits)
                unit.MarkCancelled();

            BumpVersion();
            Causes(new SupplierReservationRejected(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                reason));
        }

        public void Cancel(Instant occurredAt, string eventId)
        {
            EnsureCanTransitionTo(SupplierReservationStatus.Cancelled);

            Status = SupplierReservationStatus.Cancelled;
            CancelledAt = occurredAt;

            foreach (var unit in _fulfillmentUnits.Where(unit => unit.IsActive))
                unit.MarkCancelled();

            BumpVersion();
            Causes(new SupplierReservationCancelled(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id));
        }

        public void MarkFulfillmentUnitCompleted(FulfillmentUnitId unitId, Instant occurredAt, string eventId)
        {
            var unit = _fulfillmentUnits.SingleOrDefault(candidate => candidate.Id == unitId)
                ?? throw ExceptionFactory.FulfillmentUnitNotFound(unitId);

            unit.MarkCompleted();

            if (_fulfillmentUnits.All(candidate => candidate.Status is FulfillmentUnitStatus.Completed))
            {
                EnsureCanTransitionTo(SupplierReservationStatus.Completed);
                Status = SupplierReservationStatus.Completed;

                BumpVersion();
                Causes(new SupplierReservationCompleted(
                    eventId,
                    Id.ToString(),
                    occurredAt.ToDateTimeOffset(),
                    AggregateVersion,
                    Id));
                return;
            }

            BumpVersion();
        }

        public void CorrectExternalConfirmation(string? externalConfirmationNumber) =>
            ExternalConfirmationNumber = externalConfirmationNumber;

        // FUL-043: cancelled or rejected reservations cannot complete.
        private void EnsureCanTransitionTo(SupplierReservationStatus target)
        {
            if (!SupplierReservationStateMachine.CanTransition(Status, target))
                throw ExceptionFactory.SupplierReservationCannotTransition(Id, Status, target);
        }

        private void BumpVersion() => AggregateVersion++;
    }

    public sealed record FulfillmentUnitDraft(
        OrderItemId OrderItemId,
        EntitlementId EntitlementId,
        FulfillmentUnitType UnitType,
        TravelerId? TravelerId = null,
        JourneySegmentId? JourneySegmentId = null,
        string? ExternalReference = null,
        FulfillmentUnitId? FulfillmentUnitId = null);
}

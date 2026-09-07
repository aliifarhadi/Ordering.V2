using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;

namespace AeroTech.Ordering.Domain.Fulfillment.Entities
{
    public sealed class FulfillmentUnit : Entity<FulfillmentUnitId>
    {
        private FulfillmentUnit()
        {
        }

        private FulfillmentUnit(
            FulfillmentUnitId id,
            OrderItemId orderItemId,
            EntitlementId entitlementId,
            TravelerId? travelerId,
            JourneySegmentId? journeySegmentId,
            string? supplierId,
            FulfillmentUnitType unitType,
            FulfillmentUnitStatus status,
            string? externalReference)
        {
            Id = id;
            OrderItemId = orderItemId;
            EntitlementId = entitlementId;
            TravelerId = travelerId;
            JourneySegmentId = journeySegmentId;
            SupplierId = supplierId;
            UnitType = unitType;
            Status = status;
            ExternalReference = externalReference;
        }

        public OrderItemId OrderItemId { get; private set; }

        public EntitlementId EntitlementId { get; private set; }

        public TravelerId? TravelerId { get; private set; }

        public JourneySegmentId? JourneySegmentId { get; private set; }

        public string? SupplierId { get; private set; }

        public FulfillmentUnitType UnitType { get; private set; }

        public FulfillmentUnitStatus Status { get; private set; }

        public string? ExternalReference { get; private set; }

        public SupplierReservationId? SupplierReservationId { get; private set; }

        public bool IsActive => Status is FulfillmentUnitStatus.Pending or FulfillmentUnitStatus.Active;

        internal static FulfillmentUnit Create(
            OrderItemId orderItemId,
            EntitlementId entitlementId,
            FulfillmentUnitType unitType,
            FulfillmentUnitStatus status = FulfillmentUnitStatus.Pending,
            TravelerId? travelerId = null,
            JourneySegmentId? journeySegmentId = null,
            string? supplierId = null,
            string? externalReference = null,
            FulfillmentUnitId? fulfillmentUnitId = null) =>
            new(
                fulfillmentUnitId ?? FulfillmentUnitId.New(),
                orderItemId,
                entitlementId,
                travelerId,
                journeySegmentId,
                supplierId,
                unitType,
                status,
                externalReference);

        internal void MarkActive() => Status = FulfillmentUnitStatus.Active;

        internal void MarkCompleted() => Status = FulfillmentUnitStatus.Completed;

        internal void MarkCancelled() => Status = FulfillmentUnitStatus.Cancelled;

        internal void MarkWithdrawn() => Status = FulfillmentUnitStatus.Withdrawn;

        internal void AttachToReservation(SupplierReservationId reservationId) => SupplierReservationId = reservationId;

        internal void SetExternalReference(string? externalReference) => ExternalReference = externalReference;
    }
}

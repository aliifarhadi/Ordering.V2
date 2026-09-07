using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class FulfillmentLink : Entity<FulfillmentLinkId>
    {
        private FulfillmentLink()
        {
        }

        private FulfillmentLink(
            FulfillmentLinkId id,
            EntitlementId entitlementId,
            FulfillmentLinkType fulfillmentType,
            Guid fulfillmentAggregateId,
            Guid? fulfillmentUnitId,
            string? externalReference,
            Instant linkedAt)
        {
            Id = id;
            EntitlementId = entitlementId;
            FulfillmentType = fulfillmentType;
            FulfillmentAggregateId = fulfillmentAggregateId;
            FulfillmentUnitId = fulfillmentUnitId;
            ExternalReference = externalReference;
            LinkedAt = linkedAt;
        }

        public EntitlementId EntitlementId { get; private set; }

        public FulfillmentLinkType FulfillmentType { get; private set; }

        public Guid FulfillmentAggregateId { get; private set; }

        public Guid? FulfillmentUnitId { get; private set; }

        public string? ExternalReference { get; private set; }

        public Instant LinkedAt { get; private set; }

        internal static FulfillmentLink Create(
            EntitlementId entitlementId,
            FulfillmentLinkType fulfillmentType,
            Guid fulfillmentAggregateId,
            Guid? fulfillmentUnitId,
            string? externalReference,
            Instant linkedAt) =>
            new(
                FulfillmentLinkId.New(),
                entitlementId,
                fulfillmentType,
                fulfillmentAggregateId,
                fulfillmentUnitId,
                externalReference,
                linkedAt);
    }
}

using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.Consumption.Aggregates
{
    /// <summary>
    /// An append-only operational observation received from DCS or a supplier. A fact is never a mutation of
    /// the commercial Order: it carries no behaviour beyond its own creation.
    /// </summary>
    public sealed class ConsumptionFact : AggregateRoot<ConsumptionFactId>
    {
        private ConsumptionFact()
        {
        }

        private ConsumptionFact(
            ConsumptionFactId id,
            string sourceSystem,
            string sourceEventId,
            OrderId orderId,
            ConsumptionFactType factType,
            Instant occurredAt,
            Instant receivedAt)
        {
            Id = id;
            SourceSystem = sourceSystem;
            SourceEventId = sourceEventId;
            OrderId = orderId;
            FactType = factType;
            OccurredAt = occurredAt;
            ReceivedAt = receivedAt;
        }

        public string SourceSystem { get; private set; } = null!;

        public string SourceEventId { get; private set; } = null!;

        public OrderId OrderId { get; private set; }

        public OrderItemId? OrderItemId { get; private set; }

        public EntitlementId? EntitlementId { get; private set; }

        public FulfillmentUnitId? FulfillmentUnitId { get; private set; }

        public TravelerId? TravelerId { get; private set; }

        public JourneySegmentId? JourneySegmentId { get; private set; }

        public ConsumptionFactType FactType { get; private set; }

        public decimal? QuantityValue { get; private set; }

        public string? QuantityUnit { get; private set; }

        public string? TextValue { get; private set; }

        public Instant OccurredAt { get; private set; }

        public Instant ReceivedAt { get; private set; }

        public string? PayloadJson { get; private set; }

        public static ConsumptionFact Record(
            string sourceSystem,
            string sourceEventId,
            OrderId orderId,
            ConsumptionFactType factType,
            Instant occurredAt,
            Instant receivedAt,
            OrderItemId? orderItemId = null,
            EntitlementId? entitlementId = null,
            FulfillmentUnitId? fulfillmentUnitId = null,
            TravelerId? travelerId = null,
            JourneySegmentId? journeySegmentId = null,
            decimal? quantityValue = null,
            string? quantityUnit = null,
            string? textValue = null,
            string? payloadJson = null,
            ConsumptionFactId? factId = null)
        {
            if (string.IsNullOrWhiteSpace(sourceSystem) || string.IsNullOrWhiteSpace(sourceEventId))
                throw ExceptionFactory.ConsumptionFactSourceIsRequired();

            if (quantityValue is not null && string.IsNullOrWhiteSpace(quantityUnit))
                throw ExceptionFactory.ConsumptionQuantityUnitIsRequired();

            return new ConsumptionFact(
                factId ?? ConsumptionFactId.New(),
                sourceSystem,
                sourceEventId,
                orderId,
                factType,
                occurredAt,
                receivedAt)
            {
                OrderItemId = orderItemId,
                EntitlementId = entitlementId,
                FulfillmentUnitId = fulfillmentUnitId,
                TravelerId = travelerId,
                JourneySegmentId = journeySegmentId,
                QuantityValue = quantityValue,
                QuantityUnit = quantityUnit,
                TextValue = textValue,
                PayloadJson = payloadJson
            };
        }
    }
}

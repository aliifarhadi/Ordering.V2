using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class ValueAllocation : Entity<ValueAllocationId>
    {
        private ValueAllocation()
        {
        }

        private ValueAllocation(
            ValueAllocationId id,
            OrderItemId orderItemId,
            EntitlementId entitlementId,
            JourneySegmentId? journeySegmentId,
            TravelerId? travelerId,
            Money amount,
            string allocationVersion,
            AllocationPurpose purpose)
        {
            Id = id;
            OrderItemId = orderItemId;
            EntitlementId = entitlementId;
            JourneySegmentId = journeySegmentId;
            TravelerId = travelerId;
            Amount = amount.Amount;
            Currency = amount.Currency;
            AllocationVersion = allocationVersion;
            Purpose = purpose;
        }

        public OrderItemId OrderItemId { get; private set; }

        public EntitlementId EntitlementId { get; private set; }

        public JourneySegmentId? JourneySegmentId { get; private set; }

        public TravelerId? TravelerId { get; private set; }

        public decimal Amount { get; private set; }

        public CurrencyCode Currency { get; private set; }

        public string AllocationVersion { get; private set; } = null!;

        public AllocationPurpose Purpose { get; private set; }

        public Money Money => new(Amount, Currency);

        public static ValueAllocation Create(
            OrderItemId orderItemId,
            EntitlementId entitlementId,
            JourneySegmentId? journeySegmentId,
            TravelerId? travelerId,
            Money amount,
            string allocationVersion,
            AllocationPurpose purpose)
        {
            if (string.IsNullOrWhiteSpace(allocationVersion))
                throw ExceptionFactory.IdentifierIsRequired(nameof(allocationVersion));

            return new ValueAllocation(
                ValueAllocationId.New(),
                orderItemId,
                entitlementId,
                journeySegmentId,
                travelerId,
                amount,
                allocationVersion,
                purpose);
        }
    }
}

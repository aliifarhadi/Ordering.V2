namespace AeroTech.Ordering.Domain.SharedKernel.Identifiers
{
    public readonly record struct OrderId(Guid Value)
    {
        public static OrderId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct TravelerId(Guid Value)
    {
        public static TravelerId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct ContactId(Guid Value)
    {
        public static ContactId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct JourneyId(Guid Value)
    {
        public static JourneyId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct JourneySegmentId(Guid Value)
    {
        public static JourneySegmentId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct OrderItemId(Guid Value)
    {
        public static OrderItemId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct EntitlementId(Guid Value)
    {
        public static EntitlementId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct TimeLimitId(Guid Value)
    {
        public static TimeLimitId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct ProcessingLockId(Guid Value)
    {
        public static ProcessingLockId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct ExternalReferenceId(Guid Value)
    {
        public static ExternalReferenceId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct ServicingDelegationId(Guid Value)
    {
        public static ServicingDelegationId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct ChangeId(Guid Value)
    {
        public static ChangeId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct ValueAllocationId(Guid Value)
    {
        public static ValueAllocationId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct ChargeLineId(Guid Value)
    {
        public static ChargeLineId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct FulfillmentLinkId(Guid Value)
    {
        public static FulfillmentLinkId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct IdentityDocumentId(Guid Value)
    {
        public static IdentityDocumentId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct LoyaltyAccountId(Guid Value)
    {
        public static LoyaltyAccountId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct TravelerAssociationId(Guid Value)
    {
        public static TravelerAssociationId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct CommercialTermRestrictionId(Guid Value)
    {
        public static CommercialTermRestrictionId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct WorkflowInstanceId(Guid Value)
    {
        public static WorkflowInstanceId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }
}

namespace AeroTech.Ordering.Domain.SharedKernel.Identifiers
{
    public readonly record struct ElectronicTicketId(Guid Value)
    {
        public static ElectronicTicketId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct TicketCouponId(Guid Value)
    {
        public static TicketCouponId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct ElectronicMiscDocumentId(Guid Value)
    {
        public static ElectronicMiscDocumentId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct EmdCouponId(Guid Value)
    {
        public static EmdCouponId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct SupplierReservationId(Guid Value)
    {
        public static SupplierReservationId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct FulfillmentUnitId(Guid Value)
    {
        public static FulfillmentUnitId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct DocumentStockId(Guid Value)
    {
        public static DocumentStockId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct ConsumptionFactId(Guid Value)
    {
        public static ConsumptionFactId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }

    public readonly record struct ReconciliationCaseId(Guid Value)
    {
        public static ReconciliationCaseId New() => new(Guid.CreateVersion7());

        public override string ToString() => Value.ToString();
    }
}

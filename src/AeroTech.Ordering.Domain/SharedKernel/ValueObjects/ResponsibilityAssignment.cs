namespace AeroTech.Ordering.Domain.SharedKernel.ValueObjects
{
    public sealed record ResponsibilityAssignment(
        string? RetailerRef = null,
        string? SupplierRef = null,
        string? FulfillmentOwnerRef = null,
        string? ServicingOwnerRef = null,
        string? RefundOwnerRef = null,
        string? DisruptionOwnerRef = null,
        string? SettlementOwnerRef = null,
        string? ExternalProductRef = null,
        string? ExternalOrderRef = null,
        string? ExternalServiceRef = null)
    {
        public bool IsExternallySupplied =>
            !string.IsNullOrWhiteSpace(SupplierRef) || !string.IsNullOrWhiteSpace(FulfillmentOwnerRef);

        public bool IsEmpty =>
            string.IsNullOrWhiteSpace(RetailerRef)
            && string.IsNullOrWhiteSpace(SupplierRef)
            && string.IsNullOrWhiteSpace(FulfillmentOwnerRef)
            && string.IsNullOrWhiteSpace(ServicingOwnerRef)
            && string.IsNullOrWhiteSpace(RefundOwnerRef)
            && string.IsNullOrWhiteSpace(DisruptionOwnerRef)
            && string.IsNullOrWhiteSpace(SettlementOwnerRef)
            && string.IsNullOrWhiteSpace(ExternalProductRef)
            && string.IsNullOrWhiteSpace(ExternalOrderRef)
            && string.IsNullOrWhiteSpace(ExternalServiceRef);
    }
}

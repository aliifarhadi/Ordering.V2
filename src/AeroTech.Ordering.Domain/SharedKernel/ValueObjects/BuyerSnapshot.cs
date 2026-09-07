using AeroTech.Ordering.Domain.SharedKernel.Enums;

namespace AeroTech.Ordering.Domain.SharedKernel.ValueObjects
{
    public sealed record BuyerSnapshot(
        BuyerPartyType PartyType,
        string? PartyRef,
        string? Name,
        string? Email,
        string? Phone,
        string? TaxIdentity);
}

using AeroTech.Ordering.Domain.SharedKernel.Enums;

namespace AeroTech.Ordering.Domain.SharedKernel.ValueObjects
{
    public sealed record SettlementArrangement(
        SettlementModel Model,
        string? DebtorPartyRef,
        string? AgreementRef,
        bool CollectionRequired,
        string? ReceivableRef);
}

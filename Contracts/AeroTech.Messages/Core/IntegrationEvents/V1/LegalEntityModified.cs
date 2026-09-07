using AeroTech.Messages.Core.Enums;

namespace AeroTech.Messages.Core.IntegrationEvents.V1
{
    public sealed record LegalEntityModified(
        string Code,
        string Name,
        string LegalName,
        LegalEntityKind Kind,
        ActivationStatus Status,
        string? RegistrationNumber,
        int CountryId,
        int FunctionalCurrencyId,
        int FiscalYearEndMonth,
        string? IataCode,
        string? IcaoCode,
        string? AccountingCode,
        DateOnly EffectiveFrom,
        DateOnly? EffectiveTo) : BaseIntegrationEvent;
}

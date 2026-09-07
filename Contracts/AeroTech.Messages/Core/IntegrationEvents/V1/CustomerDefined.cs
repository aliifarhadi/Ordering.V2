using AeroTech.Messages.Core.Enums;

namespace AeroTech.Messages.Core.IntegrationEvents.V1
{
    public sealed record CustomerDefined(
        CustomerType Type,
        ActivationStatus Status,
        string? Name,
        string? Email,
        string PhoneNumber,
        int? CityId,
        string? Street,
        string? PostalCode,
        string? Unit,
        int PreferredCurrencyId,
        decimal CommissionRate,
        string? Note) : BaseIntegrationEvent;
}

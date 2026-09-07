using AeroTech.Messages.Core.Enums;

namespace AeroTech.Messages.Core.IntegrationEvents.V1
{
    public sealed record TenantActivated(
        string Code,
        string Name,
        string LegalName,
        TenantStatus Status) : BaseIntegrationEvent;
}

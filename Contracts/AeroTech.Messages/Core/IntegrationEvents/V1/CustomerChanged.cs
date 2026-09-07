using AeroTech.Messages.Core.Enums;

namespace AeroTech.Messages.Core.IntegrationEvents.V1
{
    public sealed record CustomerChanged(
        CustomerType Type) : BaseIntegrationEvent;
}

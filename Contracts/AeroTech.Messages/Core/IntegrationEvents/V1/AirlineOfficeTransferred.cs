namespace AeroTech.Messages.Core.IntegrationEvents.V1
{
    public sealed record AirlineOfficeTransferred(
        long FromLegalEntityId,
        long ToLegalEntityId) : BaseIntegrationEvent;
}

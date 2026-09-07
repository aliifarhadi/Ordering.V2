namespace AeroTech.Messages.Core.IntegrationEvents.V1
{
    public sealed record AirlineOfficeModified(
        string Name,
        string Code,
        int CityId,
        long LegalEntityId) : BaseIntegrationEvent;
}

namespace AeroTech.Messages.Core.IntegrationEvents.V1
{
    public sealed record AirlineOfficeDefined(
        string Name,
        string Code,
        int CityId,
        long LegalEntityId) : BaseIntegrationEvent;
}

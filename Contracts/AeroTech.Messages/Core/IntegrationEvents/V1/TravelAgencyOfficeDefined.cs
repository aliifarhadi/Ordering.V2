namespace AeroTech.Messages.Core.IntegrationEvents.V1
{
    public sealed record TravelAgencyOfficeDefined(
        string Name,
        string Code,
        long TravelAgencyId,
        int CityId) : BaseIntegrationEvent;
}

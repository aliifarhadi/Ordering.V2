namespace AeroTech.Messages.Core.IntegrationEvents.V1
{
    public sealed record TravelAgencyOfficeModified(
        string Name,
        string Code,
        long TravelAgencyId,
        int CityId) : BaseIntegrationEvent;
}

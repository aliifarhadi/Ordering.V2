namespace AeroTech.Messages.Core.IntegrationEvents.V1
{
    public sealed record EmployeeUserRemoved(
        long EmployeeId) : BaseIntegrationEvent;
}

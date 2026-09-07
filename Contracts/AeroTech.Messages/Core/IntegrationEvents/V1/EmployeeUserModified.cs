namespace AeroTech.Messages.Core.IntegrationEvents.V1
{
    public sealed record EmployeeUserModified(
        long EmployeeId,
        long UserId,
        long OfficeId,
        long AgentId,
        long? CustomerId) : BaseIntegrationEvent;
}

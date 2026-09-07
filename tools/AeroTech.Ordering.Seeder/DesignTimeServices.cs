using System.Security.Claims;
using AeroTech.Framework.Core.Domain.Events;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.Ordering.Seeder
{
    internal sealed class SeederIdentityService : IIdentityService
    {
        public long? CurrentUserId => 1;

        public long? CurrentCustomerId => null;

        public long RequiredCurrentUserId => 1;

        public Guid RequiredDeviceId => throw new NotSupportedException();

        public bool IsAuthenticated => true;

        public List<Claim>? Claims => null;

        public void CheckAccess(string scopeType, object scopeId)
        {
        }
    }

    internal sealed class SeederClock : IClock
    {
        public DateTimeOffset GetDateTime() => DateTimeOffset.UtcNow;

        public DateOnly GetDate() => DateOnly.FromDateTime(DateTime.UtcNow);
    }

    /// <summary>
    /// The seeder writes current state only. Domain events are collected by the aggregate and
    /// discarded here, because the outbox/integration pipeline belongs to the application layer.
    /// </summary>
    internal sealed class NoOpDomainEventDispatcher : IDomainEventDispatcher
    {
        public Task DispatchAsync(
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}

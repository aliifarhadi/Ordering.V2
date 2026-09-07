using System.Security.Claims;
using AeroTech.Framework.Core.Domain.Events;
using AeroTech.Framework.Core.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AeroTech.Ordering.Persistence
{
    public sealed class OrderingDbContextFactory : IDesignTimeDbContextFactory<OrderingDbContext>
    {
        private const string DesignTimeConnectionString =
            "Server=localhost\\SQLEXPRESS;Database=DotAirOrderNewV2;Trusted_Connection=True;TrustServerCertificate=True";

        public OrderingDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<OrderingDbContext>()
                .UseSqlServer(
                    DesignTimeConnectionString,
                    sql => sql.MigrationsHistoryTable(
                        OrderingDbContext.MigrationsHistoryTable,
                        OrderingDbContext.MigrationsHistorySchema))
                .Options;

            return new OrderingDbContext(
                options,
                new DesignTimeIdentityService(),
                new DesignTimeClock(),
                new DesignTimeDomainEventDispatcher());
        }

        private sealed class DesignTimeIdentityService : IIdentityService
        {
            public long? CurrentUserId => null;

            public long? CurrentCustomerId => null;

            public long RequiredCurrentUserId => throw new NotSupportedException();

            public Guid RequiredDeviceId => throw new NotSupportedException();

            public bool IsAuthenticated => false;

            public List<Claim>? Claims => null;

            public void CheckAccess(string scopeType, object scopeId)
            {
            }
        }

        private sealed class DesignTimeClock : IClock
        {
            public DateTimeOffset GetDateTime() => DateTimeOffset.UtcNow;

            public DateOnly GetDate() => DateOnly.FromDateTime(DateTime.UtcNow);
        }

        private sealed class DesignTimeDomainEventDispatcher : IDomainEventDispatcher
        {
            public Task DispatchAsync(
                IReadOnlyCollection<IDomainEvent> domainEvents,
                CancellationToken cancellationToken = default) => Task.CompletedTask;
        }
    }
}

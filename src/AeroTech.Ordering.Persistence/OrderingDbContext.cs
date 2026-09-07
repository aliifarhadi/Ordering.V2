using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Framework.Infrastructure.Persistence;
using AeroTech.Ordering.Domain.Consumption.Aggregates;
using AeroTech.Ordering.Domain.Fulfillment.Aggregates;
using AeroTech.Ordering.Domain.Ordering.Aggregates;
using AeroTech.Ordering.Persistence.Ordering.History;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.Ordering.Persistence
{
    public sealed class OrderingDbContext : CommandDbContext, IUnitOfWork
    {
        public const string MigrationsHistorySchema = "dbo";
        public const string MigrationsHistoryTable = "__CommandsMigrationHistory";

        public const string OrderingSchema = "ordering";
        public const string FulfillmentSchema = "fulfillment";
        public const string ConsumptionSchema = "consumption";
        public const string WorkflowSchema = "workflow";
        public const string IntegrationSchema = "integration";

        public OrderingDbContext(
            DbContextOptions<OrderingDbContext> options,
            IIdentityService identityService,
            IClock clock,
            IDomainEventDispatcher domainEventDispatcher)
            : base(options, identityService, clock, domainEventDispatcher)
        {
        }

        public DbSet<Order> Orders => Set<Order>();

        public DbSet<OrderHistoryEntry> OrderHistory => Set<OrderHistoryEntry>();

        public DbSet<OrderSnapshot> OrderSnapshots => Set<OrderSnapshot>();

        public DbSet<ElectronicTicket> ElectronicTickets => Set<ElectronicTicket>();

        public DbSet<ElectronicMiscDocument> ElectronicMiscDocuments => Set<ElectronicMiscDocument>();

        public DbSet<SupplierReservation> SupplierReservations => Set<SupplierReservation>();

        public DbSet<DocumentStock> DocumentStocks => Set<DocumentStock>();

        public DbSet<ConsumptionFact> ConsumptionFacts => Set<ConsumptionFact>();

        public DbSet<ReconciliationCase> ReconciliationCases => Set<ReconciliationCase>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(OrderingSchema);
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(19, 4);
            configurationBuilder.Properties<string>().HaveMaxLength(256);
        }
    }
}

using AeroTech.Ordering.Domain.Consumption.Aggregates;
using AeroTech.Ordering.Persistence._Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Consumption.Configurations
{
    public sealed class ConsumptionFactConfiguration : IEntityTypeConfiguration<ConsumptionFact>
    {
        public void Configure(EntityTypeBuilder<ConsumptionFact> builder)
        {
            builder.ToTable("ConsumptionFacts", OrderingDbContext.ConsumptionSchema);

            builder.HasKey(fact => fact.Id);
            builder.Property(fact => fact.Id)
                .HasConversion<ConsumptionFactIdConverter>()
                .ValueGeneratedNever();

            builder.Property(fact => fact.SourceSystem)
                .HasColumnType("varchar(64)")
                .IsRequired();

            builder.Property(fact => fact.SourceEventId)
                .HasColumnType("varchar(128)")
                .IsRequired();

            // Inbound DCS/supplier deduplication.
            builder.HasIndex(fact => new { fact.SourceSystem, fact.SourceEventId }).IsUnique();

            builder.Property(fact => fact.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasIndex(fact => fact.OrderId);

            builder.Property(fact => fact.OrderItemId).HasConversion<NullableOrderItemIdConverter>();
            builder.Property(fact => fact.EntitlementId).HasConversion<NullableEntitlementIdConverter>();
            builder.Property(fact => fact.FulfillmentUnitId).HasConversion<NullableFulfillmentUnitIdConverter>();
            builder.Property(fact => fact.TravelerId).HasConversion<NullableTravelerIdConverter>();
            builder.Property(fact => fact.JourneySegmentId).HasConversion<NullableJourneySegmentIdConverter>();

            builder.HasIndex(fact => fact.EntitlementId);
            builder.HasIndex(fact => fact.JourneySegmentId);

            builder.Property(fact => fact.FactType)
                .HasConversion<string>()
                .HasColumnType("varchar(64)")
                .IsRequired();

            builder.Property(fact => fact.QuantityValue).HasColumnType("decimal(19,4)");
            builder.Property(fact => fact.QuantityUnit).HasColumnType("varchar(16)");
            builder.Property(fact => fact.TextValue).HasMaxLength(512);

            builder.Property(fact => fact.OccurredAt)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnName("OccurredAtUtc")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(fact => fact.ReceivedAt)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnName("ReceivedAtUtc")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(fact => fact.PayloadJson).HasColumnType("nvarchar(max)");

            builder.HasIndex(fact => fact.OccurredAt);

            builder.Property(fact => fact.RowVersion).IsRowVersion();
            builder.Property(fact => fact.LastUpdateTime);
            builder.Property(fact => fact.LastUpdatedBy);
        }
    }

    public sealed class ReconciliationCaseConfiguration : IEntityTypeConfiguration<ReconciliationCase>
    {
        public void Configure(EntityTypeBuilder<ReconciliationCase> builder)
        {
            builder.ToTable("ReconciliationCases", OrderingDbContext.ConsumptionSchema);

            builder.HasKey(reconciliationCase => reconciliationCase.Id);
            builder.Property(reconciliationCase => reconciliationCase.Id)
                .HasConversion<ReconciliationCaseIdConverter>()
                .ValueGeneratedNever();

            builder.Property(reconciliationCase => reconciliationCase.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasIndex(reconciliationCase => reconciliationCase.OrderId);

            builder.Property(reconciliationCase => reconciliationCase.TravelerId)
                .HasConversion<NullableTravelerIdConverter>();

            builder.Property(reconciliationCase => reconciliationCase.EntitlementId)
                .HasConversion<NullableEntitlementIdConverter>();

            builder.Property(reconciliationCase => reconciliationCase.Type)
                .HasConversion<string>()
                .HasColumnType("varchar(40)")
                .IsRequired();

            builder.Property(reconciliationCase => reconciliationCase.Status)
                .HasConversion<string>()
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(reconciliationCase => reconciliationCase.ExpectedValue).HasColumnType("decimal(19,4)");
            builder.Property(reconciliationCase => reconciliationCase.ObservedValue).HasColumnType("decimal(19,4)");
            builder.Property(reconciliationCase => reconciliationCase.Difference).HasColumnType("decimal(19,4)");
            builder.Property(reconciliationCase => reconciliationCase.ValueUnit).HasColumnType("varchar(16)");

            builder.Property(reconciliationCase => reconciliationCase.Classification)
                .HasConversion<string>()
                .HasColumnType("varchar(40)");

            builder.Property(reconciliationCase => reconciliationCase.WorkflowInstanceId)
                .HasConversion<NullableWorkflowInstanceIdConverter>();

            builder.Property(reconciliationCase => reconciliationCase.OpenedAt)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnName("OpenedAtUtc")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(reconciliationCase => reconciliationCase.ResolvedAt)
                .HasConversion<NullableInstantToDateTimeConverter>()
                .HasColumnName("ResolvedAtUtc")
                .HasColumnType("datetime2(7)");

            // Reconciliation work queue.
            builder.HasIndex(reconciliationCase => new { reconciliationCase.Status, reconciliationCase.OpenedAt });

            builder.Property(reconciliationCase => reconciliationCase.AggregateVersion).IsRequired();
            builder.Property(reconciliationCase => reconciliationCase.RowVersion).IsRowVersion();

            builder.Property(reconciliationCase => reconciliationCase.LastUpdateTime);
            builder.Property(reconciliationCase => reconciliationCase.LastUpdatedBy);
        }
    }
}

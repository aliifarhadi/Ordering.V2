using AeroTech.Ordering.Domain.Ordering.Entities;
using AeroTech.Ordering.Persistence._Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Ordering.Configurations
{
    public sealed class ChargeLineConfiguration : IEntityTypeConfiguration<ChargeLine>
    {
        public void Configure(EntityTypeBuilder<ChargeLine> builder)
        {
            builder.ToTable("ChargeLines", OrderingDbContext.OrderingSchema);

            builder.HasKey(line => line.Id);
            builder.Property(line => line.Id)
                .HasConversion<ChargeLineIdConverter>()
                .ValueGeneratedNever();

            builder.Property(line => line.OrderItemId)
                .HasConversion<OrderItemIdConverter>()
                .IsRequired();

            builder.HasIndex(line => new { line.OrderItemId, line.Sequence }).IsUnique();

            builder.Property(line => line.Sequence).IsRequired();

            builder.Property(line => line.Type)
                .HasConversion<string>()
                .HasColumnName("ChargeType")
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(line => line.Code)
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(line => line.Description).HasMaxLength(256);

            builder.Property(line => line.Amount)
                .HasColumnType("decimal(19,4)")
                .IsRequired();

            builder.Property(line => line.Currency)
                .HasConversion<CurrencyCodeConverter>()
                .HasColumnType("char(3)")
                .IsRequired();

            builder.Property(line => line.Refundable).IsRequired();

            builder.Property(line => line.TaxJurisdiction).HasColumnType("varchar(32)");

            builder.Property(line => line.LastUpdateTime);
            builder.Property(line => line.LastUpdatedBy);

            builder.Ignore(line => line.Money);
        }
    }

    public sealed class ValueAllocationConfiguration : IEntityTypeConfiguration<ValueAllocation>
    {
        public void Configure(EntityTypeBuilder<ValueAllocation> builder)
        {
            builder.ToTable("ValueAllocations", OrderingDbContext.OrderingSchema);

            builder.HasKey(allocation => allocation.Id);
            builder.Property(allocation => allocation.Id)
                .HasConversion<ValueAllocationIdConverter>()
                .ValueGeneratedNever();

            builder.Property(allocation => allocation.OrderItemId)
                .HasConversion<OrderItemIdConverter>()
                .IsRequired();

            builder.Property(allocation => allocation.EntitlementId)
                .HasConversion<EntitlementIdConverter>()
                .IsRequired();

            builder.HasIndex(allocation => allocation.OrderItemId);
            builder.HasIndex(allocation => allocation.EntitlementId);

            builder.Property(allocation => allocation.JourneySegmentId)
                .HasConversion<NullableJourneySegmentIdConverter>();

            builder.Property(allocation => allocation.TravelerId)
                .HasConversion<NullableTravelerIdConverter>();

            builder.Property(allocation => allocation.Amount)
                .HasColumnType("decimal(19,4)")
                .IsRequired();

            builder.Property(allocation => allocation.Currency)
                .HasConversion<CurrencyCodeConverter>()
                .HasColumnType("char(3)")
                .IsRequired();

            builder.Property(allocation => allocation.AllocationVersion)
                .HasColumnType("varchar(64)")
                .IsRequired();

            builder.Property(allocation => allocation.Purpose)
                .HasConversion<string>()
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(allocation => allocation.LastUpdateTime);
            builder.Property(allocation => allocation.LastUpdatedBy);

            builder.Ignore(allocation => allocation.Money);
        }
    }
}

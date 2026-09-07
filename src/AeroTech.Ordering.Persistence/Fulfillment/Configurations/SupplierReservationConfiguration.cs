using AeroTech.Ordering.Domain.Fulfillment.Aggregates;
using AeroTech.Ordering.Domain.Fulfillment.Entities;
using AeroTech.Ordering.Persistence._Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Fulfillment.Configurations
{
    public sealed class SupplierReservationConfiguration : IEntityTypeConfiguration<SupplierReservation>
    {
        public void Configure(EntityTypeBuilder<SupplierReservation> builder)
        {
            builder.ToTable("SupplierReservations", OrderingDbContext.FulfillmentSchema);

            builder.HasKey(reservation => reservation.Id);
            builder.Property(reservation => reservation.Id)
                .HasConversion<SupplierReservationIdConverter>()
                .ValueGeneratedNever();

            builder.Property(reservation => reservation.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasIndex(reservation => reservation.OrderId);

            builder.Property(reservation => reservation.SupplierId)
                .HasColumnType("varchar(64)")
                .IsRequired();

            builder.HasIndex(reservation => reservation.SupplierId);

            builder.Property(reservation => reservation.ProductType)
                .HasConversion<string>()
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(reservation => reservation.ExternalConfirmationNumber)
                .HasColumnType("varchar(128)");

            builder.HasIndex(reservation => reservation.ExternalConfirmationNumber);

            builder.Property(reservation => reservation.Status)
                .HasConversion<string>()
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.Property(reservation => reservation.CreatedAt)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnName("CreatedAtUtc")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(reservation => reservation.ConfirmedAt)
                .HasConversion<NullableInstantToDateTimeConverter>()
                .HasColumnName("ConfirmedAtUtc")
                .HasColumnType("datetime2(7)");

            builder.Property(reservation => reservation.CancelledAt)
                .HasConversion<NullableInstantToDateTimeConverter>()
                .HasColumnName("CancelledAtUtc")
                .HasColumnType("datetime2(7)");

            builder.Property(reservation => reservation.FailureReason).HasMaxLength(1000);

            builder.Property(reservation => reservation.AggregateVersion).IsRequired();
            builder.Property(reservation => reservation.RowVersion).IsRowVersion();

            builder.Property(reservation => reservation.LastUpdateTime);
            builder.Property(reservation => reservation.LastUpdatedBy);

            builder.HasMany(reservation => reservation.FulfillmentUnits)
                .WithOne()
                .HasForeignKey(unit => unit.SupplierReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(reservation => reservation.FulfillmentUnits).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    public sealed class FulfillmentUnitConfiguration : IEntityTypeConfiguration<FulfillmentUnit>
    {
        public void Configure(EntityTypeBuilder<FulfillmentUnit> builder)
        {
            builder.ToTable("FulfillmentUnits", OrderingDbContext.FulfillmentSchema);

            builder.HasKey(unit => unit.Id);
            builder.Property(unit => unit.Id)
                .HasConversion<FulfillmentUnitIdConverter>()
                .ValueGeneratedNever();

            builder.Property(unit => unit.SupplierReservationId)
                .HasConversion<NullableSupplierReservationIdConverter>();

            builder.Property(unit => unit.OrderItemId)
                .HasConversion<OrderItemIdConverter>()
                .IsRequired();

            builder.Property(unit => unit.EntitlementId)
                .HasConversion<EntitlementIdConverter>()
                .IsRequired();

            builder.HasIndex(unit => unit.OrderItemId);
            builder.HasIndex(unit => unit.EntitlementId);

            builder.Property(unit => unit.TravelerId)
                .HasConversion<NullableTravelerIdConverter>();

            builder.Property(unit => unit.JourneySegmentId)
                .HasConversion<NullableJourneySegmentIdConverter>();

            builder.Property(unit => unit.SupplierId).HasColumnType("varchar(64)");

            builder.Property(unit => unit.UnitType)
                .HasConversion<string>()
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(unit => unit.Status)
                .HasConversion<string>()
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.Property(unit => unit.ExternalReference).HasColumnType("varchar(128)");

            builder.Property(unit => unit.LastUpdateTime);
            builder.Property(unit => unit.LastUpdatedBy);

            builder.Ignore(unit => unit.IsActive);
        }
    }

    public sealed class DocumentStockConfiguration : IEntityTypeConfiguration<DocumentStock>
    {
        public void Configure(EntityTypeBuilder<DocumentStock> builder)
        {
            builder.ToTable(
                "DocumentStocks",
                OrderingDbContext.FulfillmentSchema,
                table => table.HasCheckConstraint("CK_DocumentStocks_Range", "[RangeEnd] >= [RangeStart]"));

            builder.HasKey(stock => stock.Id);
            builder.Property(stock => stock.Id)
                .HasConversion<DocumentStockIdConverter>()
                .ValueGeneratedNever();

            builder.Property(stock => stock.OwnerCarrier)
                .HasConversion<CarrierCodeConverter>()
                .HasColumnType("varchar(3)")
                .IsRequired();

            builder.Property(stock => stock.OfficeRef).HasColumnType("varchar(64)");

            builder.Property(stock => stock.DocumentType)
                .HasConversion<string>()
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.Property(stock => stock.Prefix).HasColumnType("varchar(8)");

            builder.Property(stock => stock.RangeStart).IsRequired();
            builder.Property(stock => stock.RangeEnd).IsRequired();
            builder.Property(stock => stock.NextAvailable).IsRequired();

            builder.Property(stock => stock.Status)
                .HasConversion<string>()
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.HasIndex(stock => new { stock.OwnerCarrier, stock.DocumentType, stock.Status });

            builder.Property(stock => stock.AggregateVersion).IsRequired();
            builder.Property(stock => stock.RowVersion).IsRowVersion();

            builder.Property(stock => stock.LastUpdateTime);
            builder.Property(stock => stock.LastUpdatedBy);
        }
    }
}

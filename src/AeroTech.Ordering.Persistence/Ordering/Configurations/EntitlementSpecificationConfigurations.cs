using AeroTech.Ordering.Domain.Ordering.Specifications;
using AeroTech.Ordering.Persistence._Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Ordering.Configurations
{
    public sealed class EntitlementSpecificationConfiguration : IEntityTypeConfiguration<EntitlementSpecification>
    {
        public void Configure(EntityTypeBuilder<EntitlementSpecification> builder)
        {
            builder.UseTptMappingStrategy();

            builder.ToTable("EntitlementSpecifications", OrderingDbContext.OrderingSchema);

            builder.HasKey(specification => specification.EntitlementId);
            builder.Property(specification => specification.EntitlementId)
                .HasConversion<EntitlementIdConverter>()
                .ValueGeneratedNever();
        }
    }

    public sealed class AirTransportSpecificationConfiguration : IEntityTypeConfiguration<AirTransportSpecification>
    {
        public void Configure(EntityTypeBuilder<AirTransportSpecification> builder)
        {
            builder.ToTable("AirTransportSpecs", OrderingDbContext.OrderingSchema);

            builder.Property(specification => specification.Cabin)
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(specification => specification.Rbd).HasColumnType("varchar(4)");
            builder.Property(specification => specification.BrandCode).HasColumnType("varchar(32)");
            builder.Property(specification => specification.FareBasisCode).HasColumnType("varchar(32)");
        }
    }

    public sealed class SeatSpecificationConfiguration : IEntityTypeConfiguration<SeatSpecification>
    {
        public void Configure(EntityTypeBuilder<SeatSpecification> builder)
        {
            builder.ToTable("SeatSpecs", OrderingDbContext.OrderingSchema);

            builder.Property(specification => specification.SeatNumber).HasColumnType("varchar(8)");

            builder.PrimitiveCollection(specification => specification.Characteristics)
                .HasColumnName("Characteristics")
                .HasColumnType("nvarchar(max)")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    public sealed class BaggageSpecificationConfiguration : IEntityTypeConfiguration<BaggageSpecification>
    {
        public void Configure(EntityTypeBuilder<BaggageSpecification> builder)
        {
            builder.ToTable(
                "BaggageSpecs",
                OrderingDbContext.OrderingSchema,
                table => table.HasCheckConstraint(
                    "CK_BaggageSpecs_Allowance",
                    "([AllowanceKind] = 'Weight' AND [WeightKg] IS NOT NULL AND [WeightKg] > 0) "
                    + "OR ([AllowanceKind] = 'Piece' AND [Pieces] IS NOT NULL AND [Pieces] > 0)"));

            builder.Property(specification => specification.AllowanceKind)
                .HasConversion<string>()
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.Property(specification => specification.WeightKg).HasColumnType("decimal(9,3)");
            builder.Property(specification => specification.Pieces);
            builder.Property(specification => specification.MaxPieceWeightKg).HasColumnType("decimal(9,3)");
            builder.Property(specification => specification.DimensionRuleCode).HasColumnType("varchar(64)");
        }
    }

    public sealed class MealSpecificationConfiguration : IEntityTypeConfiguration<MealSpecification>
    {
        public void Configure(EntityTypeBuilder<MealSpecification> builder)
        {
            builder.ToTable("MealSpecs", OrderingDbContext.OrderingSchema);

            builder.Property(specification => specification.MealCode)
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.PrimitiveCollection(specification => specification.DietaryAttributes)
                .HasColumnName("DietaryAttributes")
                .HasColumnType("nvarchar(max)")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    public sealed class LoungeSpecificationConfiguration : IEntityTypeConfiguration<LoungeSpecification>
    {
        public void Configure(EntityTypeBuilder<LoungeSpecification> builder)
        {
            builder.ToTable(
                "LoungeSpecs",
                OrderingDbContext.OrderingSchema,
                table => table.HasCheckConstraint("CK_LoungeSpecs_AccessCount", "[AccessCount] > 0"));

            builder.Property(specification => specification.LoungeCode).HasColumnType("varchar(32)");
            builder.Property(specification => specification.AccessCount).IsRequired();
        }
    }

    public sealed class AccommodationSpecificationConfiguration : IEntityTypeConfiguration<AccommodationSpecification>
    {
        public void Configure(EntityTypeBuilder<AccommodationSpecification> builder)
        {
            builder.ToTable(
                "AccommodationSpecs",
                OrderingDbContext.OrderingSchema,
                table =>
                {
                    table.HasCheckConstraint("CK_AccommodationSpecs_DateRange", "[CheckOutDate] > [CheckInDate]");
                    table.HasCheckConstraint(
                        "CK_AccommodationSpecs_Occupancy",
                        "[OccupancyAdults] > 0 AND [OccupancyChildren] >= 0");
                });

            builder.Property(specification => specification.PropertyRef)
                .HasColumnType("varchar(128)")
                .IsRequired();

            builder.Property(specification => specification.PropertyName)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(specification => specification.RoomType)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(specification => specification.BoardBasis)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(specification => specification.OccupancyAdults).IsRequired();
            builder.Property(specification => specification.OccupancyChildren).IsRequired();

            builder.Property(specification => specification.CheckInDate)
                .HasConversion<LocalDateToDateOnlyConverter>()
                .HasColumnType("date")
                .IsRequired();

            builder.Property(specification => specification.CheckOutDate)
                .HasConversion<LocalDateToDateOnlyConverter>()
                .HasColumnType("date")
                .IsRequired();

            builder.Property(specification => specification.RatePlanCode).HasColumnType("varchar(64)");
            builder.Property(specification => specification.CancellationPolicyText).HasMaxLength(2000);
        }
    }

    public sealed class TransferSpecificationConfiguration : IEntityTypeConfiguration<TransferSpecification>
    {
        public void Configure(EntityTypeBuilder<TransferSpecification> builder)
        {
            builder.ToTable("TransferSpecs", OrderingDbContext.OrderingSchema);

            builder.Property(specification => specification.OriginLocation)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(specification => specification.DestinationLocation)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(specification => specification.ServiceDate)
                .HasConversion<LocalDateToDateOnlyConverter>()
                .HasColumnType("date")
                .IsRequired();

            builder.Property(specification => specification.VehicleClass).HasColumnType("varchar(32)");
        }
    }

    public sealed class InsuranceSpecificationConfiguration : IEntityTypeConfiguration<InsuranceSpecification>
    {
        public void Configure(EntityTypeBuilder<InsuranceSpecification> builder)
        {
            builder.ToTable(
                "InsuranceSpecs",
                OrderingDbContext.OrderingSchema,
                table => table.HasCheckConstraint(
                    "CK_InsuranceSpecs_CoverageRange",
                    "[CoverageEnd] >= [CoverageStart]"));

            builder.Property(specification => specification.PolicyProductCode)
                .HasColumnType("varchar(64)")
                .IsRequired();

            builder.Property(specification => specification.CoverageStart)
                .HasConversion<LocalDateToDateOnlyConverter>()
                .HasColumnType("date")
                .IsRequired();

            builder.Property(specification => specification.CoverageEnd)
                .HasConversion<LocalDateToDateOnlyConverter>()
                .HasColumnType("date")
                .IsRequired();

            builder.Property(specification => specification.CoverageSummary)
                .HasMaxLength(1000)
                .IsRequired();
        }
    }
}

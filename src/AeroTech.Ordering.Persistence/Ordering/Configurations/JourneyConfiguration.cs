using AeroTech.Ordering.Domain.Ordering.Entities;
using AeroTech.Ordering.Persistence._Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Ordering.Configurations
{
    public sealed class JourneyConfiguration : IEntityTypeConfiguration<Journey>
    {
        public void Configure(EntityTypeBuilder<Journey> builder)
        {
            builder.ToTable("Journeys", OrderingDbContext.OrderingSchema);

            builder.HasKey(journey => journey.Id);
            builder.Property(journey => journey.Id)
                .HasConversion<JourneyIdConverter>()
                .ValueGeneratedNever();

            builder.Property(journey => journey.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasIndex(journey => journey.OrderId);

            builder.Property(journey => journey.Origin)
                .HasConversion<AirportCodeConverter>()
                .HasColumnType("char(3)")
                .IsRequired();

            builder.Property(journey => journey.Destination)
                .HasConversion<AirportCodeConverter>()
                .HasColumnType("char(3)")
                .IsRequired();

            builder.Property(journey => journey.Sequence).IsRequired();

            builder.Property(journey => journey.LastUpdateTime);
            builder.Property(journey => journey.LastUpdatedBy);

            builder.HasMany(journey => journey.Segments)
                .WithOne()
                .HasForeignKey(segment => segment.JourneyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(journey => journey.Segments).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(journey => journey.SegmentRefs);
        }
    }

    public sealed class JourneySegmentConfiguration : IEntityTypeConfiguration<JourneySegment>
    {
        public void Configure(EntityTypeBuilder<JourneySegment> builder)
        {
            builder.ToTable("JourneySegments", OrderingDbContext.OrderingSchema);

            builder.HasKey(segment => segment.Id);
            builder.Property(segment => segment.Id)
                .HasConversion<JourneySegmentIdConverter>()
                .ValueGeneratedNever();

            builder.Property(segment => segment.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.Property(segment => segment.JourneyId)
                .HasConversion<JourneyIdConverter>()
                .IsRequired();

            builder.HasIndex(segment => segment.OrderId);
            builder.HasIndex(segment => new { segment.JourneyId, segment.Sequence }).IsUnique();

            builder.Property(segment => segment.MarketingCarrier)
                .HasConversion<CarrierCodeConverter>()
                .HasColumnType("varchar(3)")
                .IsRequired();

            builder.Property(segment => segment.OperatingCarrier)
                .HasConversion<CarrierCodeConverter>()
                .HasColumnType("varchar(3)")
                .IsRequired();

            builder.Property(segment => segment.FlightNumber)
                .HasColumnType("varchar(8)")
                .IsRequired();

            builder.Property(segment => segment.Origin)
                .HasConversion<AirportCodeConverter>()
                .HasColumnType("char(3)")
                .IsRequired();

            builder.Property(segment => segment.Destination)
                .HasConversion<AirportCodeConverter>()
                .HasColumnType("char(3)")
                .IsRequired();

            builder.Property(segment => segment.DepartureUtc)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(segment => segment.ArrivalUtc)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(segment => segment.DepartureLocalDate)
                .HasConversion<LocalDateToDateOnlyConverter>()
                .HasColumnType("date")
                .IsRequired();

            builder.Property(segment => segment.DepartureLocalTime)
                .HasConversion<LocalTimeToTimeOnlyConverter>()
                .HasColumnType("time(0)")
                .IsRequired();

            builder.Property(segment => segment.OriginTimeZoneId)
                .HasColumnType("varchar(64)")
                .IsRequired();

            builder.Property(segment => segment.ArrivalLocalDate)
                .HasConversion<LocalDateToDateOnlyConverter>()
                .HasColumnType("date")
                .IsRequired();

            builder.Property(segment => segment.ArrivalLocalTime)
                .HasConversion<LocalTimeToTimeOnlyConverter>()
                .HasColumnType("time(0)")
                .IsRequired();

            builder.Property(segment => segment.DestinationTimeZoneId)
                .HasColumnType("varchar(64)")
                .IsRequired();

            builder.Property(segment => segment.AircraftType).HasColumnType("varchar(16)");

            builder.Property(segment => segment.Sequence).IsRequired();

            builder.HasIndex(segment => new { segment.MarketingCarrier, segment.FlightNumber, segment.DepartureUtc });

            builder.Property(segment => segment.LastUpdateTime);
            builder.Property(segment => segment.LastUpdatedBy);
        }
    }
}

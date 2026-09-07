using AeroTech.Ordering.Domain.Fulfillment.Aggregates;
using AeroTech.Ordering.Domain.Fulfillment.Entities;
using AeroTech.Ordering.Persistence._Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Fulfillment.Configurations
{
    public sealed class ElectronicTicketConfiguration : IEntityTypeConfiguration<ElectronicTicket>
    {
        public void Configure(EntityTypeBuilder<ElectronicTicket> builder)
        {
            builder.ToTable("ElectronicTickets", OrderingDbContext.FulfillmentSchema);

            builder.HasKey(ticket => ticket.Id);
            builder.Property(ticket => ticket.Id)
                .HasConversion<ElectronicTicketIdConverter>()
                .ValueGeneratedNever();

            builder.Property(ticket => ticket.TicketNumber)
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.HasIndex(ticket => ticket.TicketNumber).IsUnique();

            // Correlation columns only: no foreign key into the ordering schema.
            builder.Property(ticket => ticket.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.Property(ticket => ticket.TravelerId)
                .HasConversion<TravelerIdConverter>()
                .IsRequired();

            builder.HasIndex(ticket => ticket.OrderId);
            builder.HasIndex(ticket => ticket.TravelerId);

            builder.Property(ticket => ticket.IssuingCarrier)
                .HasConversion<CarrierCodeConverter>()
                .HasColumnType("varchar(3)")
                .IsRequired();

            builder.Property(ticket => ticket.IssuedAt)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnName("IssuedAtUtc")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(ticket => ticket.Status)
                .HasConversion<string>()
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.Property(ticket => ticket.AggregateVersion).IsRequired();
            builder.Property(ticket => ticket.RowVersion).IsRowVersion();

            builder.Property(ticket => ticket.LastUpdateTime);
            builder.Property(ticket => ticket.LastUpdatedBy);

            builder.HasMany(ticket => ticket.Coupons)
                .WithOne()
                .HasForeignKey(coupon => coupon.ElectronicTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(ticket => ticket.Coupons).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    public sealed class TicketCouponConfiguration : IEntityTypeConfiguration<TicketCoupon>
    {
        public void Configure(EntityTypeBuilder<TicketCoupon> builder)
        {
            builder.ToTable("TicketCoupons", OrderingDbContext.FulfillmentSchema);

            builder.HasKey(coupon => coupon.Id);
            builder.Property(coupon => coupon.Id)
                .HasConversion<TicketCouponIdConverter>()
                .ValueGeneratedNever();

            builder.Property(coupon => coupon.ElectronicTicketId)
                .HasConversion<ElectronicTicketIdConverter>()
                .IsRequired();

            builder.Property(coupon => coupon.CouponNumber).IsRequired();

            builder.HasIndex(coupon => new { coupon.ElectronicTicketId, coupon.CouponNumber }).IsUnique();

            builder.Property(coupon => coupon.OrderItemId)
                .HasConversion<OrderItemIdConverter>()
                .IsRequired();

            builder.Property(coupon => coupon.EntitlementId)
                .HasConversion<EntitlementIdConverter>()
                .IsRequired();

            builder.Property(coupon => coupon.JourneySegmentId)
                .HasConversion<JourneySegmentIdConverter>()
                .IsRequired();

            builder.HasIndex(coupon => coupon.EntitlementId);
            builder.HasIndex(coupon => coupon.JourneySegmentId);

            builder.Property(coupon => coupon.Status)
                .HasConversion<string>()
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.Property(coupon => coupon.ControlHolder).HasColumnType("varchar(64)");

            builder.Property(coupon => coupon.ControlAcquiredAt)
                .HasConversion<NullableInstantToDateTimeConverter>()
                .HasColumnName("ControlAcquiredAtUtc")
                .HasColumnType("datetime2(7)");

            builder.Property(coupon => coupon.LastUpdateTime);
            builder.Property(coupon => coupon.LastUpdatedBy);

            builder.Ignore(coupon => coupon.IsTerminal);
        }
    }
}

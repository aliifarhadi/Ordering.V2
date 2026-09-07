using AeroTech.Ordering.Domain.Fulfillment.Aggregates;
using AeroTech.Ordering.Domain.Fulfillment.Entities;
using AeroTech.Ordering.Persistence._Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Fulfillment.Configurations
{
    public sealed class ElectronicMiscDocumentConfiguration : IEntityTypeConfiguration<ElectronicMiscDocument>
    {
        public void Configure(EntityTypeBuilder<ElectronicMiscDocument> builder)
        {
            builder.ToTable("ElectronicMiscDocuments", OrderingDbContext.FulfillmentSchema);

            builder.HasKey(document => document.Id);
            builder.Property(document => document.Id)
                .HasConversion<ElectronicMiscDocumentIdConverter>()
                .ValueGeneratedNever();

            builder.Property(document => document.EmdNumber)
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.HasIndex(document => document.EmdNumber).IsUnique();

            builder.Property(document => document.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.Property(document => document.TravelerId)
                .HasConversion<TravelerIdConverter>()
                .IsRequired();

            builder.HasIndex(document => document.OrderId);
            builder.HasIndex(document => document.TravelerId);

            builder.Property(document => document.Type)
                .HasConversion<string>()
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.Property(document => document.IssuedAt)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnName("IssuedAtUtc")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(document => document.Status)
                .HasConversion<string>()
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.Property(document => document.AggregateVersion).IsRequired();
            builder.Property(document => document.RowVersion).IsRowVersion();

            builder.Property(document => document.LastUpdateTime);
            builder.Property(document => document.LastUpdatedBy);

            builder.HasMany(document => document.Coupons)
                .WithOne()
                .HasForeignKey(coupon => coupon.ElectronicMiscDocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(document => document.Coupons).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    public sealed class EmdCouponConfiguration : IEntityTypeConfiguration<EmdCoupon>
    {
        public void Configure(EntityTypeBuilder<EmdCoupon> builder)
        {
            builder.ToTable("EmdCoupons", OrderingDbContext.FulfillmentSchema);

            builder.HasKey(coupon => coupon.Id);
            builder.Property(coupon => coupon.Id)
                .HasConversion<EmdCouponIdConverter>()
                .ValueGeneratedNever();

            builder.Property(coupon => coupon.ElectronicMiscDocumentId)
                .HasConversion<ElectronicMiscDocumentIdConverter>()
                .IsRequired();

            builder.Property(coupon => coupon.CouponNumber).IsRequired();

            builder.HasIndex(coupon => new { coupon.ElectronicMiscDocumentId, coupon.CouponNumber }).IsUnique();

            builder.Property(coupon => coupon.OrderItemId)
                .HasConversion<OrderItemIdConverter>()
                .IsRequired();

            builder.Property(coupon => coupon.EntitlementId)
                .HasConversion<EntitlementIdConverter>()
                .IsRequired();

            // FUL-021: segment ref is null for genuinely non-segment services.
            builder.Property(coupon => coupon.JourneySegmentId)
                .HasConversion<NullableJourneySegmentIdConverter>();

            builder.HasIndex(coupon => coupon.EntitlementId);

            builder.Property(coupon => coupon.Status)
                .HasConversion<string>()
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.Property(coupon => coupon.LastUpdateTime);
            builder.Property(coupon => coupon.LastUpdatedBy);

            builder.Ignore(coupon => coupon.IsTerminal);
        }
    }
}

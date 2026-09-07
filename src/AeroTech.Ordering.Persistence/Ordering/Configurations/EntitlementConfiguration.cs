using AeroTech.Ordering.Domain.Ordering.Entities;
using AeroTech.Ordering.Domain.Ordering.Specifications;
using AeroTech.Ordering.Persistence._Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Ordering.Configurations
{
    public sealed class EntitlementConfiguration : IEntityTypeConfiguration<Entitlement>
    {
        public void Configure(EntityTypeBuilder<Entitlement> builder)
        {
            builder.ToTable("Entitlements", OrderingDbContext.OrderingSchema);

            builder.HasKey(entitlement => entitlement.Id);
            builder.Property(entitlement => entitlement.Id)
                .HasConversion<EntitlementIdConverter>()
                .ValueGeneratedNever();

            builder.Property(entitlement => entitlement.OrderItemId)
                .HasConversion<OrderItemIdConverter>()
                .IsRequired();

            builder.Property(entitlement => entitlement.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasIndex(entitlement => entitlement.OrderId);
            builder.HasIndex(entitlement => entitlement.OrderItemId);

            builder.Property(entitlement => entitlement.Type)
                .HasConversion<string>()
                .HasColumnType("varchar(40)")
                .IsRequired();

            builder.Property(entitlement => entitlement.Status)
                .HasConversion<string>()
                .HasColumnType("varchar(24)")
                .IsRequired();

            builder.Property(entitlement => entitlement.JourneyRef)
                .HasConversion<NullableJourneyIdConverter>()
                .HasColumnName("JourneyId");

            builder.Property(entitlement => entitlement.LocationRef)
                .HasColumnName("LocationRef")
                .HasColumnType("varchar(128)");

            builder.Property(entitlement => entitlement.StartDate)
                .HasConversion<NullableLocalDateToDateOnlyConverter>()
                .HasColumnType("date");

            builder.Property(entitlement => entitlement.EndDate)
                .HasConversion<NullableLocalDateToDateOnlyConverter>()
                .HasColumnType("date");

            builder.Property(entitlement => entitlement.CreatedAt)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnName("CreatedAtUtc")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(entitlement => entitlement.CancelledAt)
                .HasConversion<NullableInstantToDateTimeConverter>()
                .HasColumnName("CancelledAtUtc")
                .HasColumnType("datetime2(7)");

            builder.Property(entitlement => entitlement.ReplacedAt)
                .HasConversion<NullableInstantToDateTimeConverter>()
                .HasColumnName("ReplacedAtUtc")
                .HasColumnType("datetime2(7)");

            builder.Property(entitlement => entitlement.LastUpdateTime);
            builder.Property(entitlement => entitlement.LastUpdatedBy);

            builder.OwnsOne(entitlement => entitlement.CapacityCommitment, capacity =>
            {
                capacity.Property(value => value.Type)
                    .HasConversion<string>()
                    .HasColumnName("CapacityType")
                    .HasColumnType("varchar(40)");

                capacity.Property(value => value.Reference)
                    .HasColumnName("CapacityReference")
                    .HasColumnType("varchar(128)");
            });

            builder.OwnsOne(entitlement => entitlement.Responsibility, responsibility =>
            {
                responsibility.Property(value => value.RetailerRef).HasColumnName("RetailerRef").HasColumnType("varchar(128)");
                responsibility.Property(value => value.SupplierRef).HasColumnName("SupplierRef").HasColumnType("varchar(128)");
                responsibility.Property(value => value.FulfillmentOwnerRef).HasColumnName("FulfillmentOwnerRef").HasColumnType("varchar(128)");
                responsibility.Property(value => value.ServicingOwnerRef).HasColumnName("ServicingOwnerRef").HasColumnType("varchar(128)");
                responsibility.Property(value => value.RefundOwnerRef).HasColumnName("RefundOwnerRef").HasColumnType("varchar(128)");
                responsibility.Property(value => value.DisruptionOwnerRef).HasColumnName("DisruptionOwnerRef").HasColumnType("varchar(128)");
                responsibility.Property(value => value.SettlementOwnerRef).HasColumnName("SettlementOwnerRef").HasColumnType("varchar(128)");
                responsibility.Property(value => value.ExternalProductRef).HasColumnName("ExternalProductRef").HasColumnType("varchar(128)");
                responsibility.Property(value => value.ExternalOrderRef).HasColumnName("ExternalOrderRef").HasColumnType("varchar(128)");
                responsibility.Property(value => value.ExternalServiceRef).HasColumnName("ExternalServiceRef").HasColumnType("varchar(128)");

                responsibility.Ignore(value => value.IsExternallySupplied);
                responsibility.Ignore(value => value.IsEmpty);
            });

            builder.OwnsMany(entitlement => entitlement.BeneficiaryRefs, beneficiary =>
            {
                beneficiary.ToTable("EntitlementBeneficiaries", OrderingDbContext.OrderingSchema);
                beneficiary.WithOwner().HasForeignKey(value => value.EntitlementId);

                beneficiary.Property(value => value.EntitlementId)
                    .HasConversion<EntitlementIdConverter>()
                    .IsRequired();

                beneficiary.Property(value => value.TravelerId)
                    .HasConversion<TravelerIdConverter>()
                    .IsRequired();

                beneficiary.HasKey(value => new { value.EntitlementId, value.TravelerId });
            });

            builder.OwnsMany(entitlement => entitlement.SegmentRefs, segment =>
            {
                segment.ToTable("EntitlementSegments", OrderingDbContext.OrderingSchema);
                segment.WithOwner().HasForeignKey(value => value.EntitlementId);

                segment.Property(value => value.EntitlementId)
                    .HasConversion<EntitlementIdConverter>()
                    .IsRequired();

                segment.Property(value => value.JourneySegmentId)
                    .HasConversion<JourneySegmentIdConverter>()
                    .IsRequired();

                segment.Property(value => value.Sequence).IsRequired();

                segment.HasKey(value => new { value.EntitlementId, value.JourneySegmentId });
            });

            builder.HasMany(entitlement => entitlement.FulfillmentLinks)
                .WithOne()
                .HasForeignKey(link => link.EntitlementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(entitlement => entitlement.Specification)
                .WithOne()
                .HasForeignKey<EntitlementSpecification>(specification => specification.EntitlementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(entitlement => entitlement.BeneficiaryRefs).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(entitlement => entitlement.SegmentRefs).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(entitlement => entitlement.FulfillmentLinks).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(entitlement => entitlement.Specification).IsRequired();

            builder.Ignore(entitlement => entitlement.BeneficiaryTravelerIds);
            builder.Ignore(entitlement => entitlement.SegmentIds);
            builder.Ignore(entitlement => entitlement.Scope);
            builder.Ignore(entitlement => entitlement.IsActive);
            builder.Ignore(entitlement => entitlement.IsTerminal);
        }
    }

    public sealed class FulfillmentLinkConfiguration : IEntityTypeConfiguration<FulfillmentLink>
    {
        public void Configure(EntityTypeBuilder<FulfillmentLink> builder)
        {
            builder.ToTable("FulfillmentLinks", OrderingDbContext.OrderingSchema);

            builder.HasKey(link => link.Id);
            builder.Property(link => link.Id)
                .HasConversion<FulfillmentLinkIdConverter>()
                .ValueGeneratedNever();

            builder.Property(link => link.EntitlementId)
                .HasConversion<EntitlementIdConverter>()
                .IsRequired();

            builder.HasIndex(link => link.EntitlementId);

            builder.Property(link => link.FulfillmentType)
                .HasConversion<string>()
                .HasColumnType("varchar(32)")
                .IsRequired();

            // No foreign key into the fulfillment schema: correlation ids only.
            builder.Property(link => link.FulfillmentAggregateId).IsRequired();
            builder.HasIndex(link => link.FulfillmentAggregateId);

            builder.Property(link => link.FulfillmentUnitId);

            builder.Property(link => link.ExternalReference).HasColumnType("varchar(128)");

            builder.Property(link => link.LinkedAt)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnName("LinkedAtUtc")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(link => link.LastUpdateTime);
            builder.Property(link => link.LastUpdatedBy);
        }
    }
}

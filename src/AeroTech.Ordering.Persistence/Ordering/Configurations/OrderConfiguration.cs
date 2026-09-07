using AeroTech.Ordering.Domain.Ordering.Aggregates;
using AeroTech.Ordering.Persistence._Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Ordering.Configurations
{
    public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", OrderingDbContext.OrderingSchema);

            builder.HasKey(order => order.Id);
            builder.Property(order => order.Id)
                .HasConversion<OrderIdConverter>()
                .ValueGeneratedNever();

            builder.Property(order => order.Reference)
                .HasConversion<OrderReferenceConverter>()
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.HasIndex(order => order.Reference).IsUnique();

            builder.Property(order => order.AggregateVersion).IsRequired();

            builder.Property(order => order.ClosedAt)
                .HasConversion<NullableInstantToDateTimeConverter>()
                .HasColumnName("ClosedAtUtc")
                .HasColumnType("datetime2(7)");

            builder.OwnsOne(order => order.Lineage, lineage =>
            {
                lineage.Property(value => value.RootOrderId)
                    .HasConversion<OrderIdConverter>()
                    .HasColumnName("RootOrderId")
                    .IsRequired();

                lineage.Property(value => value.ParentOrderId)
                    .HasConversion<NullableOrderIdConverter>()
                    .HasColumnName("ParentOrderId");

                lineage.Property(value => value.SplitFromOrderId)
                    .HasConversion<NullableOrderIdConverter>()
                    .HasColumnName("SplitFromOrderId");

                lineage.Property(value => value.SplitWorkflowId)
                    .HasConversion<NullableWorkflowInstanceIdConverter>()
                    .HasColumnName("SplitWorkflowId");

                lineage.HasIndex(value => value.RootOrderId);
            });

            builder.Navigation(order => order.Lineage).IsRequired();

            builder.OwnsOne(order => order.CreatedSalesContext, sales =>
            {
                sales.Property(value => value.SellerId)
                    .HasColumnName("CreatedSellerId")
                    .HasColumnType("varchar(64)")
                    .IsRequired();

                sales.Property(value => value.SellerOfficeId)
                    .HasColumnName("CreatedSellerOfficeId")
                    .HasColumnType("varchar(64)");

                sales.Property(value => value.ChannelCode)
                    .HasColumnName("CreatedChannelCode")
                    .HasColumnType("varchar(32)")
                    .IsRequired();

                sales.Property(value => value.PointOfSaleCountry)
                    .HasConversion<CountryCodeConverter>()
                    .HasColumnName("CreatedPointOfSaleCountry")
                    .HasColumnType("char(2)")
                    .IsRequired();

                sales.Property(value => value.SaleCurrency)
                    .HasConversion<CurrencyCodeConverter>()
                    .HasColumnName("CreatedSaleCurrency")
                    .HasColumnType("char(3)")
                    .IsRequired();

                sales.Property(value => value.SoldAt)
                    .HasConversion<InstantToDateTimeConverter>()
                    .HasColumnName("CreatedSoldAtUtc")
                    .HasColumnType("datetime2(7)")
                    .IsRequired();

                sales.Property(value => value.ActorType)
                    .HasConversion<string>()
                    .HasColumnName("CreatedActorType")
                    .HasColumnType("varchar(32)")
                    .IsRequired();

                sales.Property(value => value.UserId)
                    .HasColumnName("CreatedUserId")
                    .HasColumnType("varchar(64)")
                    .IsRequired();

                sales.HasIndex(value => value.SellerId);
            });

            builder.Navigation(order => order.CreatedSalesContext).IsRequired();

            builder.OwnsOne(order => order.Buyer, buyer =>
            {
                buyer.Property(value => value.PartyType)
                    .HasConversion<string>()
                    .HasColumnName("BuyerPartyType")
                    .HasColumnType("varchar(32)");

                buyer.Property(value => value.PartyRef)
                    .HasColumnName("BuyerPartyRef")
                    .HasColumnType("varchar(128)");

                buyer.Property(value => value.Name)
                    .HasColumnName("BuyerName")
                    .HasMaxLength(256);

                buyer.Property(value => value.Email)
                    .HasColumnName("BuyerEmail")
                    .HasMaxLength(320);

                buyer.Property(value => value.Phone)
                    .HasColumnName("BuyerPhone")
                    .HasMaxLength(64);

                buyer.Property(value => value.TaxIdentity)
                    .HasColumnName("BuyerTaxIdentity")
                    .HasMaxLength(128);
            });

            builder.OwnsOne(order => order.ServicingAuthority, authority =>
            {
                authority.Property(value => value.OwnerSellerId)
                    .HasColumnName("AuthorityOwnerSellerId")
                    .HasColumnType("varchar(64)")
                    .IsRequired();

                authority.Property(value => value.OwnerOfficeId)
                    .HasColumnName("AuthorityOwnerOfficeId")
                    .HasColumnType("varchar(64)");

                authority.Property(value => value.AirlineOverrideAllowed)
                    .HasColumnName("AirlineOverrideAllowed")
                    .IsRequired();
            });

            builder.Navigation(order => order.ServicingAuthority).IsRequired();

            builder.Property(order => order.RowVersion).IsRowVersion();

            builder.Property(order => order.LastUpdateTime);
            builder.Property(order => order.LastUpdatedBy);

            builder.HasMany(order => order.Travelers)
                .WithOne()
                .HasForeignKey(traveler => traveler.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(order => order.Contacts)
                .WithOne()
                .HasForeignKey(contact => contact.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(order => order.Journeys)
                .WithOne()
                .HasForeignKey(journey => journey.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(order => order.Items)
                .WithOne()
                .HasForeignKey(item => item.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(order => order.TimeLimits)
                .WithOne()
                .HasForeignKey(timeLimit => timeLimit.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(order => order.ProcessingLocks)
                .WithOne()
                .HasForeignKey(processingLock => processingLock.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(order => order.ExternalReferences)
                .WithOne()
                .HasForeignKey(reference => reference.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(order => order.Delegations)
                .WithOne()
                .HasForeignKey(delegation => delegation.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(order => order.Travelers).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(order => order.Contacts).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(order => order.Journeys).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(order => order.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(order => order.TimeLimits).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(order => order.ProcessingLocks).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(order => order.ExternalReferences).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(order => order.Delegations).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(order => order.Lifecycle);
            builder.Ignore(order => order.AllSegments);
            builder.Ignore(order => order.AllEntitlements);
        }
    }
}

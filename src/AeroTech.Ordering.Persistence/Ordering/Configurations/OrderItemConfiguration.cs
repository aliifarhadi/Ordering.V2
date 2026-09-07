using AeroTech.Ordering.Domain.Ordering.Entities;
using AeroTech.Ordering.Persistence._Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Ordering.Configurations
{
    public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems", OrderingDbContext.OrderingSchema);

            builder.HasKey(item => item.Id);
            builder.Property(item => item.Id)
                .HasConversion<OrderItemIdConverter>()
                .ValueGeneratedNever();

            builder.Property(item => item.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasIndex(item => item.OrderId);

            builder.Property(item => item.Status)
                .HasConversion<string>()
                .HasColumnType("varchar(24)")
                .IsRequired();

            builder.Property(item => item.CreatedAt)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnName("CreatedAtUtc")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(item => item.CancelledAt)
                .HasConversion<NullableInstantToDateTimeConverter>()
                .HasColumnName("CancelledAtUtc")
                .HasColumnType("datetime2(7)");

            builder.Property(item => item.ReplacedAt)
                .HasConversion<NullableInstantToDateTimeConverter>()
                .HasColumnName("ReplacedAtUtc")
                .HasColumnType("datetime2(7)");

            builder.Property(item => item.LineageChangeId)
                .HasConversion<NullableChangeIdConverter>()
                .HasColumnName("LineageChangeId");

            builder.Property(item => item.LastUpdateTime);
            builder.Property(item => item.LastUpdatedBy);

            ConfigureProduct(builder);
            ConfigurePrice(builder);
            ConfigureCommercialTerms(builder);
            ConfigureCommercialSource(builder);
            ConfigureItemSalesContext(builder);
            ConfigureSettlement(builder);
            ConfigureChildCollections(builder);

            builder.Ignore(item => item.Lineage);
            builder.Ignore(item => item.BeneficiaryTravelerIds);
            builder.Ignore(item => item.IsTerminal);
        }

        private static void ConfigureProduct(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(item => item.Product, product =>
            {
                product.Property(value => value.ProductId)
                    .HasColumnName("ProductId")
                    .HasColumnType("varchar(128)");

                product.Property(value => value.ProductCode)
                    .HasColumnName("ProductCode")
                    .HasColumnType("varchar(64)")
                    .IsRequired();

                product.Property(value => value.ProductType)
                    .HasConversion<string>()
                    .HasColumnName("ProductType")
                    .HasColumnType("varchar(32)")
                    .IsRequired();

                product.Property(value => value.Name)
                    .HasColumnName("ProductName")
                    .HasMaxLength(256)
                    .IsRequired();

                product.Property(value => value.Description)
                    .HasColumnName("ProductDescription")
                    .HasMaxLength(512);

                product.Property(value => value.CatalogVersion)
                    .HasColumnName("CatalogVersion")
                    .HasColumnType("varchar(64)");

                product.HasIndex(value => value.ProductType);

                product.OwnsMany(value => value.Attributes, attribute =>
                {
                    attribute.ToTable("ProductAttributes", OrderingDbContext.OrderingSchema);
                    attribute.WithOwner().HasForeignKey("OrderItemId");

                    attribute.Property(value => value.Key)
                        .HasColumnName("AttributeKey")
                        .HasColumnType("varchar(64)")
                        .IsRequired();

                    attribute.Property(value => value.Value)
                        .HasColumnName("AttributeValue")
                        .HasMaxLength(512)
                        .IsRequired();

                    attribute.HasKey("OrderItemId", nameof(Domain.Ordering.ValueObjects.ProductAttribute.Key));
                });
            });

            builder.Navigation(item => item.Product).IsRequired();
        }

        private static void ConfigurePrice(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(item => item.Price, price =>
            {
                price.Property(value => value.TotalAmount)
                    .HasColumnName("PriceTotal")
                    .HasColumnType("decimal(19,4)")
                    .IsRequired();

                price.Property(value => value.Currency)
                    .HasConversion<CurrencyCodeConverter>()
                    .HasColumnName("PriceCurrency")
                    .HasColumnType("char(3)")
                    .IsRequired();

                price.Property(value => value.PricedAt)
                    .HasConversion<InstantToDateTimeConverter>()
                    .HasColumnName("PricedAtUtc")
                    .HasColumnType("datetime2(7)")
                    .IsRequired();

                price.Property(value => value.PricingEngineVersion)
                    .HasColumnName("PricingEngineVersion")
                    .HasColumnType("varchar(64)")
                    .IsRequired();

                price.Property(value => value.RoundingPolicyVersion)
                    .HasColumnName("RoundingPolicyVersion")
                    .HasColumnType("varchar(64)")
                    .IsRequired();

                price.Ignore(value => value.Total);

                price.OwnsOne(value => value.Fx, fx =>
                {
                    fx.Property(value => value.From)
                        .HasConversion<CurrencyCodeConverter>()
                        .HasColumnName("FxFrom")
                        .HasColumnType("char(3)");

                    fx.Property(value => value.To)
                        .HasConversion<CurrencyCodeConverter>()
                        .HasColumnName("FxTo")
                        .HasColumnType("char(3)");

                    fx.Property(value => value.Rate)
                        .HasColumnName("FxRate")
                        .HasColumnType("decimal(19,8)");

                    fx.Property(value => value.Source)
                        .HasColumnName("FxSource")
                        .HasColumnType("varchar(64)");

                    fx.Property(value => value.CapturedAt)
                        .HasConversion<InstantToDateTimeConverter>()
                        .HasColumnName("FxCapturedAtUtc")
                        .HasColumnType("datetime2(7)");
                });
            });

            builder.Navigation(item => item.Price).IsRequired();
        }

        private static void ConfigureCommercialTerms(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(item => item.CommercialTerms, terms =>
            {
                terms.Property(value => value.TermsVersion)
                    .HasColumnName("TermsVersion")
                    .HasColumnType("varchar(64)")
                    .IsRequired();

                terms.Property(value => value.Refundability)
                    .HasColumnName("Refundability")
                    .HasColumnType("varchar(32)")
                    .IsRequired();

                terms.Property(value => value.Changeability)
                    .HasColumnName("Changeability")
                    .HasColumnType("varchar(32)")
                    .IsRequired();

                terms.Property(value => value.NoShowPolicyCode)
                    .HasColumnName("NoShowPolicyCode")
                    .HasColumnType("varchar(64)");

                terms.Property(value => value.ValidFrom)
                    .HasConversion<NullableInstantToDateTimeConverter>()
                    .HasColumnName("ValidFromUtc")
                    .HasColumnType("datetime2(7)");

                terms.Property(value => value.ValidUntil)
                    .HasConversion<NullableInstantToDateTimeConverter>()
                    .HasColumnName("ValidUntilUtc")
                    .HasColumnType("datetime2(7)");

                terms.OwnsMany(value => value.Restrictions, restriction =>
                {
                    restriction.ToTable("CommercialTermRestrictions", OrderingDbContext.OrderingSchema);
                    restriction.WithOwner().HasForeignKey("OrderItemId");

                    restriction.Property<Guid>("Id").ValueGeneratedOnAdd();
                    restriction.HasKey("Id");

                    restriction.Property(value => value.RestrictionCode)
                        .HasColumnName("RestrictionCode")
                        .HasColumnType("varchar(64)")
                        .IsRequired();

                    restriction.Property(value => value.Description)
                        .HasColumnName("Description")
                        .HasMaxLength(512);
                });

                terms.PrimitiveCollection(value => value.SourceRuleRefs)
                    .HasColumnName("SourceRuleRefs")
                    .HasColumnType("nvarchar(max)");
            });

            builder.Navigation(item => item.CommercialTerms).IsRequired();
        }

        private static void ConfigureCommercialSource(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(item => item.CommercialSource, source =>
            {
                source.Property(value => value.Type)
                    .HasConversion<string>()
                    .HasColumnName("CommercialSourceType")
                    .HasColumnType("varchar(40)")
                    .IsRequired();

                source.Property(value => value.Reference)
                    .HasColumnName("CommercialSourceReference")
                    .HasColumnType("varchar(128)")
                    .IsRequired();

                source.Property(value => value.Version)
                    .HasColumnName("CommercialSourceVersion")
                    .HasColumnType("varchar(64)");

                source.Property(value => value.AcceptedAt)
                    .HasConversion<InstantToDateTimeConverter>()
                    .HasColumnName("CommercialSourceAcceptedAtUtc")
                    .HasColumnType("datetime2(7)")
                    .IsRequired();

                source.HasIndex(value => value.Reference);
            });

            builder.Navigation(item => item.CommercialSource).IsRequired();
        }

        private static void ConfigureItemSalesContext(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(item => item.ItemSalesContext, sales =>
            {
                sales.Property(value => value.SellerId)
                    .HasColumnName("ItemSellerId")
                    .HasColumnType("varchar(64)")
                    .IsRequired();

                sales.Property(value => value.SellerOfficeId)
                    .HasColumnName("ItemSellerOfficeId")
                    .HasColumnType("varchar(64)");

                sales.Property(value => value.ChannelCode)
                    .HasColumnName("ItemChannelCode")
                    .HasColumnType("varchar(32)")
                    .IsRequired();

                sales.Property(value => value.PointOfSaleCountry)
                    .HasConversion<CountryCodeConverter>()
                    .HasColumnName("ItemPointOfSaleCountry")
                    .HasColumnType("char(2)")
                    .IsRequired();

                sales.Property(value => value.SaleCurrency)
                    .HasConversion<CurrencyCodeConverter>()
                    .HasColumnName("ItemSaleCurrency")
                    .HasColumnType("char(3)")
                    .IsRequired();

                sales.Property(value => value.SoldAt)
                    .HasConversion<InstantToDateTimeConverter>()
                    .HasColumnName("ItemSoldAtUtc")
                    .HasColumnType("datetime2(7)")
                    .IsRequired();

                sales.Property(value => value.ActorType)
                    .HasConversion<string>()
                    .HasColumnName("ItemActorType")
                    .HasColumnType("varchar(32)")
                    .IsRequired();

                sales.Property(value => value.UserId)
                    .HasColumnName("ItemUserId")
                    .HasColumnType("varchar(64)")
                    .IsRequired();

                sales.HasIndex(value => value.SellerId);
            });

            builder.Navigation(item => item.ItemSalesContext).IsRequired();
        }

        private static void ConfigureSettlement(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(item => item.Settlement, settlement =>
            {
                settlement.Property(value => value.Model)
                    .HasConversion<string>()
                    .HasColumnName("SettlementModel")
                    .HasColumnType("varchar(40)")
                    .IsRequired();

                settlement.Property(value => value.DebtorPartyRef)
                    .HasColumnName("SettlementDebtorPartyRef")
                    .HasColumnType("varchar(128)");

                settlement.Property(value => value.AgreementRef)
                    .HasColumnName("SettlementAgreementRef")
                    .HasColumnType("varchar(128)");

                settlement.Property(value => value.CollectionRequired)
                    .HasColumnName("SettlementCollectionRequired")
                    .IsRequired();

                settlement.Property(value => value.ReceivableRef)
                    .HasColumnName("SettlementReceivableRef")
                    .HasColumnType("varchar(128)");
            });

            builder.Navigation(item => item.Settlement).IsRequired();
        }

        private static void ConfigureChildCollections(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsMany(item => item.Beneficiaries, beneficiary =>
            {
                beneficiary.ToTable("OrderItemBeneficiaries", OrderingDbContext.OrderingSchema);
                beneficiary.WithOwner().HasForeignKey(value => value.OrderItemId);

                beneficiary.Property(value => value.OrderItemId)
                    .HasConversion<OrderItemIdConverter>()
                    .IsRequired();

                beneficiary.Property(value => value.TravelerId)
                    .HasConversion<TravelerIdConverter>()
                    .IsRequired();

                beneficiary.Property(value => value.Role).HasColumnType("varchar(32)");

                beneficiary.HasKey(value => new { value.OrderItemId, value.TravelerId });
            });

            builder.OwnsMany(item => item.PredecessorItemIds, predecessor =>
            {
                predecessor.ToTable("ItemLineagePredecessors", OrderingDbContext.OrderingSchema);
                predecessor.WithOwner().HasForeignKey(value => value.OrderItemId);

                predecessor.Property(value => value.OrderItemId)
                    .HasConversion<OrderItemIdConverter>()
                    .IsRequired();

                predecessor.Property(value => value.PredecessorOrderItemId)
                    .HasConversion<OrderItemIdConverter>()
                    .IsRequired();

                predecessor.HasKey(value => new { value.OrderItemId, value.PredecessorOrderItemId });
            });

            builder.HasMany(item => item.ChargeLines)
                .WithOne()
                .HasForeignKey(line => line.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(item => item.ValueAllocations)
                .WithOne()
                .HasForeignKey(allocation => allocation.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(item => item.Entitlements)
                .WithOne()
                .HasForeignKey(entitlement => entitlement.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(item => item.Beneficiaries).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(item => item.PredecessorItemIds).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(item => item.ChargeLines).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(item => item.ValueAllocations).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(item => item.Entitlements).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}

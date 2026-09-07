using AeroTech.Ordering.Domain.Ordering.Entities;
using AeroTech.Ordering.Persistence._Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Ordering.Configurations
{
    public sealed class TravelerConfiguration : IEntityTypeConfiguration<Traveler>
    {
        public void Configure(EntityTypeBuilder<Traveler> builder)
        {
            builder.ToTable("Travelers", OrderingDbContext.OrderingSchema);

            builder.HasKey(traveler => traveler.Id);
            builder.Property(traveler => traveler.Id)
                .HasConversion<TravelerIdConverter>()
                .ValueGeneratedNever();

            builder.Property(traveler => traveler.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasIndex(traveler => traveler.OrderId);

            builder.Property(traveler => traveler.Type)
                .HasConversion<string>()
                .HasColumnName("TravelerType")
                .HasColumnType("varchar(8)")
                .IsRequired();

            builder.OwnsOne(traveler => traveler.Name, name =>
            {
                name.Property(value => value.GivenName)
                    .HasColumnName("GivenName")
                    .HasMaxLength(128)
                    .IsRequired();

                name.Property(value => value.Surname)
                    .HasColumnName("Surname")
                    .HasMaxLength(128)
                    .IsRequired();

                name.Property(value => value.Title)
                    .HasColumnName("Title")
                    .HasMaxLength(32);

                name.HasIndex(value => value.Surname);
            });

            builder.Navigation(traveler => traveler.Name).IsRequired();

            builder.Property(traveler => traveler.DateOfBirth)
                .HasConversion<NullableLocalDateToDateOnlyConverter>()
                .HasColumnType("date");

            builder.Property(traveler => traveler.Gender).HasColumnType("varchar(16)");

            builder.Property(traveler => traveler.CustomerRef).HasColumnType("varchar(128)");
            builder.HasIndex(traveler => traveler.CustomerRef);

            builder.OwnsOne(traveler => traveler.RegulatoryData, regulatory =>
            {
                regulatory.Property(value => value.RedressNumber)
                    .HasColumnName("RedressNumber")
                    .HasMaxLength(64);

                regulatory.Property(value => value.KnownTravelerNumber)
                    .HasColumnName("KnownTravelerNumber")
                    .HasMaxLength(64);

                regulatory.Property(value => value.ResidenceCountry)
                    .HasColumnName("ResidenceCountry")
                    .HasMaxLength(2);

                regulatory.Property(value => value.DestinationAddress)
                    .HasColumnName("DestinationAddress")
                    .HasMaxLength(512);
            });

            builder.Property(traveler => traveler.LastUpdateTime);
            builder.Property(traveler => traveler.LastUpdatedBy);

            builder.HasMany(traveler => traveler.IdentityDocuments)
                .WithOne()
                .HasForeignKey(document => document.TravelerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(traveler => traveler.LoyaltyAccounts)
                .WithOne()
                .HasForeignKey(account => account.TravelerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(traveler => traveler.Associations)
                .WithOne()
                .HasForeignKey(association => association.TravelerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(traveler => traveler.IdentityDocuments).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(traveler => traveler.LoyaltyAccounts).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(traveler => traveler.Associations).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    public sealed class IdentityDocumentConfiguration : IEntityTypeConfiguration<IdentityDocument>
    {
        public void Configure(EntityTypeBuilder<IdentityDocument> builder)
        {
            builder.ToTable("TravelerIdentityDocuments", OrderingDbContext.OrderingSchema);

            builder.HasKey(document => document.Id);
            builder.Property(document => document.Id)
                .HasConversion<IdentityDocumentIdConverter>()
                .ValueGeneratedNever();

            builder.Property(document => document.TravelerId)
                .HasConversion<TravelerIdConverter>()
                .IsRequired();

            builder.Property(document => document.DocumentType)
                .HasConversion<string>()
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(document => document.DocumentNumber)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(document => document.IssuingCountry)
                .HasConversion<NullableCountryCodeConverter>()
                .HasColumnType("char(2)");

            builder.Property(document => document.ExpiryDate)
                .HasConversion<NullableLocalDateToDateOnlyConverter>()
                .HasColumnType("date");

            builder.Property(document => document.Nationality)
                .HasConversion<NullableCountryCodeConverter>()
                .HasColumnType("char(2)");

            builder.Property(document => document.LastUpdateTime);
            builder.Property(document => document.LastUpdatedBy);
        }
    }

    public sealed class LoyaltyAccountRefConfiguration : IEntityTypeConfiguration<LoyaltyAccountRef>
    {
        public void Configure(EntityTypeBuilder<LoyaltyAccountRef> builder)
        {
            builder.ToTable("TravelerLoyaltyAccounts", OrderingDbContext.OrderingSchema);

            builder.HasKey(account => account.Id);
            builder.Property(account => account.Id)
                .HasConversion<LoyaltyAccountIdConverter>()
                .ValueGeneratedNever();

            builder.Property(account => account.TravelerId)
                .HasConversion<TravelerIdConverter>()
                .IsRequired();

            builder.Property(account => account.ProgramCode)
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(account => account.AccountRef)
                .HasColumnType("varchar(128)")
                .IsRequired();

            builder.Property(account => account.TierCode).HasColumnType("varchar(32)");

            builder.HasIndex(account => new { account.ProgramCode, account.AccountRef });

            builder.Property(account => account.LastUpdateTime);
            builder.Property(account => account.LastUpdatedBy);
        }
    }

    public sealed class TravelerAssociationConfiguration : IEntityTypeConfiguration<TravelerAssociation>
    {
        public void Configure(EntityTypeBuilder<TravelerAssociation> builder)
        {
            builder.ToTable("TravelerAssociations", OrderingDbContext.OrderingSchema);

            builder.HasKey(association => association.Id);
            builder.Property(association => association.Id)
                .HasConversion<TravelerAssociationIdConverter>()
                .ValueGeneratedNever();

            builder.Property(association => association.TravelerId)
                .HasConversion<TravelerIdConverter>()
                .IsRequired();

            builder.Property(association => association.RelatedTravelerId)
                .HasConversion<TravelerIdConverter>()
                .IsRequired();

            builder.Property(association => association.AssociationType)
                .HasConversion<string>()
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(association => association.LastUpdateTime);
            builder.Property(association => association.LastUpdatedBy);
        }
    }
}

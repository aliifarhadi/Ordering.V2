using AeroTech.Ordering.Domain.Ordering.Entities;
using AeroTech.Ordering.Persistence._Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Ordering.Configurations
{
    public sealed class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ToTable("Contacts", OrderingDbContext.OrderingSchema);

            builder.HasKey(contact => contact.Id);
            builder.Property(contact => contact.Id)
                .HasConversion<ContactIdConverter>()
                .ValueGeneratedNever();

            builder.Property(contact => contact.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasIndex(contact => contact.OrderId);

            builder.Property(contact => contact.Type)
                .HasConversion<string>()
                .HasColumnName("ContactType")
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.Property(contact => contact.Value)
                .HasMaxLength(320)
                .IsRequired();

            builder.Property(contact => contact.Role)
                .HasConversion<string>()
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(contact => contact.IsPrimary).IsRequired();

            builder.Property(contact => contact.LastUpdateTime);
            builder.Property(contact => contact.LastUpdatedBy);

            builder.OwnsMany(contact => contact.TravelerRefs, reference =>
            {
                reference.ToTable("ContactTravelers", OrderingDbContext.OrderingSchema);
                reference.WithOwner().HasForeignKey(link => link.ContactId);

                reference.Property(link => link.ContactId)
                    .HasConversion<ContactIdConverter>()
                    .IsRequired();

                reference.Property(link => link.TravelerId)
                    .HasConversion<TravelerIdConverter>()
                    .IsRequired();

                reference.HasKey(link => new { link.ContactId, link.TravelerId });
            });

            builder.Navigation(contact => contact.TravelerRefs).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(contact => contact.TravelerIds);
            builder.Ignore(contact => contact.IsOrderLevel);
        }
    }
}

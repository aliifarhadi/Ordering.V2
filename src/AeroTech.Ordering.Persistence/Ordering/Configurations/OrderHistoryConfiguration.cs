using AeroTech.Ordering.Persistence._Shared.Converters;
using AeroTech.Ordering.Persistence.Ordering.History;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Ordering.Configurations
{
    public sealed class OrderHistoryEntryConfiguration : IEntityTypeConfiguration<OrderHistoryEntry>
    {
        public void Configure(EntityTypeBuilder<OrderHistoryEntry> builder)
        {
            builder.ToTable("OrderHistory", OrderingDbContext.OrderingSchema);

            builder.Property(entry => entry.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasKey(entry => new { entry.OrderId, entry.SequenceNo });

            builder.Property(entry => entry.AggregateVersion).IsRequired();

            builder.Property(entry => entry.OccurredAtUtc)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(entry => entry.CommandName)
                .HasColumnType("varchar(128)")
                .IsRequired();

            builder.Property(entry => entry.ActorType)
                .HasConversion<string>()
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(entry => entry.UserId)
                .HasColumnType("varchar(64)")
                .IsRequired();

            builder.Property(entry => entry.SellerId).HasColumnType("varchar(64)");
            builder.Property(entry => entry.SellerOfficeId).HasColumnType("varchar(64)");

            builder.Property(entry => entry.Channel)
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(entry => entry.CorrelationId)
                .HasColumnType("varchar(128)")
                .IsRequired();

            builder.Property(entry => entry.CausationId).HasColumnType("varchar(128)");

            builder.Property(entry => entry.ChangeSummary)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(entry => entry.ChangeJson).HasColumnType("nvarchar(max)");

            builder.HasIndex(entry => entry.CorrelationId);
            builder.HasIndex(entry => entry.OccurredAtUtc);
        }
    }

    public sealed class OrderSnapshotConfiguration : IEntityTypeConfiguration<OrderSnapshot>
    {
        public void Configure(EntityTypeBuilder<OrderSnapshot> builder)
        {
            builder.ToTable("OrderSnapshots", OrderingDbContext.OrderingSchema);

            builder.Property(snapshot => snapshot.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasKey(snapshot => new { snapshot.OrderId, snapshot.AggregateVersion });

            builder.Property(snapshot => snapshot.CapturedAtUtc)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(snapshot => snapshot.SnapshotJson)
                .HasColumnType("nvarchar(max)")
                .IsRequired();
        }
    }
}

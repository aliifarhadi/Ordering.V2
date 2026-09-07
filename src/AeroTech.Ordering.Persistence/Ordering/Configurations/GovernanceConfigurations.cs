using AeroTech.Ordering.Domain.Ordering.Entities;
using AeroTech.Ordering.Persistence._Shared.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.Ordering.Configurations
{
    public sealed class TimeLimitConfiguration : IEntityTypeConfiguration<TimeLimit>
    {
        public void Configure(EntityTypeBuilder<TimeLimit> builder)
        {
            builder.ToTable("TimeLimits", OrderingDbContext.OrderingSchema);

            builder.HasKey(timeLimit => timeLimit.Id);
            builder.Property(timeLimit => timeLimit.Id)
                .HasConversion<TimeLimitIdConverter>()
                .ValueGeneratedNever();

            builder.Property(timeLimit => timeLimit.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasIndex(timeLimit => timeLimit.OrderId);

            builder.Property(timeLimit => timeLimit.Type)
                .HasConversion<string>()
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(timeLimit => timeLimit.DueAt)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnName("DueAtUtc")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(timeLimit => timeLimit.Status)
                .HasConversion<string>()
                .HasColumnType("varchar(16)")
                .IsRequired();

            builder.Property(timeLimit => timeLimit.PolicyRef).HasColumnType("varchar(128)");

            builder.Property(timeLimit => timeLimit.ExtensionCount).IsRequired();

            // Supports the due-time-limit report and the expiry poller.
            builder.HasIndex(timeLimit => new { timeLimit.Status, timeLimit.DueAt });

            builder.Property(timeLimit => timeLimit.LastUpdateTime);
            builder.Property(timeLimit => timeLimit.LastUpdatedBy);

            builder.OwnsMany(timeLimit => timeLimit.OrderItemRefs, reference =>
            {
                reference.ToTable("TimeLimitItems", OrderingDbContext.OrderingSchema);
                reference.WithOwner().HasForeignKey(value => value.TimeLimitId);

                reference.Property(value => value.TimeLimitId).HasConversion<TimeLimitIdConverter>().IsRequired();
                reference.Property(value => value.OrderItemId).HasConversion<OrderItemIdConverter>().IsRequired();

                reference.HasKey(value => new { value.TimeLimitId, value.OrderItemId });
            });

            builder.OwnsMany(timeLimit => timeLimit.EntitlementRefs, reference =>
            {
                reference.ToTable("TimeLimitEntitlements", OrderingDbContext.OrderingSchema);
                reference.WithOwner().HasForeignKey(value => value.TimeLimitId);

                reference.Property(value => value.TimeLimitId).HasConversion<TimeLimitIdConverter>().IsRequired();
                reference.Property(value => value.EntitlementId).HasConversion<EntitlementIdConverter>().IsRequired();

                reference.HasKey(value => new { value.TimeLimitId, value.EntitlementId });
            });

            builder.OwnsMany(timeLimit => timeLimit.TravelerRefs, reference =>
            {
                reference.ToTable("TimeLimitTravelers", OrderingDbContext.OrderingSchema);
                reference.WithOwner().HasForeignKey(value => value.TimeLimitId);

                reference.Property(value => value.TimeLimitId).HasConversion<TimeLimitIdConverter>().IsRequired();
                reference.Property(value => value.TravelerId).HasConversion<TravelerIdConverter>().IsRequired();

                reference.HasKey(value => new { value.TimeLimitId, value.TravelerId });
            });

            builder.Navigation(timeLimit => timeLimit.OrderItemRefs).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(timeLimit => timeLimit.EntitlementRefs).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(timeLimit => timeLimit.TravelerRefs).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(timeLimit => timeLimit.OrderItemIds);
            builder.Ignore(timeLimit => timeLimit.EntitlementIds);
            builder.Ignore(timeLimit => timeLimit.TravelerIds);
            builder.Ignore(timeLimit => timeLimit.IsActive);
            builder.Ignore(timeLimit => timeLimit.IsOrderWide);
        }
    }

    public sealed class ProcessingLockConfiguration : IEntityTypeConfiguration<ProcessingLock>
    {
        public void Configure(EntityTypeBuilder<ProcessingLock> builder)
        {
            builder.ToTable("ProcessingLocks", OrderingDbContext.OrderingSchema);

            builder.HasKey(processingLock => processingLock.Id);
            builder.Property(processingLock => processingLock.Id)
                .HasConversion<ProcessingLockIdConverter>()
                .ValueGeneratedNever();

            builder.Property(processingLock => processingLock.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasIndex(processingLock => processingLock.OrderId);

            builder.Property(processingLock => processingLock.WorkflowInstanceId)
                .HasConversion<WorkflowInstanceIdConverter>()
                .IsRequired();

            builder.HasIndex(processingLock => processingLock.WorkflowInstanceId);

            builder.Property(processingLock => processingLock.Type)
                .HasConversion<string>()
                .HasColumnType("varchar(32)")
                .IsRequired();

            builder.Property(processingLock => processingLock.AcquiredAt)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnName("AcquiredAtUtc")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(processingLock => processingLock.ExpiresAt)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnName("ExpiresAtUtc")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.HasIndex(processingLock => processingLock.ExpiresAt);

            builder.Property(processingLock => processingLock.LastUpdateTime);
            builder.Property(processingLock => processingLock.LastUpdatedBy);

            builder.OwnsMany(processingLock => processingLock.OrderItemRefs, reference =>
            {
                reference.ToTable("ProcessingLockItems", OrderingDbContext.OrderingSchema);
                reference.WithOwner().HasForeignKey(value => value.ProcessingLockId);

                reference.Property(value => value.ProcessingLockId).HasConversion<ProcessingLockIdConverter>().IsRequired();
                reference.Property(value => value.OrderItemId).HasConversion<OrderItemIdConverter>().IsRequired();

                reference.HasKey(value => new { value.ProcessingLockId, value.OrderItemId });
            });

            builder.OwnsMany(processingLock => processingLock.EntitlementRefs, reference =>
            {
                reference.ToTable("ProcessingLockEntitlements", OrderingDbContext.OrderingSchema);
                reference.WithOwner().HasForeignKey(value => value.ProcessingLockId);

                reference.Property(value => value.ProcessingLockId).HasConversion<ProcessingLockIdConverter>().IsRequired();
                reference.Property(value => value.EntitlementId).HasConversion<EntitlementIdConverter>().IsRequired();

                reference.HasKey(value => new { value.ProcessingLockId, value.EntitlementId });
            });

            builder.Navigation(processingLock => processingLock.OrderItemRefs).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(processingLock => processingLock.EntitlementRefs).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(processingLock => processingLock.OrderItemIds);
            builder.Ignore(processingLock => processingLock.EntitlementIds);
            builder.Ignore(processingLock => processingLock.IsOrderWide);
        }
    }

    public sealed class ExternalReferenceConfiguration : IEntityTypeConfiguration<ExternalReference>
    {
        public void Configure(EntityTypeBuilder<ExternalReference> builder)
        {
            builder.ToTable("ExternalReferences", OrderingDbContext.OrderingSchema);

            builder.HasKey(reference => reference.Id);
            builder.Property(reference => reference.Id)
                .HasConversion<ExternalReferenceIdConverter>()
                .ValueGeneratedNever();

            builder.Property(reference => reference.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasIndex(reference => reference.OrderId);

            builder.Property(reference => reference.Scope)
                .HasConversion<string>()
                .HasColumnType("varchar(24)")
                .IsRequired();

            builder.Property(reference => reference.ScopedEntityId);

            builder.Property(reference => reference.System)
                .HasColumnType("varchar(64)")
                .IsRequired();

            builder.Property(reference => reference.Type)
                .HasColumnType("varchar(64)")
                .IsRequired();

            builder.Property(reference => reference.Value)
                .HasColumnType("varchar(128)")
                .IsRequired();

            builder.Property(reference => reference.Owner).HasColumnType("varchar(128)");

            // Search projection: locators, ticket numbers and partner references.
            builder.HasIndex(reference => new { reference.System, reference.Type, reference.Value });

            builder.Property(reference => reference.LastUpdateTime);
            builder.Property(reference => reference.LastUpdatedBy);
        }
    }

    public sealed class ServicingDelegationConfiguration : IEntityTypeConfiguration<ServicingDelegation>
    {
        public void Configure(EntityTypeBuilder<ServicingDelegation> builder)
        {
            builder.ToTable("ServicingDelegations", OrderingDbContext.OrderingSchema);

            builder.HasKey(delegation => delegation.Id);
            builder.Property(delegation => delegation.Id)
                .HasConversion<ServicingDelegationIdConverter>()
                .ValueGeneratedNever();

            builder.Property(delegation => delegation.OrderId)
                .HasConversion<OrderIdConverter>()
                .IsRequired();

            builder.HasIndex(delegation => delegation.OrderId);

            builder.Property(delegation => delegation.DelegatePartyRef)
                .HasColumnType("varchar(128)")
                .IsRequired();

            builder.Property(delegation => delegation.Scope)
                .HasColumnType("varchar(128)")
                .IsRequired();

            builder.Property(delegation => delegation.ValidFrom)
                .HasConversion<InstantToDateTimeConverter>()
                .HasColumnName("ValidFromUtc")
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(delegation => delegation.ValidUntil)
                .HasConversion<NullableInstantToDateTimeConverter>()
                .HasColumnName("ValidUntilUtc")
                .HasColumnType("datetime2(7)");

            builder.Property(delegation => delegation.LastUpdateTime);
            builder.Property(delegation => delegation.LastUpdatedBy);

            builder.PrimitiveCollection(delegation => delegation.AuthorityTypes)
                .HasColumnName("AuthorityTypes")
                .HasColumnType("nvarchar(max)")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .ElementType(element => element.HasConversion<string>());
        }
    }
}

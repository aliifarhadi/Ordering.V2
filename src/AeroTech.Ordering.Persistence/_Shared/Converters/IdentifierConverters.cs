using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AeroTech.Ordering.Persistence._Shared.Converters
{
    public sealed class OrderIdConverter : ValueConverter<OrderId, Guid>
    {
        public OrderIdConverter() : base(id => id.Value, value => new OrderId(value))
        {
        }
    }

    public sealed class NullableOrderIdConverter : ValueConverter<OrderId?, Guid?>
    {
        public NullableOrderIdConverter()
            : base(id => id == null ? null : id.Value.Value, value => value == null ? null : new OrderId(value.Value))
        {
        }
    }

    public sealed class TravelerIdConverter : ValueConverter<TravelerId, Guid>
    {
        public TravelerIdConverter() : base(id => id.Value, value => new TravelerId(value))
        {
        }
    }

    public sealed class NullableTravelerIdConverter : ValueConverter<TravelerId?, Guid?>
    {
        public NullableTravelerIdConverter()
            : base(id => id == null ? null : id.Value.Value, value => value == null ? null : new TravelerId(value.Value))
        {
        }
    }

    public sealed class ContactIdConverter : ValueConverter<ContactId, Guid>
    {
        public ContactIdConverter() : base(id => id.Value, value => new ContactId(value))
        {
        }
    }

    public sealed class JourneyIdConverter : ValueConverter<JourneyId, Guid>
    {
        public JourneyIdConverter() : base(id => id.Value, value => new JourneyId(value))
        {
        }
    }

    public sealed class NullableJourneyIdConverter : ValueConverter<JourneyId?, Guid?>
    {
        public NullableJourneyIdConverter()
            : base(id => id == null ? null : id.Value.Value, value => value == null ? null : new JourneyId(value.Value))
        {
        }
    }

    public sealed class JourneySegmentIdConverter : ValueConverter<JourneySegmentId, Guid>
    {
        public JourneySegmentIdConverter() : base(id => id.Value, value => new JourneySegmentId(value))
        {
        }
    }

    public sealed class NullableJourneySegmentIdConverter : ValueConverter<JourneySegmentId?, Guid?>
    {
        public NullableJourneySegmentIdConverter()
            : base(
                id => id == null ? null : id.Value.Value,
                value => value == null ? null : new JourneySegmentId(value.Value))
        {
        }
    }

    public sealed class OrderItemIdConverter : ValueConverter<OrderItemId, Guid>
    {
        public OrderItemIdConverter() : base(id => id.Value, value => new OrderItemId(value))
        {
        }
    }

    public sealed class NullableOrderItemIdConverter : ValueConverter<OrderItemId?, Guid?>
    {
        public NullableOrderItemIdConverter()
            : base(id => id == null ? null : id.Value.Value, value => value == null ? null : new OrderItemId(value.Value))
        {
        }
    }

    public sealed class EntitlementIdConverter : ValueConverter<EntitlementId, Guid>
    {
        public EntitlementIdConverter() : base(id => id.Value, value => new EntitlementId(value))
        {
        }
    }

    public sealed class NullableEntitlementIdConverter : ValueConverter<EntitlementId?, Guid?>
    {
        public NullableEntitlementIdConverter()
            : base(id => id == null ? null : id.Value.Value, value => value == null ? null : new EntitlementId(value.Value))
        {
        }
    }

    public sealed class TimeLimitIdConverter : ValueConverter<TimeLimitId, Guid>
    {
        public TimeLimitIdConverter() : base(id => id.Value, value => new TimeLimitId(value))
        {
        }
    }

    public sealed class ProcessingLockIdConverter : ValueConverter<ProcessingLockId, Guid>
    {
        public ProcessingLockIdConverter() : base(id => id.Value, value => new ProcessingLockId(value))
        {
        }
    }

    public sealed class ExternalReferenceIdConverter : ValueConverter<ExternalReferenceId, Guid>
    {
        public ExternalReferenceIdConverter() : base(id => id.Value, value => new ExternalReferenceId(value))
        {
        }
    }

    public sealed class ServicingDelegationIdConverter : ValueConverter<ServicingDelegationId, Guid>
    {
        public ServicingDelegationIdConverter() : base(id => id.Value, value => new ServicingDelegationId(value))
        {
        }
    }

    public sealed class NullableChangeIdConverter : ValueConverter<ChangeId?, Guid?>
    {
        public NullableChangeIdConverter()
            : base(id => id == null ? null : id.Value.Value, value => value == null ? null : new ChangeId(value.Value))
        {
        }
    }

    public sealed class ValueAllocationIdConverter : ValueConverter<ValueAllocationId, Guid>
    {
        public ValueAllocationIdConverter() : base(id => id.Value, value => new ValueAllocationId(value))
        {
        }
    }

    public sealed class ChargeLineIdConverter : ValueConverter<ChargeLineId, Guid>
    {
        public ChargeLineIdConverter() : base(id => id.Value, value => new ChargeLineId(value))
        {
        }
    }

    public sealed class FulfillmentLinkIdConverter : ValueConverter<FulfillmentLinkId, Guid>
    {
        public FulfillmentLinkIdConverter() : base(id => id.Value, value => new FulfillmentLinkId(value))
        {
        }
    }

    public sealed class IdentityDocumentIdConverter : ValueConverter<IdentityDocumentId, Guid>
    {
        public IdentityDocumentIdConverter() : base(id => id.Value, value => new IdentityDocumentId(value))
        {
        }
    }

    public sealed class LoyaltyAccountIdConverter : ValueConverter<LoyaltyAccountId, Guid>
    {
        public LoyaltyAccountIdConverter() : base(id => id.Value, value => new LoyaltyAccountId(value))
        {
        }
    }

    public sealed class TravelerAssociationIdConverter : ValueConverter<TravelerAssociationId, Guid>
    {
        public TravelerAssociationIdConverter() : base(id => id.Value, value => new TravelerAssociationId(value))
        {
        }
    }

    public sealed class WorkflowInstanceIdConverter : ValueConverter<WorkflowInstanceId, Guid>
    {
        public WorkflowInstanceIdConverter() : base(id => id.Value, value => new WorkflowInstanceId(value))
        {
        }
    }

    public sealed class NullableWorkflowInstanceIdConverter : ValueConverter<WorkflowInstanceId?, Guid?>
    {
        public NullableWorkflowInstanceIdConverter()
            : base(
                id => id == null ? null : id.Value.Value,
                value => value == null ? null : new WorkflowInstanceId(value.Value))
        {
        }
    }

    public sealed class ElectronicTicketIdConverter : ValueConverter<ElectronicTicketId, Guid>
    {
        public ElectronicTicketIdConverter() : base(id => id.Value, value => new ElectronicTicketId(value))
        {
        }
    }

    public sealed class TicketCouponIdConverter : ValueConverter<TicketCouponId, Guid>
    {
        public TicketCouponIdConverter() : base(id => id.Value, value => new TicketCouponId(value))
        {
        }
    }

    public sealed class ElectronicMiscDocumentIdConverter : ValueConverter<ElectronicMiscDocumentId, Guid>
    {
        public ElectronicMiscDocumentIdConverter() : base(id => id.Value, value => new ElectronicMiscDocumentId(value))
        {
        }
    }

    public sealed class EmdCouponIdConverter : ValueConverter<EmdCouponId, Guid>
    {
        public EmdCouponIdConverter() : base(id => id.Value, value => new EmdCouponId(value))
        {
        }
    }

    public sealed class SupplierReservationIdConverter : ValueConverter<SupplierReservationId, Guid>
    {
        public SupplierReservationIdConverter() : base(id => id.Value, value => new SupplierReservationId(value))
        {
        }
    }

    public sealed class NullableSupplierReservationIdConverter : ValueConverter<SupplierReservationId?, Guid?>
    {
        public NullableSupplierReservationIdConverter()
            : base(
                id => id == null ? null : id.Value.Value,
                value => value == null ? null : new SupplierReservationId(value.Value))
        {
        }
    }

    public sealed class FulfillmentUnitIdConverter : ValueConverter<FulfillmentUnitId, Guid>
    {
        public FulfillmentUnitIdConverter() : base(id => id.Value, value => new FulfillmentUnitId(value))
        {
        }
    }

    public sealed class NullableFulfillmentUnitIdConverter : ValueConverter<FulfillmentUnitId?, Guid?>
    {
        public NullableFulfillmentUnitIdConverter()
            : base(
                id => id == null ? null : id.Value.Value,
                value => value == null ? null : new FulfillmentUnitId(value.Value))
        {
        }
    }

    public sealed class DocumentStockIdConverter : ValueConverter<DocumentStockId, Guid>
    {
        public DocumentStockIdConverter() : base(id => id.Value, value => new DocumentStockId(value))
        {
        }
    }

    public sealed class ConsumptionFactIdConverter : ValueConverter<ConsumptionFactId, Guid>
    {
        public ConsumptionFactIdConverter() : base(id => id.Value, value => new ConsumptionFactId(value))
        {
        }
    }

    public sealed class ReconciliationCaseIdConverter : ValueConverter<ReconciliationCaseId, Guid>
    {
        public ReconciliationCaseIdConverter() : base(id => id.Value, value => new ReconciliationCaseId(value))
        {
        }
    }

    public sealed class CurrencyCodeConverter : ValueConverter<CurrencyCode, string>
    {
        public CurrencyCodeConverter() : base(code => code.Value, value => new CurrencyCode(value))
        {
        }
    }

    public sealed class AirportCodeConverter : ValueConverter<AirportCode, string>
    {
        public AirportCodeConverter() : base(code => code.Value, value => new AirportCode(value))
        {
        }
    }

    public sealed class CarrierCodeConverter : ValueConverter<CarrierCode, string>
    {
        public CarrierCodeConverter() : base(code => code.Value, value => new CarrierCode(value))
        {
        }
    }

    public sealed class CountryCodeConverter : ValueConverter<CountryCode, string>
    {
        public CountryCodeConverter() : base(code => code.Value, value => new CountryCode(value))
        {
        }
    }

    public sealed class NullableCountryCodeConverter : ValueConverter<CountryCode?, string?>
    {
        public NullableCountryCodeConverter()
            : base(
                code => code == null ? null : code.Value.Value,
                value => value == null ? null : new CountryCode(value))
        {
        }
    }

    public sealed class OrderReferenceConverter : ValueConverter<OrderReference, string>
    {
        public OrderReferenceConverter() : base(reference => reference.Value, value => new OrderReference(value))
        {
        }
    }
}

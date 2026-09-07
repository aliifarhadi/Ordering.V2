using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.Ordering.Specifications;
using AeroTech.Ordering.Domain.Ordering.StateMachines;
using AeroTech.Ordering.Domain.Ordering.ValueObjects;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class Entitlement : Entity<EntitlementId>
    {
        private readonly List<EntitlementBeneficiary> _beneficiaryRefs = [];
        private readonly List<EntitlementSegmentRef> _segmentRefs = [];
        private readonly List<FulfillmentLink> _fulfillmentLinks = [];

        private Entitlement()
        {
        }

        private Entitlement(
            EntitlementId id,
            OrderId orderId,
            OrderItemId orderItemId,
            EntitlementType type,
            EntitlementCommercialStatus status,
            Applicability applicability,
            EntitlementSpecification specification,
            CapacityCommitmentRef? capacityCommitment,
            ResponsibilityAssignment? responsibility,
            Instant createdAt,
            IEnumerable<TravelerId> beneficiaryRefs)
        {
            Id = id;
            OrderId = orderId;
            OrderItemId = orderItemId;
            Type = type;
            Status = status;
            JourneyRef = applicability.JourneyRef;
            LocationRef = applicability.LocationRef;
            StartDate = applicability.StartDate;
            EndDate = applicability.EndDate;
            Specification = specification;
            CapacityCommitment = capacityCommitment;
            Responsibility = responsibility;
            CreatedAt = createdAt;

            _beneficiaryRefs.AddRange(beneficiaryRefs.Distinct().Select(travelerId => new EntitlementBeneficiary(id, travelerId)));

            var sequence = 1;
            foreach (var segmentId in applicability.SegmentRefs)
                _segmentRefs.Add(new EntitlementSegmentRef(id, segmentId, sequence++));
        }

        public OrderId OrderId { get; private set; }

        public OrderItemId OrderItemId { get; private set; }

        public EntitlementType Type { get; private set; }

        public EntitlementCommercialStatus Status { get; private set; }

        public JourneyId? JourneyRef { get; private set; }

        public string? LocationRef { get; private set; }

        public LocalDate? StartDate { get; private set; }

        public LocalDate? EndDate { get; private set; }

        public EntitlementSpecification Specification { get; private set; } = null!;

        public CapacityCommitmentRef? CapacityCommitment { get; private set; }

        public ResponsibilityAssignment? Responsibility { get; private set; }

        public Instant CreatedAt { get; private set; }

        public Instant? CancelledAt { get; private set; }

        public Instant? ReplacedAt { get; private set; }

        public IReadOnlyCollection<EntitlementBeneficiary> BeneficiaryRefs => _beneficiaryRefs.AsReadOnly();

        public IReadOnlyCollection<EntitlementSegmentRef> SegmentRefs => _segmentRefs.AsReadOnly();

        public IReadOnlyCollection<FulfillmentLink> FulfillmentLinks => _fulfillmentLinks.AsReadOnly();

        public IEnumerable<TravelerId> BeneficiaryTravelerIds => _beneficiaryRefs.Select(reference => reference.TravelerId);

        public IEnumerable<JourneySegmentId> SegmentIds => _segmentRefs.Select(reference => reference.JourneySegmentId);

        public bool IsActive => Status is EntitlementCommercialStatus.Active;

        public bool IsTerminal => EntitlementStateMachine.IsTerminal(Status);

        public Applicability Scope => new(JourneyRef, SegmentIds, LocationRef, StartDate, EndDate);

        internal static Entitlement Create(
            EntitlementId id,
            OrderId orderId,
            OrderItemId orderItemId,
            EntitlementType type,
            IEnumerable<TravelerId> beneficiaryRefs,
            Applicability applicability,
            EntitlementSpecification specification,
            Instant createdAt,
            CapacityCommitmentRef? capacityCommitment = null,
            ResponsibilityAssignment? responsibility = null,
            EntitlementCommercialStatus status = EntitlementCommercialStatus.Pending)
        {
            var beneficiaries = beneficiaryRefs.Distinct().ToArray();
            if (beneficiaries.Length == 0)
                throw ExceptionFactory.EntitlementRequiresBeneficiary(id);

            if (!specification.SupportsEntitlementType(type))
                throw ExceptionFactory.EntitlementSpecificationMismatch(id, type, specification.GetType().Name);

            specification.EntitlementId = id;
            specification.Validate(id);

            var entitlement = new Entitlement(
                id,
                orderId,
                orderItemId,
                type,
                status,
                applicability,
                specification,
                capacityCommitment,
                responsibility,
                createdAt,
                beneficiaries);

            entitlement.ValidateScope();
            return entitlement;
        }

        internal void TransitionTo(EntitlementCommercialStatus target, Instant occurredAt)
        {
            if (IsTerminal)
                throw ExceptionFactory.EntitlementIsTerminal(Id, Status);

            if (!EntitlementStateMachine.CanTransition(Status, target))
                throw ExceptionFactory.EntitlementCannotTransition(Id, Status, target);

            Status = target;
            switch (target)
            {
                case EntitlementCommercialStatus.Cancelled:
                    CancelledAt = occurredAt;
                    break;
                case EntitlementCommercialStatus.Replaced:
                    ReplacedAt = occurredAt;
                    break;
            }
        }

        internal void Activate(Instant occurredAt)
        {
            // INV-045: capacity-controlled AirTransport requires a capacity commitment before confirmation.
            if (Type is EntitlementType.AirTransport && CapacityCommitment is null)
                throw ExceptionFactory.CapacityCommitmentIsRequired(Id);

            TransitionTo(EntitlementCommercialStatus.Active, occurredAt);
        }

        internal void AssignCapacityCommitment(CapacityCommitmentRef capacityCommitment) =>
            CapacityCommitment = capacityCommitment;

        internal void AssignResponsibility(ResponsibilityAssignment responsibility) =>
            Responsibility = responsibility;

        internal void AttachFulfillmentLink(FulfillmentLink link) => _fulfillmentLinks.Add(link);

        internal void ReassignToItem(OrderItemId orderItemId) => OrderItemId = orderItemId;

        internal void ReassignToOrder(OrderId orderId) => OrderId = orderId;

        private void ValidateScope()
        {
            switch (Type)
            {
                // INV-040: Seat applies to exactly one traveller and one segment.
                case EntitlementType.SeatAssignment
                    when _beneficiaryRefs.Count != 1 || _segmentRefs.Count != 1:
                    throw ExceptionFactory.SeatEntitlementScopeIsInvalid(Id);

                // INV-041: AirTransport applies to exactly one traveller and at least one segment.
                case EntitlementType.AirTransport
                    when _beneficiaryRefs.Count != 1 || _segmentRefs.Count == 0:
                    throw ExceptionFactory.AirTransportEntitlementScopeIsInvalid(Id);

                // INV-043: Accommodation requires at least one beneficiary.
                case EntitlementType.AccommodationStay when _beneficiaryRefs.Count == 0:
                    throw ExceptionFactory.AccommodationRequiresBeneficiary(Id);
            }

            // INV-046: externally supplied entitlements require a responsibility assignment.
            if (Responsibility is not null && Responsibility.IsEmpty)
                throw ExceptionFactory.ResponsibilityAssignmentIsRequired(Id);
        }
    }

    public sealed record EntitlementSegmentRef(EntitlementId EntitlementId, JourneySegmentId JourneySegmentId, int Sequence);

    public sealed record EntitlementBeneficiary(EntitlementId EntitlementId, TravelerId TravelerId);
}

using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.Ordering.Entities;
using AeroTech.Ordering.Domain.Ordering.Events;
using AeroTech.Ordering.Domain.Ordering.Specifications;
using AeroTech.Ordering.Domain.Ordering.StateMachines;
using AeroTech.Ordering.Domain.Ordering.ValueObjects;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.Aggregates
{
    public sealed class Order : AggregateRoot<OrderId>
    {
        private readonly List<Traveler> _travelers = [];
        private readonly List<Contact> _contacts = [];
        private readonly List<Journey> _journeys = [];
        private readonly List<OrderItem> _items = [];
        private readonly List<TimeLimit> _timeLimits = [];
        private readonly List<ProcessingLock> _processingLocks = [];
        private readonly List<ExternalReference> _externalReferences = [];
        private readonly List<ServicingDelegation> _delegations = [];

        private Order()
        {
        }

        private Order(
            OrderId id,
            OrderReference reference,
            SalesContext createdSalesContext,
            BuyerSnapshot? buyer,
            ServicingAuthority servicingAuthority,
            OrderLineage lineage)
        {
            Id = id;
            Reference = reference;
            CreatedSalesContext = createdSalesContext;
            Buyer = buyer;
            ServicingAuthority = servicingAuthority;
            Lineage = lineage;
            AggregateVersion = 1;
        }

        public OrderReference Reference { get; private set; }

        public long AggregateVersion { get; private set; }

        public SalesContext CreatedSalesContext { get; private set; } = null!;

        public BuyerSnapshot? Buyer { get; private set; }

        public ServicingAuthority ServicingAuthority { get; private set; } = null!;

        public OrderLineage Lineage { get; private set; } = null!;

        public Instant? ClosedAt { get; private set; }

        public IReadOnlyCollection<Traveler> Travelers => _travelers.AsReadOnly();

        public IReadOnlyCollection<Contact> Contacts => _contacts.AsReadOnly();

        public IReadOnlyCollection<Journey> Journeys => _journeys.AsReadOnly();

        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public IReadOnlyCollection<TimeLimit> TimeLimits => _timeLimits.AsReadOnly();

        public IReadOnlyCollection<ProcessingLock> ProcessingLocks => _processingLocks.AsReadOnly();

        public IReadOnlyCollection<ExternalReference> ExternalReferences => _externalReferences.AsReadOnly();

        public IReadOnlyCollection<ServicingDelegation> Delegations => _delegations.AsReadOnly();

        public IEnumerable<JourneySegment> AllSegments => _journeys.SelectMany(journey => journey.Segments);

        public IEnumerable<Entitlement> AllEntitlements => _items.SelectMany(item => item.Entitlements);

        /// <summary>Derived commercial lifecycle. Never persisted as the source of truth.</summary>
        public OrderLifecycle Lifecycle
        {
            get
            {
                if (ClosedAt is not null)
                    return OrderLifecycle.Closed;

                if (_items.Count > 0 && _items.All(item => item.IsTerminal))
                    return OrderLifecycle.Cancelled;

                if (_items.Any(item => item.Status is OrderItemCommercialStatus.Confirmed
                        or OrderItemCommercialStatus.PartiallyChanged))
                    return OrderLifecycle.Active;

                return OrderLifecycle.Draft;
            }
        }

        public static Order CreateDraft(
            OrderId id,
            OrderReference reference,
            SalesContext createdSalesContext,
            ServicingAuthority servicingAuthority,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            BuyerSnapshot? buyer = null,
            OrderLineage? lineage = null)
        {
            var order = new Order(
                id,
                reference,
                createdSalesContext,
                buyer,
                servicingAuthority,
                lineage ?? OrderLineage.ForRoot(id));

            order.Causes(new OrderCreated(
                eventId,
                id.ToString(),
                occurredAt.ToDateTimeOffset(),
                order.AggregateVersion,
                id,
                reference.Value,
                order.Lineage.RootOrderId,
                order.Lineage.ParentOrderId,
                createdSalesContext.SellerId,
                createdSalesContext.ChannelCode));

            return order;
        }

        // ---------------------------------------------------------------- travellers

        public TravelerId AddTraveler(
            TravelerName name,
            TravelerType type,
            LocalDate? dateOfBirth,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            string? gender = null,
            string? customerRef = null,
            TravelerId? travelerId = null)
        {
            EnsureMutable(actor, ServicingAuthorityType.UpdateTraveler, occurredAt);

            var id = travelerId ?? TravelerId.New();
            if (_travelers.Any(traveler => traveler.Id == id))
                throw ExceptionFactory.DuplicateTravelerId(id);

            var traveler = Traveler.Create(id, Id, name, type, dateOfBirth, gender, customerRef);
            _travelers.Add(traveler);

            BumpVersion();
            Causes(new TravelerAdded(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, id, type));
            return id;
        }

        public void AssociateTravelers(
            TravelerId travelerId,
            TravelerId relatedTravelerId,
            TravelerAssociationType associationType,
            ActorContext actor,
            Instant occurredAt,
            string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.UpdateTraveler, occurredAt);

            var traveler = RequireTraveler(travelerId);
            var related = RequireTraveler(relatedTravelerId);

            // INV-007 support: the associated adult must genuinely be an adult.
            if (associationType is TravelerAssociationType.AssociatedAdult && related.Type is not TravelerType.ADT)
                throw ExceptionFactory.AssociatedAdultMustBeAdult(relatedTravelerId);

            traveler.AddAssociation(TravelerAssociation.Create(travelerId, relatedTravelerId, associationType));

            BumpVersion();
            Causes(new TravelerUpdated(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, travelerId));
        }

        public void UpdateTravelerDetails(
            TravelerId travelerId,
            TravelerName name,
            LocalDate? dateOfBirth,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            string? gender = null,
            string? customerRef = null)
        {
            EnsureMutable(actor, ServicingAuthorityType.UpdateTraveler, occurredAt);

            RequireTraveler(travelerId).UpdateDetails(name, dateOfBirth, gender, customerRef);

            BumpVersion();
            Causes(new TravelerUpdated(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, travelerId));
        }

        public void AddTravelerIdentityDocument(
            TravelerId travelerId,
            IdentityDocumentType documentType,
            string documentNumber,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            CountryCode? issuingCountry = null,
            LocalDate? expiryDate = null,
            CountryCode? nationality = null)
        {
            EnsureMutable(actor, ServicingAuthorityType.UpdateTraveler, occurredAt);

            RequireTraveler(travelerId).AddIdentityDocument(
                IdentityDocument.Create(travelerId, documentType, documentNumber, issuingCountry, expiryDate, nationality));

            BumpVersion();
            Causes(new TravelerUpdated(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, travelerId));
        }

        public void AddTravelerLoyaltyAccount(
            TravelerId travelerId,
            string programCode,
            string accountRef,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            string? tierCode = null)
        {
            EnsureMutable(actor, ServicingAuthorityType.UpdateTraveler, occurredAt);

            RequireTraveler(travelerId).AddLoyaltyAccount(
                LoyaltyAccountRef.Create(travelerId, programCode, accountRef, tierCode));

            BumpVersion();
            Causes(new TravelerUpdated(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, travelerId));
        }

        public void RemoveTraveler(TravelerId travelerId, ActorContext actor, Instant occurredAt, string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.UpdateTraveler, occurredAt);

            var traveler = RequireTraveler(travelerId);

            // INV-009: cannot remove a traveller referenced by a non-terminal item or active entitlement.
            var isReferenced = _items.Any(item =>
                !item.IsTerminal && item.BeneficiaryTravelerIds.Contains(travelerId))
                || AllEntitlements.Any(entitlement =>
                    entitlement.IsActive && entitlement.BeneficiaryTravelerIds.Contains(travelerId));

            if (isReferenced)
                throw ExceptionFactory.CannotRemoveReferencedTraveler(travelerId);

            _travelers.Remove(traveler);

            foreach (var contact in _contacts.Where(contact => contact.TravelerIds.Contains(travelerId)).ToArray())
                contact.Update(contact.Type, contact.Value, contact.Role, contact.IsPrimary,
                    contact.TravelerIds.Where(reference => reference != travelerId));

            BumpVersion();
            Causes(new TravelerRemoved(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, travelerId));
        }

        // ---------------------------------------------------------------- contacts

        public ContactId AddContact(
            ContactType type,
            string value,
            ContactRole role,
            bool isPrimary,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            IEnumerable<TravelerId>? travelerRefs = null,
            ContactId? contactId = null)
        {
            EnsureMutable(actor, ServicingAuthorityType.UpdateTraveler, occurredAt);

            var id = contactId ?? ContactId.New();
            if (_contacts.Any(contact => contact.Id == id))
                throw ExceptionFactory.DuplicateContactId(id);

            var references = travelerRefs?.ToArray() ?? [];
            foreach (var travelerId in references)
            {
                if (_travelers.All(traveler => traveler.Id != travelerId))
                    throw ExceptionFactory.ContactTravelerMustExistInOrder(id, travelerId);
            }

            if (isPrimary)
                DemoteExistingPrimaryContacts(role, type);

            _contacts.Add(Contact.Create(id, Id, type, value, role, isPrimary, references));

            BumpVersion();
            Causes(new ContactAdded(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, id));
            return id;
        }

        public void UpdateContact(
            ContactId contactId,
            ContactType type,
            string value,
            ContactRole role,
            bool isPrimary,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            IEnumerable<TravelerId>? travelerRefs = null)
        {
            EnsureMutable(actor, ServicingAuthorityType.UpdateTraveler, occurredAt);

            var contact = _contacts.SingleOrDefault(candidate => candidate.Id == contactId)
                ?? throw ExceptionFactory.ContactNotFound(contactId);

            var references = travelerRefs?.ToArray() ?? [];
            foreach (var travelerId in references)
            {
                if (_travelers.All(traveler => traveler.Id != travelerId))
                    throw ExceptionFactory.ContactTravelerMustExistInOrder(contactId, travelerId);
            }

            if (isPrimary)
                DemoteExistingPrimaryContacts(role, type, contactId);

            contact.Update(type, value, role, isPrimary, references);

            BumpVersion();
            Causes(new ContactUpdated(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, contactId));
        }

        public void RemoveContact(ContactId contactId, ActorContext actor, Instant occurredAt, string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.UpdateTraveler, occurredAt);

            var contact = _contacts.SingleOrDefault(candidate => candidate.Id == contactId)
                ?? throw ExceptionFactory.ContactNotFound(contactId);

            _contacts.Remove(contact);

            BumpVersion();
            Causes(new ContactRemoved(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, contactId));
        }

        // ---------------------------------------------------------------- journeys

        public JourneyId AddJourney(
            AirportCode origin,
            AirportCode destination,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            JourneyId? journeyId = null,
            int? sequence = null)
        {
            EnsureMutable(actor, ServicingAuthorityType.VoluntaryChange, occurredAt);

            var id = journeyId ?? JourneyId.New();
            if (_journeys.Any(journey => journey.Id == id))
                throw ExceptionFactory.DuplicateJourneyId(id);

            var journeySequence = sequence ?? (_journeys.Count == 0 ? 1 : _journeys.Max(journey => journey.Sequence) + 1);
            _journeys.Add(Journey.Create(id, Id, origin, destination, journeySequence));

            BumpVersion();
            Causes(new JourneyAdded(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, id));
            return id;
        }

        public JourneySegmentId AddJourneySegment(
            JourneyId journeyId,
            CarrierCode marketingCarrier,
            CarrierCode operatingCarrier,
            string flightNumber,
            AirportCode origin,
            AirportCode destination,
            Instant departureUtc,
            Instant arrivalUtc,
            LocalDate departureLocalDate,
            LocalTime departureLocalTime,
            string originTimeZoneId,
            LocalDate arrivalLocalDate,
            LocalTime arrivalLocalTime,
            string destinationTimeZoneId,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            string? aircraftType = null,
            JourneySegmentId? segmentId = null,
            int? sequence = null)
        {
            EnsureMutable(actor, ServicingAuthorityType.VoluntaryChange, occurredAt);

            var journey = RequireJourney(journeyId);

            var id = segmentId ?? JourneySegmentId.New();
            if (AllSegments.Any(segment => segment.Id == id))
                throw ExceptionFactory.DuplicateJourneySegmentId(id);

            var segment = JourneySegment.Create(
                id,
                Id,
                journeyId,
                marketingCarrier,
                operatingCarrier,
                flightNumber,
                origin,
                destination,
                departureUtc,
                arrivalUtc,
                departureLocalDate,
                departureLocalTime,
                originTimeZoneId,
                arrivalLocalDate,
                arrivalLocalTime,
                destinationTimeZoneId,
                aircraftType,
                sequence ?? journey.NextSegmentSequence());

            journey.AddSegment(segment);

            // INV-006: segments of a journey must form a contiguous ascending sequence.
            if (!journey.HasContiguousSegmentSequence())
                throw ExceptionFactory.JourneySegmentSequenceIsInvalid(journeyId);

            BumpVersion();
            Causes(new JourneySegmentAdded(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, journeyId, id));
            return id;
        }

        public void ApplyScheduleSnapshotChange(
            JourneySegmentId segmentId,
            CarrierCode marketingCarrier,
            CarrierCode operatingCarrier,
            string flightNumber,
            Instant departureUtc,
            Instant arrivalUtc,
            LocalDate departureLocalDate,
            LocalTime departureLocalTime,
            LocalDate arrivalLocalDate,
            LocalTime arrivalLocalTime,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            string? aircraftType = null)
        {
            EnsureMutable(actor, ServicingAuthorityType.VoluntaryChange, occurredAt);

            RequireSegment(segmentId).ApplyScheduleSnapshotChange(
                marketingCarrier,
                operatingCarrier,
                flightNumber,
                departureUtc,
                arrivalUtc,
                departureLocalDate,
                departureLocalTime,
                arrivalLocalDate,
                arrivalLocalTime,
                aircraftType);

            BumpVersion();
            Causes(new JourneySegmentScheduleChanged(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, segmentId));
        }

        public void RemoveUnusedJourneySegment(
            JourneySegmentId segmentId,
            ActorContext actor,
            Instant occurredAt,
            string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.VoluntaryChange, occurredAt);

            var segment = RequireSegment(segmentId);

            // INV-010: a segment referenced by an active entitlement cannot be removed.
            if (AllEntitlements.Any(entitlement => entitlement.IsActive && entitlement.SegmentIds.Contains(segmentId)))
                throw ExceptionFactory.CannotRemoveReferencedJourneySegment(segmentId);

            RequireJourney(segment.JourneyId).RemoveSegment(segmentId);

            BumpVersion();
            Causes(new JourneySegmentRemoved(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, segmentId));
        }

        // ---------------------------------------------------------------- commercial items

        public OrderItemId AddOrderItem(
            ProductSnapshot product,
            PriceSnapshot price,
            CommercialTermsSnapshot commercialTerms,
            CommercialSource commercialSource,
            SalesContext itemSalesContext,
            SettlementArrangement settlement,
            IEnumerable<TravelerId> beneficiaryTravelerIds,
            IEnumerable<ChargeLineDraft> chargeLines,
            IEnumerable<EntitlementDraft> entitlementDrafts,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            IEnumerable<ValueAllocationDraft>? valueAllocations = null,
            ItemLineage? lineage = null,
            OrderItemId? orderItemId = null)
        {
            EnsureMutable(actor, ServicingAuthorityType.AddAncillary, occurredAt);

            var id = orderItemId ?? OrderItemId.New();
            if (_items.Any(item => item.Id == id))
                throw ExceptionFactory.DuplicateOrderItemId(id);

            var beneficiaryIds = beneficiaryTravelerIds.Distinct().ToArray();

            // INV-002: every item beneficiary must be a traveller in this order.
            foreach (var travelerId in beneficiaryIds)
            {
                if (_travelers.All(traveler => traveler.Id != travelerId))
                    throw ExceptionFactory.ItemBeneficiaryMustBeOrderTraveler(id, travelerId);
            }

            var itemLineage = lineage ?? ItemLineage.Original();

            // INV-103: predecessors must exist in this order.
            foreach (var predecessorId in itemLineage.PredecessorItemIds)
            {
                if (_items.All(item => item.Id != predecessorId))
                    throw ExceptionFactory.PredecessorItemMustExistInOrder(predecessorId);
            }

            var item = OrderItem.Create(
                id,
                Id,
                product,
                price,
                commercialTerms,
                commercialSource,
                itemSalesContext,
                settlement,
                beneficiaryIds.Select(travelerId => new Beneficiary(id, travelerId)),
                chargeLines.Select(draft => ChargeLine.Create(
                    id,
                    draft.Sequence,
                    draft.Type,
                    draft.Code,
                    draft.Description,
                    draft.Amount,
                    draft.Refundable,
                    draft.TaxJurisdiction)),
                itemLineage,
                occurredAt);

            foreach (var draft in entitlementDrafts)
                item.AddEntitlement(BuildEntitlement(id, draft, occurredAt));

            if (valueAllocations is not null)
            {
                item.SetValueAllocations(valueAllocations.Select(draft => ValueAllocation.Create(
                    id,
                    draft.EntitlementId,
                    draft.JourneySegmentId,
                    draft.TravelerId,
                    draft.Amount,
                    draft.AllocationVersion,
                    draft.Purpose)));
            }

            _items.Add(item);

            BumpVersion();
            Causes(new OrderItemAdded(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                id,
                product.ProductType));

            return id;
        }

        public void ConfirmOrderItems(
            IEnumerable<OrderItemId> orderItemIds,
            ActorContext actor,
            Instant occurredAt,
            string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.AddAncillary, occurredAt);

            // INV-001: an order needs at least one traveller and one item before confirmation.
            if (_travelers.Count == 0 || _items.Count == 0)
                throw ExceptionFactory.OrderRequiresTravelerAndItemToConfirm();

            var ids = orderItemIds.Distinct().ToArray();
            var items = ids.Select(RequireItem).ToArray();

            foreach (var item in items)
                item.Confirm(occurredAt);

            BumpVersion();
            Causes(new OrderItemsConfirmed(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, ids));
        }

        public void ApplyCancellation(
            IEnumerable<EntitlementId> entitlementIds,
            ActorContext actor,
            Instant occurredAt,
            string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.Cancel, occurredAt);

            var ids = entitlementIds.Distinct().ToArray();
            var touchedItems = new HashSet<OrderItemId>();

            foreach (var entitlementId in ids)
            {
                var entitlement = RequireEntitlement(entitlementId);
                entitlement.TransitionTo(EntitlementCommercialStatus.Cancelled, occurredAt);
                touchedItems.Add(entitlement.OrderItemId);
            }

            foreach (var itemId in touchedItems)
                RequireItem(itemId).RecomputeStatus(occurredAt);

            BumpVersion();
            Causes(new EntitlementsCancelled(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, ids));
        }

        public void ApplyReplacement(
            IEnumerable<OrderItemId> predecessorItemIds,
            IEnumerable<OrderItemId> successorItemIds,
            ActorContext actor,
            Instant occurredAt,
            string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.VoluntaryChange, occurredAt);

            var predecessors = predecessorItemIds.Distinct().ToArray();
            var successors = successorItemIds.Distinct().ToArray();

            foreach (var successorId in successors)
                RequireItem(successorId);

            foreach (var predecessorId in predecessors)
            {
                var item = RequireItem(predecessorId);

                foreach (var entitlement in item.Entitlements.Where(entitlement => !entitlement.IsTerminal))
                    entitlement.TransitionTo(EntitlementCommercialStatus.Replaced, occurredAt);

                item.MarkReplaced(occurredAt);
            }

            BumpVersion();
            Causes(new OrderItemsReplaced(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                predecessors,
                successors));
        }

        public void ApplyPartialChange(
            OrderItemId orderItemId,
            IEnumerable<EntitlementId> cancelledEntitlementIds,
            IEnumerable<EntitlementId> replacedEntitlementIds,
            ActorContext actor,
            Instant occurredAt,
            string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.VoluntaryChange, occurredAt);

            var item = RequireItem(orderItemId);

            foreach (var entitlementId in cancelledEntitlementIds.Distinct())
            {
                var entitlement = item.FindEntitlement(entitlementId)
                    ?? throw ExceptionFactory.EntitlementNotFound(entitlementId);
                entitlement.TransitionTo(EntitlementCommercialStatus.Cancelled, occurredAt);
            }

            foreach (var entitlementId in replacedEntitlementIds.Distinct())
            {
                var entitlement = item.FindEntitlement(entitlementId)
                    ?? throw ExceptionFactory.EntitlementNotFound(entitlementId);
                entitlement.TransitionTo(EntitlementCommercialStatus.Replaced, occurredAt);
            }

            item.RecomputeStatus(occurredAt);

            BumpVersion();
            Causes(new OrderItemPartiallyChanged(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, orderItemId));
        }

        public void AttachFulfillmentLink(
            EntitlementId entitlementId,
            FulfillmentLinkType fulfillmentType,
            Guid fulfillmentAggregateId,
            Guid? fulfillmentUnitId,
            string? externalReference,
            ActorContext actor,
            Instant occurredAt)
        {
            EnsureMutable(actor, ServicingAuthorityType.AddAncillary, occurredAt);

            RequireEntitlement(entitlementId).AttachFulfillmentLink(FulfillmentLink.Create(
                entitlementId,
                fulfillmentType,
                fulfillmentAggregateId,
                fulfillmentUnitId,
                externalReference,
                occurredAt));

            BumpVersion();
        }

        // ---------------------------------------------------------------- authority and references

        public ServicingDelegationId GrantServicingDelegation(
            string delegatePartyRef,
            string scope,
            Instant validFrom,
            Instant? validUntil,
            IEnumerable<ServicingAuthorityType> authorityTypes,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            ServicingDelegationId? delegationId = null)
        {
            EnsureMutable(actor, ServicingAuthorityType.ManageExternalReferences, occurredAt);

            var id = delegationId ?? ServicingDelegationId.New();
            if (_delegations.Any(delegation => delegation.Id == id))
                throw ExceptionFactory.DuplicateDelegationId(id);

            _delegations.Add(ServicingDelegation.Create(id, Id, delegatePartyRef, scope, validFrom, validUntil, authorityTypes));

            BumpVersion();
            Causes(new ServicingDelegationGranted(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                id,
                delegatePartyRef));

            return id;
        }

        public void RevokeServicingDelegation(
            ServicingDelegationId delegationId,
            ActorContext actor,
            Instant occurredAt,
            string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.ManageExternalReferences, occurredAt);

            var delegation = _delegations.SingleOrDefault(candidate => candidate.Id == delegationId)
                ?? throw ExceptionFactory.DelegationNotFound(delegationId);

            delegation.Revoke(occurredAt);

            BumpVersion();
            Causes(new ServicingDelegationRevoked(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, delegationId));
        }

        public ExternalReferenceId AddExternalReference(
            ExternalReferenceScope scope,
            Guid? scopedEntityId,
            string system,
            string type,
            string value,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            string? owner = null,
            ExternalReferenceId? externalReferenceId = null)
        {
            EnsureMutable(actor, ServicingAuthorityType.ManageExternalReferences, occurredAt);

            if (_externalReferences.Any(reference => reference.Matches(system, type, value, owner)))
                throw ExceptionFactory.DuplicateExternalReference(system, type, value);

            EnsureScopedEntityExists(scope, scopedEntityId);

            var id = externalReferenceId ?? ExternalReferenceId.New();
            _externalReferences.Add(ExternalReference.Create(id, Id, scope, scopedEntityId, system, type, value, owner));

            BumpVersion();
            Causes(new ExternalReferenceAdded(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                id,
                system,
                type,
                value));

            return id;
        }

        public void RemoveExternalReference(
            ExternalReferenceId externalReferenceId,
            ActorContext actor,
            Instant occurredAt,
            string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.ManageExternalReferences, occurredAt);

            var reference = _externalReferences.SingleOrDefault(candidate => candidate.Id == externalReferenceId)
                ?? throw ExceptionFactory.ExternalReferenceNotFound(externalReferenceId);

            _externalReferences.Remove(reference);

            BumpVersion();
            Causes(new ExternalReferenceRemoved(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, externalReferenceId));
        }

        // ---------------------------------------------------------------- time limits

        public TimeLimitId AddTimeLimit(
            TimeLimitType type,
            Instant dueAt,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            string? policyRef = null,
            IEnumerable<OrderItemId>? orderItemRefs = null,
            IEnumerable<EntitlementId>? entitlementRefs = null,
            IEnumerable<TravelerId>? travelerRefs = null,
            TimeLimitId? timeLimitId = null)
        {
            EnsureMutable(actor, ServicingAuthorityType.ManageTimeLimits, occurredAt);

            var id = timeLimitId ?? TimeLimitId.New();
            if (_timeLimits.Any(timeLimit => timeLimit.Id == id))
                throw ExceptionFactory.DuplicateTimeLimitId(id);

            var itemRefs = orderItemRefs?.ToArray() ?? [];
            var entitlements = entitlementRefs?.ToArray() ?? [];
            var travelers = travelerRefs?.ToArray() ?? [];

            foreach (var itemId in itemRefs)
            {
                if (_items.All(item => item.Id != itemId))
                    throw ExceptionFactory.TimeLimitScopeMustExistInOrder(id, nameof(OrderItem), itemId);
            }

            foreach (var entitlementId in entitlements)
            {
                if (AllEntitlements.All(entitlement => entitlement.Id != entitlementId))
                    throw ExceptionFactory.TimeLimitScopeMustExistInOrder(id, nameof(Entitlement), entitlementId);
            }

            foreach (var travelerId in travelers)
            {
                if (_travelers.All(traveler => traveler.Id != travelerId))
                    throw ExceptionFactory.TimeLimitScopeMustExistInOrder(id, nameof(Traveler), travelerId);
            }

            _timeLimits.Add(TimeLimit.Create(id, Id, type, dueAt, occurredAt, policyRef, itemRefs, entitlements, travelers));

            BumpVersion();
            Causes(new TimeLimitAdded(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, id, type));
            return id;
        }

        public void ExtendTimeLimit(
            TimeLimitId timeLimitId,
            Instant newDueAt,
            int maximumExtensions,
            ActorContext actor,
            Instant occurredAt,
            string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.ManageTimeLimits, occurredAt);

            RequireTimeLimit(timeLimitId).Extend(newDueAt, maximumExtensions);

            BumpVersion();
            Causes(new TimeLimitExtended(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, timeLimitId));
        }

        public void MarkTimeLimitMet(TimeLimitId timeLimitId, ActorContext actor, Instant occurredAt, string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.ManageTimeLimits, occurredAt);

            RequireTimeLimit(timeLimitId).MarkMet();

            BumpVersion();
            Causes(new TimeLimitMet(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, timeLimitId));
        }

        public void ExpireTimeLimit(TimeLimitId timeLimitId, ActorContext actor, Instant occurredAt, string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.ManageTimeLimits, occurredAt);

            var timeLimit = RequireTimeLimit(timeLimitId);

            // INV-082: expiry cannot touch items covered by an active PaymentCompletion lock.
            var blockingLock = _processingLocks.FirstOrDefault(processingLock =>
                ProcessingLockPolicy.BlocksTimeLimitExpiry(processingLock.Type)
                && !processingLock.IsExpiredAt(occurredAt)
                && ScopesIntersect(timeLimit, processingLock));

            if (blockingLock is not null)
                throw ExceptionFactory.ExpiryBlockedByOpenPaymentCompletion(timeLimitId);

            timeLimit.Expire();

            BumpVersion();
            Causes(new TimeLimitExpired(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                timeLimitId,
                timeLimit.Type));
        }

        public void CancelTimeLimit(TimeLimitId timeLimitId, ActorContext actor, Instant occurredAt, string eventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.ManageTimeLimits, occurredAt);

            RequireTimeLimit(timeLimitId).Cancel();

            BumpVersion();
            Causes(new TimeLimitCancelled(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, timeLimitId));
        }

        // ---------------------------------------------------------------- processing locks

        public ProcessingLockId AcquireProcessingLock(
            WorkflowInstanceId workflowInstanceId,
            ProcessingLockType type,
            Instant expiresAt,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            IEnumerable<OrderItemId>? orderItemRefs = null,
            IEnumerable<EntitlementId>? entitlementRefs = null,
            ProcessingLockId? processingLockId = null)
        {
            EnsureNotClosed();

            var id = processingLockId ?? ProcessingLockId.New();
            if (_processingLocks.Any(processingLock => processingLock.Id == id))
                throw ExceptionFactory.DuplicateProcessingLockId(id);

            var itemRefs = orderItemRefs?.ToArray() ?? [];
            var entitlementIds = entitlementRefs?.ToArray() ?? [];

            foreach (var itemId in itemRefs)
            {
                if (_items.All(item => item.Id != itemId))
                    throw ExceptionFactory.ProcessingLockScopeMustExistInOrder(id, nameof(OrderItem), itemId);
            }

            foreach (var entitlementId in entitlementIds)
            {
                if (AllEntitlements.All(entitlement => entitlement.Id != entitlementId))
                    throw ExceptionFactory.ProcessingLockScopeMustExistInOrder(id, nameof(Entitlement), entitlementId);
            }

            var candidate = ProcessingLock.Create(id, Id, workflowInstanceId, type, occurredAt, expiresAt, itemRefs, entitlementIds);

            // INV-083: incompatible locks cannot overlap on the same scope.
            var conflicting = _processingLocks.FirstOrDefault(existing =>
                !existing.IsExpiredAt(occurredAt)
                && ProcessingLockPolicy.AreIncompatible(existing.Type, type)
                && existing.ScopeOverlaps(candidate));

            if (conflicting is not null)
                throw ExceptionFactory.ProcessingLockConflict(type, conflicting.Type, conflicting.Id);

            _processingLocks.Add(candidate);

            BumpVersion();
            Causes(new ProcessingLockAcquired(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, id, type));
            return id;
        }

        public void ReleaseProcessingLock(
            ProcessingLockId processingLockId,
            ActorContext actor,
            Instant occurredAt,
            string eventId)
        {
            EnsureNotClosed();

            var processingLock = _processingLocks.SingleOrDefault(candidate => candidate.Id == processingLockId)
                ?? throw ExceptionFactory.ProcessingLockNotFound(processingLockId);

            _processingLocks.Remove(processingLock);

            BumpVersion();
            Causes(new ProcessingLockReleased(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id, processingLockId));
        }

        // ---------------------------------------------------------------- split and lifecycle

        public void ValidateSplit(IEnumerable<TravelerId> travelerIds, Instant occurredAt)
        {
            var movingTravelers = travelerIds.Distinct().ToArray();

            if (movingTravelers.Length == 0)
                throw ExceptionFactory.SplitRequiresTravelers();

            if (movingTravelers.Length >= _travelers.Count)
                throw ExceptionFactory.SplitCannotMoveAllTravelers();

            foreach (var travelerId in movingTravelers)
            {
                if (_travelers.All(traveler => traveler.Id != travelerId))
                    throw ExceptionFactory.SplitTravelerMustExistInOrder(travelerId);
            }

            var blockingLock = _processingLocks.FirstOrDefault(processingLock => !processingLock.IsExpiredAt(occurredAt));
            if (blockingLock is not null)
                throw ExceptionFactory.SplitBlockedByProcessingLock(blockingLock.Id);

            // INV-101: an item whose beneficiaries straddle the split cannot be divided.
            foreach (var item in _items.Where(item => !item.IsTerminal))
            {
                var beneficiaryIds = item.BeneficiaryTravelerIds.ToArray();
                var moving = beneficiaryIds.Count(movingTravelers.Contains);
                if (moving > 0 && moving != beneficiaryIds.Length)
                    throw ExceptionFactory.SplitWouldOrphanOrderItem(item.Id);
            }
        }

        public Order CompleteSplit(
            OrderId targetOrderId,
            OrderReference targetReference,
            WorkflowInstanceId splitWorkflowId,
            IEnumerable<TravelerId> travelerIds,
            ActorContext actor,
            Instant occurredAt,
            string eventId,
            string targetEventId)
        {
            EnsureMutable(actor, ServicingAuthorityType.Split, occurredAt);

            var movingTravelers = travelerIds.Distinct().ToArray();
            ValidateSplit(movingTravelers, occurredAt);

            var target = new Order(
                targetOrderId,
                targetReference,
                CreatedSalesContext,
                Buyer,
                ServicingAuthority,
                new OrderLineage(Lineage.RootOrderId, Id, Id, splitWorkflowId));

            target.Causes(new OrderCreated(
                targetEventId,
                targetOrderId.ToString(),
                occurredAt.ToDateTimeOffset(),
                target.AggregateVersion,
                targetOrderId,
                targetReference.Value,
                target.Lineage.RootOrderId,
                target.Lineage.ParentOrderId,
                CreatedSalesContext.SellerId,
                CreatedSalesContext.ChannelCode));

            // INV-100 and the Vernon id rule: globally unique GUIDs are retained when moved.
            foreach (var traveler in _travelers.Where(traveler => movingTravelers.Contains(traveler.Id)).ToArray())
            {
                _travelers.Remove(traveler);
                target._travelers.Add(traveler);
            }

            foreach (var item in _items.Where(item => item.BeneficiaryTravelerIds.Any(movingTravelers.Contains)).ToArray())
            {
                _items.Remove(item);
                item.ReassignToOrder(targetOrderId);
                target._items.Add(item);
            }

            foreach (var journey in _journeys)
                target._journeys.Add(journey);

            foreach (var reference in _externalReferences
                         .Where(reference => reference.Scope is ExternalReferenceScope.Traveler
                             && reference.ScopedEntityId is not null
                             && movingTravelers.Any(travelerId => travelerId.Value == reference.ScopedEntityId))
                         .ToArray())
            {
                _externalReferences.Remove(reference);
                reference.ReassignToOrder(targetOrderId);
                target._externalReferences.Add(reference);
            }

            BumpVersion();
            Causes(new OrderSplitCompleted(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                targetOrderId,
                movingTravelers));

            return target;
        }

        public void CloseOrder(ActorContext actor, Instant occurredAt, string eventId)
        {
            EnsureAuthority(actor, ServicingAuthorityType.Cancel, occurredAt);

            if (ClosedAt is not null)
                throw ExceptionFactory.OrderAlreadyClosed();

            if (AllEntitlements.Any(entitlement => entitlement.IsActive) || _timeLimits.Any(timeLimit => timeLimit.IsActive))
                throw ExceptionFactory.OrderCannotCloseWithActiveContent();

            ClosedAt = occurredAt;

            BumpVersion();
            Causes(new OrderClosed(eventId, Id.ToString(), occurredAt.ToDateTimeOffset(), AggregateVersion, Id));
        }

        // ---------------------------------------------------------------- invariant checks

        /// <summary>Re-evaluates the structural invariants that span more than one child collection.</summary>
        public void CheckInvariants()
        {
            foreach (var traveler in _travelers)
            {
                // INV-007: an infant needs an AssociatedAdult in the same order.
                if (traveler.Type is TravelerType.INF)
                {
                    var hasAdult = traveler.Associations.Any(association =>
                        association.AssociationType is TravelerAssociationType.AssociatedAdult
                        && _travelers.Any(candidate => candidate.Id == association.RelatedTravelerId));

                    if (!hasAdult)
                        throw ExceptionFactory.InfantRequiresAssociatedAdultTraveler(traveler.Id);
                }

                foreach (var association in traveler.Associations)
                {
                    if (_travelers.All(candidate => candidate.Id != association.RelatedTravelerId))
                        throw ExceptionFactory.AssociatedTravelerMustExistInOrder(traveler.Id, association.RelatedTravelerId);
                }
            }

            var segmentIds = AllSegments.Select(segment => segment.Id).ToHashSet();
            var journeyIds = _journeys.Select(journey => journey.Id).ToHashSet();
            var travelerIds = _travelers.Select(traveler => traveler.Id).ToHashSet();

            foreach (var entitlement in AllEntitlements)
            {
                // INV-003
                foreach (var beneficiaryId in entitlement.BeneficiaryTravelerIds)
                {
                    if (!travelerIds.Contains(beneficiaryId))
                        throw ExceptionFactory.EntitlementBeneficiaryMustBeOrderTraveler(entitlement.Id, beneficiaryId);
                }

                // INV-004
                foreach (var segmentId in entitlement.SegmentIds)
                {
                    if (!segmentIds.Contains(segmentId))
                        throw ExceptionFactory.EntitlementSegmentMustExistInOrder(entitlement.Id, segmentId);
                }

                // INV-005
                if (entitlement.JourneyRef is { } journeyRef)
                {
                    if (!journeyIds.Contains(journeyRef))
                        throw ExceptionFactory.EntitlementJourneyMustExistInOrder(entitlement.Id, journeyRef);

                    // INV-044: journey-scoped segments must belong to the referenced journey.
                    var journeySegmentIds = RequireJourney(journeyRef).SegmentRefs.ToHashSet();
                    if (entitlement.SegmentIds.Any(segmentId => !journeySegmentIds.Contains(segmentId)))
                        throw ExceptionFactory.JourneyScopedSegmentsMustBelongToJourney(entitlement.Id, journeyRef);
                }
            }

            // INV-006
            foreach (var journey in _journeys.Where(journey => !journey.HasContiguousSegmentSequence()))
                throw ExceptionFactory.JourneySegmentSequenceIsInvalid(journey.Id);

            // INV-027: an active entitlement belongs to exactly one order item.
            var activeEntitlementIds = AllEntitlements.Where(entitlement => entitlement.IsActive).Select(entitlement => entitlement.Id);
            var duplicated = activeEntitlementIds.GroupBy(entitlementId => entitlementId).FirstOrDefault(group => group.Count() > 1);
            if (duplicated is not null)
                throw ExceptionFactory.DuplicateEntitlementId(duplicated.Key);
        }

        // ---------------------------------------------------------------- helpers

        private Entitlement BuildEntitlement(OrderItemId orderItemId, EntitlementDraft draft, Instant occurredAt)
        {
            var id = draft.EntitlementId ?? EntitlementId.New();

            if (AllEntitlements.Any(entitlement => entitlement.Id == id))
                throw ExceptionFactory.DuplicateEntitlementId(id);

            foreach (var beneficiaryId in draft.BeneficiaryRefs)
            {
                if (_travelers.All(traveler => traveler.Id != beneficiaryId))
                    throw ExceptionFactory.EntitlementBeneficiaryMustBeOrderTraveler(id, beneficiaryId);
            }

            foreach (var segmentId in draft.Applicability.SegmentRefs)
            {
                if (AllSegments.All(segment => segment.Id != segmentId))
                    throw ExceptionFactory.EntitlementSegmentMustExistInOrder(id, segmentId);
            }

            if (draft.Applicability.JourneyRef is { } journeyRef && _journeys.All(journey => journey.Id != journeyRef))
                throw ExceptionFactory.EntitlementJourneyMustExistInOrder(id, journeyRef);

            return Entitlement.Create(
                id,
                Id,
                orderItemId,
                draft.Type,
                draft.BeneficiaryRefs,
                draft.Applicability,
                draft.Specification,
                occurredAt,
                draft.CapacityCommitment,
                draft.Responsibility);
        }

        private void DemoteExistingPrimaryContacts(ContactRole role, ContactType type, ContactId? excluding = null)
        {
            foreach (var contact in _contacts.Where(contact =>
                         contact.IsPrimary
                         && contact.Role == role
                         && contact.Type == type
                         && (excluding is null || contact.Id != excluding)))
            {
                contact.Demote();
            }
        }

        private void EnsureScopedEntityExists(ExternalReferenceScope scope, Guid? scopedEntityId)
        {
            if (scope is ExternalReferenceScope.Order or ExternalReferenceScope.Fulfillment)
                return;

            if (scopedEntityId is null)
                throw ExceptionFactory.ExternalReferenceScopeTargetMissing(scope);

            var exists = scope switch
            {
                ExternalReferenceScope.Traveler => _travelers.Any(traveler => traveler.Id.Value == scopedEntityId),
                ExternalReferenceScope.OrderItem => _items.Any(item => item.Id.Value == scopedEntityId),
                ExternalReferenceScope.Entitlement => AllEntitlements.Any(entitlement => entitlement.Id.Value == scopedEntityId),
                _ => true
            };

            if (!exists)
                throw ExceptionFactory.ExternalReferenceScopeTargetMissing(scope);
        }

        private static bool ScopesIntersect(TimeLimit timeLimit, ProcessingLock processingLock)
        {
            if (timeLimit.IsOrderWide || processingLock.IsOrderWide)
                return true;

            return timeLimit.OrderItemIds.Any(processingLock.CoversItem)
                || timeLimit.EntitlementIds.Any(processingLock.CoversEntitlement);
        }

        private void BumpVersion() => AggregateVersion++;

        private void EnsureMutable(ActorContext actor, ServicingAuthorityType authorityType, Instant occurredAt)
        {
            EnsureNotClosed();
            EnsureAuthority(actor, authorityType, occurredAt);
        }

        private void EnsureNotClosed()
        {
            if (ClosedAt is not null)
                throw ExceptionFactory.OrderIsClosed();
        }

        // INV-061: mutation requires seller/office authority, a valid delegation, or airline override.
        private void EnsureAuthority(ActorContext actor, ServicingAuthorityType authorityType, Instant occurredAt)
        {
            if (actor.HasAirlineOverride && ServicingAuthority.AirlineOverrideAllowed)
                return;

            if (actor.SellerId is not null && actor.SellerId == ServicingAuthority.OwnerSellerId)
                return;

            if (actor.SellerId is not null && _delegations.Any(delegation =>
                    delegation.DelegatePartyRef == actor.SellerId
                    && delegation.IsValidAt(occurredAt)
                    && delegation.Grants(authorityType)))
            {
                return;
            }

            throw ExceptionFactory.ActorLacksServicingAuthority();
        }

        private Traveler RequireTraveler(TravelerId travelerId) =>
            _travelers.SingleOrDefault(traveler => traveler.Id == travelerId)
            ?? throw ExceptionFactory.TravelerNotFound(travelerId);

        private Journey RequireJourney(JourneyId journeyId) =>
            _journeys.SingleOrDefault(journey => journey.Id == journeyId)
            ?? throw ExceptionFactory.JourneyNotFound(journeyId);

        private JourneySegment RequireSegment(JourneySegmentId segmentId) =>
            AllSegments.SingleOrDefault(segment => segment.Id == segmentId)
            ?? throw ExceptionFactory.JourneySegmentNotFound(segmentId);

        private OrderItem RequireItem(OrderItemId orderItemId) =>
            _items.SingleOrDefault(item => item.Id == orderItemId)
            ?? throw ExceptionFactory.OrderItemNotFound(orderItemId);

        private Entitlement RequireEntitlement(EntitlementId entitlementId) =>
            AllEntitlements.SingleOrDefault(entitlement => entitlement.Id == entitlementId)
            ?? throw ExceptionFactory.EntitlementNotFound(entitlementId);

        private TimeLimit RequireTimeLimit(TimeLimitId timeLimitId) =>
            _timeLimits.SingleOrDefault(timeLimit => timeLimit.Id == timeLimitId)
            ?? throw ExceptionFactory.TimeLimitNotFound(timeLimitId);
    }

    public sealed record ChargeLineDraft(
        int Sequence,
        ChargeType Type,
        string Code,
        Money Amount,
        bool Refundable,
        string? Description = null,
        string? TaxJurisdiction = null);

    public sealed record EntitlementDraft(
        EntitlementType Type,
        IReadOnlyCollection<TravelerId> BeneficiaryRefs,
        Applicability Applicability,
        EntitlementSpecification Specification,
        CapacityCommitmentRef? CapacityCommitment = null,
        ResponsibilityAssignment? Responsibility = null,
        EntitlementId? EntitlementId = null);

    public sealed record ValueAllocationDraft(
        EntitlementId EntitlementId,
        Money Amount,
        string AllocationVersion,
        AllocationPurpose Purpose,
        JourneySegmentId? JourneySegmentId = null,
        TravelerId? TravelerId = null);
}

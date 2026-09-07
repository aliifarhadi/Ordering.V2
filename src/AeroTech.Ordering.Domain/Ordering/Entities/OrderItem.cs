using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.Ordering.StateMachines;
using AeroTech.Ordering.Domain.Ordering.ValueObjects;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class OrderItem : Entity<OrderItemId>
    {
        private readonly List<Beneficiary> _beneficiaries = [];
        private readonly List<Entitlement> _entitlements = [];
        private readonly List<ChargeLine> _chargeLines = [];
        private readonly List<ValueAllocation> _valueAllocations = [];
        private readonly List<ItemLineagePredecessor> _predecessorItemIds = [];

        private OrderItem()
        {
        }

        private OrderItem(
            OrderItemId id,
            OrderId orderId,
            ProductSnapshot product,
            PriceSnapshot price,
            CommercialTermsSnapshot commercialTerms,
            CommercialSource commercialSource,
            SalesContext itemSalesContext,
            SettlementArrangement settlement,
            ItemLineage lineage,
            Instant createdAt)
        {
            Id = id;
            OrderId = orderId;
            Product = product;
            Price = price;
            CommercialTerms = commercialTerms;
            CommercialSource = commercialSource;
            ItemSalesContext = itemSalesContext;
            Settlement = settlement;
            LineageChangeId = lineage.ChangeId;
            Status = OrderItemCommercialStatus.Pending;
            CreatedAt = createdAt;

            _predecessorItemIds.AddRange(
                lineage.PredecessorItemIds.Select(predecessorId => new ItemLineagePredecessor(id, predecessorId)));
        }

        public OrderId OrderId { get; private set; }

        public ProductSnapshot Product { get; private set; } = null!;

        public PriceSnapshot Price { get; private set; } = null!;

        public CommercialTermsSnapshot CommercialTerms { get; private set; } = null!;

        public CommercialSource CommercialSource { get; private set; } = null!;

        public SalesContext ItemSalesContext { get; private set; } = null!;

        public SettlementArrangement Settlement { get; private set; } = null!;

        public OrderItemCommercialStatus Status { get; private set; }

        public ChangeId? LineageChangeId { get; private set; }

        public Instant CreatedAt { get; private set; }

        public Instant? CancelledAt { get; private set; }

        public Instant? ReplacedAt { get; private set; }

        public IReadOnlyCollection<Beneficiary> Beneficiaries => _beneficiaries.AsReadOnly();

        public IReadOnlyCollection<Entitlement> Entitlements => _entitlements.AsReadOnly();

        public IReadOnlyCollection<ChargeLine> ChargeLines => _chargeLines.AsReadOnly();

        public IReadOnlyCollection<ValueAllocation> ValueAllocations => _valueAllocations.AsReadOnly();

        public IReadOnlyCollection<ItemLineagePredecessor> PredecessorItemIds => _predecessorItemIds.AsReadOnly();

        public ItemLineage Lineage =>
            new(LineageChangeId, _predecessorItemIds.Select(predecessor => predecessor.PredecessorOrderItemId));

        public IEnumerable<TravelerId> BeneficiaryTravelerIds => _beneficiaries.Select(beneficiary => beneficiary.TravelerId);

        public bool IsTerminal => OrderItemStateMachine.IsTerminal(Status);

        internal static OrderItem Create(
            OrderItemId id,
            OrderId orderId,
            ProductSnapshot product,
            PriceSnapshot price,
            CommercialTermsSnapshot commercialTerms,
            CommercialSource commercialSource,
            SalesContext itemSalesContext,
            SettlementArrangement settlement,
            IEnumerable<Beneficiary> beneficiaries,
            IEnumerable<ChargeLine> chargeLines,
            ItemLineage lineage,
            Instant createdAt)
        {
            var item = new OrderItem(
                id,
                orderId,
                product,
                price,
                commercialTerms,
                commercialSource,
                itemSalesContext,
                settlement,
                lineage,
                createdAt);

            item._beneficiaries.AddRange(beneficiaries);

            foreach (var chargeLine in chargeLines)
            {
                if (item._chargeLines.Any(line => line.Sequence == chargeLine.Sequence))
                    throw ExceptionFactory.DuplicateChargeLineSequence(id, chargeLine.Sequence);

                if (chargeLine.Currency != price.Currency)
                    throw ExceptionFactory.ChargeLineCurrencyMustMatchPrice(chargeLine.Id);

                item._chargeLines.Add(chargeLine);
            }

            // INV-023: charge lines must sum exactly to the price total.
            if (item._chargeLines.Count == 0)
                throw ExceptionFactory.PriceRequiresAtLeastOneChargeLine();

            var chargeTotal = Money.Sum(item._chargeLines.Select(line => line.Money), price.Currency);
            if (chargeTotal != price.Total)
                throw ExceptionFactory.ChargeLinesMustSumToPriceTotal(id);

            return item;
        }

        internal void AddEntitlement(Entitlement entitlement)
        {
            if (_entitlements.Any(existing => existing.Id == entitlement.Id))
                throw ExceptionFactory.DuplicateEntitlementId(entitlement.Id);

            // The union of entitlement beneficiaries must be a subset of item beneficiaries.
            foreach (var beneficiaryId in entitlement.BeneficiaryTravelerIds)
            {
                if (!_beneficiaries.Any(beneficiary => beneficiary.TravelerId == beneficiaryId))
                    throw ExceptionFactory.ItemBeneficiaryMustBeOrderTraveler(Id, beneficiaryId);
            }

            _entitlements.Add(entitlement);
        }

        internal void SetValueAllocations(IEnumerable<ValueAllocation> allocations)
        {
            var materialized = allocations.ToArray();

            foreach (var allocation in materialized)
            {
                if (allocation.Currency != Price.Currency)
                    throw ExceptionFactory.AllocationCurrencyMustMatchPrice(allocation.Id);

                if (!_entitlements.Any(entitlement => entitlement.Id == allocation.EntitlementId))
                    throw ExceptionFactory.AllocationEntitlementMustBelongToItem(allocation.EntitlementId, Id);
            }

            if (materialized.Length > 0)
            {
                // INV-024: required allocations must sum exactly to the price total.
                var allocationTotal = Money.Sum(materialized.Select(allocation => allocation.Money), Price.Currency);
                if (allocationTotal != Price.Total)
                    throw ExceptionFactory.ValueAllocationsMustSumToPriceTotal(Id);
            }

            _valueAllocations.Clear();
            _valueAllocations.AddRange(materialized);
        }

        internal Entitlement? FindEntitlement(EntitlementId entitlementId) =>
            _entitlements.SingleOrDefault(entitlement => entitlement.Id == entitlementId);

        internal void Confirm(Instant occurredAt)
        {
            // INV-026: a confirmed item must contain at least one entitlement.
            if (_entitlements.Count == 0)
                throw ExceptionFactory.ConfirmedItemRequiresEntitlement(Id);

            // INV-025: multi-segment AirTransport requires complete allocations before fulfilment.
            if (RequiresValueAllocations() && _valueAllocations.Count == 0)
                throw ExceptionFactory.MultiSegmentAirTransportRequiresAllocations(Id);

            foreach (var entitlement in _entitlements.Where(entitlement => entitlement.Status is EntitlementCommercialStatus.Pending))
                entitlement.Activate(occurredAt);

            RecomputeStatus(occurredAt);
        }

        internal void RecomputeStatus(Instant occurredAt)
        {
            if (_entitlements.Count == 0)
                return;

            var target = DeriveStatus();
            if (target == Status)
                return;

            if (!OrderItemStateMachine.CanTransition(Status, target))
                throw ExceptionFactory.OrderItemCannotTransition(Id, Status, target);

            Status = target;
            switch (target)
            {
                case OrderItemCommercialStatus.Cancelled:
                    CancelledAt = occurredAt;
                    break;
                case OrderItemCommercialStatus.Replaced:
                    ReplacedAt = occurredAt;
                    break;
            }
        }

        internal void MarkCancelled(Instant occurredAt)
        {
            if (IsTerminal)
                throw ExceptionFactory.OrderItemIsTerminal(Id, Status);

            if (!OrderItemStateMachine.CanTransition(Status, OrderItemCommercialStatus.Cancelled))
                throw ExceptionFactory.OrderItemCannotTransition(Id, Status, OrderItemCommercialStatus.Cancelled);

            Status = OrderItemCommercialStatus.Cancelled;
            CancelledAt = occurredAt;
        }

        internal void MarkReplaced(Instant occurredAt)
        {
            if (IsTerminal)
                throw ExceptionFactory.OrderItemIsTerminal(Id, Status);

            if (!OrderItemStateMachine.CanTransition(Status, OrderItemCommercialStatus.Replaced))
                throw ExceptionFactory.OrderItemCannotTransition(Id, Status, OrderItemCommercialStatus.Replaced);

            Status = OrderItemCommercialStatus.Replaced;
            ReplacedAt = occurredAt;
        }

        internal void ReassignToOrder(OrderId orderId)
        {
            OrderId = orderId;
            foreach (var entitlement in _entitlements)
                entitlement.ReassignToOrder(orderId);
        }

        internal bool RequiresValueAllocations() =>
            Product.ProductType is ProductType.AirTransport
            && _entitlements.Any(entitlement => entitlement.SegmentRefs.Count > 1);

        private OrderItemCommercialStatus DeriveStatus()
        {
            var statuses = _entitlements.Select(entitlement => entitlement.Status).ToArray();

            if (statuses.All(status => status is EntitlementCommercialStatus.Pending))
                return OrderItemCommercialStatus.Pending;
            if (statuses.All(status => status is EntitlementCommercialStatus.Cancelled))
                return OrderItemCommercialStatus.Cancelled;
            if (statuses.All(status => status is EntitlementCommercialStatus.Replaced))
                return OrderItemCommercialStatus.Replaced;
            if (statuses.All(status => status is EntitlementCommercialStatus.Active))
                return OrderItemCommercialStatus.Confirmed;

            var hasActive = statuses.Any(status => status is EntitlementCommercialStatus.Active);
            var hasTerminal = statuses.Any(status =>
                status is EntitlementCommercialStatus.Cancelled or EntitlementCommercialStatus.Replaced);

            if (hasActive && hasTerminal)
                return OrderItemCommercialStatus.PartiallyChanged;

            if (statuses.All(status =>
                    status is EntitlementCommercialStatus.Cancelled or EntitlementCommercialStatus.Replaced))
                return OrderItemCommercialStatus.Cancelled;

            if (hasActive)
                return OrderItemCommercialStatus.Confirmed;

            throw ExceptionFactory.OrderItemStatusIsIndeterminate(Id);
        }
    }

    public sealed record Beneficiary(OrderItemId OrderItemId, TravelerId TravelerId, string? Role = null);

    public sealed record ItemLineagePredecessor(OrderItemId OrderItemId, OrderItemId PredecessorOrderItemId);
}

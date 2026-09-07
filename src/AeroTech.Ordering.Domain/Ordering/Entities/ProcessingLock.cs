using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class ProcessingLock : Entity<ProcessingLockId>
    {
        private readonly List<ProcessingLockItemRef> _orderItemRefs = [];
        private readonly List<ProcessingLockEntitlementRef> _entitlementRefs = [];

        private ProcessingLock()
        {
        }

        private ProcessingLock(
            ProcessingLockId id,
            OrderId orderId,
            WorkflowInstanceId workflowInstanceId,
            ProcessingLockType type,
            Instant acquiredAt,
            Instant expiresAt)
        {
            Id = id;
            OrderId = orderId;
            WorkflowInstanceId = workflowInstanceId;
            Type = type;
            AcquiredAt = acquiredAt;
            ExpiresAt = expiresAt;
        }

        public OrderId OrderId { get; private set; }

        public WorkflowInstanceId WorkflowInstanceId { get; private set; }

        public ProcessingLockType Type { get; private set; }

        public Instant AcquiredAt { get; private set; }

        public Instant ExpiresAt { get; private set; }

        public IReadOnlyCollection<ProcessingLockItemRef> OrderItemRefs => _orderItemRefs.AsReadOnly();

        public IReadOnlyCollection<ProcessingLockEntitlementRef> EntitlementRefs => _entitlementRefs.AsReadOnly();

        public IEnumerable<OrderItemId> OrderItemIds => _orderItemRefs.Select(reference => reference.OrderItemId);

        public IEnumerable<EntitlementId> EntitlementIds => _entitlementRefs.Select(reference => reference.EntitlementId);

        public bool IsOrderWide => _orderItemRefs.Count == 0 && _entitlementRefs.Count == 0;

        public bool IsExpiredAt(Instant instant) => instant >= ExpiresAt;

        internal static ProcessingLock Create(
            ProcessingLockId id,
            OrderId orderId,
            WorkflowInstanceId workflowInstanceId,
            ProcessingLockType type,
            Instant acquiredAt,
            Instant expiresAt,
            IEnumerable<OrderItemId>? orderItemRefs = null,
            IEnumerable<EntitlementId>? entitlementRefs = null)
        {
            if (expiresAt <= acquiredAt)
                throw ExceptionFactory.ProcessingLockExpiryMustBeLater();

            var processingLock = new ProcessingLock(id, orderId, workflowInstanceId, type, acquiredAt, expiresAt);

            if (orderItemRefs is not null)
                processingLock._orderItemRefs.AddRange(orderItemRefs.Distinct().Select(itemId => new ProcessingLockItemRef(id, itemId)));
            if (entitlementRefs is not null)
                processingLock._entitlementRefs.AddRange(entitlementRefs.Distinct().Select(entitlementId => new ProcessingLockEntitlementRef(id, entitlementId)));

            return processingLock;
        }

        internal bool ScopeOverlaps(ProcessingLock other)
        {
            if (IsOrderWide || other.IsOrderWide)
                return true;

            return OrderItemIds.Intersect(other.OrderItemIds).Any()
                || EntitlementIds.Intersect(other.EntitlementIds).Any();
        }

        internal bool CoversItem(OrderItemId orderItemId) =>
            IsOrderWide || OrderItemIds.Contains(orderItemId);

        internal bool CoversEntitlement(EntitlementId entitlementId) =>
            IsOrderWide || EntitlementIds.Contains(entitlementId);
    }

    public sealed record ProcessingLockItemRef(ProcessingLockId ProcessingLockId, OrderItemId OrderItemId);

    public sealed record ProcessingLockEntitlementRef(ProcessingLockId ProcessingLockId, EntitlementId EntitlementId);
}

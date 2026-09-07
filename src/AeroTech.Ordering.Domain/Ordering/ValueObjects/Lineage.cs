using AeroTech.Ordering.Domain.SharedKernel.Identifiers;

namespace AeroTech.Ordering.Domain.Ordering.ValueObjects
{
    public sealed record OrderLineage
    {
        private OrderLineage()
        {
        }

        public OrderLineage(
            OrderId rootOrderId,
            OrderId? parentOrderId = null,
            OrderId? splitFromOrderId = null,
            WorkflowInstanceId? splitWorkflowId = null)
        {
            RootOrderId = rootOrderId;
            ParentOrderId = parentOrderId;
            SplitFromOrderId = splitFromOrderId;
            SplitWorkflowId = splitWorkflowId;
        }

        public OrderId RootOrderId { get; private set; }

        public OrderId? ParentOrderId { get; private set; }

        public OrderId? SplitFromOrderId { get; private set; }

        public WorkflowInstanceId? SplitWorkflowId { get; private set; }

        public bool IsSplitTarget => SplitFromOrderId.HasValue;

        public static OrderLineage ForRoot(OrderId orderId) => new(orderId);
    }

    public sealed record ItemLineage
    {
        private readonly IReadOnlyList<OrderItemId> _predecessorItemIds;

        public ItemLineage(ChangeId? changeId = null, IEnumerable<OrderItemId>? predecessorItemIds = null)
        {
            ChangeId = changeId;
            _predecessorItemIds = predecessorItemIds?.Distinct().ToArray() ?? [];
        }

        public ChangeId? ChangeId { get; }

        public IReadOnlyList<OrderItemId> PredecessorItemIds => _predecessorItemIds;

        public bool HasPredecessors => _predecessorItemIds.Count > 0;

        public static ItemLineage Original() => new();
    }
}

using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;

namespace AeroTech.Ordering.Persistence.Ordering.History
{
    /// <summary>
    /// Append-only audit row written in the same SQL transaction as the accepted Order mutation.
    /// This is a persistence concern, not part of the Order aggregate.
    /// </summary>
    public sealed class OrderHistoryEntry
    {
        private OrderHistoryEntry()
        {
        }

        public OrderId OrderId { get; private set; }

        public long SequenceNo { get; private set; }

        public long AggregateVersion { get; private set; }

        public DateTime OccurredAtUtc { get; private set; }

        public string CommandName { get; private set; } = null!;

        public ActorType ActorType { get; private set; }

        public string UserId { get; private set; } = null!;

        public string? SellerId { get; private set; }

        public string? SellerOfficeId { get; private set; }

        public string Channel { get; private set; } = null!;

        public string CorrelationId { get; private set; } = null!;

        public string? CausationId { get; private set; }

        public string ChangeSummary { get; private set; } = null!;

        public string? ChangeJson { get; private set; }

        public static OrderHistoryEntry Create(
            OrderId orderId,
            long sequenceNo,
            long aggregateVersion,
            DateTime occurredAtUtc,
            string commandName,
            ActorType actorType,
            string userId,
            string channel,
            string correlationId,
            string changeSummary,
            string? sellerId = null,
            string? sellerOfficeId = null,
            string? causationId = null,
            string? changeJson = null) =>
            new()
            {
                OrderId = orderId,
                SequenceNo = sequenceNo,
                AggregateVersion = aggregateVersion,
                OccurredAtUtc = occurredAtUtc,
                CommandName = commandName,
                ActorType = actorType,
                UserId = userId,
                Channel = channel,
                CorrelationId = correlationId,
                ChangeSummary = changeSummary,
                SellerId = sellerId,
                SellerOfficeId = sellerOfficeId,
                CausationId = causationId,
                ChangeJson = changeJson
            };
    }

    /// <summary>Optional point-in-time reconstruction accelerator. Never the current write model.</summary>
    public sealed class OrderSnapshot
    {
        private OrderSnapshot()
        {
        }

        public OrderId OrderId { get; private set; }

        public long AggregateVersion { get; private set; }

        public DateTime CapturedAtUtc { get; private set; }

        public string SnapshotJson { get; private set; } = null!;

        public static OrderSnapshot Create(
            OrderId orderId,
            long aggregateVersion,
            DateTime capturedAtUtc,
            string snapshotJson) =>
            new()
            {
                OrderId = orderId,
                AggregateVersion = aggregateVersion,
                CapturedAtUtc = capturedAtUtc,
                SnapshotJson = snapshotJson
            };
    }
}

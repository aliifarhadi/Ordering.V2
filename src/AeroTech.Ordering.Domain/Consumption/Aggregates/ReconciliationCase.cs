using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.Consumption.Aggregates
{
    public sealed class ReconciliationCase : AggregateRoot<ReconciliationCaseId>
    {
        private static readonly Dictionary<ReconciliationCaseStatus, ReconciliationCaseStatus[]> Allowed = new()
        {
            [ReconciliationCaseStatus.Open] =
            [
                ReconciliationCaseStatus.AwaitingCommercialization,
                ReconciliationCaseStatus.AwaitingManualReview,
                ReconciliationCaseStatus.Resolved,
                ReconciliationCaseStatus.Ignored
            ],
            [ReconciliationCaseStatus.AwaitingCommercialization] =
            [
                ReconciliationCaseStatus.AwaitingManualReview,
                ReconciliationCaseStatus.Resolved,
                ReconciliationCaseStatus.Ignored
            ],
            [ReconciliationCaseStatus.AwaitingManualReview] =
            [
                ReconciliationCaseStatus.AwaitingCommercialization,
                ReconciliationCaseStatus.Resolved,
                ReconciliationCaseStatus.Ignored
            ],
            [ReconciliationCaseStatus.Resolved] = [],
            [ReconciliationCaseStatus.Ignored] = []
        };

        private ReconciliationCase()
        {
        }

        private ReconciliationCase(
            ReconciliationCaseId id,
            OrderId orderId,
            ReconciliationCaseType type,
            Instant openedAt)
        {
            Id = id;
            OrderId = orderId;
            Type = type;
            Status = ReconciliationCaseStatus.Open;
            OpenedAt = openedAt;
            AggregateVersion = 1;
        }

        public OrderId OrderId { get; private set; }

        public TravelerId? TravelerId { get; private set; }

        public EntitlementId? EntitlementId { get; private set; }

        public ReconciliationCaseType Type { get; private set; }

        public ReconciliationCaseStatus Status { get; private set; }

        public decimal? ExpectedValue { get; private set; }

        public decimal? ObservedValue { get; private set; }

        public decimal? Difference { get; private set; }

        public string? ValueUnit { get; private set; }

        public ReconciliationClassification? Classification { get; private set; }

        public WorkflowInstanceId? WorkflowInstanceId { get; private set; }

        public Instant OpenedAt { get; private set; }

        public Instant? ResolvedAt { get; private set; }

        public long AggregateVersion { get; private set; }

        public static ReconciliationCase Open(
            ReconciliationCaseId id,
            OrderId orderId,
            ReconciliationCaseType type,
            Instant openedAt,
            TravelerId? travelerId = null,
            EntitlementId? entitlementId = null,
            decimal? expectedValue = null,
            decimal? observedValue = null,
            string? valueUnit = null,
            WorkflowInstanceId? workflowInstanceId = null) =>
            new(id, orderId, type, openedAt)
            {
                TravelerId = travelerId,
                EntitlementId = entitlementId,
                ExpectedValue = expectedValue,
                ObservedValue = observedValue,
                Difference = expectedValue is not null && observedValue is not null
                    ? observedValue - expectedValue
                    : null,
                ValueUnit = valueUnit,
                WorkflowInstanceId = workflowInstanceId
            };

        public void Classify(ReconciliationClassification classification)
        {
            EnsureNotTerminal();
            Classification = classification;
            AggregateVersion++;
        }

        public void MoveTo(ReconciliationCaseStatus target)
        {
            EnsureNotTerminal();

            if (!Allowed.TryGetValue(Status, out var targets) || !targets.Contains(target))
                throw ExceptionFactory.ReconciliationCaseCannotTransition(Id, Status, target);

            Status = target;
            AggregateVersion++;
        }

        public void Resolve(Instant resolvedAt)
        {
            EnsureNotTerminal();

            if (Classification is null)
                throw ExceptionFactory.ReconciliationResolutionRequiresClassification(Id);

            MoveToInternal(ReconciliationCaseStatus.Resolved);
            ResolvedAt = resolvedAt;
        }

        public void Ignore(Instant resolvedAt)
        {
            EnsureNotTerminal();
            MoveToInternal(ReconciliationCaseStatus.Ignored);
            ResolvedAt = resolvedAt;
        }

        public void AttachWorkflow(WorkflowInstanceId workflowInstanceId)
        {
            WorkflowInstanceId = workflowInstanceId;
            AggregateVersion++;
        }

        private void MoveToInternal(ReconciliationCaseStatus target)
        {
            if (!Allowed.TryGetValue(Status, out var targets) || !targets.Contains(target))
                throw ExceptionFactory.ReconciliationCaseCannotTransition(Id, Status, target);

            Status = target;
            AggregateVersion++;
        }

        private void EnsureNotTerminal()
        {
            if (Status is ReconciliationCaseStatus.Resolved or ReconciliationCaseStatus.Ignored)
                throw ExceptionFactory.ReconciliationCaseIsResolved(Id, Status);
        }
    }
}

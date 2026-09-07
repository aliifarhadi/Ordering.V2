using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.Fulfillment.Events;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.Fulfillment.Aggregates
{
    public sealed class DocumentStock : AggregateRoot<DocumentStockId>
    {
        private DocumentStock()
        {
        }

        private DocumentStock(
            DocumentStockId id,
            CarrierCode ownerCarrier,
            string? officeRef,
            DocumentStockType documentType,
            string? prefix,
            long rangeStart,
            long rangeEnd)
        {
            Id = id;
            OwnerCarrier = ownerCarrier;
            OfficeRef = officeRef;
            DocumentType = documentType;
            Prefix = prefix;
            RangeStart = rangeStart;
            RangeEnd = rangeEnd;
            NextAvailable = rangeStart;
            Status = DocumentStockStatus.Active;
            AggregateVersion = 1;
        }

        public CarrierCode OwnerCarrier { get; private set; }

        public string? OfficeRef { get; private set; }

        public DocumentStockType DocumentType { get; private set; }

        public string? Prefix { get; private set; }

        public long RangeStart { get; private set; }

        public long RangeEnd { get; private set; }

        public long NextAvailable { get; private set; }

        public DocumentStockStatus Status { get; private set; }

        public long AggregateVersion { get; private set; }

        public static DocumentStock Create(
            DocumentStockId id,
            CarrierCode ownerCarrier,
            DocumentStockType documentType,
            long rangeStart,
            long rangeEnd,
            string? officeRef = null,
            string? prefix = null)
        {
            if (rangeEnd < rangeStart)
                throw ExceptionFactory.DocumentStockRangeIsInvalid();

            return new DocumentStock(id, ownerCarrier, officeRef, documentType, prefix, rangeStart, rangeEnd);
        }

        /// <summary>
        /// Allocates the next document number. The caller must run this inside the same SQL transaction that
        /// inserts the owning document, so a number is never consumed before the document is persisted.
        /// </summary>
        public string AllocateNext(Instant occurredAt, string eventId)
        {
            if (Status is not DocumentStockStatus.Active)
                throw ExceptionFactory.DocumentStockIsNotActive(Id);

            if (NextAvailable > RangeEnd)
            {
                Status = DocumentStockStatus.Exhausted;
                throw ExceptionFactory.DocumentStockIsExhausted(Id);
            }

            var allocated = NextAvailable;
            NextAvailable++;

            if (NextAvailable > RangeEnd)
                Status = DocumentStockStatus.Exhausted;

            var documentNumber = string.IsNullOrWhiteSpace(Prefix) ? allocated.ToString() : $"{Prefix}{allocated}";

            AggregateVersion++;
            Causes(new DocumentNumberAllocated(
                eventId,
                Id.ToString(),
                occurredAt.ToDateTimeOffset(),
                AggregateVersion,
                Id,
                documentNumber));

            return documentNumber;
        }

        public void Suspend() => Status = DocumentStockStatus.Suspended;

        public void Reactivate()
        {
            if (NextAvailable > RangeEnd)
            {
                Status = DocumentStockStatus.Exhausted;
                return;
            }

            Status = DocumentStockStatus.Active;
        }

        public void Close() => Status = DocumentStockStatus.Closed;
    }
}

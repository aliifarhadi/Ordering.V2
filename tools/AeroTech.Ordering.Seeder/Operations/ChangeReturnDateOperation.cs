using AeroTech.Ordering.Domain.Fulfillment.Aggregates;
using AeroTech.Ordering.Domain.Ordering.Aggregates;
using AeroTech.Ordering.Domain.Ordering.Entities;
using AeroTech.Ordering.Domain.Ordering.Specifications;
using AeroTech.Ordering.Domain.Ordering.ValueObjects;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using AeroTech.Ordering.Seeder.Scenarios;
using NodaTime;

namespace AeroTech.Ordering.Seeder.Operations
{
    /// <summary>
    /// Voluntary reissue of one traveller's return flight to a later date, following the
    /// VoluntaryChangeProcess in the spec and the reissue/exchange model used across the industry.
    ///
    /// This is an exchange, not a revalidation: the change carries a penalty and a fare difference,
    /// so a new priced OrderItem is created rather than the existing schedule snapshot being edited.
    /// Revalidation (ApplyScheduleSnapshotChange) is only correct when fare basis, RBD and price are
    /// all unchanged.
    ///
    /// The outbound entitlement stays Active throughout — only the return leg is exchanged — so the
    /// original item lands in PartiallyChanged, matching scenario S07.
    /// </summary>
    internal sealed class ChangeReturnDateOperation
    {
        private const string PricingEngineVersion = "pricing-1.0.0";
        private const string RoundingPolicyVersion = "rounding-1.0.0";

        private readonly Instant _now;
        private readonly IEventIdFactory _eventIds;

        public ChangeReturnDateOperation(Instant now, IEventIdFactory eventIds)
        {
            _now = now;
            _eventIds = eventIds;
        }

        public ChangeResult Execute(
            Order order,
            ElectronicTicket ticket,
            IReadOnlyCollection<ElectronicTicket> travelerTickets,
            int daysLater,
            Money changePenalty,
            Money fareDifference)
        {
            var actor = new ActorContext(
                ActorType.AirlineAgent,
                "seeder-change",
                order.ServicingAuthority.OwnerSellerId,
                order.ServicingAuthority.OwnerOfficeId,
                true);

            var travelerId = ticket.TravelerId;

            // The traveller's air items, newest first: after a reissue there is more than one, and
            // the live itinerary is carried by the most recent non-terminal item.
            var airItems = order.Items
                .Where(item => item.Product.ProductType is ProductType.AirTransport
                    && item.BeneficiaryTravelerIds.Contains(travelerId))
                .OrderByDescending(item => item.CreatedAt)
                .ToArray();

            if (airItems.Length == 0)
                throw new InvalidOperationException("This traveller holds no air transport item.");

            var airItem = airItems.FirstOrDefault(candidate => !candidate.IsTerminal)
                ?? throw new InvalidOperationException(
                    $"Air item for this traveller is {airItems[0].Status} and cannot be changed. "
                    + "A cancelled or replaced entitlement can never be reinstated (INV-048).");

            var segmentsById = order.AllSegments.ToDictionary(segment => segment.Id);

            // Scan every air item the traveller holds: after one reissue the live return leg lives
            // on the newest item while the outbound still sits on the original.
            var returnEntitlement = airItems
                .SelectMany(item => item.Entitlements.Select(entitlement => new { Item = item, Entitlement = entitlement }))
                .Where(pair => pair.Entitlement.IsActive)
                .Select(pair => new
                {
                    pair.Item,
                    pair.Entitlement,
                    Segment = segmentsById[pair.Entitlement.SegmentIds.Single()]
                })
                .OrderByDescending(pair => pair.Segment.DepartureUtc)
                .FirstOrDefault()
                ?? throw new InvalidOperationException("No active return entitlement was found for this traveller.");

            // The change applies to whichever item currently owns the return leg.
            airItem = returnEntitlement.Item;

            var oldSegment = returnEntitlement.Segment;
            var oldEntitlement = returnEntitlement.Entitlement;

            // The coupon covering the return leg must still be Open: a flown or controlled coupon
            // cannot be exchanged (FUL-004, FUL-005). After an earlier reissue that coupon lives on
            // the replacement ticket, so search every document held by this traveller.
            var couponHolder = travelerTickets
                .Select(candidate => new
                {
                    Ticket = candidate,
                    Coupon = candidate.Coupons.FirstOrDefault(coupon => coupon.EntitlementId == oldEntitlement.Id)
                })
                .FirstOrDefault(pair => pair.Coupon is not null)
                ?? throw new InvalidOperationException("No ticket coupon covers the return entitlement.");

            var returnCoupon = couponHolder.Coupon!;
            var exchangingTicket = couponHolder.Ticket;

            if (returnCoupon.Status is not TicketCouponStatus.Open)
                throw new InvalidOperationException(
                    $"Return coupon #{returnCoupon.CouponNumber} on ticket {exchangingTicket.TicketNumber} "
                    + $"is {returnCoupon.Status} and cannot be exchanged.");

            // 1. Acquire the VoluntaryChange lock over the affected entitlement.
            var workflowId = WorkflowInstanceId.New();
            var lockId = order.AcquireProcessingLock(
                workflowId,
                ProcessingLockType.VoluntaryChange,
                _now.Plus(Duration.FromMinutes(30)),
                actor,
                _now,
                _eventIds.Next(),
                entitlementRefs: [oldEntitlement.Id]);

            // 2. Secure the replacement capacity before releasing the old: the new segment is added
            //    to the same return journey as a new commercial schedule snapshot.
            var newSegmentId = order.AddJourneySegment(
                oldSegment.JourneyId,
                oldSegment.MarketingCarrier,
                oldSegment.OperatingCarrier,
                oldSegment.FlightNumber,
                oldSegment.Origin,
                oldSegment.Destination,
                oldSegment.DepartureUtc.Plus(Duration.FromDays(daysLater)),
                oldSegment.ArrivalUtc.Plus(Duration.FromDays(daysLater)),
                oldSegment.DepartureLocalDate.PlusDays(daysLater),
                oldSegment.DepartureLocalTime,
                oldSegment.OriginTimeZoneId,
                oldSegment.ArrivalLocalDate.PlusDays(daysLater),
                oldSegment.ArrivalLocalTime,
                oldSegment.DestinationTimeZoneId,
                actor,
                _now,
                _eventIds.Next(),
                oldSegment.AircraftType);

            // 3. The servicing offer: penalty plus fare difference. Pricing produces these amounts;
            //    Ordering only snapshots them.
            var newTotal = changePenalty + fareDifference;
            var newEntitlementId = EntitlementId.New();

            var newItemId = order.AddOrderItem(
                new ProductSnapshot(
                    "PRD-AIR-CHG",
                    "AIR-RT-ECO-CHG",
                    ProductType.AirTransport,
                    $"Reissue: return leg moved to {oldSegment.DepartureLocalDate.PlusDays(daysLater)}",
                    $"Voluntary change of W5 {oldSegment.FlightNumber} {oldSegment.Origin}-{oldSegment.Destination}",
                    "catalog-2026.09",
                    [new ProductAttribute("ChangeType", "VoluntaryReissue")]),
                new PriceSnapshot(newTotal, _now, PricingEngineVersion, RoundingPolicyVersion),
                new CommercialTermsSnapshot(
                    "terms-2026.09",
                    "NonRefundable",
                    "ChangeableWithPenalty",
                    "NOSHOW-STD",
                    sourceRuleRefs: ["RULE-W5-ECO-CHG"]),
                // The commercial source is a ServicingOffer, not a RetailOffer: this is the audit
                // trail proving the change was priced rather than invented by the servicing agent.
                new CommercialSource(
                    CommercialSourceType.ServicingOffer,
                    $"SO-{order.Reference.Value}-CHG1",
                    "1",
                    _now),
                BuildSalesContext(order),
                new SettlementArrangement(SettlementModel.PassengerRetail, null, null, true, null),
                [travelerId],
                BuildChargeLines(changePenalty, fareDifference),
                [
                    new EntitlementDraft(
                        EntitlementType.AirTransport,
                        [travelerId],
                        new Applicability(oldSegment.JourneyId, [newSegmentId]),
                        new AirTransportSpecification("Economy", "Y", "ECOFLEX", "YRT"),
                        new CapacityCommitmentRef(
                            CapacityCommitmentType.ConfirmedInventory,
                            $"INV-{oldSegment.FlightNumber}R-{travelerId.Value:N}"),
                        EntitlementId: newEntitlementId)
                ],
                actor,
                _now,
                _eventIds.Next(),
                // Lineage: the new item supersedes the original air item (INV-103).
                lineage: new ItemLineage(ChangeId.New(), [airItem.Id]));

            order.ConfirmOrderItems([newItemId], actor, _now, _eventIds.Next());

            // 4. Withdraw the exchanged coupon, then record the commercial replacement of just the
            //    return entitlement. The outbound entitlement is untouched.
            exchangingTicket.ExchangeCoupons([returnCoupon.Id], _now, _eventIds.Next());

            order.ApplyPartialChange(
                airItem.Id,
                cancelledEntitlementIds: [],
                replacedEntitlementIds: [oldEntitlement.Id],
                actor,
                _now,
                _eventIds.Next());

            // 5. Issue the replacement document. In industry terms this is the conjunction ticket
            //    carrying the reissued coupon; the original ticket keeps its flown/open history.
            var newTicketId = ElectronicTicketId.New();
            var newTicketNumber = NextTicketNumber(exchangingTicket.TicketNumber);

            var newTicket = ElectronicTicket.Issue(
                newTicketId,
                newTicketNumber,
                order.Id,
                travelerId,
                exchangingTicket.IssuingCarrier,
                _now,
                [new TicketCouponDraft(newItemId, newEntitlementId, newSegmentId)],
                _eventIds.Next());

            order.AttachFulfillmentLink(
                newEntitlementId,
                FulfillmentLinkType.ElectronicTicket,
                newTicketId.Value,
                null,
                newTicketNumber,
                actor,
                _now);

            // 6. Release the lock and re-check the aggregate.
            order.ReleaseProcessingLock(lockId, actor, _now, _eventIds.Next());
            order.CheckInvariants();

            return new ChangeResult(
                airItem,
                order.Items.Single(item => item.Id == newItemId),
                oldSegment,
                order.AllSegments.Single(segment => segment.Id == newSegmentId),
                exchangingTicket,
                newTicket,
                newTotal);
        }

        private static IEnumerable<ChargeLineDraft> BuildChargeLines(Money changePenalty, Money fareDifference)
        {
            var lines = new List<ChargeLineDraft>
            {
                new(1, ChargeType.Penalty, "CHGFEE", changePenalty, false, "Voluntary change penalty")
            };

            // A zero fare difference still deserves no line at all rather than a zero-value line.
            if (!fareDifference.IsZero)
                lines.Add(new ChargeLineDraft(2, ChargeType.Base, "FAREDIFF", fareDifference, true, "Fare difference"));

            return lines;
        }

        private static SalesContext BuildSalesContext(Order order)
        {
            var created = order.CreatedSalesContext;

            // A fresh instance: SalesContext is a record and sharing one across owned locations
            // collapses them under EF identity resolution.
            return new SalesContext(
                created.SellerId,
                created.SellerOfficeId,
                created.ChannelCode,
                created.PointOfSaleCountry,
                created.SaleCurrency,
                created.SoldAt,
                created.ActorType,
                created.UserId);
        }

        private static string NextTicketNumber(string original)
        {
            var separator = original.LastIndexOf('-');
            if (separator < 0 || !long.TryParse(original[(separator + 1)..], out var serial))
                return original + "R";

            return $"{original[..separator]}-{serial + 100:0000000}";
        }
    }

    internal sealed record ChangeResult(
        OrderItem OriginalItem,
        OrderItem NewItem,
        JourneySegment OldSegment,
        JourneySegment NewSegment,
        ElectronicTicket OriginalTicket,
        ElectronicTicket NewTicket,
        Money AmountCollected);
}

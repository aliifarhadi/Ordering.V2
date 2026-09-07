using AeroTech.Ordering.Domain.Fulfillment.Aggregates;
using AeroTech.Ordering.Domain.Ordering.Aggregates;
using AeroTech.Ordering.Domain.Ordering.Specifications;
using AeroTech.Ordering.Domain.Ordering.ValueObjects;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Seeder.Scenarios
{
    /// <summary>
    /// THR to MHD and back for two adults on W5-1084 / W5-1085, with extra baggage on both
    /// segments and lounge access at THR, confirmed and issued as one ticket per traveller
    /// plus one EMD per traveller for the ancillaries.
    /// </summary>
    internal sealed class ThrMhdRoundTripScenario
    {
        private const string SellerId = "AIRLINE-001";
        private const string SellerOfficeId = "AIR-OFF-THR";
        private const string ChannelCode = "DirectWeb";
        private const string IssuingCarrier = "W5";
        private const string PricingEngineVersion = "pricing-1.0.0";
        private const string RoundingPolicyVersion = "rounding-1.0.0";

        private readonly Instant _now;
        private readonly IEventIdFactory _eventIds;

        public ThrMhdRoundTripScenario(Instant now, IEventIdFactory eventIds)
        {
            _now = now;
            _eventIds = eventIds;
        }

        public ScenarioResult Build(OrderReference reference)
        {
            var currency = new CurrencyCode("IRR");
            var actor = new ActorContext(ActorType.AirlineAgent, "seeder", SellerId, SellerOfficeId, true);

            // Each owned SalesContext needs its own instance: SalesContext is a record, so sharing
            // one instance across the order and its items collapses them under EF identity resolution.
            var salesContext = BuildSalesContext(currency);

            var orderId = OrderId.New();
            var order = Order.CreateDraft(
                orderId,
                reference,
                salesContext,
                new ServicingAuthority(SellerId, SellerOfficeId, true),
                actor,
                _now,
                _eventIds.Next(),
                new BuyerSnapshot(
                    BuyerPartyType.Individual,
                    null,
                    "Ali Rezaei",
                    "ali.rezaei@example.com",
                    "+989121234567",
                    null));

            // ---- travellers -------------------------------------------------------------
            var traveler1 = order.AddTraveler(
                new TravelerName("Ali", "Rezaei", "MR"),
                TravelerType.ADT,
                new LocalDate(1988, 4, 12),
                actor,
                _now,
                _eventIds.Next(),
                "M");

            var traveler2 = order.AddTraveler(
                new TravelerName("Sara", "Mohammadi", "MRS"),
                TravelerType.ADT,
                new LocalDate(1991, 9, 3),
                actor,
                _now,
                _eventIds.Next(),
                "F");

            order.AddContact(
                ContactType.Email,
                "ali.rezaei@example.com",
                ContactRole.BookingContact,
                true,
                actor,
                _now,
                _eventIds.Next());

            order.AddContact(
                ContactType.Mobile,
                "+989121234567",
                ContactRole.BookingContact,
                true,
                actor,
                _now,
                _eventIds.Next());

            // ---- journeys ---------------------------------------------------------------
            var thr = new AirportCode("THR");
            var mhd = new AirportCode("MHD");
            var carrier = new CarrierCode(IssuingCarrier);
            const string tehranZone = "Asia/Tehran";

            var outboundJourney = order.AddJourney(thr, mhd, actor, _now, _eventIds.Next());
            var outboundSegment = order.AddJourneySegment(
                outboundJourney,
                carrier,
                carrier,
                "1084",
                thr,
                mhd,
                InstantAt(2026, 9, 2, 6, 30),
                InstantAt(2026, 9, 2, 7, 55),
                new LocalDate(2026, 9, 2),
                new LocalTime(10, 0),
                tehranZone,
                new LocalDate(2026, 9, 2),
                new LocalTime(11, 25),
                tehranZone,
                actor,
                _now,
                _eventIds.Next(),
                "A320");

            var inboundJourney = order.AddJourney(mhd, thr, actor, _now, _eventIds.Next());
            var inboundSegment = order.AddJourneySegment(
                inboundJourney,
                carrier,
                carrier,
                "1085",
                mhd,
                thr,
                InstantAt(2026, 9, 9, 9, 15),
                InstantAt(2026, 9, 9, 10, 40),
                new LocalDate(2026, 9, 9),
                new LocalTime(12, 45),
                tehranZone,
                new LocalDate(2026, 9, 9),
                new LocalTime(14, 10),
                tehranZone,
                actor,
                _now,
                _eventIds.Next(),
                "A320");

            var travelers = new[] { traveler1, traveler2 };
            var segments = new[] { outboundSegment, inboundSegment };
            var journeys = new[] { outboundJourney, inboundJourney };

            // ---- air transport: one item per traveller ----------------------------------
            var airItems = new List<OrderItemId>();
            var airEntitlements = new Dictionary<TravelerId, List<(EntitlementId Entitlement, JourneySegmentId Segment, OrderItemId Item)>>();

            foreach (var travelerId in travelers)
            {
                var baseFare = new Money(38_000_000m, currency);
                var tax = new Money(4_200_000m, currency);
                var total = baseFare + tax;

                var outboundEntitlementId = EntitlementId.New();
                var inboundEntitlementId = EntitlementId.New();

                var itemId = order.AddOrderItem(
                    new ProductSnapshot(
                        "PRD-AIR-RT",
                        "AIR-RT-ECO",
                        ProductType.AirTransport,
                        "Round trip THR-MHD-THR, Economy",
                        "W5 1084 / W5 1085 economy round trip",
                        "catalog-2026.09",
                        [new ProductAttribute("FareFamily", "EconomyFlex")]),
                    new PriceSnapshot(total, _now, PricingEngineVersion, RoundingPolicyVersion),
                    new CommercialTermsSnapshot(
                        "terms-2026.09",
                        "RefundableWithPenalty",
                        "ChangeableWithPenalty",
                        "NOSHOW-STD",
                        restrictions: [new CommercialTermRestriction("MINSTAY", "Minimum stay 2 nights")],
                        sourceRuleRefs: ["RULE-W5-ECO-01"]),
                    new CommercialSource(CommercialSourceType.RetailOffer, $"OFFER-{reference.Value}-AIR", "1", _now),
                    BuildSalesContext(currency),
                    new SettlementArrangement(SettlementModel.PassengerRetail, null, null, true, null),
                    [travelerId],
                    [
                        new ChargeLineDraft(1, ChargeType.Base, "BASE", baseFare, true),
                        new ChargeLineDraft(2, ChargeType.Tax, "I6", tax, true, "Iranian domestic tax")
                    ],
                    [
                        new EntitlementDraft(
                            EntitlementType.AirTransport,
                            [travelerId],
                            new Applicability(outboundJourney, [outboundSegment]),
                            new AirTransportSpecification("Economy", "Y", "ECOFLEX", "YRT"),
                            new CapacityCommitmentRef(CapacityCommitmentType.ConfirmedInventory, $"INV-1084-{travelerId.Value:N}"),
                            EntitlementId: outboundEntitlementId),
                        new EntitlementDraft(
                            EntitlementType.AirTransport,
                            [travelerId],
                            new Applicability(inboundJourney, [inboundSegment]),
                            new AirTransportSpecification("Economy", "Y", "ECOFLEX", "YRT"),
                            new CapacityCommitmentRef(CapacityCommitmentType.ConfirmedInventory, $"INV-1085-{travelerId.Value:N}"),
                            EntitlementId: inboundEntitlementId)
                    ],
                    actor,
                    _now,
                    _eventIds.Next(),
                    [
                        new ValueAllocationDraft(
                            outboundEntitlementId,
                            new Money(21_100_000m, currency),
                            "alloc-1",
                            AllocationPurpose.Accounting,
                            outboundSegment,
                            travelerId),
                        new ValueAllocationDraft(
                            inboundEntitlementId,
                            new Money(21_100_000m, currency),
                            "alloc-1",
                            AllocationPurpose.Accounting,
                            inboundSegment,
                            travelerId)
                    ]);

                airItems.Add(itemId);
                airEntitlements[travelerId] =
                [
                    (outboundEntitlementId, outboundSegment, itemId),
                    (inboundEntitlementId, inboundSegment, itemId)
                ];
            }

            // ---- extra baggage: per traveller, per segment ------------------------------
            var baggageItems = new List<OrderItemId>();
            var baggageEntitlements = new List<(TravelerId Traveler, EntitlementId Entitlement, JourneySegmentId Segment, OrderItemId Item)>();

            for (var index = 0; index < travelers.Length; index++)
            {
                var travelerId = travelers[index];

                for (var segmentIndex = 0; segmentIndex < segments.Length; segmentIndex++)
                {
                    var price = new Money(3_500_000m, currency);
                    var entitlementId = EntitlementId.New();
                    var flightNumber = segmentIndex == 0 ? "1084" : "1085";

                    var itemId = order.AddOrderItem(
                        new ProductSnapshot(
                            "PRD-BAG-10",
                            "BAG-EXTRA-10KG",
                            ProductType.Baggage,
                            "Extra checked baggage 10 kg",
                            $"Additional 10 kg allowance on W5 {flightNumber}",
                            "catalog-2026.09",
                            [new ProductAttribute("AllowanceKg", "10")]),
                        new PriceSnapshot(price, _now, PricingEngineVersion, RoundingPolicyVersion),
                        new CommercialTermsSnapshot("terms-2026.09", "NonRefundable", "NonChangeable"),
                        new CommercialSource(
                            CommercialSourceType.RetailOffer,
                            $"OFFER-{reference.Value}-BAG-{index + 1}-{segmentIndex + 1}",
                            "1",
                            _now),
                        BuildSalesContext(currency),
                        new SettlementArrangement(SettlementModel.PassengerRetail, null, null, true, null),
                        [travelerId],
                        [new ChargeLineDraft(1, ChargeType.ServiceFee, "BAG", price, false)],
                        [
                            new EntitlementDraft(
                                EntitlementType.CheckedBaggage,
                                [travelerId],
                                new Applicability(journeys[segmentIndex], [segments[segmentIndex]]),
                                new BaggageSpecification(BaggageAllowanceKind.Weight, 10m, null, null, "DIM-STD"),
                                EntitlementId: entitlementId)
                        ],
                        actor,
                        _now,
                        _eventIds.Next());

                    baggageItems.Add(itemId);
                    baggageEntitlements.Add((travelerId, entitlementId, segments[segmentIndex], itemId));
                }
            }

            // ---- lounge access at THR only ----------------------------------------------
            var loungeItems = new List<OrderItemId>();
            var loungeEntitlements = new List<(TravelerId Traveler, EntitlementId Entitlement, OrderItemId Item)>();

            for (var index = 0; index < travelers.Length; index++)
            {
                var travelerId = travelers[index];
                var price = new Money(2_200_000m, currency);
                var entitlementId = EntitlementId.New();

                var itemId = order.AddOrderItem(
                    new ProductSnapshot(
                        "PRD-LNG-THR",
                        "LOUNGE-THR",
                        ProductType.Lounge,
                        "Lounge access at Tehran Mehrabad",
                        "Single lounge entry before departure",
                        "catalog-2026.09",
                        [new ProductAttribute("Location", "THR")]),
                    new PriceSnapshot(price, _now, PricingEngineVersion, RoundingPolicyVersion),
                    new CommercialTermsSnapshot("terms-2026.09", "NonRefundable", "NonChangeable"),
                    new CommercialSource(
                        CommercialSourceType.RetailOffer,
                        $"OFFER-{reference.Value}-LNG-{index + 1}",
                        "1",
                        _now),
                    BuildSalesContext(currency),
                    new SettlementArrangement(SettlementModel.PassengerRetail, null, null, true, null),
                    [travelerId],
                    [new ChargeLineDraft(1, ChargeType.ServiceFee, "LNG", price, false)],
                    [
                        new EntitlementDraft(
                            EntitlementType.LoungeAccess,
                            [travelerId],
                            // Location-scoped, not segment-scoped: the entitlement is the airport lounge.
                            new Applicability(locationRef: "THR"),
                            new LoungeSpecification("THR-CIP-1", 1),
                            EntitlementId: entitlementId)
                    ],
                    actor,
                    _now,
                    _eventIds.Next());

                loungeItems.Add(itemId);
                loungeEntitlements.Add((travelerId, entitlementId, itemId));
            }

            // ---- confirm every item ------------------------------------------------------
            var allItems = airItems.Concat(baggageItems).Concat(loungeItems).ToArray();
            order.ConfirmOrderItems(allItems, actor, _now, _eventIds.Next());

            order.AddExternalReference(
                ExternalReferenceScope.Order,
                null,
                "W5-PSS",
                "PNR",
                BuildPnr(reference),
                actor,
                _now,
                _eventIds.Next());

            order.CheckInvariants();

            // ---- issue: one ticket + one EMD per traveller -------------------------------
            var tickets = new List<ElectronicTicket>();
            var documents = new List<ElectronicMiscDocument>();

            for (var index = 0; index < travelers.Length; index++)
            {
                var travelerId = travelers[index];
                var ticketNumber = $"217-{2400000 + index:0000000}";
                var ticketId = ElectronicTicketId.New();

                var ticket = ElectronicTicket.Issue(
                    ticketId,
                    ticketNumber,
                    orderId,
                    travelerId,
                    carrier,
                    _now,
                    airEntitlements[travelerId].Select(air =>
                        new TicketCouponDraft(air.Item, air.Entitlement, air.Segment)),
                    _eventIds.Next());

                tickets.Add(ticket);

                foreach (var air in airEntitlements[travelerId])
                {
                    order.AttachFulfillmentLink(
                        air.Entitlement,
                        FulfillmentLinkType.ElectronicTicket,
                        ticketId.Value,
                        null,
                        ticketNumber,
                        actor,
                        _now);
                }

                // EMD carries this traveller's ancillaries: baggage per segment plus lounge.
                var emdNumber = $"217-{9600000 + index:0000000}";
                var emdId = ElectronicMiscDocumentId.New();

                var travelerBaggage = baggageEntitlements.Where(baggage => baggage.Traveler == travelerId).ToArray();
                var travelerLounge = loungeEntitlements.Single(lounge => lounge.Traveler == travelerId);

                var emdDrafts = travelerBaggage
                    .Select(baggage => new EmdCouponDraft(baggage.Item, baggage.Entitlement, baggage.Segment))
                    .Append(new EmdCouponDraft(travelerLounge.Item, travelerLounge.Entitlement))
                    .ToArray();

                var document = ElectronicMiscDocument.Issue(
                    emdId,
                    emdNumber,
                    orderId,
                    travelerId,
                    EmdType.Associated,
                    _now,
                    emdDrafts,
                    _eventIds.Next());

                documents.Add(document);

                foreach (var baggage in travelerBaggage)
                {
                    order.AttachFulfillmentLink(
                        baggage.Entitlement,
                        FulfillmentLinkType.ElectronicMiscDocument,
                        emdId.Value,
                        null,
                        emdNumber,
                        actor,
                        _now);
                }

                order.AttachFulfillmentLink(
                    travelerLounge.Entitlement,
                    FulfillmentLinkType.ElectronicMiscDocument,
                    emdId.Value,
                    null,
                    emdNumber,
                    actor,
                    _now);
            }

            order.CheckInvariants();

            return new ScenarioResult(order, tickets, documents);
        }

        private static SalesContext BuildSalesContext(CurrencyCode currency) =>
            new(
                SellerId,
                SellerOfficeId,
                ChannelCode,
                new CountryCode("IR"),
                currency,
                InstantAt(2026, 8, 7, 12, 0),
                ActorType.AirlineAgent,
                "seeder");

        private static Instant InstantAt(int year, int month, int day, int hour, int minute) =>
            Instant.FromUtc(year, month, day, hour, minute);

        private static string BuildPnr(OrderReference reference) =>
            reference.Value[..Math.Min(6, reference.Value.Length)];
    }

    internal sealed record ScenarioResult(
        Order Order,
        IReadOnlyCollection<ElectronicTicket> Tickets,
        IReadOnlyCollection<ElectronicMiscDocument> Documents);

    internal interface IEventIdFactory
    {
        string Next();
    }
}

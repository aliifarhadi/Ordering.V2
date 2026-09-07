using AeroTech.Framework.Core.Domain.Exceptions;

namespace AeroTech.Ordering.Domain._Shared.Resources
{
    public static class ExceptionFactory
    {
        // Structural invariants I-01..I-08: 2001-2029
        public static BusinessException OrderRequiresTravelerAndItem() =>
            new(2001, ExceptionMessages.OrderRequiresTravelerAndItem) { HttpStatus = 422 };

        public static BusinessException ItemRequiresExistingTraveler(params object?[] args) =>
            new(2002, ExceptionMessages.ItemRequiresExistingTraveler, args) { HttpStatus = 422 };

        public static BusinessException FlightItemRequiresJourneyReference() =>
            new(2003, ExceptionMessages.FlightItemRequiresJourneyReference) { HttpStatus = 422 };

        public static BusinessException AncillaryRequiresSingleJourneyReference() =>
            new(2004, ExceptionMessages.AncillaryRequiresSingleJourneyReference) { HttpStatus = 422 };

        public static BusinessException AncillaryRequiresConfirmedFlightItem(params object?[] args) =>
            new(2005, ExceptionMessages.AncillaryRequiresConfirmedFlightItem, args) { HttpStatus = 409 };

        public static BusinessException InfantRequiresAssociatedAdult(params object?[] args) =>
            new(2006, ExceptionMessages.InfantRequiresAssociatedAdult, args) { HttpStatus = 422 };

        public static BusinessException SegmentRequiresBothRefsPresent(params object?[] args) =>
            new(2007, ExceptionMessages.SegmentRequiresBothRefsPresent, args) { HttpStatus = 422 };

        public static BusinessException SegmentAlreadyExists(params object?[] args) =>
            new(2008, ExceptionMessages.SegmentAlreadyExists, args) { HttpStatus = 409 };

        public static BusinessException FlightItemRequiresSegmentPerJourney(params object?[] args) =>
            new(2009, ExceptionMessages.FlightItemRequiresSegmentPerJourney, args) { HttpStatus = 422 };

        public static BusinessException SaleCurrencyImmutableAfterPayment() =>
            new(2010, ExceptionMessages.SaleCurrencyImmutableAfterPayment) { HttpStatus = 409 };

        // Commercial invariants I-09..I-15: 2030-2059
        public static BusinessException PriceTotalMustEqualSumOfLines() =>
            new(2030, ExceptionMessages.PriceTotalMustEqualSumOfLines) { HttpStatus = 422 };

        public static BusinessException PriceLinesMustShareCurrency() =>
            new(2031, ExceptionMessages.PriceLinesMustShareCurrency) { HttpStatus = 422 };

        public static BusinessException PriceRequiresAtLeastOneLine() =>
            new(2032, ExceptionMessages.PriceRequiresAtLeastOneLine) { HttpStatus = 422 };

        public static BusinessException NegativeAmountNotPermittedForChargeType(params object?[] args) =>
            new(2033, ExceptionMessages.NegativeAmountNotPermittedForChargeType, args) { HttpStatus = 422 };

        public static BusinessException AmountDoesNotConformToCurrencyScale(params object?[] args) =>
            new(2034, ExceptionMessages.AmountDoesNotConformToCurrencyScale, args) { HttpStatus = 422 };

        public static BusinessException AmountDoesNotConformToCurrencyIncrement(params object?[] args) =>
            new(2035, ExceptionMessages.AmountDoesNotConformToCurrencyIncrement, args) { HttpStatus = 422 };

        public static BusinessException CurrencyMismatch(params object?[] args) =>
            new(2036, ExceptionMessages.CurrencyMismatch, args) { HttpStatus = 422 };

        public static BusinessException AllocationsCannotExceedAmount(params object?[] args) =>
            new(2037, ExceptionMessages.AllocationsCannotExceedAmount, args) { HttpStatus = 422 };

        public static BusinessException DeliveryRequiresSettledOrCredit(params object?[] args) =>
            new(2038, ExceptionMessages.DeliveryRequiresSettledOrCredit, args) { HttpStatus = 409 };

        public static BusinessException MultiSegmentRequiresValueAllocation() =>
            new(2039, ExceptionMessages.MultiSegmentRequiresValueAllocation) { HttpStatus = 422 };

        public static BusinessException ValueAllocationMustMatchItemTotal() =>
            new(2040, ExceptionMessages.ValueAllocationMustMatchItemTotal) { HttpStatus = 422 };

        // Lifecycle invariants I-16..I-22: 2060-2089
        public static BusinessException CannotCancelFlownItem(params object?[] args) =>
            new(2060, ExceptionMessages.CannotCancelFlownItem, args) { HttpStatus = 409 };

        public static BusinessException CannotRemoveTravelerWithActiveItem(params object?[] args) =>
            new(2061, ExceptionMessages.CannotRemoveTravelerWithActiveItem, args) { HttpStatus = 409 };

        public static BusinessException CannotRemoveJourneyElementWithActiveSegment(params object?[] args) =>
            new(2062, ExceptionMessages.CannotRemoveJourneyElementWithActiveSegment, args) { HttpStatus = 409 };

        public static BusinessException ExpiryOnlyAppliesToPendingPayment() =>
            new(2063, ExceptionMessages.ExpiryOnlyAppliesToPendingPayment) { HttpStatus = 409 };

        public static BusinessException ItemCannotTransition(params object?[] args) =>
            new(2064, ExceptionMessages.ItemCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException SegmentCannotTransition(params object?[] args) =>
            new(2065, ExceptionMessages.SegmentCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException DeliveryStatusCannotTransition(params object?[] args) =>
            new(2066, ExceptionMessages.DeliveryStatusCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException DeliveredCancellationRequiresVoidedUnits(params object?[] args) =>
            new(2067, ExceptionMessages.DeliveredCancellationRequiresVoidedUnits, args) { HttpStatus = 409 };

        // Governance invariants I-23..I-27: 2090-2109
        public static BusinessException ServicingRequiresMatchingSellerOrOverride() =>
            new(2090, ExceptionMessages.ServicingRequiresMatchingSellerOrOverride) { HttpStatus = 403 };

        public static BusinessException RejectedWhileUnderExternalControl(params object?[] args) =>
            new(2091, ExceptionMessages.RejectedWhileUnderExternalControl, args) { HttpStatus = 409 };

        public static BusinessException OrderIsSuspended() =>
            new(2092, ExceptionMessages.OrderIsSuspended) { HttpStatus = 409 };

        public static BusinessException OrderIsUnderLegalHold() =>
            new(2093, ExceptionMessages.OrderIsUnderLegalHold) { HttpStatus = 409 };

        public static BusinessException OrderIsClosed() =>
            new(2094, ExceptionMessages.OrderIsClosed) { HttpStatus = 409 };

        public static BusinessException PaymentIsUnderDispute(params object?[] args) =>
            new(2095, ExceptionMessages.PaymentIsUnderDispute, args) { HttpStatus = 409 };

        // Time limits: 2110-2129
        public static BusinessException TimeLimitNotFound(params object?[] args) =>
            new(2110, ExceptionMessages.TimeLimitNotFound, args) { HttpStatus = 404 };

        public static BusinessException TimeLimitNotActive(params object?[] args) =>
            new(2111, ExceptionMessages.TimeLimitNotActive, args) { HttpStatus = 409 };

        public static BusinessException TimeLimitExtensionsExhausted(params object?[] args) =>
            new(2112, ExceptionMessages.TimeLimitExtensionsExhausted, args) { HttpStatus = 409 };

        public static BusinessException TimeLimitMustBeInTheFuture() =>
            new(2113, ExceptionMessages.TimeLimitMustBeInTheFuture) { HttpStatus = 422 };

        public static BusinessException TimeLimitExtensionMustBeLater() =>
            new(2114, ExceptionMessages.TimeLimitExtensionMustBeLater) { HttpStatus = 422 };

        public static BusinessException ExpiryBlockedByOpenPaymentCompletion(params object?[] args) =>
            new(2115, ExceptionMessages.ExpiryBlockedByOpenPaymentCompletion, args) { HttpStatus = 409 };

        // Lookups and composition: 2130-2159
        public static BusinessException TravelerNotFound(params object?[] args) =>
            new(2130, ExceptionMessages.TravelerNotFound, args) { HttpStatus = 404 };

        public static BusinessException JourneyElementNotFound(params object?[] args) =>
            new(2131, ExceptionMessages.JourneyElementNotFound, args) { HttpStatus = 404 };

        public static BusinessException OrderItemNotFound(params object?[] args) =>
            new(2132, ExceptionMessages.OrderItemNotFound, args) { HttpStatus = 404 };

        public static BusinessException PaymentRecordNotFound(params object?[] args) =>
            new(2133, ExceptionMessages.PaymentRecordNotFound, args) { HttpStatus = 404 };

        public static BusinessException SegmentNotFound(params object?[] args) =>
            new(2134, ExceptionMessages.SegmentNotFound, args) { HttpStatus = 404 };

        public static BusinessException DuplicateTravelerId(params object?[] args) =>
            new(2135, ExceptionMessages.DuplicateTravelerId, args) { HttpStatus = 409 };

        public static BusinessException DuplicateJourneyElementId(params object?[] args) =>
            new(2136, ExceptionMessages.DuplicateJourneyElementId, args) { HttpStatus = 409 };

        public static BusinessException DuplicateOrderItemId(params object?[] args) =>
            new(2137, ExceptionMessages.DuplicateOrderItemId, args) { HttpStatus = 409 };

        public static BusinessException DuplicatePaymentRecordId(params object?[] args) =>
            new(2138, ExceptionMessages.DuplicatePaymentRecordId, args) { HttpStatus = 409 };

        // Limits: 2160-2179
        public static BusinessException TravelerLimitExceeded(params object?[] args) =>
            new(2160, ExceptionMessages.TravelerLimitExceeded, args) { HttpStatus = 422 };

        public static BusinessException JourneyElementLimitExceeded(params object?[] args) =>
            new(2161, ExceptionMessages.JourneyElementLimitExceeded, args) { HttpStatus = 422 };

        public static BusinessException ItemLimitExceeded(params object?[] args) =>
            new(2162, ExceptionMessages.ItemLimitExceeded, args) { HttpStatus = 422 };

        public static BusinessException NameIsTooLong(params object?[] args) =>
            new(2163, ExceptionMessages.NameIsTooLong, args) { HttpStatus = 422 };

        // Capabilities: 2180-2189
        public static BusinessException ItemTypeNotEnabled(params object?[] args) =>
            new(2180, ExceptionMessages.ItemTypeNotEnabled, args) { HttpStatus = 422 };

        public static BusinessException FormOfPaymentNotEnabled(params object?[] args) =>
            new(2181, ExceptionMessages.FormOfPaymentNotEnabled, args) { HttpStatus = 422 };

        public static BusinessException ChannelNotEnabled(params object?[] args) =>
            new(2182, ExceptionMessages.ChannelNotEnabled, args) { HttpStatus = 422 };

        // Composition validation: 2190-2219
        public static BusinessException IdentifierIsRequired(params object?[] args) =>
            new(2190, ExceptionMessages.IdentifierIsRequired, args) { HttpStatus = 422 };

        public static BusinessException GivenNameIsRequired() =>
            new(2191, ExceptionMessages.GivenNameIsRequired) { HttpStatus = 422 };

        public static BusinessException SurnameIsRequired() =>
            new(2192, ExceptionMessages.SurnameIsRequired) { HttpStatus = 422 };

        public static BusinessException DateOfBirthRequiredForPassengerType(params object?[] args) =>
            new(2193, ExceptionMessages.DateOfBirthRequiredForPassengerType, args) { HttpStatus = 422 };

        public static BusinessException ContactValueIsRequired() =>
            new(2194, ExceptionMessages.ContactValueIsRequired) { HttpStatus = 422 };

        public static BusinessException OnlyOnePrimaryContactPerType(params object?[] args) =>
            new(2195, ExceptionMessages.OnlyOnePrimaryContactPerType, args) { HttpStatus = 422 };

        public static BusinessException CurrencyCodeIsInvalid(params object?[] args) =>
            new(2196, ExceptionMessages.CurrencyCodeIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException CancellationRequiresReason() =>
            new(2197, ExceptionMessages.CancellationRequiresReason) { HttpStatus = 422 };

        public static BusinessException AssociatedAdultMustNotBeSelf(params object?[] args) =>
            new(2198, ExceptionMessages.AssociatedAdultMustNotBeSelf, args) { HttpStatus = 422 };

        public static BusinessException AssociatedAdultMustBeAdult(params object?[] args) =>
            new(2199, ExceptionMessages.AssociatedAdultMustBeAdult, args) { HttpStatus = 422 };

        // Groups I-28, I-29: 2240-2269
        public static BusinessException CannotReleaseMoreSeatsThanHeld(params object?[] args) =>
            new(2240, ExceptionMessages.CannotReleaseMoreSeatsThanHeld, args) { HttpStatus = 422 };

        public static BusinessException AllocatedSlotsCannotExceedHeldBlock(params object?[] args) =>
            new(2241, ExceptionMessages.AllocatedSlotsCannotExceedHeldBlock, args) { HttpStatus = 422 };

        public static BusinessException SeatBlockNotFound(params object?[] args) =>
            new(2242, ExceptionMessages.SeatBlockNotFound, args) { HttpStatus = 404 };

        public static BusinessException NameSlotNotFound(params object?[] args) =>
            new(2243, ExceptionMessages.NameSlotNotFound, args) { HttpStatus = 404 };

        public static BusinessException SlotIsNotUnallocated(params object?[] args) =>
            new(2244, ExceptionMessages.SlotIsNotUnallocated, args) { HttpStatus = 409 };

        public static BusinessException SlotIsNotAllocated(params object?[] args) =>
            new(2245, ExceptionMessages.SlotIsNotAllocated, args) { HttpStatus = 409 };

        public static BusinessException SlotCannotTransition(params object?[] args) =>
            new(2246, ExceptionMessages.SlotCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException BlocksMustBeConfirmedBeforeAllocation() =>
            new(2247, ExceptionMessages.BlocksMustBeConfirmedBeforeAllocation) { HttpStatus = 409 };

        public static BusinessException GroupIsSuspended() =>
            new(2248, ExceptionMessages.GroupIsSuspended) { HttpStatus = 409 };

        public static BusinessException GroupIsClosed() =>
            new(2249, ExceptionMessages.GroupIsClosed) { HttpStatus = 409 };

        public static BusinessException DuplicateBlockId(params object?[] args) =>
            new(2250, ExceptionMessages.DuplicateBlockId, args) { HttpStatus = 409 };

        public static BusinessException DuplicateSlotId(params object?[] args) =>
            new(2251, ExceptionMessages.DuplicateSlotId, args) { HttpStatus = 409 };

        public static BusinessException SeatsHeldMustBePositive() =>
            new(2252, ExceptionMessages.SeatsHeldMustBePositive) { HttpStatus = 422 };

        // Delivery I-30, I-33, I-34: 2270-2299
        public static BusinessException DeliveryUnitUnderExternalControl(params object?[] args) =>
            new(2270, ExceptionMessages.DeliveryUnitUnderExternalControl, args) { HttpStatus = 409 };

        public static BusinessException SegmentDeliveryCannotTransition(params object?[] args) =>
            new(2271, ExceptionMessages.SegmentDeliveryCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException ServiceDeliveryCannotTransition(params object?[] args) =>
            new(2272, ExceptionMessages.ServiceDeliveryCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException VoidWindowExpired(params object?[] args) =>
            new(2273, ExceptionMessages.VoidWindowExpired, args) { HttpStatus = 409 };

        public static BusinessException AllUnitsMustBeOpenToVoid(params object?[] args) =>
            new(2274, ExceptionMessages.AllUnitsMustBeOpenToVoid, args) { HttpStatus = 409 };

        public static BusinessException DeliveryRecordRequiresAtLeastOneUnit() =>
            new(2275, ExceptionMessages.DeliveryRecordRequiresAtLeastOneUnit) { HttpStatus = 422 };

        public static BusinessException SegmentUnitNotFound(params object?[] args) =>
            new(2276, ExceptionMessages.SegmentUnitNotFound, args) { HttpStatus = 404 };

        public static BusinessException ServiceUnitNotFound(params object?[] args) =>
            new(2277, ExceptionMessages.ServiceUnitNotFound, args) { HttpStatus = 404 };

        public static BusinessException UnitValuesMustMatchAllocation() =>
            new(2278, ExceptionMessages.UnitValuesMustMatchAllocation) { HttpStatus = 422 };

        public static BusinessException ControlAlreadyHeld(params object?[] args) =>
            new(2279, ExceptionMessages.ControlAlreadyHeld, args) { HttpStatus = 409 };

        public static BusinessException ControlNotHeld(params object?[] args) =>
            new(2280, ExceptionMessages.ControlNotHeld, args) { HttpStatus = 409 };

        public static BusinessException ValueReturnedBeforeUnitsWithdrawn(params object?[] args) =>
            new(2281, ExceptionMessages.ValueReturnedBeforeUnitsWithdrawn, args) { HttpStatus = 409 };

        // Tax documents I-31, I-32: 2300-2319
        public static BusinessException ReversalRequiresReversedDocument(params object?[] args) =>
            new(2300, ExceptionMessages.ReversalRequiresReversedDocument, args) { HttpStatus = 422 };

        public static BusinessException InvoiceCannotReferenceReversedDocument() =>
            new(2301, ExceptionMessages.InvoiceCannotReferenceReversedDocument) { HttpStatus = 422 };

        public static BusinessException TaxDocumentRequiresAtLeastOneLine() =>
            new(2302, ExceptionMessages.TaxDocumentRequiresAtLeastOneLine) { HttpStatus = 422 };

        public static BusinessException TaxDocumentTotalsMustMatchLines() =>
            new(2303, ExceptionMessages.TaxDocumentTotalsMustMatchLines) { HttpStatus = 422 };

        public static BusinessException SubmissionCannotTransition(params object?[] args) =>
            new(2304, ExceptionMessages.SubmissionCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException SpecVersionIsRequired() =>
            new(2305, ExceptionMessages.SpecVersionIsRequired) { HttpStatus = 422 };

        public static BusinessException PassengerIdentityIsRequired() =>
            new(2306, ExceptionMessages.PassengerIdentityIsRequired) { HttpStatus = 422 };

        // Document number ranges I-30: 2320-2339
        public static BusinessException RangeIsNotActive(params object?[] args) =>
            new(2320, ExceptionMessages.RangeIsNotActive, args) { HttpStatus = 409 };

        public static BusinessException RangeIsExhausted(params object?[] args) =>
            new(2321, ExceptionMessages.RangeIsExhausted, args) { HttpStatus = 409 };

        public static BusinessException RangeBoundsAreInvalid() =>
            new(2322, ExceptionMessages.RangeBoundsAreInvalid) { HttpStatus = 422 };

        public static BusinessException RangeKindMismatch(params object?[] args) =>
            new(2323, ExceptionMessages.RangeKindMismatch, args) { HttpStatus = 422 };

        // Operations: 2220-2239
        public static BusinessException OperationCannotTransition(params object?[] args) =>
            new(2220, ExceptionMessages.OperationCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException OperationIsTerminal(params object?[] args) =>
            new(2221, ExceptionMessages.OperationIsTerminal, args) { HttpStatus = 409 };

        public static BusinessException OperationStepNotOpen(params object?[] args) =>
            new(2222, ExceptionMessages.OperationStepNotOpen, args) { HttpStatus = 409 };

        // Shared kernel value objects: 2400-2419
        public static BusinessException AirportCodeIsInvalid(params object?[] args) =>
            new(2400, ExceptionMessages.AirportCodeIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException CarrierCodeIsInvalid(params object?[] args) =>
            new(2401, ExceptionMessages.CarrierCodeIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException CountryCodeIsInvalid(params object?[] args) =>
            new(2402, ExceptionMessages.CountryCodeIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException OrderReferenceIsInvalid(params object?[] args) =>
            new(2403, ExceptionMessages.OrderReferenceIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException ReferenceValueIsRequired(params object?[] args) =>
            new(2404, ExceptionMessages.ReferenceValueIsRequired, args) { HttpStatus = 422 };

        // Structural invariants INV-001..INV-012: 2420-2449
        public static BusinessException OrderRequiresTravelerAndItemToConfirm() =>
            new(2420, ExceptionMessages.OrderRequiresTravelerAndItemToConfirm) { HttpStatus = 422 };

        public static BusinessException ItemBeneficiaryMustBeOrderTraveler(params object?[] args) =>
            new(2421, ExceptionMessages.ItemBeneficiaryMustBeOrderTraveler, args) { HttpStatus = 422 };

        public static BusinessException EntitlementBeneficiaryMustBeOrderTraveler(params object?[] args) =>
            new(2422, ExceptionMessages.EntitlementBeneficiaryMustBeOrderTraveler, args) { HttpStatus = 422 };

        public static BusinessException EntitlementSegmentMustExistInOrder(params object?[] args) =>
            new(2423, ExceptionMessages.EntitlementSegmentMustExistInOrder, args) { HttpStatus = 422 };

        public static BusinessException EntitlementJourneyMustExistInOrder(params object?[] args) =>
            new(2424, ExceptionMessages.EntitlementJourneyMustExistInOrder, args) { HttpStatus = 422 };

        public static BusinessException JourneySegmentSequenceIsInvalid(params object?[] args) =>
            new(2425, ExceptionMessages.JourneySegmentSequenceIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException InfantRequiresAssociatedAdultTraveler(params object?[] args) =>
            new(2426, ExceptionMessages.InfantRequiresAssociatedAdultTraveler, args) { HttpStatus = 422 };

        public static BusinessException DateOfBirthRequiredForTravelerType(params object?[] args) =>
            new(2427, ExceptionMessages.DateOfBirthRequiredForTravelerType, args) { HttpStatus = 422 };

        public static BusinessException CannotRemoveReferencedTraveler(params object?[] args) =>
            new(2428, ExceptionMessages.CannotRemoveReferencedTraveler, args) { HttpStatus = 409 };

        public static BusinessException CannotRemoveReferencedJourneySegment(params object?[] args) =>
            new(2429, ExceptionMessages.CannotRemoveReferencedJourneySegment, args) { HttpStatus = 409 };

        public static BusinessException OrderReferenceIsImmutable() =>
            new(2430, ExceptionMessages.OrderReferenceIsImmutable) { HttpStatus = 409 };

        public static BusinessException CreatedSalesContextIsImmutable() =>
            new(2431, ExceptionMessages.CreatedSalesContextIsImmutable) { HttpStatus = 409 };

        public static BusinessException DuplicateJourneyId(params object?[] args) =>
            new(2432, ExceptionMessages.DuplicateJourneyId, args) { HttpStatus = 409 };

        public static BusinessException DuplicateJourneySegmentId(params object?[] args) =>
            new(2433, ExceptionMessages.DuplicateJourneySegmentId, args) { HttpStatus = 409 };

        public static BusinessException DuplicateEntitlementId(params object?[] args) =>
            new(2434, ExceptionMessages.DuplicateEntitlementId, args) { HttpStatus = 409 };

        public static BusinessException DuplicateContactId(params object?[] args) =>
            new(2435, ExceptionMessages.DuplicateContactId, args) { HttpStatus = 409 };

        public static BusinessException JourneyNotFound(params object?[] args) =>
            new(2436, ExceptionMessages.JourneyNotFound, args) { HttpStatus = 404 };

        public static BusinessException JourneySegmentNotFound(params object?[] args) =>
            new(2437, ExceptionMessages.JourneySegmentNotFound, args) { HttpStatus = 404 };

        public static BusinessException EntitlementNotFound(params object?[] args) =>
            new(2438, ExceptionMessages.EntitlementNotFound, args) { HttpStatus = 404 };

        public static BusinessException ContactNotFound(params object?[] args) =>
            new(2439, ExceptionMessages.ContactNotFound, args) { HttpStatus = 404 };

        public static BusinessException ContactTravelerMustExistInOrder(params object?[] args) =>
            new(2440, ExceptionMessages.ContactTravelerMustExistInOrder, args) { HttpStatus = 422 };

        public static BusinessException AssociatedTravelerMustExistInOrder(params object?[] args) =>
            new(2441, ExceptionMessages.AssociatedTravelerMustExistInOrder, args) { HttpStatus = 422 };

        // Commercial invariants INV-020..INV-031: 2450-2479
        public static BusinessException ChargeLinesMustSumToPriceTotal(params object?[] args) =>
            new(2450, ExceptionMessages.ChargeLinesMustSumToPriceTotal, args) { HttpStatus = 422 };

        public static BusinessException ValueAllocationsMustSumToPriceTotal(params object?[] args) =>
            new(2451, ExceptionMessages.ValueAllocationsMustSumToPriceTotal, args) { HttpStatus = 422 };

        public static BusinessException MultiSegmentAirTransportRequiresAllocations(params object?[] args) =>
            new(2452, ExceptionMessages.MultiSegmentAirTransportRequiresAllocations, args) { HttpStatus = 422 };

        public static BusinessException ConfirmedItemRequiresEntitlement(params object?[] args) =>
            new(2453, ExceptionMessages.ConfirmedItemRequiresEntitlement, args) { HttpStatus = 422 };

        public static BusinessException NegativeChargeLineNotPermitted(params object?[] args) =>
            new(2454, ExceptionMessages.NegativeChargeLineNotPermitted, args) { HttpStatus = 422 };

        public static BusinessException ChargeLineCurrencyMustMatchPrice(params object?[] args) =>
            new(2455, ExceptionMessages.ChargeLineCurrencyMustMatchPrice, args) { HttpStatus = 422 };

        public static BusinessException AllocationCurrencyMustMatchPrice(params object?[] args) =>
            new(2456, ExceptionMessages.AllocationCurrencyMustMatchPrice, args) { HttpStatus = 422 };

        public static BusinessException AllocationEntitlementMustBelongToItem(params object?[] args) =>
            new(2457, ExceptionMessages.AllocationEntitlementMustBelongToItem, args) { HttpStatus = 422 };

        public static BusinessException DuplicateChargeLineSequence(params object?[] args) =>
            new(2458, ExceptionMessages.DuplicateChargeLineSequence, args) { HttpStatus = 409 };

        public static BusinessException PriceRequiresAtLeastOneChargeLine() =>
            new(2459, ExceptionMessages.PriceRequiresAtLeastOneChargeLine) { HttpStatus = 422 };

        public static BusinessException EntitlementBelongsToAnotherItem(params object?[] args) =>
            new(2460, ExceptionMessages.EntitlementBelongsToAnotherItem, args) { HttpStatus = 409 };

        // Entitlement invariants INV-040..INV-048: 2480-2509
        public static BusinessException SeatEntitlementScopeIsInvalid(params object?[] args) =>
            new(2480, ExceptionMessages.SeatEntitlementScopeIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException AirTransportEntitlementScopeIsInvalid(params object?[] args) =>
            new(2481, ExceptionMessages.AirTransportEntitlementScopeIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException BaggageAllowanceIsIncomplete(params object?[] args) =>
            new(2482, ExceptionMessages.BaggageAllowanceIsIncomplete, args) { HttpStatus = 422 };

        public static BusinessException AccommodationDatesAreInvalid(params object?[] args) =>
            new(2483, ExceptionMessages.AccommodationDatesAreInvalid, args) { HttpStatus = 422 };

        public static BusinessException AccommodationRequiresBeneficiary(params object?[] args) =>
            new(2484, ExceptionMessages.AccommodationRequiresBeneficiary, args) { HttpStatus = 422 };

        public static BusinessException JourneyScopedSegmentsMustBelongToJourney(params object?[] args) =>
            new(2485, ExceptionMessages.JourneyScopedSegmentsMustBelongToJourney, args) { HttpStatus = 422 };

        public static BusinessException CapacityCommitmentIsRequired(params object?[] args) =>
            new(2486, ExceptionMessages.CapacityCommitmentIsRequired, args) { HttpStatus = 422 };

        public static BusinessException ResponsibilityAssignmentIsRequired(params object?[] args) =>
            new(2487, ExceptionMessages.ResponsibilityAssignmentIsRequired, args) { HttpStatus = 422 };

        public static BusinessException EntitlementCannotTransition(params object?[] args) =>
            new(2488, ExceptionMessages.EntitlementCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException EntitlementIsTerminal(params object?[] args) =>
            new(2489, ExceptionMessages.EntitlementIsTerminal, args) { HttpStatus = 409 };

        public static BusinessException EntitlementRequiresBeneficiary(params object?[] args) =>
            new(2490, ExceptionMessages.EntitlementRequiresBeneficiary, args) { HttpStatus = 422 };

        public static BusinessException EntitlementSpecificationMismatch(params object?[] args) =>
            new(2491, ExceptionMessages.EntitlementSpecificationMismatch, args) { HttpStatus = 422 };

        public static BusinessException OccupancyMustBePositive(params object?[] args) =>
            new(2492, ExceptionMessages.OccupancyMustBePositive, args) { HttpStatus = 422 };

        public static BusinessException OccupancyCannotBeNegative(params object?[] args) =>
            new(2493, ExceptionMessages.OccupancyCannotBeNegative, args) { HttpStatus = 422 };

        public static BusinessException LoungeAccessCountMustBePositive(params object?[] args) =>
            new(2494, ExceptionMessages.LoungeAccessCountMustBePositive, args) { HttpStatus = 422 };

        public static BusinessException InsuranceCoverageDatesAreInvalid(params object?[] args) =>
            new(2495, ExceptionMessages.InsuranceCoverageDatesAreInvalid, args) { HttpStatus = 422 };

        // Item status invariants and transitions: 2510-2529
        public static BusinessException OrderItemCannotTransition(params object?[] args) =>
            new(2510, ExceptionMessages.OrderItemCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException OrderItemIsTerminal(params object?[] args) =>
            new(2511, ExceptionMessages.OrderItemIsTerminal, args) { HttpStatus = 409 };

        public static BusinessException OrderItemStatusIsIndeterminate(params object?[] args) =>
            new(2512, ExceptionMessages.OrderItemStatusIsIndeterminate, args) { HttpStatus = 500 };

        public static BusinessException PriceSnapshotIsImmutable(params object?[] args) =>
            new(2513, ExceptionMessages.PriceSnapshotIsImmutable, args) { HttpStatus = 409 };

        public static BusinessException CommercialSourceIsImmutable(params object?[] args) =>
            new(2514, ExceptionMessages.CommercialSourceIsImmutable, args) { HttpStatus = 409 };

        public static BusinessException SettlementArrangementIsImmutable(params object?[] args) =>
            new(2515, ExceptionMessages.SettlementArrangementIsImmutable, args) { HttpStatus = 409 };

        // Time limits and locks INV-080..INV-085: 2530-2549
        public static BusinessException TimeLimitScopeMustExistInOrder(params object?[] args) =>
            new(2530, ExceptionMessages.TimeLimitScopeMustExistInOrder, args) { HttpStatus = 422 };

        public static BusinessException TimeLimitIsTerminal(params object?[] args) =>
            new(2531, ExceptionMessages.TimeLimitIsTerminal, args) { HttpStatus = 409 };

        public static BusinessException DuplicateTimeLimitId(params object?[] args) =>
            new(2532, ExceptionMessages.DuplicateTimeLimitId, args) { HttpStatus = 409 };

        public static BusinessException ProcessingLockConflict(params object?[] args) =>
            new(2533, ExceptionMessages.ProcessingLockConflict, args) { HttpStatus = 409 };

        public static BusinessException ProcessingLockNotFound(params object?[] args) =>
            new(2534, ExceptionMessages.ProcessingLockNotFound, args) { HttpStatus = 404 };

        public static BusinessException DuplicateProcessingLockId(params object?[] args) =>
            new(2535, ExceptionMessages.DuplicateProcessingLockId, args) { HttpStatus = 409 };

        public static BusinessException ProcessingLockScopeMustExistInOrder(params object?[] args) =>
            new(2536, ExceptionMessages.ProcessingLockScopeMustExistInOrder, args) { HttpStatus = 422 };

        public static BusinessException ProcessingLockExpiryMustBeLater() =>
            new(2537, ExceptionMessages.ProcessingLockExpiryMustBeLater) { HttpStatus = 422 };

        // Authority and lifecycle: 2550-2569
        public static BusinessException ActorLacksServicingAuthority() =>
            new(2550, ExceptionMessages.ActorLacksServicingAuthority) { HttpStatus = 403 };

        public static BusinessException DelegationNotFound(params object?[] args) =>
            new(2551, ExceptionMessages.DelegationNotFound, args) { HttpStatus = 404 };

        public static BusinessException DuplicateDelegationId(params object?[] args) =>
            new(2552, ExceptionMessages.DuplicateDelegationId, args) { HttpStatus = 409 };

        public static BusinessException DelegationValidityIsInvalid() =>
            new(2553, ExceptionMessages.DelegationValidityIsInvalid) { HttpStatus = 422 };

        public static BusinessException OrderAlreadyClosed() =>
            new(2554, ExceptionMessages.OrderAlreadyClosed) { HttpStatus = 409 };

        public static BusinessException OrderCannotCloseWithActiveContent() =>
            new(2555, ExceptionMessages.OrderCannotCloseWithActiveContent) { HttpStatus = 409 };

        public static BusinessException DuplicateExternalReference(params object?[] args) =>
            new(2556, ExceptionMessages.DuplicateExternalReference, args) { HttpStatus = 409 };

        public static BusinessException ExternalReferenceNotFound(params object?[] args) =>
            new(2557, ExceptionMessages.ExternalReferenceNotFound, args) { HttpStatus = 404 };

        public static BusinessException ExternalReferenceScopeTargetMissing(params object?[] args) =>
            new(2558, ExceptionMessages.ExternalReferenceScopeTargetMissing, args) { HttpStatus = 422 };

        // Split and lineage INV-100..INV-104: 2570-2589
        public static BusinessException SplitRequiresTravelers() =>
            new(2570, ExceptionMessages.SplitRequiresTravelers) { HttpStatus = 422 };

        public static BusinessException SplitCannotMoveAllTravelers() =>
            new(2571, ExceptionMessages.SplitCannotMoveAllTravelers) { HttpStatus = 422 };

        public static BusinessException SplitTravelerMustExistInOrder(params object?[] args) =>
            new(2572, ExceptionMessages.SplitTravelerMustExistInOrder, args) { HttpStatus = 422 };

        public static BusinessException SplitWouldOrphanOrderItem(params object?[] args) =>
            new(2573, ExceptionMessages.SplitWouldOrphanOrderItem, args) { HttpStatus = 409 };

        public static BusinessException SplitBlockedByProcessingLock(params object?[] args) =>
            new(2574, ExceptionMessages.SplitBlockedByProcessingLock, args) { HttpStatus = 409 };

        public static BusinessException PredecessorItemMustExistInOrder(params object?[] args) =>
            new(2575, ExceptionMessages.PredecessorItemMustExistInOrder, args) { HttpStatus = 422 };

        // Fulfillment invariants FUL-001..FUL-043: 2600-2649
        public static BusinessException TicketCouponCannotTransition(params object?[] args) =>
            new(2600, ExceptionMessages.TicketCouponCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException TicketCouponRequiresControlAuthority(params object?[] args) =>
            new(2601, ExceptionMessages.TicketCouponRequiresControlAuthority, args) { HttpStatus = 409 };

        public static BusinessException FlownCouponCannotBeVoided(params object?[] args) =>
            new(2602, ExceptionMessages.FlownCouponCannotBeVoided, args) { HttpStatus = 409 };

        public static BusinessException TicketRequiresAtLeastOneCoupon() =>
            new(2603, ExceptionMessages.TicketRequiresAtLeastOneCoupon) { HttpStatus = 422 };

        public static BusinessException DuplicateCouponNumber(params object?[] args) =>
            new(2604, ExceptionMessages.DuplicateCouponNumber, args) { HttpStatus = 409 };

        public static BusinessException DuplicateActiveFulfillmentUnit(params object?[] args) =>
            new(2605, ExceptionMessages.DuplicateActiveFulfillmentUnit, args) { HttpStatus = 409 };

        public static BusinessException TicketCouponNotFound(params object?[] args) =>
            new(2606, ExceptionMessages.TicketCouponNotFound, args) { HttpStatus = 404 };

        public static BusinessException EmdCouponNotFound(params object?[] args) =>
            new(2607, ExceptionMessages.EmdCouponNotFound, args) { HttpStatus = 404 };

        public static BusinessException EmdCouponCannotTransition(params object?[] args) =>
            new(2608, ExceptionMessages.EmdCouponCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException EmdRequiresAtLeastOneCoupon() =>
            new(2609, ExceptionMessages.EmdRequiresAtLeastOneCoupon) { HttpStatus = 422 };

        public static BusinessException SupplierReservationCannotTransition(params object?[] args) =>
            new(2610, ExceptionMessages.SupplierReservationCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException ConfirmedReservationRequiresConfirmationNumber(params object?[] args) =>
            new(2611, ExceptionMessages.ConfirmedReservationRequiresConfirmationNumber, args) { HttpStatus = 422 };

        public static BusinessException ReservationRequiresFulfillmentUnit(params object?[] args) =>
            new(2612, ExceptionMessages.ReservationRequiresFulfillmentUnit, args) { HttpStatus = 422 };

        public static BusinessException FulfillmentUnitNotFound(params object?[] args) =>
            new(2613, ExceptionMessages.FulfillmentUnitNotFound, args) { HttpStatus = 404 };

        public static BusinessException ControlHolderIsRequired() =>
            new(2614, ExceptionMessages.ControlHolderIsRequired) { HttpStatus = 422 };

        public static BusinessException DocumentNumberIsInvalid(params object?[] args) =>
            new(2615, ExceptionMessages.DocumentNumberIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException DocumentStockRangeIsInvalid() =>
            new(2616, ExceptionMessages.DocumentStockRangeIsInvalid) { HttpStatus = 422 };

        public static BusinessException DocumentStockIsExhausted(params object?[] args) =>
            new(2617, ExceptionMessages.DocumentStockIsExhausted, args) { HttpStatus = 409 };

        public static BusinessException DocumentStockIsNotActive(params object?[] args) =>
            new(2618, ExceptionMessages.DocumentStockIsNotActive, args) { HttpStatus = 409 };

        public static BusinessException CouponSegmentIsRequired(params object?[] args) =>
            new(2619, ExceptionMessages.CouponSegmentIsRequired, args) { HttpStatus = 422 };

        // Consumption and reconciliation: 2650-2679
        public static BusinessException ConsumptionFactSourceIsRequired() =>
            new(2650, ExceptionMessages.ConsumptionFactSourceIsRequired) { HttpStatus = 422 };

        public static BusinessException ConsumptionQuantityUnitIsRequired() =>
            new(2651, ExceptionMessages.ConsumptionQuantityUnitIsRequired) { HttpStatus = 422 };

        public static BusinessException ReconciliationCaseIsResolved(params object?[] args) =>
            new(2652, ExceptionMessages.ReconciliationCaseIsResolved, args) { HttpStatus = 409 };

        public static BusinessException ReconciliationCaseCannotTransition(params object?[] args) =>
            new(2653, ExceptionMessages.ReconciliationCaseCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException ReconciliationResolutionRequiresClassification(params object?[] args) =>
            new(2654, ExceptionMessages.ReconciliationResolutionRequiresClassification, args) { HttpStatus = 422 };
    }
}

namespace AeroTech.Ordering.Domain._Shared.Resources
{
    public static class ExceptionMessages
    {
        // Structural invariants I-01..I-08
        public const string OrderRequiresTravelerAndItem = "An order must have at least one traveller and one item before it leaves Draft.";
        public const string ItemRequiresExistingTraveler = "Order item '{0}' references traveller '{1}', which is not present in this order.";
        public const string FlightItemRequiresJourneyReference = "A flight item must reference at least one journey element.";
        public const string AncillaryRequiresSingleJourneyReference = "An ancillary item must reference exactly one journey element.";
        public const string AncillaryRequiresConfirmedFlightItem = "An ancillary item requires a flight item that is Confirmed or beyond on journey element '{0}'.";
        public const string InfantRequiresAssociatedAdult = "Traveller '{0}' is an infant and must reference an associated adult present in this order.";
        public const string SegmentRequiresBothRefsPresent = "A traveller journey segment requires both traveller '{0}' and journey element '{1}' to be present in this order.";
        public const string SegmentAlreadyExists = "A traveller journey segment already exists for traveller '{0}' and journey element '{1}'.";
        public const string FlightItemRequiresSegmentPerJourney = "Flight item '{0}' requires a traveller journey segment for traveller '{1}' on every referenced journey element.";
        public const string SaleCurrencyImmutableAfterPayment = "The sale currency cannot be changed once a payment record exists.";

        // Commercial invariants I-09..I-15
        public const string PriceTotalMustEqualSumOfLines = "The price breakdown total does not equal the sum of its charge lines.";
        public const string PriceLinesMustShareCurrency = "All charge lines in a price breakdown must share the breakdown currency.";
        public const string PriceRequiresAtLeastOneLine = "A price breakdown must contain at least one charge line.";
        public const string NegativeAmountNotPermittedForChargeType = "A negative amount is permitted only for Discount and Penalty charge lines, not for '{0}'.";
        public const string AmountDoesNotConformToCurrencyScale = "Amount '{0}' does not conform to the scale defined for currency '{1}'.";
        public const string AmountDoesNotConformToCurrencyIncrement = "Amount '{0}' does not conform to the increment defined for currency '{1}'.";
        public const string CurrencyMismatch = "Cannot operate on amounts in different currencies: '{0}' and '{1}'.";
        public const string AllocationsCannotExceedAmount = "The sum of allocations on payment record '{0}' cannot exceed its amount.";
        public const string DeliveryRequiresSettledOrCredit = "Item '{0}' cannot become Delivered: its allocated balance is not settled and no credit authority is recorded.";
        public const string MultiSegmentRequiresValueAllocation = "A multi-segment flight item requires a complete value allocation whose sum equals the item total.";
        public const string ValueAllocationMustMatchItemTotal = "The value allocation sum does not equal the item total.";

        // Lifecycle invariants I-16..I-22
        public const string CannotCancelFlownItem = "Item '{0}' has a delivery unit that is Flown or NoShow and cannot be cancelled; it may only be refunded.";
        public const string CannotRemoveTravelerWithActiveItem = "Traveller '{0}' cannot be removed while holding a non-cancelled item.";
        public const string CannotRemoveJourneyElementWithActiveSegment = "Journey element '{0}' cannot be removed while a non-cancelled traveller journey segment references it.";
        public const string ExpiryOnlyAppliesToPendingPayment = "A time limit may expire only items in PendingPayment.";
        public const string ItemCannotTransition = "Item '{0}' cannot transition from '{1}' to '{2}'.";
        public const string SegmentCannotTransition = "Segment status cannot transition from '{0}' to '{1}'.";
        public const string DeliveryStatusCannotTransition = "Delivery status cannot transition from '{0}' to '{1}'.";
        public const string DeliveredCancellationRequiresVoidedUnits = "Item '{0}' is Delivered and cannot be cancelled until its delivery units are voided.";

        // Governance invariants I-23..I-27
        public const string ServicingRequiresMatchingSellerOrOverride = "The acting seller does not match the order's seller and the actor does not hold an airline override.";
        public const string RejectedWhileUnderExternalControl = "The delivery unit for traveller '{0}' on journey element '{1}' is under external control and the servicing command is rejected.";
        public const string OrderIsSuspended = "The order is suspended; only unsuspend is permitted.";
        public const string OrderIsUnderLegalHold = "The order is under legal hold.";
        public const string OrderIsClosed = "The order is closed and cannot be serviced.";
        public const string PaymentIsUnderDispute = "Payment record '{0}' is under dispute.";

        // Time limits
        public const string TimeLimitNotFound = "Time limit '{0}' was not found on this order.";
        public const string TimeLimitNotActive = "Time limit '{0}' is not Active.";
        public const string TimeLimitExtensionsExhausted = "Time limit '{0}' has reached the permitted number of extensions.";
        public const string TimeLimitMustBeInTheFuture = "A time limit due instant must be later than the current instant.";
        public const string TimeLimitExtensionMustBeLater = "A time limit extension must move the due instant later.";

        // Lookups and composition
        public const string TravelerNotFound = "Traveller '{0}' was not found on this order.";
        public const string JourneyElementNotFound = "Journey element '{0}' was not found on this order.";
        public const string OrderItemNotFound = "Order item '{0}' was not found on this order.";
        public const string PaymentRecordNotFound = "Payment record '{0}' was not found on this order.";
        public const string SegmentNotFound = "No traveller journey segment exists for traveller '{0}' on journey element '{1}'.";
        public const string DuplicateTravelerId = "Traveller '{0}' already exists on this order.";
        public const string DuplicateJourneyElementId = "Journey element '{0}' already exists on this order.";
        public const string DuplicateOrderItemId = "Order item '{0}' already exists on this order.";
        public const string DuplicatePaymentRecordId = "Payment record '{0}' already exists on this order.";

        // Limits
        public const string TravelerLimitExceeded = "The order exceeds the permitted number of travellers ({0}).";
        public const string JourneyElementLimitExceeded = "The order exceeds the permitted number of journey elements ({0}).";
        public const string ItemLimitExceeded = "The order exceeds the permitted number of items ({0}).";
        public const string NameIsTooLong = "The name exceeds the permitted length ({0}).";

        // Capabilities
        public const string ItemTypeNotEnabled = "Item type '{0}' is not enabled.";
        public const string FormOfPaymentNotEnabled = "Form of payment '{0}' is not enabled for channel '{1}'.";
        public const string ChannelNotEnabled = "Channel '{0}' is not enabled.";

        // Composition validation
        public const string IdentifierIsRequired = "{0} requires a non-empty value.";
        public const string GivenNameIsRequired = "A traveller given name is required.";
        public const string SurnameIsRequired = "A traveller surname is required.";
        public const string DateOfBirthRequiredForPassengerType = "A date of birth is required for passenger type '{0}'.";
        public const string ContactValueIsRequired = "A contact point requires a value.";
        public const string OnlyOnePrimaryContactPerType = "Only one primary contact point is permitted per contact type ('{0}').";
        public const string CurrencyCodeIsInvalid = "'{0}' is not a valid ISO 4217 alpha-3 currency code.";
        public const string CancellationRequiresReason = "A cancellation reason is required when an item is cancelled.";
        public const string AssociatedAdultMustNotBeSelf = "Traveller '{0}' cannot be its own associated adult.";
        public const string AssociatedAdultMustBeAdult = "The associated adult of traveller '{0}' must be an adult traveller.";

        // Groups I-28, I-29
        public const string CannotReleaseMoreSeatsThanHeld = "Seat block '{0}' cannot release more seats than it holds.";
        public const string AllocatedSlotsCannotExceedHeldBlock = "The group cannot allocate more name slots than the held block ({0}).";
        public const string SeatBlockNotFound = "Seat block '{0}' was not found on this group booking.";
        public const string NameSlotNotFound = "Name slot '{0}' was not found on this group booking.";
        public const string SlotIsNotUnallocated = "Name slot '{0}' is not Unallocated.";
        public const string SlotIsNotAllocated = "Name slot '{0}' is not Allocated.";
        public const string SlotCannotTransition = "Name slot '{0}' cannot transition from '{1}' to '{2}'.";
        public const string BlocksMustBeConfirmedBeforeAllocation = "Group seat blocks must be confirmed before a name is allocated.";
        public const string GroupIsSuspended = "The group booking is suspended.";
        public const string GroupIsClosed = "The group booking is closed.";
        public const string DuplicateBlockId = "Seat block '{0}' already exists on this group booking.";
        public const string DuplicateSlotId = "Name slot '{0}' already exists on this group booking.";
        public const string SeatsHeldMustBePositive = "A seat block must hold a positive number of seats.";

        // Delivery I-30, I-33, I-34
        public const string DeliveryUnitUnderExternalControl = "Delivery unit '{0}' is under external control by '{1}'; the servicing command is rejected.";
        public const string SegmentDeliveryCannotTransition = "Segment delivery unit '{0}' cannot transition from '{1}' to '{2}'.";
        public const string ServiceDeliveryCannotTransition = "Service delivery unit '{0}' cannot transition from '{1}' to '{2}'.";
        public const string VoidWindowExpired = "The void window for delivery record '{0}' has expired.";
        public const string AllUnitsMustBeOpenToVoid = "Delivery record '{0}' can be voided only while every unit is Open.";
        public const string DeliveryRecordRequiresAtLeastOneUnit = "A delivery record requires at least one segment or service unit.";
        public const string SegmentUnitNotFound = "Segment delivery unit '{0}' was not found on this delivery record.";
        public const string ServiceUnitNotFound = "Service delivery unit '{0}' was not found on this delivery record.";
        public const string UnitValuesMustMatchAllocation = "Segment unit values must sum to the value allocation of their originating items.";
        public const string ControlAlreadyHeld = "Delivery unit '{0}' is already under control of '{1}'.";
        public const string ControlNotHeld = "Delivery unit '{0}' is not under external control.";
        public const string ValueReturnedBeforeUnitsWithdrawn = "Value cannot be returned for item '{0}' before its delivery units are withdrawn.";

        // Tax documents I-31, I-32
        public const string ReversalRequiresReversedDocument = "A '{0}' document must reference the document it reverses.";
        public const string InvoiceCannotReferenceReversedDocument = "An Invoice must not reference a reversed document.";
        public const string TaxDocumentRequiresAtLeastOneLine = "A tax document requires at least one line.";
        public const string TaxDocumentTotalsMustMatchLines = "The tax document totals do not equal the sum of its lines.";
        public const string SubmissionCannotTransition = "Tax document '{0}' cannot move submission status from '{1}' to '{2}'.";
        public const string SpecVersionIsRequired = "A tax document must record the specification version that produced it.";
        public const string PassengerIdentityIsRequired = "A tax document requires a passenger identity.";

        // Document number ranges I-30
        public const string RangeIsNotActive = "Document number range '{0}' is not Active.";
        public const string RangeIsExhausted = "Document number range '{0}' is exhausted.";
        public const string RangeBoundsAreInvalid = "A document number range must start at or before its end.";
        public const string RangeKindMismatch = "Document number range '{0}' does not issue numbers of kind '{1}'.";

        // Operations
        public const string OperationCannotTransition = "Operation '{0}' cannot transition from '{1}' to '{2}'.";
        public const string OperationIsTerminal = "Operation '{0}' is in terminal status '{1}'.";
        public const string OperationStepNotOpen = "Operation '{0}' has no open step to complete.";
        public const string ExpiryBlockedByOpenPaymentCompletion = "Item '{0}' is covered by an open PaymentCompletion operation and its time limit may not expire.";

        // Shared kernel value objects: 2400-2419
        public const string AirportCodeIsInvalid = "Airport code '{0}' is invalid; it must be three letters.";
        public const string CarrierCodeIsInvalid = "Carrier code '{0}' is invalid; it must be two or three alphanumeric characters.";
        public const string CountryCodeIsInvalid = "Country code '{0}' is invalid; it must be two letters.";
        public const string OrderReferenceIsInvalid = "Order reference '{0}' is invalid; it must be 10 to 12 unambiguous Base32 characters.";
        public const string ReferenceValueIsRequired = "A '{0}' reference value is required.";

        // Structural invariants INV-001..INV-012: 2420-2449
        public const string OrderRequiresTravelerAndItemToConfirm = "An order must contain at least one traveller and one order item before an item can be confirmed.";
        public const string ItemBeneficiaryMustBeOrderTraveler = "Order item '{0}' names beneficiary '{1}', who is not a traveller in this order.";
        public const string EntitlementBeneficiaryMustBeOrderTraveler = "Entitlement '{0}' names beneficiary '{1}', who is not a traveller in this order.";
        public const string EntitlementSegmentMustExistInOrder = "Entitlement '{0}' references journey segment '{1}', which does not exist in this order.";
        public const string EntitlementJourneyMustExistInOrder = "Entitlement '{0}' references journey '{1}', which does not exist in this order.";
        public const string JourneySegmentSequenceIsInvalid = "Journey '{0}' must reference its segments in a contiguous ascending sequence.";
        public const string InfantRequiresAssociatedAdultTraveler = "Infant traveller '{0}' requires an AssociatedAdult association to a traveller in this order.";
        public const string DateOfBirthRequiredForTravelerType = "Traveller '{0}' of type '{1}' requires a date of birth.";
        public const string CannotRemoveReferencedTraveler = "Traveller '{0}' cannot be removed while referenced by a non-terminal order item or active entitlement.";
        public const string CannotRemoveReferencedJourneySegment = "Journey segment '{0}' cannot be removed while referenced by an active entitlement.";
        public const string OrderReferenceIsImmutable = "The order reference is immutable after order creation.";
        public const string CreatedSalesContextIsImmutable = "The created sales context is immutable.";
        public const string DuplicateJourneyId = "Journey '{0}' already exists in this order.";
        public const string DuplicateJourneySegmentId = "Journey segment '{0}' already exists in this order.";
        public const string DuplicateEntitlementId = "Entitlement '{0}' already exists in this order.";
        public const string DuplicateContactId = "Contact '{0}' already exists in this order.";
        public const string JourneyNotFound = "Journey '{0}' was not found on this order.";
        public const string JourneySegmentNotFound = "Journey segment '{0}' was not found on this order.";
        public const string EntitlementNotFound = "Entitlement '{0}' was not found on this order.";
        public const string ContactNotFound = "Contact '{0}' was not found on this order.";
        public const string ContactTravelerMustExistInOrder = "Contact '{0}' references traveller '{1}', who is not a traveller in this order.";
        public const string AssociatedTravelerMustExistInOrder = "Traveller '{0}' is associated with traveller '{1}', who is not in this order.";

        // Commercial invariants INV-020..INV-031: 2450-2479
        public const string ChargeLinesMustSumToPriceTotal = "The charge lines of order item '{0}' do not sum to the price snapshot total.";
        public const string ValueAllocationsMustSumToPriceTotal = "The value allocations of order item '{0}' do not sum to the price snapshot total.";
        public const string MultiSegmentAirTransportRequiresAllocations = "Multi-segment AirTransport item '{0}' requires complete value allocations before fulfilment.";
        public const string ConfirmedItemRequiresEntitlement = "Order item '{0}' cannot be confirmed without at least one entitlement.";
        public const string NegativeChargeLineNotPermitted = "A negative amount is not permitted on a charge line of type '{0}'.";
        public const string ChargeLineCurrencyMustMatchPrice = "Charge line '{0}' does not use the price snapshot currency.";
        public const string AllocationCurrencyMustMatchPrice = "Value allocation '{0}' does not use the price snapshot currency.";
        public const string AllocationEntitlementMustBelongToItem = "Value allocation references entitlement '{0}', which does not belong to order item '{1}'.";
        public const string DuplicateChargeLineSequence = "Order item '{0}' already has a charge line at sequence '{1}'.";
        public const string PriceRequiresAtLeastOneChargeLine = "A price snapshot requires at least one charge line.";
        public const string EntitlementBelongsToAnotherItem = "Entitlement '{0}' already belongs to order item '{1}'.";

        // Entitlement invariants INV-040..INV-048: 2480-2509
        public const string SeatEntitlementScopeIsInvalid = "Seat entitlement '{0}' must apply to exactly one traveller and exactly one segment.";
        public const string AirTransportEntitlementScopeIsInvalid = "AirTransport entitlement '{0}' must apply to exactly one traveller and at least one segment.";
        public const string BaggageAllowanceIsIncomplete = "Baggage entitlement '{0}' must define a '{1}' allowance consistent with its allowance kind.";
        public const string AccommodationDatesAreInvalid = "Accommodation entitlement '{0}' requires a check-out date later than its check-in date.";
        public const string AccommodationRequiresBeneficiary = "Accommodation entitlement '{0}' requires at least one beneficiary.";
        public const string JourneyScopedSegmentsMustBelongToJourney = "Entitlement '{0}' references segments that do not belong to journey '{1}'.";
        public const string CapacityCommitmentIsRequired = "Entitlement '{0}' requires a capacity commitment reference before it can become active.";
        public const string ResponsibilityAssignmentIsRequired = "Entitlement '{0}' is externally supplied and requires a responsibility assignment.";
        public const string EntitlementCannotTransition = "Entitlement '{0}' cannot transition from '{1}' to '{2}'.";
        public const string EntitlementIsTerminal = "Entitlement '{0}' is in terminal status '{1}' and cannot become active again.";
        public const string EntitlementRequiresBeneficiary = "Entitlement '{0}' requires at least one beneficiary.";
        public const string EntitlementSpecificationMismatch = "Entitlement '{0}' of type '{1}' cannot carry a '{2}' specification.";
        public const string OccupancyMustBePositive = "Accommodation entitlement '{0}' requires a positive adult occupancy.";
        public const string OccupancyCannotBeNegative = "Accommodation entitlement '{0}' cannot carry a negative child occupancy.";
        public const string LoungeAccessCountMustBePositive = "Lounge entitlement '{0}' requires a positive access count.";
        public const string InsuranceCoverageDatesAreInvalid = "Insurance entitlement '{0}' requires a coverage end on or after its coverage start.";

        // Item status invariants INV-062 and transitions: 2510-2529
        public const string OrderItemCannotTransition = "Order item '{0}' cannot transition from '{1}' to '{2}'.";
        public const string OrderItemIsTerminal = "Order item '{0}' is in terminal status '{1}'.";
        public const string OrderItemStatusIsIndeterminate = "The entitlement states of order item '{0}' do not map to a valid commercial status.";
        public const string PriceSnapshotIsImmutable = "The price snapshot of order item '{0}' is immutable; repricing must create a new order item.";
        public const string CommercialSourceIsImmutable = "The commercial source of order item '{0}' is immutable.";
        public const string SettlementArrangementIsImmutable = "The settlement arrangement of order item '{0}' is immutable.";

        // Time limits and locks INV-080..INV-085: 2530-2549
        public const string TimeLimitScopeMustExistInOrder = "Time limit '{0}' references '{1}' '{2}', which does not exist in this order.";
        public const string TimeLimitIsTerminal = "Time limit '{0}' is in terminal status '{1}' and cannot change.";
        public const string DuplicateTimeLimitId = "Time limit '{0}' already exists in this order.";
        public const string ProcessingLockConflict = "A '{0}' processing lock conflicts with the active '{1}' lock '{2}' over an overlapping scope.";
        public const string ProcessingLockNotFound = "Processing lock '{0}' was not found on this order.";
        public const string DuplicateProcessingLockId = "Processing lock '{0}' already exists in this order.";
        public const string ProcessingLockScopeMustExistInOrder = "Processing lock '{0}' references '{1}' '{2}', which does not exist in this order.";
        public const string ProcessingLockExpiryMustBeLater = "A processing lock must expire after it is acquired.";

        // Authority and lifecycle: 2550-2569
        public const string ActorLacksServicingAuthority = "The acting party lacks servicing authority over this order and holds no valid delegation or airline override.";
        public const string DelegationNotFound = "Servicing delegation '{0}' was not found on this order.";
        public const string DuplicateDelegationId = "Servicing delegation '{0}' already exists on this order.";
        public const string DelegationValidityIsInvalid = "A servicing delegation must remain valid until after it becomes valid.";
        public const string OrderAlreadyClosed = "The order is already closed.";
        public const string OrderCannotCloseWithActiveContent = "The order cannot be closed while it holds active entitlements or open time limits.";
        public const string DuplicateExternalReference = "An external reference for system '{0}', type '{1}' and value '{2}' already exists on this order.";
        public const string ExternalReferenceNotFound = "External reference '{0}' was not found on this order.";
        public const string ExternalReferenceScopeTargetMissing = "External reference scope '{0}' requires a scoped entity that exists in this order.";

        // Split and lineage INV-100..INV-104: 2570-2589
        public const string SplitRequiresTravelers = "A split must move at least one traveller.";
        public const string SplitCannotMoveAllTravelers = "A split cannot move every traveller out of the source order.";
        public const string SplitTravelerMustExistInOrder = "Split traveller '{0}' does not exist in the source order.";
        public const string SplitWouldOrphanOrderItem = "Order item '{0}' has beneficiaries on both sides of the split and cannot be divided.";
        public const string SplitBlockedByProcessingLock = "A split cannot proceed while processing lock '{0}' covers the affected scope.";
        public const string PredecessorItemMustExistInOrder = "Predecessor order item '{0}' does not exist in this order.";

        // Fulfillment invariants FUL-001..FUL-043: 2600-2649
        public const string TicketCouponCannotTransition = "Ticket coupon '{0}' cannot transition from '{1}' to '{2}'.";
        public const string TicketCouponRequiresControlAuthority = "Ticket coupon '{0}' is in status '{1}' and requires control authority to service.";
        public const string FlownCouponCannotBeVoided = "Ticket coupon '{0}' is Flown and cannot be voided.";
        public const string TicketRequiresAtLeastOneCoupon = "An electronic ticket requires at least one coupon.";
        public const string DuplicateCouponNumber = "Coupon number '{0}' already exists on this document.";
        public const string DuplicateActiveFulfillmentUnit = "An active fulfilment unit already exists for entitlement '{0}'.";
        public const string TicketCouponNotFound = "Ticket coupon '{0}' was not found on this ticket.";
        public const string EmdCouponNotFound = "EMD coupon '{0}' was not found on this document.";
        public const string EmdCouponCannotTransition = "EMD coupon '{0}' cannot transition from '{1}' to '{2}'.";
        public const string EmdRequiresAtLeastOneCoupon = "An electronic miscellaneous document requires at least one coupon.";
        public const string SupplierReservationCannotTransition = "Supplier reservation '{0}' cannot transition from '{1}' to '{2}'.";
        public const string ConfirmedReservationRequiresConfirmationNumber = "Supplier reservation '{0}' requires an external confirmation number to be confirmed.";
        public const string ReservationRequiresFulfillmentUnit = "Supplier reservation '{0}' requires at least one fulfilment unit.";
        public const string FulfillmentUnitNotFound = "Fulfilment unit '{0}' was not found on this aggregate.";
        public const string ControlHolderIsRequired = "Transferring coupon control requires a control holder.";
        public const string DocumentNumberIsInvalid = "Document number '{0}' is invalid.";
        public const string DocumentStockRangeIsInvalid = "A document stock range must start at or before its end.";
        public const string DocumentStockIsExhausted = "Document stock '{0}' is exhausted.";
        public const string DocumentStockIsNotActive = "Document stock '{0}' is not active.";
        public const string CouponSegmentIsRequired = "Ticket coupon '{0}' requires a journey segment reference.";

        // Consumption and reconciliation: 2650-2679
        public const string ConsumptionFactSourceIsRequired = "A consumption fact requires a source system and source event id.";
        public const string ConsumptionQuantityUnitIsRequired = "A consumption fact with a quantity value requires a quantity unit.";
        public const string ReconciliationCaseIsResolved = "Reconciliation case '{0}' is already in terminal status '{1}'.";
        public const string ReconciliationCaseCannotTransition = "Reconciliation case '{0}' cannot transition from '{1}' to '{2}'.";
        public const string ReconciliationResolutionRequiresClassification = "Reconciliation case '{0}' requires a classification before it can be resolved.";
    }
}

namespace AeroTech.Ordering.Domain.SharedKernel.Enums
{
    public enum OrderItemCommercialStatus
    {
        Pending = 1,
        Confirmed = 2,
        PartiallyChanged = 3,
        Cancelled = 4,
        Replaced = 5
    }

    public enum EntitlementCommercialStatus
    {
        Pending = 1,
        Active = 2,
        Cancelled = 3,
        Replaced = 4
    }

    public enum OrderLifecycle
    {
        Draft = 1,
        Active = 2,
        Cancelled = 3,
        Closed = 4
    }

    public enum ProductType
    {
        AirTransport = 1,
        Seat = 2,
        Baggage = 3,
        Meal = 4,
        Wifi = 5,
        Lounge = 6,
        FastTrack = 7,
        Upgrade = 8,
        Accommodation = 9,
        Transfer = 10,
        Insurance = 11,
        Fee = 12,
        Other = 13
    }

    public enum EntitlementType
    {
        AirTransport = 1,
        SeatAssignment = 2,
        CheckedBaggage = 3,
        CabinBaggage = 4,
        Meal = 5,
        WifiAccess = 6,
        LoungeAccess = 7,
        FastTrack = 8,
        Upgrade = 9,
        AccommodationStay = 10,
        TransferRide = 11,
        InsuranceCoverage = 12,
        Assistance = 13,
        Other = 14
    }

    public enum CommercialSourceType
    {
        RetailOffer = 1,
        ServicingOffer = 2,
        CharterAllocation = 3,
        GroupAgreement = 4,
        CorporateAgreement = 5,
        DisruptionAuthority = 6,
        StaffTravelAuthority = 7,
        GoodwillAuthority = 8,
        ManualCommercialAuthority = 9
    }

    public enum SettlementModel
    {
        PassengerRetail = 1,
        AgencyCredit = 2,
        AgencyPrepaid = 3,
        Corporate = 4,
        CharterPrepaid = 5,
        CharterPostpaid = 6,
        GroupDepositAllocation = 7,
        Complimentary = 8,
        LoyaltyRedemption = 9
    }

    public enum CapacityCommitmentType
    {
        PublicInventoryHold = 1,
        ConfirmedInventory = 2,
        GroupBlock = 3,
        CharterAllocation = 4,
        AgencyAllotment = 5,
        PartnerInventory = 6,
        SupplierInventory = 7
    }

    public enum ChargeType
    {
        Base = 1,
        Tax = 2,
        CarrierFee = 3,
        ServiceFee = 4,
        Discount = 5,
        Penalty = 6,
        OtherCharge = 7
    }

    public enum AllocationPurpose
    {
        Accounting = 1,
        Fulfillment = 2,
        Settlement = 3,
        Reporting = 4
    }

    public enum BaggageAllowanceKind
    {
        Weight = 1,
        Piece = 2
    }
}

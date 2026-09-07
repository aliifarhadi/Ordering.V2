namespace AeroTech.Ordering.Domain.SharedKernel.Enums
{
    public enum ConsumptionFactType
    {
        PassengerCheckedIn = 1,
        PassengerBoarded = 2,
        PassengerOffloaded = 3,
        PassengerFlown = 4,
        PassengerNoShow = 5,
        BaggageAccepted = 6,
        BagLoaded = 7,
        BagTransferred = 8,
        BagDelivered = 9,
        MealDelivered = 10,
        LoungeEntered = 11,
        WifiActivated = 12,
        HotelCheckedIn = 13,
        HotelCheckedOut = 14,
        TransferCompleted = 15
    }

    public enum ReconciliationCaseType
    {
        BaggageDiscrepancy = 1,
        UnfulfilledEntitlement = 2,
        UnexpectedConsumption = 3,
        DuplicateFulfillment = 4,
        Other = 5
    }

    public enum ReconciliationCaseStatus
    {
        Open = 1,
        AwaitingCommercialization = 2,
        AwaitingManualReview = 3,
        Resolved = 4,
        Ignored = 5
    }

    public enum ReconciliationClassification
    {
        PurchasedAtAirport = 1,
        Waived = 2,
        LoyaltyBenefit = 3,
        StaffOverride = 4,
        UncollectedRevenue = 5,
        MeasurementCorrection = 6,
        OperationalException = 7
    }

    public enum WorkflowStatus
    {
        Running = 1,
        AwaitingExternal = 2,
        AwaitingManualReview = 3,
        Completed = 4,
        Failed = 5,
        Compensated = 6
    }
}

namespace AeroTech.Ordering.Domain.SharedKernel.Enums
{
    public enum TimeLimitType
    {
        Payment = 1,
        OfferAcceptance = 2,
        InventoryHold = 3,
        Ticketing = 4,
        NameEntry = 5,
        SupplierConfirmation = 6,
        PartnerConfirmation = 7
    }

    public enum TimeLimitStatus
    {
        Active = 1,
        Met = 2,
        Expired = 3,
        Cancelled = 4
    }

    public enum ProcessingLockType
    {
        PaymentCompletion = 1,
        Fulfillment = 2,
        VoluntaryChange = 3,
        Cancellation = 4,
        Refund = 5,
        Void = 6,
        Split = 7,
        PassengerChange = 8,
        Reaccommodation = 9,
        SupplierBooking = 10,
        Reconciliation = 11
    }

    public enum ExternalReferenceScope
    {
        Order = 1,
        Traveler = 2,
        OrderItem = 3,
        Entitlement = 4,
        Fulfillment = 5
    }

    public enum ContactRole
    {
        BookingContact = 1,
        TravelerContact = 2,
        EmergencyContact = 3,
        NotificationContact = 4,
        AgencyContact = 5
    }

    public enum ContactType
    {
        Email = 1,
        Mobile = 2,
        Phone = 3
    }

    public enum TravelerType
    {
        ADT = 1,
        CHD = 2,
        INF = 3
    }

    public enum TravelerAssociationType
    {
        AssociatedAdult = 1,
        ResponsibleParty = 2,
        Escort = 3,
        Companion = 4
    }

    public enum ActorType
    {
        AirlineAgent = 1,
        AgencyAgent = 2,
        Customer = 3,
        System = 4,
        Partner = 5
    }

    public enum BuyerPartyType
    {
        Individual = 1,
        Corporate = 2,
        Agency = 3,
        CharterOperator = 4
    }

    public enum IdentityDocumentType
    {
        Passport = 1,
        NationalIdentityCard = 2,
        Visa = 3,
        ResidencePermit = 4,
        Other = 5
    }

    public enum ServicingAuthorityType
    {
        Cancel = 1,
        VoluntaryChange = 2,
        Refund = 3,
        AddAncillary = 4,
        UpdateTraveler = 5,
        Split = 6,
        ManageTimeLimits = 7,
        ManageExternalReferences = 8
    }

    public enum FulfillmentLinkType
    {
        ElectronicTicket = 1,
        ElectronicMiscDocument = 2,
        SupplierReservation = 3
    }
}

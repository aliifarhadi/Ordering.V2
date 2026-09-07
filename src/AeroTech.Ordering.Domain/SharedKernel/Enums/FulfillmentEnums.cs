namespace AeroTech.Ordering.Domain.SharedKernel.Enums
{
    public enum ElectronicTicketStatus
    {
        Issued = 1,
        Void = 2,
        Exchanged = 3,
        Closed = 4
    }

    public enum TicketCouponStatus
    {
        Open = 1,
        Controlled = 2,
        CheckedIn = 3,
        Boarded = 4,
        Flown = 5,
        NoShow = 6,
        Void = 7,
        Exchanged = 8,
        Refunded = 9
    }

    public enum ElectronicMiscDocumentStatus
    {
        Issued = 1,
        Void = 2,
        Exchanged = 3,
        Closed = 4
    }

    public enum EmdCouponStatus
    {
        Open = 1,
        Consumed = 2,
        Void = 3,
        Exchanged = 4,
        Refunded = 5
    }

    public enum EmdType
    {
        Associated = 1,
        Standalone = 2
    }

    public enum SupplierReservationStatus
    {
        Pending = 1,
        Confirmed = 2,
        Cancelled = 3,
        Rejected = 4,
        Completed = 5
    }

    public enum FulfillmentUnitStatus
    {
        Pending = 1,
        Active = 2,
        Completed = 3,
        Cancelled = 4,
        Withdrawn = 5
    }

    public enum FulfillmentUnitType
    {
        TicketCoupon = 1,
        EmdCoupon = 2,
        AccommodationStay = 3,
        TransferRide = 4,
        LoungeAccess = 5,
        Other = 6
    }

    public enum DocumentStockType
    {
        Ticket = 1,
        Emd = 2
    }

    public enum DocumentStockStatus
    {
        Active = 1,
        Exhausted = 2,
        Suspended = 3,
        Closed = 4
    }
}

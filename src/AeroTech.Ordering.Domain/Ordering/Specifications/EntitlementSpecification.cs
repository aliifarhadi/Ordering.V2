using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.Specifications
{
    public abstract class EntitlementSpecification
    {
        protected EntitlementSpecification()
        {
        }

        public EntitlementId EntitlementId { get; internal set; }

        public abstract bool SupportsEntitlementType(EntitlementType type);

        internal abstract void Validate(EntitlementId entitlementId);
    }

    public sealed class AirTransportSpecification : EntitlementSpecification
    {
        private AirTransportSpecification()
        {
        }

        public AirTransportSpecification(string cabin, string? rbd, string? brandCode, string? fareBasisCode)
        {
            if (string.IsNullOrWhiteSpace(cabin))
                throw ExceptionFactory.IdentifierIsRequired(nameof(cabin));

            Cabin = cabin;
            Rbd = rbd;
            BrandCode = brandCode;
            FareBasisCode = fareBasisCode;
        }

        public string Cabin { get; private set; } = null!;

        public string? Rbd { get; private set; }

        public string? BrandCode { get; private set; }

        public string? FareBasisCode { get; private set; }

        public override bool SupportsEntitlementType(EntitlementType type) =>
            type is EntitlementType.AirTransport or EntitlementType.Upgrade;

        internal override void Validate(EntitlementId entitlementId)
        {
        }
    }

    public sealed class SeatSpecification : EntitlementSpecification
    {
        private readonly List<string> _characteristics = [];

        private SeatSpecification()
        {
        }

        public SeatSpecification(string? seatNumber, IEnumerable<string>? characteristics = null)
        {
            SeatNumber = seatNumber;
            if (characteristics is not null)
                _characteristics.AddRange(characteristics);
        }

        public string? SeatNumber { get; private set; }

        public IReadOnlyCollection<string> Characteristics => _characteristics.AsReadOnly();

        public override bool SupportsEntitlementType(EntitlementType type) => type is EntitlementType.SeatAssignment;

        internal override void Validate(EntitlementId entitlementId)
        {
        }
    }

    public sealed class BaggageSpecification : EntitlementSpecification
    {
        private BaggageSpecification()
        {
        }

        public BaggageSpecification(
            BaggageAllowanceKind allowanceKind,
            decimal? weightKg,
            int? pieces,
            decimal? maxPieceWeightKg,
            string? dimensionRuleCode)
        {
            AllowanceKind = allowanceKind;
            WeightKg = weightKg;
            Pieces = pieces;
            MaxPieceWeightKg = maxPieceWeightKg;
            DimensionRuleCode = dimensionRuleCode;
        }

        public BaggageAllowanceKind AllowanceKind { get; private set; }

        public decimal? WeightKg { get; private set; }

        public int? Pieces { get; private set; }

        public decimal? MaxPieceWeightKg { get; private set; }

        public string? DimensionRuleCode { get; private set; }

        public override bool SupportsEntitlementType(EntitlementType type) =>
            type is EntitlementType.CheckedBaggage or EntitlementType.CabinBaggage;

        internal override void Validate(EntitlementId entitlementId)
        {
            switch (AllowanceKind)
            {
                case BaggageAllowanceKind.Weight when WeightKg is null or <= 0:
                    throw ExceptionFactory.BaggageAllowanceIsIncomplete(entitlementId, nameof(BaggageAllowanceKind.Weight));
                case BaggageAllowanceKind.Piece when Pieces is null or <= 0:
                    throw ExceptionFactory.BaggageAllowanceIsIncomplete(entitlementId, nameof(BaggageAllowanceKind.Piece));
            }
        }
    }

    public sealed class MealSpecification : EntitlementSpecification
    {
        private readonly List<string> _dietaryAttributes = [];

        private MealSpecification()
        {
        }

        public MealSpecification(string mealCode, IEnumerable<string>? dietaryAttributes = null)
        {
            if (string.IsNullOrWhiteSpace(mealCode))
                throw ExceptionFactory.IdentifierIsRequired(nameof(mealCode));

            MealCode = mealCode;
            if (dietaryAttributes is not null)
                _dietaryAttributes.AddRange(dietaryAttributes);
        }

        public string MealCode { get; private set; } = null!;

        public IReadOnlyCollection<string> DietaryAttributes => _dietaryAttributes.AsReadOnly();

        public override bool SupportsEntitlementType(EntitlementType type) => type is EntitlementType.Meal;

        internal override void Validate(EntitlementId entitlementId)
        {
        }
    }

    public sealed class LoungeSpecification : EntitlementSpecification
    {
        private LoungeSpecification()
        {
        }

        public LoungeSpecification(string? loungeCode, int accessCount)
        {
            LoungeCode = loungeCode;
            AccessCount = accessCount;
        }

        public string? LoungeCode { get; private set; }

        public int AccessCount { get; private set; }

        public override bool SupportsEntitlementType(EntitlementType type) => type is EntitlementType.LoungeAccess;

        internal override void Validate(EntitlementId entitlementId)
        {
            if (AccessCount <= 0)
                throw ExceptionFactory.LoungeAccessCountMustBePositive(entitlementId);
        }
    }

    public sealed class AccommodationSpecification : EntitlementSpecification
    {
        private AccommodationSpecification()
        {
        }

        public AccommodationSpecification(
            string propertyRef,
            string propertyName,
            string roomType,
            string boardBasis,
            int occupancyAdults,
            int occupancyChildren,
            LocalDate checkInDate,
            LocalDate checkOutDate,
            string? ratePlanCode = null,
            string? cancellationPolicyText = null)
        {
            if (string.IsNullOrWhiteSpace(propertyRef))
                throw ExceptionFactory.IdentifierIsRequired(nameof(propertyRef));
            if (string.IsNullOrWhiteSpace(propertyName))
                throw ExceptionFactory.IdentifierIsRequired(nameof(propertyName));
            if (string.IsNullOrWhiteSpace(roomType))
                throw ExceptionFactory.IdentifierIsRequired(nameof(roomType));
            if (string.IsNullOrWhiteSpace(boardBasis))
                throw ExceptionFactory.IdentifierIsRequired(nameof(boardBasis));

            PropertyRef = propertyRef;
            PropertyName = propertyName;
            RoomType = roomType;
            BoardBasis = boardBasis;
            OccupancyAdults = occupancyAdults;
            OccupancyChildren = occupancyChildren;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            RatePlanCode = ratePlanCode;
            CancellationPolicyText = cancellationPolicyText;
        }

        public string PropertyRef { get; private set; } = null!;

        public string PropertyName { get; private set; } = null!;

        public string RoomType { get; private set; } = null!;

        public string BoardBasis { get; private set; } = null!;

        public int OccupancyAdults { get; private set; }

        public int OccupancyChildren { get; private set; }

        public LocalDate CheckInDate { get; private set; }

        public LocalDate CheckOutDate { get; private set; }

        public string? RatePlanCode { get; private set; }

        public string? CancellationPolicyText { get; private set; }

        public override bool SupportsEntitlementType(EntitlementType type) => type is EntitlementType.AccommodationStay;

        internal override void Validate(EntitlementId entitlementId)
        {
            if (CheckOutDate <= CheckInDate)
                throw ExceptionFactory.AccommodationDatesAreInvalid(entitlementId);
            if (OccupancyAdults <= 0)
                throw ExceptionFactory.OccupancyMustBePositive(entitlementId);
            if (OccupancyChildren < 0)
                throw ExceptionFactory.OccupancyCannotBeNegative(entitlementId);
        }
    }

    public sealed class TransferSpecification : EntitlementSpecification
    {
        private TransferSpecification()
        {
        }

        public TransferSpecification(
            string originLocation,
            string destinationLocation,
            LocalDate serviceDate,
            string? vehicleClass = null)
        {
            if (string.IsNullOrWhiteSpace(originLocation))
                throw ExceptionFactory.IdentifierIsRequired(nameof(originLocation));
            if (string.IsNullOrWhiteSpace(destinationLocation))
                throw ExceptionFactory.IdentifierIsRequired(nameof(destinationLocation));

            OriginLocation = originLocation;
            DestinationLocation = destinationLocation;
            ServiceDate = serviceDate;
            VehicleClass = vehicleClass;
        }

        public string OriginLocation { get; private set; } = null!;

        public string DestinationLocation { get; private set; } = null!;

        public LocalDate ServiceDate { get; private set; }

        public string? VehicleClass { get; private set; }

        public override bool SupportsEntitlementType(EntitlementType type) => type is EntitlementType.TransferRide;

        internal override void Validate(EntitlementId entitlementId)
        {
        }
    }

    public sealed class InsuranceSpecification : EntitlementSpecification
    {
        private InsuranceSpecification()
        {
        }

        public InsuranceSpecification(
            string policyProductCode,
            LocalDate coverageStart,
            LocalDate coverageEnd,
            string coverageSummary)
        {
            if (string.IsNullOrWhiteSpace(policyProductCode))
                throw ExceptionFactory.IdentifierIsRequired(nameof(policyProductCode));
            if (string.IsNullOrWhiteSpace(coverageSummary))
                throw ExceptionFactory.IdentifierIsRequired(nameof(coverageSummary));

            PolicyProductCode = policyProductCode;
            CoverageStart = coverageStart;
            CoverageEnd = coverageEnd;
            CoverageSummary = coverageSummary;
        }

        public string PolicyProductCode { get; private set; } = null!;

        public LocalDate CoverageStart { get; private set; }

        public LocalDate CoverageEnd { get; private set; }

        public string CoverageSummary { get; private set; } = null!;

        public override bool SupportsEntitlementType(EntitlementType type) => type is EntitlementType.InsuranceCoverage;

        internal override void Validate(EntitlementId entitlementId)
        {
            if (CoverageEnd < CoverageStart)
                throw ExceptionFactory.InsuranceCoverageDatesAreInvalid(entitlementId);
        }
    }
}

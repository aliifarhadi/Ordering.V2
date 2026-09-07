using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class IdentityDocument : Entity<IdentityDocumentId>
    {
        private IdentityDocument()
        {
        }

        private IdentityDocument(
            IdentityDocumentId id,
            TravelerId travelerId,
            IdentityDocumentType documentType,
            string documentNumber,
            CountryCode? issuingCountry,
            LocalDate? expiryDate,
            CountryCode? nationality)
        {
            Id = id;
            TravelerId = travelerId;
            DocumentType = documentType;
            DocumentNumber = documentNumber;
            IssuingCountry = issuingCountry;
            ExpiryDate = expiryDate;
            Nationality = nationality;
        }

        public TravelerId TravelerId { get; private set; }

        public IdentityDocumentType DocumentType { get; private set; }

        public string DocumentNumber { get; private set; } = null!;

        public CountryCode? IssuingCountry { get; private set; }

        public LocalDate? ExpiryDate { get; private set; }

        public CountryCode? Nationality { get; private set; }

        internal static IdentityDocument Create(
            TravelerId travelerId,
            IdentityDocumentType documentType,
            string documentNumber,
            CountryCode? issuingCountry = null,
            LocalDate? expiryDate = null,
            CountryCode? nationality = null)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                throw ExceptionFactory.IdentifierIsRequired(nameof(documentNumber));

            return new IdentityDocument(
                IdentityDocumentId.New(),
                travelerId,
                documentType,
                documentNumber,
                issuingCountry,
                expiryDate,
                nationality);
        }
    }

    public sealed class LoyaltyAccountRef : Entity<LoyaltyAccountId>
    {
        private LoyaltyAccountRef()
        {
        }

        private LoyaltyAccountRef(
            LoyaltyAccountId id,
            TravelerId travelerId,
            string programCode,
            string accountRef,
            string? tierCode)
        {
            Id = id;
            TravelerId = travelerId;
            ProgramCode = programCode;
            AccountRef = accountRef;
            TierCode = tierCode;
        }

        public TravelerId TravelerId { get; private set; }

        public string ProgramCode { get; private set; } = null!;

        public string AccountRef { get; private set; } = null!;

        public string? TierCode { get; private set; }

        internal static LoyaltyAccountRef Create(
            TravelerId travelerId,
            string programCode,
            string accountRef,
            string? tierCode = null)
        {
            if (string.IsNullOrWhiteSpace(programCode))
                throw ExceptionFactory.IdentifierIsRequired(nameof(programCode));
            if (string.IsNullOrWhiteSpace(accountRef))
                throw ExceptionFactory.IdentifierIsRequired(nameof(accountRef));

            return new LoyaltyAccountRef(LoyaltyAccountId.New(), travelerId, programCode, accountRef, tierCode);
        }
    }

    public sealed class TravelerAssociation : Entity<TravelerAssociationId>
    {
        private TravelerAssociation()
        {
        }

        private TravelerAssociation(
            TravelerAssociationId id,
            TravelerId travelerId,
            TravelerId relatedTravelerId,
            TravelerAssociationType associationType)
        {
            Id = id;
            TravelerId = travelerId;
            RelatedTravelerId = relatedTravelerId;
            AssociationType = associationType;
        }

        public TravelerId TravelerId { get; private set; }

        public TravelerId RelatedTravelerId { get; private set; }

        public TravelerAssociationType AssociationType { get; private set; }

        internal static TravelerAssociation Create(
            TravelerId travelerId,
            TravelerId relatedTravelerId,
            TravelerAssociationType associationType)
        {
            if (travelerId == relatedTravelerId)
                throw ExceptionFactory.AssociatedAdultMustNotBeSelf(travelerId);

            return new TravelerAssociation(TravelerAssociationId.New(), travelerId, relatedTravelerId, associationType);
        }
    }
}

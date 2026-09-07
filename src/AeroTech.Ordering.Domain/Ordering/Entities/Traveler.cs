using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.Ordering.ValueObjects;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class Traveler : Entity<TravelerId>
    {
        private readonly List<IdentityDocument> _identityDocuments = [];
        private readonly List<LoyaltyAccountRef> _loyaltyAccounts = [];
        private readonly List<TravelerAssociation> _associations = [];

        private Traveler()
        {
        }

        private Traveler(
            TravelerId id,
            OrderId orderId,
            TravelerName name,
            TravelerType type,
            LocalDate? dateOfBirth,
            string? gender,
            string? customerRef)
        {
            Id = id;
            OrderId = orderId;
            Name = name;
            Type = type;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            CustomerRef = customerRef;
        }

        public OrderId OrderId { get; private set; }

        public TravelerName Name { get; private set; } = null!;

        public TravelerType Type { get; private set; }

        public LocalDate? DateOfBirth { get; private set; }

        public string? Gender { get; private set; }

        public string? CustomerRef { get; private set; }

        public RegulatoryDataSnapshot? RegulatoryData { get; private set; }

        public IReadOnlyCollection<IdentityDocument> IdentityDocuments => _identityDocuments.AsReadOnly();

        public IReadOnlyCollection<LoyaltyAccountRef> LoyaltyAccounts => _loyaltyAccounts.AsReadOnly();

        public IReadOnlyCollection<TravelerAssociation> Associations => _associations.AsReadOnly();

        internal static Traveler Create(
            TravelerId id,
            OrderId orderId,
            TravelerName name,
            TravelerType type,
            LocalDate? dateOfBirth,
            string? gender = null,
            string? customerRef = null)
        {
            // INV-008: CHD and INF require DateOfBirth.
            if (type is TravelerType.CHD or TravelerType.INF && dateOfBirth is null)
                throw ExceptionFactory.DateOfBirthRequiredForTravelerType(id, type);

            return new Traveler(id, orderId, name, type, dateOfBirth, gender, customerRef);
        }

        internal void UpdateDetails(TravelerName name, LocalDate? dateOfBirth, string? gender, string? customerRef)
        {
            if (Type is TravelerType.CHD or TravelerType.INF && dateOfBirth is null)
                throw ExceptionFactory.DateOfBirthRequiredForTravelerType(Id, Type);

            Name = name;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            CustomerRef = customerRef;
        }

        internal void SetRegulatoryData(RegulatoryDataSnapshot? regulatoryData) => RegulatoryData = regulatoryData;

        internal void AddIdentityDocument(IdentityDocument document) => _identityDocuments.Add(document);

        internal void RemoveIdentityDocument(IdentityDocumentId documentId) =>
            _identityDocuments.RemoveAll(document => document.Id == documentId);

        internal void AddLoyaltyAccount(LoyaltyAccountRef account) => _loyaltyAccounts.Add(account);

        internal void RemoveLoyaltyAccount(LoyaltyAccountId accountId) =>
            _loyaltyAccounts.RemoveAll(account => account.Id == accountId);

        internal void AddAssociation(TravelerAssociation association) => _associations.Add(association);

        internal void RemoveAssociation(TravelerAssociationId associationId) =>
            _associations.RemoveAll(association => association.Id == associationId);

        internal bool HasAssociatedAdult() =>
            _associations.Any(association => association.AssociationType is TravelerAssociationType.AssociatedAdult);
    }
}

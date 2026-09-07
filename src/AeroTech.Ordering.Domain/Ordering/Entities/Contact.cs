using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class Contact : Entity<ContactId>
    {
        private readonly List<ContactTravelerRef> _travelerRefs = [];

        private Contact()
        {
        }

        private Contact(
            ContactId id,
            OrderId orderId,
            ContactType type,
            string value,
            ContactRole role,
            bool isPrimary,
            IEnumerable<TravelerId> travelerRefs)
        {
            Id = id;
            OrderId = orderId;
            Type = type;
            Value = value;
            Role = role;
            IsPrimary = isPrimary;
            _travelerRefs.AddRange(travelerRefs.Distinct().Select(travelerId => new ContactTravelerRef(id, travelerId)));
        }

        public OrderId OrderId { get; private set; }

        public ContactType Type { get; private set; }

        public string Value { get; private set; } = null!;

        public ContactRole Role { get; private set; }

        public bool IsPrimary { get; private set; }

        public IReadOnlyCollection<ContactTravelerRef> TravelerRefs => _travelerRefs.AsReadOnly();

        public IEnumerable<TravelerId> TravelerIds => _travelerRefs.Select(reference => reference.TravelerId);

        public bool IsOrderLevel => _travelerRefs.Count == 0;

        internal static Contact Create(
            ContactId id,
            OrderId orderId,
            ContactType type,
            string value,
            ContactRole role,
            bool isPrimary,
            IEnumerable<TravelerId>? travelerRefs = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.ContactValueIsRequired();

            return new Contact(id, orderId, type, value, role, isPrimary, travelerRefs ?? []);
        }

        internal void Update(
            ContactType type,
            string value,
            ContactRole role,
            bool isPrimary,
            IEnumerable<TravelerId>? travelerRefs)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.ContactValueIsRequired();

            Type = type;
            Value = value;
            Role = role;
            IsPrimary = isPrimary;

            _travelerRefs.Clear();
            if (travelerRefs is not null)
                _travelerRefs.AddRange(travelerRefs.Distinct().Select(travelerId => new ContactTravelerRef(Id, travelerId)));
        }

        internal void Demote() => IsPrimary = false;
    }

    public sealed record ContactTravelerRef(ContactId ContactId, TravelerId TravelerId);
}

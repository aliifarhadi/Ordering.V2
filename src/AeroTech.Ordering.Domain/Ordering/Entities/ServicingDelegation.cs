using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class ServicingDelegation : Entity<ServicingDelegationId>
    {
        private readonly List<ServicingAuthorityType> _authorityTypes = [];

        private ServicingDelegation()
        {
        }

        private ServicingDelegation(
            ServicingDelegationId id,
            OrderId orderId,
            string delegatePartyRef,
            string scope,
            Instant validFrom,
            Instant? validUntil)
        {
            Id = id;
            OrderId = orderId;
            DelegatePartyRef = delegatePartyRef;
            Scope = scope;
            ValidFrom = validFrom;
            ValidUntil = validUntil;
        }

        public OrderId OrderId { get; private set; }

        public string DelegatePartyRef { get; private set; } = null!;

        public string Scope { get; private set; } = null!;

        public Instant ValidFrom { get; private set; }

        public Instant? ValidUntil { get; private set; }

        public IReadOnlyCollection<ServicingAuthorityType> AuthorityTypes => _authorityTypes.AsReadOnly();

        internal static ServicingDelegation Create(
            ServicingDelegationId id,
            OrderId orderId,
            string delegatePartyRef,
            string scope,
            Instant validFrom,
            Instant? validUntil,
            IEnumerable<ServicingAuthorityType> authorityTypes)
        {
            if (string.IsNullOrWhiteSpace(delegatePartyRef))
                throw ExceptionFactory.IdentifierIsRequired(nameof(delegatePartyRef));
            if (string.IsNullOrWhiteSpace(scope))
                throw ExceptionFactory.IdentifierIsRequired(nameof(scope));
            if (validUntil is not null && validUntil <= validFrom)
                throw ExceptionFactory.DelegationValidityIsInvalid();

            var delegation = new ServicingDelegation(id, orderId, delegatePartyRef, scope, validFrom, validUntil);
            delegation._authorityTypes.AddRange(authorityTypes.Distinct());
            return delegation;
        }

        internal void Revoke(Instant revokedAt) => ValidUntil = revokedAt;

        internal bool IsValidAt(Instant instant) =>
            instant >= ValidFrom && (ValidUntil is null || instant < ValidUntil);

        internal bool Grants(ServicingAuthorityType authorityType) =>
            _authorityTypes.Count == 0 || _authorityTypes.Contains(authorityType);
    }
}

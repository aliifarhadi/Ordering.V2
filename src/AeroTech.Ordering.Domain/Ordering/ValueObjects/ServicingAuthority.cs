using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain.Ordering.ValueObjects
{
    public sealed record ServicingAuthority
    {
        private ServicingAuthority()
        {
        }

        public ServicingAuthority(string ownerSellerId, string? ownerOfficeId, bool airlineOverrideAllowed)
        {
            if (string.IsNullOrWhiteSpace(ownerSellerId))
                throw ExceptionFactory.IdentifierIsRequired(nameof(ownerSellerId));

            OwnerSellerId = ownerSellerId;
            OwnerOfficeId = ownerOfficeId;
            AirlineOverrideAllowed = airlineOverrideAllowed;
        }

        public string OwnerSellerId { get; private set; } = null!;

        public string? OwnerOfficeId { get; private set; }

        public bool AirlineOverrideAllowed { get; private set; }
    }
}

using AeroTech.Ordering.Domain.SharedKernel.Enums;

namespace AeroTech.Ordering.Domain.SharedKernel.ValueObjects
{
    public sealed record ActorContext
    {
        private readonly IReadOnlyList<string> _roles;

        public ActorContext(
            ActorType actorType,
            string userId,
            string? sellerId,
            string? sellerOfficeId,
            bool hasAirlineOverride,
            IEnumerable<string>? roles = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw _Shared.Resources.ExceptionFactory.IdentifierIsRequired(nameof(userId));

            ActorType = actorType;
            UserId = userId;
            SellerId = sellerId;
            SellerOfficeId = sellerOfficeId;
            HasAirlineOverride = hasAirlineOverride;
            _roles = roles?.ToArray() ?? [];
        }

        public ActorType ActorType { get; }

        public string UserId { get; }

        public string? SellerId { get; }

        public string? SellerOfficeId { get; }

        public bool HasAirlineOverride { get; }

        public IReadOnlyList<string> Roles => _roles;
    }
}

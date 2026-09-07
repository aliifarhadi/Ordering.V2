using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using NodaTime;

namespace AeroTech.Ordering.Domain.SharedKernel.ValueObjects
{
    public sealed record SalesContext
    {
        private SalesContext()
        {
        }

        public SalesContext(
            string sellerId,
            string? sellerOfficeId,
            string channelCode,
            CountryCode pointOfSaleCountry,
            CurrencyCode saleCurrency,
            Instant soldAt,
            ActorType actorType,
            string userId)
        {
            if (string.IsNullOrWhiteSpace(sellerId))
                throw ExceptionFactory.IdentifierIsRequired(nameof(sellerId));
            if (string.IsNullOrWhiteSpace(channelCode))
                throw ExceptionFactory.IdentifierIsRequired(nameof(channelCode));
            if (string.IsNullOrWhiteSpace(userId))
                throw ExceptionFactory.IdentifierIsRequired(nameof(userId));

            SellerId = sellerId;
            SellerOfficeId = sellerOfficeId;
            ChannelCode = channelCode;
            PointOfSaleCountry = pointOfSaleCountry;
            SaleCurrency = saleCurrency;
            SoldAt = soldAt;
            ActorType = actorType;
            UserId = userId;
        }

        public string SellerId { get; private set; } = null!;

        public string? SellerOfficeId { get; private set; }

        public string ChannelCode { get; private set; } = null!;

        public CountryCode PointOfSaleCountry { get; private set; }

        public CurrencyCode SaleCurrency { get; private set; }

        public Instant SoldAt { get; private set; }

        public ActorType ActorType { get; private set; }

        public string UserId { get; private set; } = null!;
    }
}

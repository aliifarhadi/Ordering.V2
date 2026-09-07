using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.ValueObjects
{
    public sealed record PriceSnapshot
    {
        private PriceSnapshot()
        {
        }

        public PriceSnapshot(
            Money total,
            Instant pricedAt,
            string pricingEngineVersion,
            string roundingPolicyVersion,
            FxSnapshot? fx = null)
        {
            if (string.IsNullOrWhiteSpace(pricingEngineVersion))
                throw ExceptionFactory.IdentifierIsRequired(nameof(pricingEngineVersion));
            if (string.IsNullOrWhiteSpace(roundingPolicyVersion))
                throw ExceptionFactory.IdentifierIsRequired(nameof(roundingPolicyVersion));

            TotalAmount = total.Amount;
            Currency = total.Currency;
            PricedAt = pricedAt;
            PricingEngineVersion = pricingEngineVersion;
            RoundingPolicyVersion = roundingPolicyVersion;
            Fx = fx;
        }

        public decimal TotalAmount { get; private set; }

        public CurrencyCode Currency { get; private set; }

        public Money Total => new(TotalAmount, Currency);

        public Instant PricedAt { get; private set; }

        public string PricingEngineVersion { get; private set; } = null!;

        public string RoundingPolicyVersion { get; private set; } = null!;

        public FxSnapshot? Fx { get; private set; }
    }
}

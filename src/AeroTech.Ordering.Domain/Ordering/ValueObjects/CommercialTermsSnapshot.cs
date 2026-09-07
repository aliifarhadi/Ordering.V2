using AeroTech.Ordering.Domain._Shared.Resources;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.ValueObjects
{
    public sealed record CommercialTermRestriction
    {
        public CommercialTermRestriction(string restrictionCode, string? description)
        {
            if (string.IsNullOrWhiteSpace(restrictionCode))
                throw ExceptionFactory.IdentifierIsRequired(nameof(restrictionCode));

            RestrictionCode = restrictionCode;
            Description = description;
        }

        public string RestrictionCode { get; }

        public string? Description { get; }
    }

    public sealed record CommercialTermsSnapshot
    {
        private readonly List<CommercialTermRestriction> _restrictions = [];
        private readonly List<string> _sourceRuleRefs = [];

        private CommercialTermsSnapshot()
        {
        }

        public CommercialTermsSnapshot(
            string termsVersion,
            string refundability,
            string changeability,
            string? noShowPolicyCode = null,
            Instant? validFrom = null,
            Instant? validUntil = null,
            IEnumerable<CommercialTermRestriction>? restrictions = null,
            IEnumerable<string>? sourceRuleRefs = null)
        {
            if (string.IsNullOrWhiteSpace(termsVersion))
                throw ExceptionFactory.IdentifierIsRequired(nameof(termsVersion));
            if (string.IsNullOrWhiteSpace(refundability))
                throw ExceptionFactory.IdentifierIsRequired(nameof(refundability));
            if (string.IsNullOrWhiteSpace(changeability))
                throw ExceptionFactory.IdentifierIsRequired(nameof(changeability));

            TermsVersion = termsVersion;
            Refundability = refundability;
            Changeability = changeability;
            NoShowPolicyCode = noShowPolicyCode;
            ValidFrom = validFrom;
            ValidUntil = validUntil;

            if (restrictions is not null)
                _restrictions.AddRange(restrictions);
            if (sourceRuleRefs is not null)
                _sourceRuleRefs.AddRange(sourceRuleRefs);
        }

        public string TermsVersion { get; private set; } = null!;

        public string Refundability { get; private set; } = null!;

        public string Changeability { get; private set; } = null!;

        public string? NoShowPolicyCode { get; private set; }

        public Instant? ValidFrom { get; private set; }

        public Instant? ValidUntil { get; private set; }

        public IReadOnlyCollection<CommercialTermRestriction> Restrictions => _restrictions.AsReadOnly();

        public IReadOnlyCollection<string> SourceRuleRefs => _sourceRuleRefs.AsReadOnly();
    }
}

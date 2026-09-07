using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using NodaTime;

namespace AeroTech.Ordering.Domain.SharedKernel.ValueObjects
{
    public sealed record CommercialSource
    {
        private CommercialSource()
        {
        }

        public CommercialSource(
            CommercialSourceType type,
            string reference,
            string? version,
            Instant acceptedAt)
        {
            if (string.IsNullOrWhiteSpace(reference))
                throw ExceptionFactory.ReferenceValueIsRequired(nameof(CommercialSource));

            Type = type;
            Reference = reference;
            Version = version;
            AcceptedAt = acceptedAt;
        }

        public CommercialSourceType Type { get; private set; }

        public string Reference { get; private set; } = null!;

        public string? Version { get; private set; }

        public Instant AcceptedAt { get; private set; }
    }
}

using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;

namespace AeroTech.Ordering.Domain.SharedKernel.ValueObjects
{
    public sealed record CapacityCommitmentRef
    {
        private CapacityCommitmentRef()
        {
        }

        public CapacityCommitmentRef(CapacityCommitmentType type, string reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
                throw ExceptionFactory.ReferenceValueIsRequired(nameof(CapacityCommitmentRef));

            Type = type;
            Reference = reference;
        }

        public CapacityCommitmentType Type { get; private set; }

        public string Reference { get; private set; } = null!;
    }
}

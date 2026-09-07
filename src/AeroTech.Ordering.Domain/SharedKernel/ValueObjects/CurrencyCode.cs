using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain.SharedKernel.ValueObjects
{
    public readonly record struct CurrencyCode
    {
        public CurrencyCode(string value)
        {
            if (!IsSyntacticallyValid(value))
                throw ExceptionFactory.CurrencyCodeIsInvalid(value);

            Value = value.ToUpperInvariant();
        }

        public string Value { get; }

        public override string ToString() => Value;

        private static bool IsSyntacticallyValid(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 3)
                return false;

            foreach (var character in value)
            {
                if (!char.IsAsciiLetter(character))
                    return false;
            }

            return true;
        }
    }
}

using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain.SharedKernel.ValueObjects
{
    public readonly record struct AirportCode
    {
        public AirportCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 3 || !IsAllAsciiLetters(value))
                throw ExceptionFactory.AirportCodeIsInvalid(value);

            Value = value.ToUpperInvariant();
        }

        public string Value { get; }

        public override string ToString() => Value;

        private static bool IsAllAsciiLetters(string value)
        {
            foreach (var character in value)
            {
                if (!char.IsAsciiLetter(character))
                    return false;
            }

            return true;
        }
    }

    public readonly record struct CarrierCode
    {
        public CarrierCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length is < 2 or > 3 || !IsAllAsciiLetterOrDigit(value))
                throw ExceptionFactory.CarrierCodeIsInvalid(value);

            Value = value.ToUpperInvariant();
        }

        public string Value { get; }

        public override string ToString() => Value;

        private static bool IsAllAsciiLetterOrDigit(string value)
        {
            foreach (var character in value)
            {
                if (!char.IsAsciiLetterOrDigit(character))
                    return false;
            }

            return true;
        }
    }

    public readonly record struct CountryCode
    {
        public CountryCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 2 || !char.IsAsciiLetter(value[0]) || !char.IsAsciiLetter(value[1]))
                throw ExceptionFactory.CountryCodeIsInvalid(value);

            Value = value.ToUpperInvariant();
        }

        public string Value { get; }

        public override string ToString() => Value;
    }

    public readonly record struct OrderReference
    {
        public const int MinLength = 10;
        public const int MaxLength = 12;

        private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        public OrderReference(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.OrderReferenceIsInvalid(value);

            var normalized = value.ToUpperInvariant();
            if (normalized.Length is < MinLength or > MaxLength)
                throw ExceptionFactory.OrderReferenceIsInvalid(value);

            foreach (var character in normalized)
            {
                if (!Alphabet.Contains(character))
                    throw ExceptionFactory.OrderReferenceIsInvalid(value);
            }

            Value = normalized;
        }

        public string Value { get; }

        public override string ToString() => Value;
    }
}

using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain.SharedKernel.ValueObjects
{
    public readonly record struct Money(decimal Amount, CurrencyCode Currency)
    {
        public static Money Zero(CurrencyCode currency) => new(decimal.Zero, currency);

        public bool IsZero => Amount == decimal.Zero;

        public bool IsNegative => Amount < decimal.Zero;

        public static Money operator +(Money left, Money right)
        {
            EnsureSameCurrency(left, right);
            return new Money(left.Amount + right.Amount, left.Currency);
        }

        public static Money operator -(Money left, Money right)
        {
            EnsureSameCurrency(left, right);
            return new Money(left.Amount - right.Amount, left.Currency);
        }

        public static Money operator -(Money value) => new(-value.Amount, value.Currency);

        public static bool operator >(Money left, Money right)
        {
            EnsureSameCurrency(left, right);
            return left.Amount > right.Amount;
        }

        public static bool operator <(Money left, Money right)
        {
            EnsureSameCurrency(left, right);
            return left.Amount < right.Amount;
        }

        public static bool operator >=(Money left, Money right)
        {
            EnsureSameCurrency(left, right);
            return left.Amount >= right.Amount;
        }

        public static bool operator <=(Money left, Money right)
        {
            EnsureSameCurrency(left, right);
            return left.Amount <= right.Amount;
        }

        public static Money Sum(IEnumerable<Money> values, CurrencyCode currency)
        {
            var total = Zero(currency);
            foreach (var value in values)
                total += value;

            return total;
        }

        public override string ToString() => $"{Amount} {Currency}";

        private static void EnsureSameCurrency(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw ExceptionFactory.CurrencyMismatch(left.Currency.Value, right.Currency.Value);
        }
    }
}

using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class ChargeLine : Entity<ChargeLineId>
    {
        private ChargeLine()
        {
        }

        private ChargeLine(
            ChargeLineId id,
            OrderItemId orderItemId,
            int sequence,
            ChargeType type,
            string code,
            string? description,
            Money amount,
            bool refundable,
            string? taxJurisdiction)
        {
            Id = id;
            OrderItemId = orderItemId;
            Sequence = sequence;
            Type = type;
            Code = code;
            Description = description;
            Amount = amount.Amount;
            Currency = amount.Currency;
            Refundable = refundable;
            TaxJurisdiction = taxJurisdiction;
        }

        public OrderItemId OrderItemId { get; private set; }

        public int Sequence { get; private set; }

        public ChargeType Type { get; private set; }

        public string Code { get; private set; } = null!;

        public string? Description { get; private set; }

        public decimal Amount { get; private set; }

        public CurrencyCode Currency { get; private set; }

        public bool Refundable { get; private set; }

        public string? TaxJurisdiction { get; private set; }

        public Money Money => new(Amount, Currency);

        public static ChargeLine Create(
            OrderItemId orderItemId,
            int sequence,
            ChargeType type,
            string code,
            string? description,
            Money amount,
            bool refundable,
            string? taxJurisdiction = null)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw ExceptionFactory.IdentifierIsRequired(nameof(code));

            if (amount.IsNegative && !AllowsNegativeAmount(type))
                throw ExceptionFactory.NegativeChargeLineNotPermitted(type);

            return new ChargeLine(
                ChargeLineId.New(),
                orderItemId,
                sequence,
                type,
                code,
                description,
                amount,
                refundable,
                taxJurisdiction);
        }

        private static bool AllowsNegativeAmount(ChargeType type) =>
            type is ChargeType.Discount or ChargeType.OtherCharge;
    }
}

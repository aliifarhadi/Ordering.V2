using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.ValueObjects
{
    public sealed record FxSnapshot(
        CurrencyCode From,
        CurrencyCode To,
        decimal Rate,
        string Source,
        Instant CapturedAt);
}

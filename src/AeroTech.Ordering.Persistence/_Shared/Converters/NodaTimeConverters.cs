using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;

namespace AeroTech.Ordering.Persistence._Shared.Converters
{
    public sealed class InstantToDateTimeConverter : ValueConverter<Instant, DateTime>
    {
        public InstantToDateTimeConverter()
            : base(
                instant => instant.ToDateTimeUtc(),
                value => Instant.FromDateTimeUtc(DateTime.SpecifyKind(value, DateTimeKind.Utc)))
        {
        }
    }

    public sealed class NullableInstantToDateTimeConverter : ValueConverter<Instant?, DateTime?>
    {
        public NullableInstantToDateTimeConverter()
            : base(
                instant => instant == null ? null : instant.Value.ToDateTimeUtc(),
                value => value == null
                    ? null
                    : Instant.FromDateTimeUtc(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)))
        {
        }
    }

    public sealed class LocalDateToDateOnlyConverter : ValueConverter<LocalDate, DateOnly>
    {
        public LocalDateToDateOnlyConverter()
            : base(
                date => date.ToDateOnly(),
                value => LocalDate.FromDateOnly(value))
        {
        }
    }

    public sealed class NullableLocalDateToDateOnlyConverter : ValueConverter<LocalDate?, DateOnly?>
    {
        public NullableLocalDateToDateOnlyConverter()
            : base(
                date => date == null ? null : date.Value.ToDateOnly(),
                value => value == null ? null : LocalDate.FromDateOnly(value.Value))
        {
        }
    }

    public sealed class LocalTimeToTimeOnlyConverter : ValueConverter<LocalTime, TimeOnly>
    {
        public LocalTimeToTimeOnlyConverter()
            : base(
                time => new TimeOnly(time.Hour, time.Minute, time.Second),
                value => new LocalTime(value.Hour, value.Minute, value.Second))
        {
        }
    }
}

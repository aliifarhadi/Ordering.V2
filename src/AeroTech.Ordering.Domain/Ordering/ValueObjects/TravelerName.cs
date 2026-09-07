using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain.Ordering.ValueObjects
{
    public sealed record TravelerName
    {
        public const int MaxPartLength = 128;

        private TravelerName()
        {
        }

        public TravelerName(string givenName, string surname, string? title = null)
        {
            if (string.IsNullOrWhiteSpace(givenName))
                throw ExceptionFactory.GivenNameIsRequired();
            if (string.IsNullOrWhiteSpace(surname))
                throw ExceptionFactory.SurnameIsRequired();
            if (givenName.Length > MaxPartLength)
                throw ExceptionFactory.NameIsTooLong(givenName);
            if (surname.Length > MaxPartLength)
                throw ExceptionFactory.NameIsTooLong(surname);

            GivenName = givenName;
            Surname = surname;
            Title = title;
        }

        public string GivenName { get; private set; } = null!;

        public string Surname { get; private set; } = null!;

        public string? Title { get; private set; }

        public override string ToString() => $"{Surname}/{GivenName}";
    }

    public sealed record RegulatoryDataSnapshot(
        string? RedressNumber,
        string? KnownTravelerNumber,
        string? ResidenceCountry,
        string? DestinationAddress);
}

using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class JourneySegment : Entity<JourneySegmentId>
    {
        private JourneySegment()
        {
        }

        private JourneySegment(
            JourneySegmentId id,
            OrderId orderId,
            JourneyId journeyId,
            CarrierCode marketingCarrier,
            CarrierCode operatingCarrier,
            string flightNumber,
            AirportCode origin,
            AirportCode destination,
            Instant departureUtc,
            Instant arrivalUtc,
            LocalDate departureLocalDate,
            LocalTime departureLocalTime,
            string originTimeZoneId,
            LocalDate arrivalLocalDate,
            LocalTime arrivalLocalTime,
            string destinationTimeZoneId,
            string? aircraftType,
            int sequence)
        {
            Id = id;
            OrderId = orderId;
            JourneyId = journeyId;
            MarketingCarrier = marketingCarrier;
            OperatingCarrier = operatingCarrier;
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            DepartureUtc = departureUtc;
            ArrivalUtc = arrivalUtc;
            DepartureLocalDate = departureLocalDate;
            DepartureLocalTime = departureLocalTime;
            OriginTimeZoneId = originTimeZoneId;
            ArrivalLocalDate = arrivalLocalDate;
            ArrivalLocalTime = arrivalLocalTime;
            DestinationTimeZoneId = destinationTimeZoneId;
            AircraftType = aircraftType;
            Sequence = sequence;
        }

        public OrderId OrderId { get; private set; }

        public JourneyId JourneyId { get; private set; }

        public CarrierCode MarketingCarrier { get; private set; }

        public CarrierCode OperatingCarrier { get; private set; }

        public string FlightNumber { get; private set; } = null!;

        public AirportCode Origin { get; private set; }

        public AirportCode Destination { get; private set; }

        public Instant DepartureUtc { get; private set; }

        public Instant ArrivalUtc { get; private set; }

        public LocalDate DepartureLocalDate { get; private set; }

        public LocalTime DepartureLocalTime { get; private set; }

        public string OriginTimeZoneId { get; private set; } = null!;

        public LocalDate ArrivalLocalDate { get; private set; }

        public LocalTime ArrivalLocalTime { get; private set; }

        public string DestinationTimeZoneId { get; private set; } = null!;

        public string? AircraftType { get; private set; }

        public int Sequence { get; private set; }

        internal static JourneySegment Create(
            JourneySegmentId id,
            OrderId orderId,
            JourneyId journeyId,
            CarrierCode marketingCarrier,
            CarrierCode operatingCarrier,
            string flightNumber,
            AirportCode origin,
            AirportCode destination,
            Instant departureUtc,
            Instant arrivalUtc,
            LocalDate departureLocalDate,
            LocalTime departureLocalTime,
            string originTimeZoneId,
            LocalDate arrivalLocalDate,
            LocalTime arrivalLocalTime,
            string destinationTimeZoneId,
            string? aircraftType,
            int sequence)
        {
            if (string.IsNullOrWhiteSpace(flightNumber))
                throw ExceptionFactory.IdentifierIsRequired(nameof(flightNumber));
            if (string.IsNullOrWhiteSpace(originTimeZoneId))
                throw ExceptionFactory.IdentifierIsRequired(nameof(originTimeZoneId));
            if (string.IsNullOrWhiteSpace(destinationTimeZoneId))
                throw ExceptionFactory.IdentifierIsRequired(nameof(destinationTimeZoneId));

            return new JourneySegment(
                id,
                orderId,
                journeyId,
                marketingCarrier,
                operatingCarrier,
                flightNumber,
                origin,
                destination,
                departureUtc,
                arrivalUtc,
                departureLocalDate,
                departureLocalTime,
                originTimeZoneId,
                arrivalLocalDate,
                arrivalLocalTime,
                destinationTimeZoneId,
                aircraftType,
                sequence);
        }

        internal void ApplyScheduleSnapshotChange(
            CarrierCode marketingCarrier,
            CarrierCode operatingCarrier,
            string flightNumber,
            Instant departureUtc,
            Instant arrivalUtc,
            LocalDate departureLocalDate,
            LocalTime departureLocalTime,
            LocalDate arrivalLocalDate,
            LocalTime arrivalLocalTime,
            string? aircraftType)
        {
            if (string.IsNullOrWhiteSpace(flightNumber))
                throw ExceptionFactory.IdentifierIsRequired(nameof(flightNumber));

            MarketingCarrier = marketingCarrier;
            OperatingCarrier = operatingCarrier;
            FlightNumber = flightNumber;
            DepartureUtc = departureUtc;
            ArrivalUtc = arrivalUtc;
            DepartureLocalDate = departureLocalDate;
            DepartureLocalTime = departureLocalTime;
            ArrivalLocalDate = arrivalLocalDate;
            ArrivalLocalTime = arrivalLocalTime;
            AircraftType = aircraftType;
        }
    }
}

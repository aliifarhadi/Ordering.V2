using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class Journey : Entity<JourneyId>
    {
        private readonly List<JourneySegment> _segments = [];

        private Journey()
        {
        }

        private Journey(
            JourneyId id,
            OrderId orderId,
            AirportCode origin,
            AirportCode destination,
            int sequence)
        {
            Id = id;
            OrderId = orderId;
            Origin = origin;
            Destination = destination;
            Sequence = sequence;
        }

        public OrderId OrderId { get; private set; }

        public AirportCode Origin { get; private set; }

        public AirportCode Destination { get; private set; }

        public int Sequence { get; private set; }

        public IReadOnlyCollection<JourneySegment> Segments => _segments.AsReadOnly();

        public IEnumerable<JourneySegmentId> SegmentRefs => _segments.Select(segment => segment.Id);

        internal static Journey Create(
            JourneyId id,
            OrderId orderId,
            AirportCode origin,
            AirportCode destination,
            int sequence) =>
            new(id, orderId, origin, destination, sequence);

        internal void AddSegment(JourneySegment segment) => _segments.Add(segment);

        internal bool RemoveSegment(JourneySegmentId segmentId) =>
            _segments.RemoveAll(segment => segment.Id == segmentId) > 0;

        internal JourneySegment? FindSegment(JourneySegmentId segmentId) =>
            _segments.SingleOrDefault(segment => segment.Id == segmentId);

        internal bool ContainsSegment(JourneySegmentId segmentId) =>
            _segments.Any(segment => segment.Id == segmentId);

        internal int NextSegmentSequence() =>
            _segments.Count == 0 ? 1 : _segments.Max(segment => segment.Sequence) + 1;

        internal bool HasContiguousSegmentSequence()
        {
            if (_segments.Count == 0)
                return true;

            var ordered = _segments.Select(segment => segment.Sequence).OrderBy(sequence => sequence).ToArray();
            for (var index = 0; index < ordered.Length; index++)
            {
                if (ordered[index] != index + 1)
                    return false;
            }

            return true;
        }
    }
}

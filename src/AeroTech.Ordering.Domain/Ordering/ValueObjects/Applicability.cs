using AeroTech.Ordering.Domain.SharedKernel.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.Ordering.ValueObjects
{
    public sealed record Applicability
    {
        private readonly IReadOnlyList<JourneySegmentId> _segmentRefs;

        public Applicability(
            JourneyId? journeyRef = null,
            IEnumerable<JourneySegmentId>? segmentRefs = null,
            string? locationRef = null,
            LocalDate? startDate = null,
            LocalDate? endDate = null)
        {
            JourneyRef = journeyRef;
            _segmentRefs = segmentRefs?.Distinct().ToArray() ?? [];
            LocationRef = locationRef;
            StartDate = startDate;
            EndDate = endDate;
        }

        public JourneyId? JourneyRef { get; }

        public IReadOnlyList<JourneySegmentId> SegmentRefs => _segmentRefs;

        public string? LocationRef { get; }

        public LocalDate? StartDate { get; }

        public LocalDate? EndDate { get; }
    }
}

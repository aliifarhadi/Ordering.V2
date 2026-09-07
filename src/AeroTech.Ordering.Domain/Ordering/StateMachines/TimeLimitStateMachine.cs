using AeroTech.Ordering.Domain.SharedKernel.Enums;

namespace AeroTech.Ordering.Domain.Ordering.StateMachines
{
    public static class TimeLimitStateMachine
    {
        private static readonly Dictionary<TimeLimitStatus, TimeLimitStatus[]> Allowed = new()
        {
            [TimeLimitStatus.Active] =
            [
                TimeLimitStatus.Met,
                TimeLimitStatus.Expired,
                TimeLimitStatus.Cancelled
            ],
            [TimeLimitStatus.Met] = [],
            [TimeLimitStatus.Expired] = [],
            [TimeLimitStatus.Cancelled] = []
        };

        public static bool IsTerminal(TimeLimitStatus status) => status is not TimeLimitStatus.Active;

        public static bool CanTransition(TimeLimitStatus from, TimeLimitStatus to) =>
            Allowed.TryGetValue(from, out var targets) && targets.Contains(to);
    }
}

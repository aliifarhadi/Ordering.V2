using AeroTech.Ordering.Domain.SharedKernel.Enums;

namespace AeroTech.Ordering.Domain.Ordering.StateMachines
{
    public static class ProcessingLockPolicy
    {
        private static readonly ProcessingLockType[] ScopeExclusiveTypes =
        [
            ProcessingLockType.Refund,
            ProcessingLockType.Cancellation,
            ProcessingLockType.VoluntaryChange,
            ProcessingLockType.Reaccommodation
        ];

        public static bool AreIncompatible(ProcessingLockType left, ProcessingLockType right)
        {
            if (left is ProcessingLockType.Split || right is ProcessingLockType.Split)
                return true;

            if (ScopeExclusiveTypes.Contains(left) && ScopeExclusiveTypes.Contains(right))
                return true;

            return left == right;
        }

        public static bool BlocksTimeLimitExpiry(ProcessingLockType type) =>
            type is ProcessingLockType.PaymentCompletion;
    }
}

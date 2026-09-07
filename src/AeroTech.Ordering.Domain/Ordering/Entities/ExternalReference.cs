using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.Identifiers;

namespace AeroTech.Ordering.Domain.Ordering.Entities
{
    public sealed class ExternalReference : Entity<ExternalReferenceId>
    {
        private ExternalReference()
        {
        }

        private ExternalReference(
            ExternalReferenceId id,
            OrderId orderId,
            ExternalReferenceScope scope,
            Guid? scopedEntityId,
            string system,
            string type,
            string value,
            string? owner)
        {
            Id = id;
            OrderId = orderId;
            Scope = scope;
            ScopedEntityId = scopedEntityId;
            System = system;
            Type = type;
            Value = value;
            Owner = owner;
        }

        public OrderId OrderId { get; private set; }

        public ExternalReferenceScope Scope { get; private set; }

        public Guid? ScopedEntityId { get; private set; }

        public string System { get; private set; } = null!;

        public string Type { get; private set; } = null!;

        public string Value { get; private set; } = null!;

        public string? Owner { get; private set; }

        internal static ExternalReference Create(
            ExternalReferenceId id,
            OrderId orderId,
            ExternalReferenceScope scope,
            Guid? scopedEntityId,
            string system,
            string type,
            string value,
            string? owner = null)
        {
            if (string.IsNullOrWhiteSpace(system))
                throw ExceptionFactory.IdentifierIsRequired(nameof(system));
            if (string.IsNullOrWhiteSpace(type))
                throw ExceptionFactory.IdentifierIsRequired(nameof(type));
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.ReferenceValueIsRequired(nameof(ExternalReference));

            if (scope is not ExternalReferenceScope.Order && scopedEntityId is null)
                throw ExceptionFactory.ExternalReferenceScopeTargetMissing(scope);

            return new ExternalReference(id, orderId, scope, scopedEntityId, system, type, value, owner);
        }

        internal void ReassignToOrder(OrderId orderId) => OrderId = orderId;

        internal bool Matches(string system, string type, string value, string? owner) =>
            System == system && Type == type && Value == value && Owner == owner;
    }
}

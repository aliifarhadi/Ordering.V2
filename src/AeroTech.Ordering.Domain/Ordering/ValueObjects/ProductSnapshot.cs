using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.SharedKernel.Enums;

namespace AeroTech.Ordering.Domain.Ordering.ValueObjects
{
    public sealed record ProductAttribute
    {
        public ProductAttribute(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw ExceptionFactory.IdentifierIsRequired(nameof(key));

            Key = key;
            Value = value;
        }

        public string Key { get; }

        public string Value { get; }
    }

    public sealed record ProductSnapshot
    {
        private readonly List<ProductAttribute> _attributes = [];

        private ProductSnapshot()
        {
        }

        public ProductSnapshot(
            string? productId,
            string productCode,
            ProductType productType,
            string name,
            string? description,
            string? catalogVersion,
            IEnumerable<ProductAttribute>? attributes = null)
        {
            if (string.IsNullOrWhiteSpace(productCode))
                throw ExceptionFactory.IdentifierIsRequired(nameof(productCode));
            if (string.IsNullOrWhiteSpace(name))
                throw ExceptionFactory.IdentifierIsRequired(nameof(name));

            ProductId = productId;
            ProductCode = productCode;
            ProductType = productType;
            Name = name;
            Description = description;
            CatalogVersion = catalogVersion;

            if (attributes is not null)
                _attributes.AddRange(attributes);
        }

        public string? ProductId { get; private set; }

        public string ProductCode { get; private set; } = null!;

        public ProductType ProductType { get; private set; }

        public string Name { get; private set; } = null!;

        public string? Description { get; private set; }

        public string? CatalogVersion { get; private set; }

        public IReadOnlyCollection<ProductAttribute> Attributes => _attributes.AsReadOnly();
    }
}

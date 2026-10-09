// ReSharper disable CheckNamespace

public static class Traits
{
    public const string Category = "Category";
    public const string UnitTests = "UnitTests";
    public const string IntegrationTests = "IntegrationTests";
}

public static class Collections
{
    /// <summary>
    /// The classes in this collection call the live services, so their tests run one after another. The attribute
    /// applies to the whole partial class, unit tests included; classes outside the collection run in parallel.
    /// </summary>
    public const string IntegrationTests = "IntegrationTests";
}
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Library.Tests.Core;

[AttributeUsage(AttributeTargets.Class)]
[TraitDiscoverer("Library.Tests.Core.FunctionalTestsTraitDiscoverer", "Library.Tests.Core")]
public class FunctionalTestsAttribute : Attribute, ITraitAttribute
{
}

public class FunctionalTestsTraitDiscoverer : ITraitDiscoverer
{
    private const string CategoryName = "Category";
    private const string CategoryValue = "FunctionalTests";

    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        => [new KeyValuePair<string, string>(CategoryName, CategoryValue)];
}
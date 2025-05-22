using Xunit.Abstractions;
using Xunit.Sdk;

namespace Library.Tests.Core;

[AttributeUsage(AttributeTargets.Class)]
[TraitDiscoverer("Library.Tests.Core.UnitTestsTraitDiscoverer", "Library.Tests.Core")]
public class UnitTestsAttribute : Attribute, ITraitAttribute
{
}

public class UnitTestsTraitDiscoverer : ITraitDiscoverer
{
    private const string CategoryName = "Category";
    private const string CategoryValue = "UnitTests";

    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        => [new KeyValuePair<string, string>(CategoryName, CategoryValue)];
}
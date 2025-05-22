using System;
using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Library.Tests.Core;

[AttributeUsage(AttributeTargets.Class)]
[TraitDiscoverer("Library.Tests.Core.IntegrationTestsTraitDiscoverer", "Library.Tests.Core")]
public class IntegrationTestsAttribute : Attribute, ITraitAttribute
{
}

public class IntegrationTestsTraitDiscoverer : ITraitDiscoverer
{
    private const string CategoryName = "Category";
    private const string CategoryValue = "IntegrationTests";

    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        => [new KeyValuePair<string, string>(CategoryName, CategoryValue)];
}
using Senswave.Abstractions.Entities;
using System.Reflection;

namespace Senswave.ArchitectureTests.Modules;

[Trait("Collection", "ArchitectureTests")]
public class DomainTests : BaseModuleTest
{
    [Fact]
    public void DomainsDontHaveDependencyOnSenswave()
    {
        //Arrange
        IList<AssemblyName> excludedAssembilies = SharedAssemblies
            .Select(x => x.GetName())
            .ToList();

        var domain = GetDomainAssemblies();

        //Act

        var invalidRefs = new Dictionary<Assembly, List<AssemblyName>>();

        foreach (var domainAssembly in domain)
        {
            var referencedAssemblies = domainAssembly
                .GetReferencedAssemblies();

            var invalidReferences = referencedAssemblies
                .Where(x => !excludedAssembilies.Any(y => y.Name == x.Name))
                .Where(x => !x.FullName.Contains("Senswave.Shared"))
                .Where(x => x.FullName.Contains("Senswave"))
                .ToList();

            invalidRefs.Add(domainAssembly, invalidReferences);
        }

        //Assert

        Assert.NotEmpty(domain);

        foreach (var invalidRef in invalidRefs)
        {
            Assert.Empty(invalidRef.Value);
        }
    }

    [Fact]
    public void AllEntitiesHaveId()
    {
        //Arrange
        var domains = GetDomainAssemblies();
        var baseEntity = typeof(Entity);

        //Act
        var entities = domains.SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && (t.Namespace?.Contains("Entities") ?? false))
            .ToList();

        //Assert
        foreach (var entity in entities)
        {
            Assert.True(entity.IsSubclassOf(baseEntity));
        }
    }

    [Fact]
    public void AllValueObjectsDontDeriveFromEntity()
    {
        //Arrange
        var domains = GetDomainAssemblies();
        var baseEntity = typeof(Entity);

        //Act

        var valueObjects = domains.SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && (t.Namespace?.Contains("ValueObjects") ?? false))
            .ToList();

        //Assert
        foreach (var valueObject in valueObjects)
        {
            Assert.False(valueObject.IsSubclassOf(baseEntity));
        }
    }
}

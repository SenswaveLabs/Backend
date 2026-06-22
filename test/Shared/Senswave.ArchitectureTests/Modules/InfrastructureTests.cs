using System.Reflection;

namespace Senswave.ArchitectureTests.Modules;

[Trait("Collection", "ArchitectureTests")]
public class InfrastructureTests : BaseModuleTest
{
    [Fact]
    public void CanHaveDependencyOnDomainAndApplication()
    {
        //Arrange
        var domain = GetDomainAssemblies();
        var application = GetApplicationAssemblies();
        var infrastructure = GetInfrastructureAssemblies();
        var excluded = SharedAssemblies;
        var infraRefs = new Dictionary<Assembly, List<AssemblyName>>();

        //Act
        foreach (var infrastructureAssembly in infrastructure)
        {
            var referencedAssemblies = infrastructureAssembly
                .GetReferencedAssemblies();

            var invalidReferences = referencedAssemblies
                .Where(x => x.FullName.Contains("Senswave"))
                .Where(x => !excluded.Any(y => y.GetName().Name == x.Name))
                .Where(x => !domain.Any(y => y.GetName().Name == x.Name))
                .Where(x => !application.Any(y => y.GetName().Name == x.Name))
                .ToList();

            infraRefs.Add(infrastructureAssembly, invalidReferences);
        }

        //Assert

        Assert.NotEmpty(infraRefs);

        foreach (var infraRef in infraRefs)
        {
            if (infraRef.Key.ToString().Contains("User"))
                infraRef.Value.Remove(infraRef.Value.First(x => x.Name!.Contains("Infrastructure.Web")));

            Assert.Empty(infraRef.Value);
        }
    }
}

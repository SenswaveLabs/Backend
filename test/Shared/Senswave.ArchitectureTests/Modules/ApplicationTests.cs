using NetArchTest.Rules;
using Senswave.Abstractions.Commands;
using Senswave.Abstractions.Queries;
using System.Reflection;

namespace Senswave.ArchitectureTests.Modules;

[Trait("Collection", "ArchitectureTests")]
public class ApplicationTests : BaseModuleTest
{
    [Fact]
    public void ApplicationCanHaveDepenedencyOnlyOnDomain()
    {
        //Arrange
        var application = GetApplicationAssemblies();
        var domain = GetDomainAssemblies();
        var excluded = SharedAssemblies;

        //Act

        var invalidRefs = new Dictionary<Assembly, List<AssemblyName>>();

        foreach (var applicationAssembly in application)
        {
            var referencedAssemblies = applicationAssembly
                .GetReferencedAssemblies();

            var invalidReferences = referencedAssemblies
                .Where(x => x.FullName.Contains("Senswave"))
                .Where(x => !x.FullName.Contains("Senswave.Shared"))
                .Where(x => !excluded.Any(y => y.GetName().Name == x.Name))
                .Where(x => !domain.Any(y => y.GetName().Name == x.Name))
                .ToList();

            invalidRefs.Add(applicationAssembly, invalidReferences);
        }

        //Assert

        Assert.NotEmpty(invalidRefs);

        foreach (var invalidRef in invalidRefs)
        {
            Assert.Empty(invalidRef.Value);
        }
    }

    [Fact]
    public void AllQueryImplementIQuery()
    {
        //Arrange
        var application = GetApplicationAssemblies();
        var baseQuery = typeof(IQuery<object>);

        //Act

        var results = new List<ConditionList>();

        foreach (var assembly in application)
        {
            var conditions = Types.InAssembly(assembly)
                .That()
                .ImplementInterface(baseQuery)
                .Should()
                .HaveNameEndingWith("Query");

            results.Add(conditions);
        }

        //Assert

        Assert.NotEmpty(results);

        foreach (var result in results)
        {
            Assert.True(result.GetResult().IsSuccessful);
        }
    }

    [Fact]
    public async Task AllCommandImplementICommand()
    {
        //Arrange
        var application = GetApplicationAssemblies();
        var baseQuery = typeof(ICommand);

        //Act

        var results = new List<ConditionList>();

        foreach (var assembly in application)
        {
            var conditions = Types.InAssembly(assembly)
                .That()
                .ImplementInterface(baseQuery)
                .Should()
                .HaveNameEndingWith("Command");

            results.Add(conditions);
        }

        //Assert

        Assert.NotEmpty(results);

        foreach (var result in results)
        {
            var validation = result.GetResult();
            Assert.True(validation.IsSuccessful);
        }
    }
}

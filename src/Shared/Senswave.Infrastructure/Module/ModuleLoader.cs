using Senswave.Abstractions.Modules;
using System.Reflection;

namespace Senswave.Infrastructure.Module;

public static class ModuleLoader
{
    public static IList<Assembly> LoadAssemblies()
    {
        var assemblies = new List<Assembly>();

        var files = Directory
            .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*Senswave*.dll")
            .ToList();

        files.ForEach(x =>
        {
            try
            {
                var assemblyName = AssemblyName.GetAssemblyName(x);
                assemblies.Add(AppDomain.CurrentDomain.Load(assemblyName));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load assembly from file {x}: {ex.Message}");
                // Safety when having dll in bin folder that are not used currently in app
            }
        });

        return assemblies;
    }

    public static IList<ISenswaveModule> LoadModules(IEnumerable<Assembly> assemblies)
        => assemblies.SelectMany(x => x.GetTypes())
            .Where(x => typeof(ISenswaveModule).IsAssignableFrom(x) && !x.IsInterface)
            .OrderBy(x => x.Name)
            .Select(Activator.CreateInstance)
            .Cast<ISenswaveModule>()
            .ToList();
}

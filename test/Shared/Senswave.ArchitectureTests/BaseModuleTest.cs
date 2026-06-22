using Senswave.Abstractions.Commands;
using Senswave.Abstractions.Resulting;
using Senswave.Infrastructure;
using Senswave.Integration.DataTransfer.MessageReceivedFromDevice;
using System.Reflection;

namespace Senswave.ArchitectureTests;

public abstract class BaseModuleTest
{
    protected Assembly Infrastructure = typeof(InfrastructureModule).Assembly;
    protected Assembly Presentation = typeof(Senswave.Api.Program).Assembly;

    protected static Assembly[] SharedAssemblies =>
        [
            typeof(ICommand).Assembly,
            typeof(InfrastructureModule).Assembly,
            typeof(MessageReceivedFromDeviceEvent).Assembly,
            typeof(Result).Assembly,
        ];

    protected static Assembly[] GetSenswaveAssemblies()
    {
        var names = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Where(f => Path.GetFileName(f).Contains("Senswave"))
                .ToArray();

        return names.Select(Assembly.LoadFrom).ToArray();
    }

    protected static Assembly[] GetDomainAssemblies() => GetSenswaveAssemblies()
        .Where(a => a.FullName!.Contains("Domain"))
        .ToArray();

    protected static Assembly[] GetApplicationAssemblies() => GetSenswaveAssemblies()
        .Where(a => a.FullName!.Contains("Application"))
        .ToArray();

    protected Assembly[] GetInfrastructureAssemblies() => GetSenswaveAssemblies()
        .Where(a => a.FullName!.Contains("Infrastructure") && a.FullName != Infrastructure.FullName)
        .ToArray();

    protected Assembly[] GetApiAssemblies() => GetSenswaveAssemblies()
        .Where(a => a.FullName!.Contains("Api") && a.FullName != Presentation.FullName)
        .ToArray();
}

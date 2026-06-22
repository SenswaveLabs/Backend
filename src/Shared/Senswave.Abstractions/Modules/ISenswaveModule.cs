using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Senswave.Abstractions.Modules;

public interface ISenswaveModule
{
    string Name { get; }
    void Register(IServiceCollection services, IConfiguration configuration);
}

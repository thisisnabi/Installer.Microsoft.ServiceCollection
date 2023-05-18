using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public interface IServiceCollectionInstaller
{
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}

public static class ServiceCollectionInstaller
{
    public static void InstallFromAssembly<T>(this IServiceCollection services, IConfiguration configuration)
    {
        // get all public classes that implement from IInstaller
        var installerItems = typeof(T).Assembly
                                    .GetExportedTypes()
                                        .Where(x => typeof(IServiceCollectionInstaller).IsAssignableFrom(x) && 
                                                    x is { IsAbstract:false, IsInterface:false})
                                        .Select(Activator.CreateInstance)
                                        .Cast<IServiceCollectionInstaller>()
                                        .ToList();

        // configure ServiceCollection for all founded classes 
        foreach (var installer in installerItems)
        {
            installer.ConfigureServices(services, configuration);
        }                                
    }
}
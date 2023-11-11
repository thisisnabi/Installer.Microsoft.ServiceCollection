using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public class InstallerException : Exception
{
    private readonly string _installerName;
    public override string Message => $"Assemby '{_installerName}' doesn't include any Installer.";

    public InstallerException(string installerName)
    {
        _installerName = installerName;
    }
}


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
                                                    x is { IsAbstract: false, IsInterface: false })
                                        .Select(Activator.CreateInstance)
                                        .Cast<IServiceCollectionInstaller>()
                                        .ToList();

        // configure ServiceCollection for all founded classes 
        foreach (var installer in installerItems)
        {
            installer.ConfigureServices(services, configuration);
        }
    }

    public static WebApplication BuildIt<T>(this WebApplicationBuilder builder)
    {
        builder.Services.InstallFromAssembly<T>(builder.Configuration);
        return builder.Build();
    }

    public static IStepableServiceCollectionInstaller Installer<T>(this IServiceCollection services, IConfiguration configuration) =>
         new StepableServiceCollectionInstaller(services, configuration).NextOne<T>();

}
 
public interface IStepableServiceCollectionInstaller
{
    IStepableServiceCollectionInstaller NextOne<T>();

    void Finish<T>();
    void Finish();
}

public class StepableServiceCollectionInstaller : IStepableServiceCollectionInstaller
{
    private List<IServiceCollectionInstaller> _installers;
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;

    public StepableServiceCollectionInstaller(IServiceCollection services, IConfiguration configuration)
    {
        _services = services;
        _configuration = configuration;
        _installers = new List<IServiceCollectionInstaller>();
    }

    public IStepableServiceCollectionInstaller NextOne<T>()
    {
        var installerItems = typeof(T).Assembly
                              .GetExportedTypes()
                                  .Where(x => typeof(IServiceCollectionInstaller).IsAssignableFrom(x) &&
                                              x is { IsAbstract: false, IsInterface: false })
                                  .Select(Activator.CreateInstance)
                                  .Cast<IServiceCollectionInstaller>()
                                  .ToList();

        if (installerItems is null || !installerItems.Any())
        {
            throw new InstallerException(typeof(T).Name);
        }

        _installers.AddRange(installerItems);
        return this;
    }

    public void Finish<T>() => NextOne<T>().Finish();

    public void Finish()
    {
        foreach (var installer in _installers)
        {
            installer.ConfigureServices(_services, _configuration);
        }
    }
}


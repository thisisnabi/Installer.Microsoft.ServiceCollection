# Installer.Microsoft.ServiceCollection

Simplify install and organized your ServiceCollection items with installer approach.


### Step 1 - Add Package

```nuget
dotnet add package ServiceCollectionInstaller
```

 

### Step 2 - Define an Installer in your `assembly` that inplement IServiceCollectionInstaller

```csharp
public class InfrastructureInstaller : IServiceCollectionInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration["defult-connection"]);
        });

        // other configuration
    }
}
```

### Step 3 - Use in Program.cs
```csharp

// IAssemblyMaker is a simple interface that refer to assembly, you can use another type
using assemblyApplication = Installer.ConsumerWebAppl.IAssemblyMaker;
using assemblyInfrastructure = Installer.ConsumerWebApp.Infrastructure.IAssemblyMaker;

var builder = WebApplication.CreateBuilder(args);
{
    // get Configuration
    var config = builder.Configuration;
    
    // Install from your assemblies
    builder.Services.InstallFromAssembly<assemblyApplication>(config);
    builder.Services.InstallFromAssembly<assemblyInfrastructure>(config);
}
var app = builder.Build();
 
app.Run();
```


### NEW - Chain Adding Program.cs
```csharp

// IAssemblyMaker is a simple interface that refer to assembly, you can use another type
using assemblyApplication = Installer.ConsumerWebAppl.IAssemblyMaker;
using assemblyInfrastructure = Installer.ConsumerWebApp.Infrastructure.IAssemblyMaker;

var builder = WebApplication.CreateBuilder(args);
{
    // get Configuration
    var config = builder.Configuration;
    
    // Install from your assemblies - 1
    builder.Services.Installer<assemblyApplication>(config).Finish();
    
    // Install from your assemblies - 2
    builder.Services.Installer<assemblyApplication>(config)
                          .NextOne<assemblyInfrastructure>()
                            //.NextOne<T>() ...
                              .Finish();
    
    // Install from your assemblies - 2
    builder.Services.Installer<assemblyApplication>(config)
                          .Finish<assemblyInfrastructure>();
    
    
}
var app = builder.Build();
 
app.Run();
```

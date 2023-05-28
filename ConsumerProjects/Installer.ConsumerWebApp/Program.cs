
using assemblyPresentation = App.IAssemblyMaker;
using assemblyInfrastructure = App.Infrastructure.IAssemblyMaker;
using assemblyApplication = App.IAssemblyMaker;


var builder = WebApplication.CreateBuilder(args);
{
    var config = builder.Configuration;

    // old version
    builder.Services.InstallFromAssembly<assemblyPresentation>(config);
    builder.Services.InstallFromAssembly<assemblyInfrastructure>(config);
    builder.Services.InstallFromAssembly<assemblyApplication>(config);

    // new
    builder.Services.Installer<assemblyPresentation>(config)
                        .NextOne<assemblyInfrastructure>()
                            .NextOne<assemblyApplication>()
                                .Finish();
    // Or
    builder.Services.Installer<assemblyPresentation>(config)
                        .NextOne<assemblyInfrastructure>()
                            .Finish<assemblyApplication>();
}


var app = builder.Build();
 
app.Run();

 
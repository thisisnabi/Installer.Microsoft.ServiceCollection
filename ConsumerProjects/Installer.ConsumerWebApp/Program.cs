
using assemblyApplication = Installer.ConsumerWebAppl.IAssemblyMaker;
using assemblyInfrastructure = Installer.ConsumerWebApp.Infrastructure.IAssemblyMaker;

var builder = WebApplication.CreateBuilder(args);
{
    var config = builder.Configuration;

    // IAssemblyMaker is a simple interface that refer to assembly, you can use another type
    builder.Services.InstallFromAssembly<assemblyApplication>(config);
    builder.Services.InstallFromAssembly<assemblyInfrastructure>(config);
}


var app = builder.Build();
 
app.Run();

 
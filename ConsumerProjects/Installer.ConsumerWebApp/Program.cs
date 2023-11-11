using Installer.ConsumerWebAppSupperClean;

WebApplication.CreateBuilder(args)
              .BuildIt<IAssemblyMaker>()
              .Run();
 
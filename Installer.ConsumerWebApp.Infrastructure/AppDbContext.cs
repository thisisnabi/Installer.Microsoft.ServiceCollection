using Microsoft.EntityFrameworkCore;

namespace Installer.ConsumerWebApp.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
        
    }
}
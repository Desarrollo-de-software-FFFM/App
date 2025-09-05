using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExploraYa1.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class ExploraYa1DbContextFactory : IDesignTimeDbContextFactory<ExploraYa1DbContext>
{
    public ExploraYa1DbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        ExploraYa1EfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<ExploraYa1DbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new ExploraYa1DbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ExploraYa1.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}

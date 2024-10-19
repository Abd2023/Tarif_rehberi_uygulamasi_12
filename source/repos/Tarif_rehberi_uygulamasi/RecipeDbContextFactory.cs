using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class RecipeDbContextFactory : IDesignTimeDbContextFactory<RecipeDbContext>
{
    public RecipeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RecipeDbContext>();

        // Load configuration from appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

        var connectionString = configuration.GetConnectionString("RecipeDatabase");
        optionsBuilder.UseSqlServer(connectionString);


        return new RecipeDbContext(optionsBuilder.Options);
    }
}

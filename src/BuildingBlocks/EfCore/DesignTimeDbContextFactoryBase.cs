using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration.FileExtensions;
//using Microsoft.Extensions.Configuration.Json; 
// ova dva mora da se instaliraju, ali ne trebaju ovde, jer su extension za Microsoft.Extension.Configuration

namespace BuildingBlocks.EfCore
{   
    // Pogledaj DesignTimeDbContextFactory.txt
    public abstract class DesignTimeDbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext> where TContext : DbContext
    {   
        // Interface metoda koju poziva EF Core prilikom migracije 
        public TContext CreateDbContext(string[] args)
        {
            // Environment.GetEnvironmentVariable procita iz ASPNETCORE_ENVIRONMENT iz docker-compose.yml ili xml 
            return Create(Directory.GetCurrentDirectory(), Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!);
        }

        // Implementiram u konkretnom DesignTimeDbContextFactory 
        protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);

        public TContext Create()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;
            var basePath = AppContext.BaseDirectory;

            return Create(basePath, environmentName);
        }

        private TContext Create(string basePath, string environmentName)
        {   
            // Poenta je dohvatiti Connection String iz appsettings/appsettings.development/.env bez pokretanja aplikacije 

            var configurationBuilder = new ConfigurationBuilder().SetBasePath(basePath) // basePath = Directory.GetCurrentDirecotry() = root projekta 
                                                    .AddJsonFile("appsettings.json") // appsettings.json is in root projekta i mora odma nakon basePath
                                                    .AddJsonFile($"appsettings.{environmentName}.json", true) // ako postoji appsettings.development.json
                                                    .AddEnvironmentVariables(); // ako postoji u .env nesto
                                                    
            var configuration = configurationBuilder.Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection"); // = builder.Configuration.GetConnectionString("DefaultConnection") iz Program.cs

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Could not find a connection string named 'Default' ");
            
            return Create(connectionString);
        }

        private TContext Create(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException($"{nameof(connectionString)} is null or empty.", nameof(connectionString));

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();

            optionsBuilder.UseSqlServer(connectionString);

            var options = optionsBuilder.Options;

            return CreateNewInstance(options); // Zavisi od tipa DbContexta koji prosledim u child ove klase jer ce njega da kreira
        } 
    }
}

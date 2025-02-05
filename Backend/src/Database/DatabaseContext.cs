using Microsoft.EntityFrameworkCore;
using WebApi.Enviroment;
using WebApi.StudyInfo;

namespace WebApi.Database;

public class DatabaseContext : DbContext
{
    private readonly EnvConfig  _envConfig;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, EnvConfig envConfig) : base(options)
    {
        _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
    }

    /*public DatabaseContext(EnvConfig envConfig)
    {
        _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
    }*/

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = @$"Server={_envConfig.Get("DB_SERVER")};Database={_envConfig.Get("DB_DATABASE")};User ID={_envConfig.Get("DB_USERNAME")};Password={_envConfig.Get("DB_PASSWORD")}";

            optionsBuilder.UseSqlServer(connectionString);

            Console.WriteLine("Connection to database was succesfull");
        }
    }

    public DbSet<Subject> Subject { get; set; }
    public DbSet<Degree> Degree { get; set; }
}
using Microsoft.EntityFrameworkCore;
using WebApi.Services.BackgroundServices;
using WebApi.Database;
using WebApi.Enviroment;

namespace WebApi.Configure;

public class Configuration
{
    private void SetUpDataBaseConnection(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<DatabaseContext>((serviceProvider, options) =>
        {
            EnvConfig envConfig = serviceProvider.GetRequiredService<EnvConfig>();
            //string connectionString = @$"Data source=\\{envConfig.Get("DB_IP")}\{envConfig.Get("DB_SHAREDFOLDER")}\{envConfig.Get("DB_DATABASE")}";
            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), @"LocalDatabase\PersonalWebsite.db");
            Console.WriteLine("Resolved DB Path: " + dbPath);

            string connectionString = $@"Data source={dbPath}";

            options.UseSqlite(connectionString)
                .EnableSensitiveDataLogging();

            Console.WriteLine("Connection to database was succesfull");
        });
    }

    public void SetupApp(WebApplicationBuilder builder)
    {
        SetUpDataBaseConnection(builder);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "";
                options.SwaggerEndpoint("/swagger/v0.1/swagger.json", "Personal Website");
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    private void AddSingletonForBuilder(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<EnvConfig>();
        //builder.Services.AddSingleton<IHostedService, StudyProgressionBackgroundSerice>();
    }

    private void AddScopedForBuilder(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<Configuration>();
    }

    private void AddHostedServiceForBuilder(WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<StudyProgressionBackgroundSerice>();
    }

    private void AddDefaultSerivesForBuilder(WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v0.1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Personal Website",
                Version = "v0.1",
                Description = "Personal Website",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "Jarno Daniël Vis",
                    Email = "jd.vis@hotmail.nl",
                    //Url = new Uri("https://JDVServer.com")
                }
            });
        });
    }

    private void SetLogginForBuilder(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
        builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information);
    }

    public WebApplicationBuilder SetupBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseUrls("http://localhost:5072"/*, "https://localhost:7147"*/);
        
        builder.Services.AddCors(options => 
        {
            options.AddPolicy("AllowSpecificOrgigins", policy => 
            policy.WithOrigins("")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin());
        });

        AddDefaultSerivesForBuilder(builder);
        AddScopedForBuilder(builder);
        AddSingletonForBuilder(builder);
        AddHostedServiceForBuilder(builder);
        SetLogginForBuilder(builder);

        return builder;
    }
}
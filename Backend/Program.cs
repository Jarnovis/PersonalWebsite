using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApi;

public class Program
{
    private static void SetupApp(WebApplicationBuilder builder)
    {
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

    private static WebApplicationBuilder SetupBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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

        builder.WebHost.UseUrls("http://localhost:5072"/*, "https://localhost:7147"*/);


        return builder;
    }

    public static void Main(string[] args)
    {
        var builder = SetupBuilder(args);

        SetupApp(builder);
    }
}
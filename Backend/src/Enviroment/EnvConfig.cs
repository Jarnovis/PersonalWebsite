using dotenv.net;

namespace WebApi.Enviroment;

public class EnvConfig : IDisposable
{
    private readonly IConfiguration _configuration;
    private bool _disposed = false;

    public EnvConfig()
    {
        DotEnv.Load();

        var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables();
        
        _configuration = builder.Build();
    }

    public string Get(string key)
    {
        string? value = _configuration[key];

        if (string.IsNullOrEmpty(value))
        {
            Console.WriteLine($"Warning: {key} not found in enviroment");
        }
        
        return value;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                (_configuration as IDisposable)?.Dispose();
            }

            _disposed = true;
        }
    }

    ~EnvConfig()
    {
        Dispose(false);
    }
}
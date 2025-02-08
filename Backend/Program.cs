using WebApi.Configure;
namespace Backend;

public class Program
{
    public static void Main(string[] args)
    {
        Configuration configuration = new Configuration();
        configuration.SetupApp(configuration.SetupBuilder(args));
    }
}
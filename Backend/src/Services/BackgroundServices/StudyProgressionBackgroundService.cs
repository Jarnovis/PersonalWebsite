
using WebApi.Services.EmailServices;
using WebApi.Database;
using WebApi.StudyInfo;

namespace WebApi.Services.BackgroundServices;

public class StudyProgressionBackgroundSerice : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public StudyProgressionBackgroundSerice(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    var collectStudyInfoProgression = new CollectStudyInfoProgression(dbContext);

                    await Task.Run(async () => await collectStudyInfoProgression.LoadProgressionPage());
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CollectStudyInfoProgression: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
        }
    }
}
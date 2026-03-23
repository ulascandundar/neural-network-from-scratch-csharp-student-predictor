using Microsoft.EntityFrameworkCore;
using StudentMarksPredictor.API.Services;
using StudentMarksPredictor.Data;

namespace StudentMarksPredictor.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<Repository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<TrainService>();
        services.AddScoped<FineTuneService>();
        services.AddScoped<TrainingDataService>();
        services.AddScoped<TrainingDataQueryService>();
        services.AddScoped<PredictService>();
        services.AddScoped<ScoreService>();
        services.AddScoped<TrainHistoryService>();

        return services;
    }
}

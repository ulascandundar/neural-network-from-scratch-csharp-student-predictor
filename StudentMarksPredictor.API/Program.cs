using Microsoft.EntityFrameworkCore;
using StudentMarksPredictor.API.Middlewares;
using StudentMarksPredictor.API.Services;
using StudentMarksPredictor.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=studentmarks.db"));

builder.Services.AddScoped<Repository>();
builder.Services.AddScoped<TrainService>();
builder.Services.AddScoped<FineTuneService>();
builder.Services.AddScoped<TrainingDataService>();
builder.Services.AddScoped<PredictService>();
builder.Services.AddScoped<ScoreService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    var repo = scope.ServiceProvider.GetRequiredService<Repository>();
    var csvPath = Path.Combine(app.Environment.ContentRootPath, "..", "Student_Marks.csv");
    if (File.Exists(csvPath))
        await repo.SeedFromCsvAsync(csvPath);

    var trainService = scope.ServiceProvider.GetRequiredService<TrainService>();
    await trainService.TrainIfNoModelExistsAsync();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using StudentMarksPredictor.Data.Models;

namespace StudentMarksPredictor.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TrainingRecord> TrainingRecords => Set<TrainingRecord>();
    public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
    public DbSet<ModelWeight> ModelWeights => Set<ModelWeight>();
}

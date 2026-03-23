using Microsoft.EntityFrameworkCore;
using StudentMarksPredictor.Data.Models;

namespace StudentMarksPredictor.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TrainingRecord> TrainingRecords => Set<TrainingRecord>();
    public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
    public DbSet<ModelWeight> ModelWeights => Set<ModelWeight>();

    public override int SaveChanges()
    {
        SetBaseEntityFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetBaseEntityFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetBaseEntityFields()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.Id == Guid.Empty)
                    entry.Entity.Id = Guid.NewGuid();

                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
        }
    }
}

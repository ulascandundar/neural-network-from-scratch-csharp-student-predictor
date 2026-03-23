namespace StudentMarksPredictor.Data.Models;

public class ModelWeight : BaseEntity
{
    public Guid SessionId { get; set; }
    public string Layer { get; set; } = "";
    public int Row { get; set; }
    public int Col { get; set; }
    public double Value { get; set; }

    public TrainingSession Session { get; set; } = null!;
}

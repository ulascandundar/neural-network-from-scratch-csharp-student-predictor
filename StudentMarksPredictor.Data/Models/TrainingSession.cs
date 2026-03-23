namespace StudentMarksPredictor.Data.Models;

public class TrainingSession
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Epochs { get; set; }
    public double LearningRate { get; set; }
    public double FinalMSE { get; set; }
    public int HiddenSize { get; set; }
    public double InputMin0 { get; set; }
    public double InputMax0 { get; set; }
    public double InputMin1 { get; set; }
    public double InputMax1 { get; set; }
    public double OutputMin { get; set; }
    public double OutputMax { get; set; }

    public List<ModelWeight> Weights { get; set; } = new();
}

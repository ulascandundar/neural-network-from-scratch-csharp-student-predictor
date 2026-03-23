namespace StudentMarksPredictor.API.DTOs;

public class TrainHistoryItem
{
    public Guid SessionId { get; set; }
    public int Epochs { get; set; }
    public double LearningRate { get; set; }
    public double FinalMSE { get; set; }
    public int HiddenSize { get; set; }
    public DateTime CreatedAt { get; set; }
}

namespace StudentMarksPredictor.API.DTOs;

public class TrainResponse
{
    public Guid SessionId { get; set; }
    public double TrainMSE { get; set; }
    public double TestMSE { get; set; }
    public int Epochs { get; set; }
    public double LearningRate { get; set; }
    public int HiddenSize { get; set; }
    public int TrainRecords { get; set; }
    public int TestRecords { get; set; }
    public DateTime CreatedAt { get; set; }
}

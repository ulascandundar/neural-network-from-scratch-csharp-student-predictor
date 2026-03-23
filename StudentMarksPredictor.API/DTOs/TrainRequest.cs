namespace StudentMarksPredictor.API.DTOs;

public class TrainRequest
{
    public int Epochs { get; set; } = 1000;
    public double LearningRate { get; set; } = 0.001;
    public int HiddenSize { get; set; } = 8;
    public double TestSplitRatio { get; set; } = 0.2;
}

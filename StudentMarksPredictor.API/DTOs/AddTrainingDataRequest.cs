namespace StudentMarksPredictor.API.DTOs;

public class AddTrainingDataRequest
{
    public List<TrainingDataItem> Records { get; set; } = new();
}

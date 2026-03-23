namespace StudentMarksPredictor.API.DTOs;

public class GetTrainingDataResponse
{
    public int TotalRecords { get; set; }
    public List<TrainingDataItem> Records { get; set; } = new();
}

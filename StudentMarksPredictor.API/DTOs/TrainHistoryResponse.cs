namespace StudentMarksPredictor.API.DTOs;

public class TrainHistoryResponse
{
    public int TotalSessions { get; set; }
    public List<TrainHistoryItem> Sessions { get; set; } = new();
}

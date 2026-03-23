namespace StudentMarksPredictor.API.DTOs;

public class ScoreResponse
{
    public double MSE { get; set; }
    public double MAE { get; set; }
    public double R2 { get; set; }
    public int TotalRecords { get; set; }
    public Guid SessionId { get; set; }
}

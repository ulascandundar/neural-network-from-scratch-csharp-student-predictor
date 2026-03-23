namespace StudentMarksPredictor.API.DTOs;

public class PredictResponse
{
    public double NumberCourses { get; set; }
    public double TimeStudy { get; set; }
    public double PredictedMarks { get; set; }
    public int SessionId { get; set; }
}

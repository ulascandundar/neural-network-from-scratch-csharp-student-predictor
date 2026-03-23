namespace StudentMarksPredictor.API.DTOs;

public class PredictResponse
{
    public int NumberCourses { get; set; }
    public double TimeStudy { get; set; }
    public double PredictedMarks { get; set; }
    public Guid SessionId { get; set; }
}

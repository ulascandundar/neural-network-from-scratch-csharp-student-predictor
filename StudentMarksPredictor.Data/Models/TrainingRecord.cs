namespace StudentMarksPredictor.Data.Models;

public class TrainingRecord : BaseEntity
{
    public double NumberCourses { get; set; }
    public double TimeStudy { get; set; }
    public double Marks { get; set; }
}

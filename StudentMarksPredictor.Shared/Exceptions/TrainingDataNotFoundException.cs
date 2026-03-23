namespace StudentMarksPredictor.Shared.Exceptions;

public class TrainingDataNotFoundException : LoggableException
{
    public TrainingDataNotFoundException()
        : base("Veritabaninda egitim verisi bulunamadi", 404)
    {
    }
}

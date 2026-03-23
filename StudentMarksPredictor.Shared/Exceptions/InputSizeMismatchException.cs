namespace StudentMarksPredictor.Shared.Exceptions;

public class InputSizeMismatchException : LoggableException
{
    public InputSizeMismatchException(int expected, int actual)
        : base($"Beklenen input boyutu {expected}, gelen {actual}", 400)
    {
    }
}

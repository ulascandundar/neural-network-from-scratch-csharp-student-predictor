namespace StudentMarksPredictor.Shared.Exceptions;

public class ValidationException : UnloggableException
{
    public ValidationException(string message) : base(message, 400)
    {
    }
}

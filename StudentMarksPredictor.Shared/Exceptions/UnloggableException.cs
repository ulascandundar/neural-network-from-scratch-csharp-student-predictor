namespace StudentMarksPredictor.Shared.Exceptions;

public abstract class UnloggableException : BaseException
{
    protected UnloggableException(string message, int statusCode = 400) : base(message, statusCode)
    {
    }
}

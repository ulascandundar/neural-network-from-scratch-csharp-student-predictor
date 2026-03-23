namespace StudentMarksPredictor.Shared.Exceptions;

public abstract class LoggableException : BaseException
{
    protected LoggableException(string message, int statusCode = 500) : base(message, statusCode)
    {
    }
}

namespace BallastLane.TaskManager.Application.Exceptions;

public sealed class ApplicationNotFoundException : Exception
{
    public ApplicationNotFoundException(string message)
        : base(message)
    {
    }
}

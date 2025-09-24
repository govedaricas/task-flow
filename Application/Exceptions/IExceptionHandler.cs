namespace Application.Exceptions
{
    public interface IExceptionHandler
    {
        ValueTask<bool> TryHandleAsync(Exception exception, CancellationToken cancellationToken);
    }
}

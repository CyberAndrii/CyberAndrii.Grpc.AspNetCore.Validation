namespace CyberAndrii.Grpc.AspNetCore.Validation.Helpers;

internal class AsyncStreamReaderAction<TRequest> : IAsyncStreamReader<TRequest>
{
    private readonly IAsyncStreamReader<TRequest> _innerReader;
    private readonly Func<TRequest, CancellationToken, Task> _asyncAction;

    public AsyncStreamReaderAction(
        IAsyncStreamReader<TRequest> innerReader,
        Func<TRequest, CancellationToken, Task> asyncAction)
    {
        _innerReader = innerReader;
        _asyncAction = asyncAction;
    }

    public TRequest Current => _innerReader.Current;

    public async Task<bool> MoveNext(CancellationToken cancellationToken)
    {
        var success = await _innerReader.MoveNext(cancellationToken).ConfigureAwait(false);

        if (success)
        {
            await _asyncAction(Current, cancellationToken);
        }

        return success;
    }
}

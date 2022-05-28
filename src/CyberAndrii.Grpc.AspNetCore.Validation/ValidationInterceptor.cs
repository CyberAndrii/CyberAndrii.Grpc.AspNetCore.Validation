using Grpc.Core.Interceptors;

namespace CyberAndrii.Grpc.AspNetCore.Validation;

public sealed class ValidationInterceptor : Interceptor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IValidationResultHandler _validationResultHandler;

    public ValidationInterceptor(IServiceProvider serviceProvider, IValidationResultHandler validationResultHandler)
    {
        _serviceProvider = serviceProvider;
        _validationResultHandler = validationResultHandler;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        await ValidateRequestAsync(request, context.CancellationToken);
        return await continuation(request, context);
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        requestStream = ConvertStreamWithValidation(requestStream);
        return await continuation(requestStream, context);
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await ValidateRequestAsync(request, context.CancellationToken);
        await continuation(request, responseStream, context);
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        requestStream = ConvertStreamWithValidation(requestStream);
        await continuation(requestStream, responseStream, context);
    }

    private IAsyncStreamReader<TRequest> ConvertStreamWithValidation<TRequest>(
        IAsyncStreamReader<TRequest> requestStream)
        where TRequest : class
    {
        return new AsyncStreamReaderAction<TRequest>(requestStream, ValidateRequestAsync);
    }

    private async Task ValidateRequestAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : class
    {
        var validator = _serviceProvider.GetService<IValidator<TRequest>>();

        if (validator == null)
        {
            throw new InvalidOperationException(
                $"No validator for type '{typeof(TRequest).FullName}' has been registered.");
        }

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (result.IsValid)
        {
            return;
        }

        _validationResultHandler.Handle(result);
    }
}

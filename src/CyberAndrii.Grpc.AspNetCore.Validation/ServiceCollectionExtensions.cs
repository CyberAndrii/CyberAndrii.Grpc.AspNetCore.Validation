using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CyberAndrii.Grpc.AspNetCore.Validation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcValidation(
        this IServiceCollection services,
        Action<ValidationOptions>? options = null)
    {
        services.TryAddScoped<IValidationResultHandler, DefaultValidationResultHandler>();

        if (options != null)
        {
            services.Configure(options);
        }

        return services;
    }

    public static IServiceCollection AddGrpcValidationGlobally(
        this IServiceCollection services,
        Action<ValidationOptions>? options = null)
    {
        return services
            .AddGrpcValidation(options)
            .Configure((Action<GrpcServiceOptions>) (grpcOptions => grpcOptions.EnableRequestValidation()));
    }

    public static IServiceCollection AddValidator<TValidator, TTypeToValidate>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TValidator : class, IValidator<TTypeToValidate>
        where TTypeToValidate : class
    {
        services.Add(new ServiceDescriptor(typeof(IValidator<TTypeToValidate>), typeof(TValidator), lifetime));
        return services;
    }
}

namespace CyberAndrii.Grpc.AspNetCore.Validation;

public static class GrpcServiceOptionsExtensions
{
    public static GrpcServiceOptions EnableRequestValidation(this GrpcServiceOptions options)
    {
        options.Interceptors.Add<ValidationInterceptor>();
        return options;
    }
}

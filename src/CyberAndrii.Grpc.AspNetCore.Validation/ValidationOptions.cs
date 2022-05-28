namespace CyberAndrii.Grpc.AspNetCore.Validation;

public sealed class ValidationOptions
{
    public bool IncludeErrorMessage { get; set; } = true;

    public bool IncludeMessagePlaceholders { get; set; } = true;

    public bool IncludeAttemptedValue { get; set; } = false;

    public bool IncludeSeverity { get; set; } = false;

    public bool IncludeErrorCode { get; set; } = false;
}

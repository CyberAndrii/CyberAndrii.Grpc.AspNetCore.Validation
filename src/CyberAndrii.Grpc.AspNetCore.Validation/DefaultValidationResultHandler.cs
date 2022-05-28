using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;

namespace CyberAndrii.Grpc.AspNetCore.Validation;

public sealed class DefaultValidationResultHandler : IValidationResultHandler
{
    private readonly ValidationOptions _options;

    public DefaultValidationResultHandler(IOptions<ValidationOptions> options)
    {
        _options = options.Value;
    }

    public void Handle(ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return;
        }

        var status = BuildStatus(validationResult);
        var trailers = BuildTrailers(validationResult);

        throw new RpcException(status, trailers);
    }

    private static Status BuildStatus(ValidationResult validationResult)
    {
        var detail = validationResult.Errors.Count == 1
            ? "'" + validationResult.Errors.Single().PropertyName + "' failed validation"
            : "Multiple properties failed validation";

        return new Status(StatusCode.InvalidArgument, detail);
    }

    private Metadata BuildTrailers(ValidationResult validationResult)
    {
        var response = Serialize(validationResult);
        var metadata = new Metadata {new("validation-errors-text", response)};

        return metadata;
    }

    private string Serialize(ValidationResult validationResult)
    {
        var errorsNode = new JsonArray();

        foreach (var error in validationResult.Errors)
        {
            var node = new JsonObject {{"propertyName", error.PropertyName},};

            if (_options.IncludeErrorMessage)
            {
                node.Add("errorMessage", error.ErrorMessage);
            }

            if (_options.IncludeMessagePlaceholders && error.FormattedMessagePlaceholderValues is {Count: > 0})
            {
                node.Add("placeholders", JsonValue.Create(error.FormattedMessagePlaceholderValues));
            }

            if (_options.IncludeAttemptedValue)
            {
                node.Add("attemptedValue", JsonValue.Create(error.AttemptedValue));
            }

            if (_options.IncludeSeverity)
            {
                node.Add("severity", JsonValue.Create(error.Severity));
            }

            if (_options.IncludeErrorCode && !string.IsNullOrWhiteSpace(error.ErrorCode))
            {
                node.Add("errorCode", error.ErrorCode);
            }

            errorsNode.Add(node);
        }

        var serializedBytes = JsonSerializer.SerializeToUtf8Bytes(errorsNode);
        var encodedString = Convert.ToBase64String(serializedBytes);

        return encodedString;
    }
}

# CyberAndrii.Grpc.AspNetCore.Validation

GRPC request validator middleware with detailed error response and proper client side localization support.

## Setup

Create a new validator using [FluentValidation](https://docs.fluentvalidation.net/en/latest/) library:

```csharp
using FluentValidation;

public class HelloRequestValidator : AbstractValidator<HelloRequest>
{
    public HelloRequestValidator()
    {
        RuleFor(request => request.Name)
            .Length(3, 30);
    }
}
```

Configure response and register your validators:

```csharp
builder.Services
    .AddGrpcValidationGlobally(options =>
    {
        options.IncludeErrorMessage = true;
        options.IncludeMessagePlaceholders = true;
        options.IncludeAttemptedValue = true;
        options.IncludeSeverity = true;
        options.IncludeErrorCode = true;
    })
    .AddValidator<HelloRequestValidator, HelloRequest>();

// If you want to use client side localization. Usefull with IncludeMessagePlaceholders enabled.
ValidatorOptions.Global.MessageFormatterFactory = () => new KeepPlaceholdersMessageFormatter();
```

If you are making requests from a browser or any JavaScript client,
don't forget to setup grpc-web and CORS rules with the following headers:

```csharp
.WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding")
.WithExposedHeaders("validation-errors-text");
```

## Retrieving validation errors

Errors come as a base64-encoded JSON object in the trailer response headers:

```yaml
validation-errors-text: W3sicHJvcGVydHlOYW1lIjoiTmFtZSIsImVycm9yTWVzc2FnZSI6Ilx1MDAyN3tQcm9wZXJ0eU5hbWV9XHUwMDI3IG11c3QgYmUgYmV0d2VlbiB7TWluTGVuZ3RofSBhbmQge01heExlbmd0aH0gY2hhcmFjdGVycy4gWW91IGVudGVyZWQge1RvdGFsTGVuZ3RofSBjaGFyYWN0ZXJzLiIsInBsYWNlaG9sZGVycyI6eyJNaW5MZW5ndGgiOjMsIk1heExlbmd0aCI6MzAsIlRvdGFsTGVuZ3RoIjoxLCJQcm9wZXJ0eU5hbWUiOiJOYW1lIiwiUHJvcGVydHlWYWx1ZSI6IngifSwiYXR0ZW1wdGVkVmFsdWUiOiJ4Iiwic2V2ZXJpdHkiOjAsImVycm9yQ29kZSI6Ikxlbmd0aFZhbGlkYXRvciJ9XQ==
```

After decoding it will look like this:

```json
[
    {
        "propertyName": "Name",
        "errorMessage": "'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.",
        "placeholders": {
            "MinLength": 3,
            "MaxLength": 30,
            "TotalLength": 1,
            "PropertyName": "Name",
            "PropertyValue": "x"
        },
        "attemptedValue": "x",
        "severity": 0,
        "errorCode": "LengthValidator"
    }
]
```

Follow [FluentValidation docs](https://docs.fluentvalidation.net/en/latest/) for more details.

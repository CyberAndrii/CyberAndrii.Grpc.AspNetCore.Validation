namespace CyberAndrii.Grpc.AspNetCore.Validation;

public interface IValidationResultHandler
{
    void Handle(ValidationResult validationResult);
}

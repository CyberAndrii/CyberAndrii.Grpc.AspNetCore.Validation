using FluentValidation.Internal;

namespace CyberAndrii.Grpc.AspNetCore.Validation.Helpers;

public class KeepPlaceholdersMessageFormatter : MessageFormatter
{
    public override string BuildMessage(string messageTemplate)
    {
        return messageTemplate;
    }
}

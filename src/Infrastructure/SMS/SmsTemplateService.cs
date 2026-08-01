using System;
using System.Collections.Generic;
using Application.Abstractions.SMS;

namespace Infrastructure.SMS;

internal sealed class SmsTemplateService : ISmsTemplateService
{
    private static readonly Dictionary<SmsTemplateType, string> Templates = new()
    {
        {
            SmsTemplateType.OtpVerification,
            "{ApplicationName} verification code: {OTP}. Valid for 10 minutes. Do not share this code with anyone."
        }
    };

    public string RenderTemplate(SmsTemplateType type, IDictionary<string, string> placeholders)
    {
        if (!Templates.TryGetValue(type, out string? template))
        {
            throw new ArgumentOutOfRangeException(nameof(type), $"SMS Template '{type}' is not registered.");
        }

        string result = template;

        if (!placeholders.ContainsKey("ApplicationName"))
        {
            placeholders["ApplicationName"] = "Practice121";
        }

        foreach (var (key, value) in placeholders)
        {
            result = result.Replace($"{{{key}}}", value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }
}

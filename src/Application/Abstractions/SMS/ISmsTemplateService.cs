using System.Collections.Generic;

namespace Application.Abstractions.SMS;

public enum SmsTemplateType
{
    OtpVerification,
}

public interface ISmsTemplateService
{
    string RenderTemplate(SmsTemplateType type, IDictionary<string, string> placeholders);
}

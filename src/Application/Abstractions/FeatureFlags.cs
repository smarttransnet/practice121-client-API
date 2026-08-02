namespace Application.Abstractions;

/// <summary>
/// Feature-flag settings populated by Infrastructure from the "Features" configuration section.
/// Toggle individual flags in appsettings.json (or environment overrides) without code changes.
/// </summary>
public sealed class FeatureFlags
{
    /// <summary>
    /// When <c>false</c> (testing mode), no SMS is dispatched and any 6-digit numeric code
    /// is accepted during OTP verification so the application flow can be tested without
    /// incurring SMS costs. Set to <c>true</c> to re-enable real SMS delivery.
    /// </summary>
    public bool SmsOtpEnabled { get; set; } = true;
}

namespace Application.Abstractions.Authentication;

public interface ISmsService
{
    Task SendSmsAsync(string mobileNumber, string message, CancellationToken cancellationToken = default);
}

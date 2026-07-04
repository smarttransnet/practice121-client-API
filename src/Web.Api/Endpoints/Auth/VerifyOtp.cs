using Application.Abstractions.Messaging;
using Application.Doctors.VerifyOtp;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

internal sealed class VerifyOtp : IEndpoint
{
    public sealed record Request(Guid SessionId, string Code);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/verify-otp", async (
            Request request,
            HttpContext httpContext,
            ICommandHandler<VerifyOtpCommand, TokenResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new VerifyOtpCommand(request.SessionId, request.Code);

            Result<TokenResponse> result = await handler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                // Append HttpOnly cookie for Web clients
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None, // Allow cross-site request if dev runs on different ports
                    Expires = DateTime.UtcNow.AddDays(7)
                };
                httpContext.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, cookieOptions);
            }

            return result.ToApiResponse();
        })
        .WithTags(Tags.Auth);
    }
}

using Application.Abstractions.Messaging;
using Application.Doctors.RefreshToken;
using Application.Doctors.VerifyOtp;
using Domain.Doctors;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

internal sealed class RefreshToken : IEndpoint
{
    public sealed record Request(string? RefreshToken);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/refresh-token", async (
            Request request,
            HttpContext httpContext,
            ICommandHandler<RefreshTokenCommand, TokenResponse> handler,
            CancellationToken cancellationToken) =>
        {
            // Try request body first (mobile), fallback to HttpOnly cookie (web)
            string? token = request.RefreshToken;
            if (string.IsNullOrEmpty(token))
            {
                token = httpContext.Request.Cookies["refreshToken"];
            }

            if (string.IsNullOrEmpty(token))
            {
                var errorResult = Result.Failure<TokenResponse>(DoctorErrors.Unauthorized);
                return errorResult.ToApiResponse();
            }

            var command = new RefreshTokenCommand(token);

            Result<TokenResponse> result = await handler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                // Rotate and append the new HttpOnly cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                };
                httpContext.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, cookieOptions);
            }

            return result.ToApiResponse();
        })
        .WithTags(Tags.Auth);
    }
}

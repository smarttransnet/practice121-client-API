using Application.Abstractions.Messaging;
using Application.Doctors.Logout;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

internal sealed class Logout : IEndpoint
{
    public sealed record Request(string? RefreshToken);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/logout", async (
            Request request,
            HttpContext httpContext,
            ICommandHandler<LogoutCommand> handler,
            CancellationToken cancellationToken) =>
        {
            // Extract token from request body or HttpOnly cookie
            string? token = request.RefreshToken;
            if (string.IsNullOrEmpty(token))
            {
                token = httpContext.Request.Cookies["refreshToken"];
            }

            if (!string.IsNullOrEmpty(token))
            {
                var command = new LogoutCommand(token);
                await handler.Handle(command, cancellationToken);
            }

            // Always clear the cookie on logout
            httpContext.Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            var successResult = Result.Success();
            return successResult.ToApiResponse();
        })
        .WithTags(Tags.Auth);
    }
}

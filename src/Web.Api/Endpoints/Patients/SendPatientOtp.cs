using Application.Abstractions.Messaging;
using Application.Patients.Otp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Patients;

internal sealed class SendPatientOtp : IEndpoint
{
    public sealed record Request(string MobileNumber);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/patients/otp/send", async (
            Request request,
            ICommandHandler<SendPatientOtpCommand, SendPatientOtpResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SendPatientOtpCommand(request.MobileNumber);
            Result<SendPatientOtpResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Patients);
    }
}

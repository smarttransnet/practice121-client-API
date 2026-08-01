using System;
using Application.Abstractions.Messaging;
using Application.Patients.Otp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Patients;

internal sealed class ResendPatientOtp : IEndpoint
{
    public sealed record Request(Guid SessionId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/patients/otp/resend", async (
            Request request,
            ICommandHandler<ResendPatientOtpCommand, ResendPatientOtpResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ResendPatientOtpCommand(request.SessionId);
            Result<ResendPatientOtpResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Patients);
    }
}

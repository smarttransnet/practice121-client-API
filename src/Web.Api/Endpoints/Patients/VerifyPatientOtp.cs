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

internal sealed class VerifyPatientOtp : IEndpoint
{
    public sealed record Request(Guid SessionId, string OtpCode);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/patients/otp/verify", async (
            Request request,
            ICommandHandler<VerifyPatientOtpCommand, VerifyPatientOtpResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new VerifyPatientOtpCommand(request.SessionId, request.OtpCode);
            Result<VerifyPatientOtpResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Patients);
    }
}

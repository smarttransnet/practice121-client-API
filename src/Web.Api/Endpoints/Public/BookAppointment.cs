using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Appointments.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Public;

public class BookAppointment : IEndpoint
{
    public sealed record Request(
        string PatientMobile,
        Guid DoctorAccountId,
        Guid PracticeCentreId,
        DateOnly VisitDate);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/public/appointments",
            async (
                Request request,
                ICommandHandler<BookAppointmentCommand, BookAppointmentResult> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new BookAppointmentCommand(
                    request.PatientMobile,
                    request.DoctorAccountId,
                    request.PracticeCentreId,
                    request.VisitDate);

                var result = await handler.Handle(command, cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Value) : CustomResults.Problem(result);
            })
        .WithTags("Public")
        .AllowAnonymous();
    }
}

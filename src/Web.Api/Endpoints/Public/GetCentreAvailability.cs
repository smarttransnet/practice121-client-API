using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Appointments.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Public;

public class GetCentreAvailability : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "api/public/doctors/{accountId}/practice-centres/{centreId}/availability",
            async (
                Guid accountId,
                Guid centreId,
                [Microsoft.AspNetCore.Mvc.FromQuery] string? from,
                [Microsoft.AspNetCore.Mvc.FromQuery] string? to,
                [Microsoft.AspNetCore.Mvc.FromServices] IQueryHandler<GetCentreAvailabilityQuery, System.Collections.Generic.List<DayAvailabilityResponse>> handler,
                CancellationToken cancellationToken) =>
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var fromDate = from is not null && DateOnly.TryParse(from, System.Globalization.CultureInfo.InvariantCulture, out var f) ? f : today;
                var toDate = to is not null && DateOnly.TryParse(to, System.Globalization.CultureInfo.InvariantCulture, out var t) ? t : today.AddDays(27);

                var query = new GetCentreAvailabilityQuery(accountId, centreId, fromDate, toDate);
                var result = await handler.Handle(query, cancellationToken);

                return result.IsSuccess ? Results.Ok(result.Value) : CustomResults.Problem(result);
            })
        .WithTags("Public")
        .AllowAnonymous();
    }
}

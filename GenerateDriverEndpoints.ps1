$basePath = "src\Web.Api\Endpoints"

function Write-Endpoint {
    param($folder, $filename, $content)
    $dir = Join-Path $basePath $folder
    if (!(Test-Path $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
    Set-Content -Path (Join-Path $dir $filename) -Value $content
}

# --- Drivers ---

$createDriver = @"
using Application.Abstractions.Messaging;
using Application.Drivers.CreateDriver;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Drivers;

internal sealed class Create : IEndpoint
{
    public sealed record Request(
        string EmployeeNumber,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string LicenceNumber,
        DateOnly LicenceExpiryDate,
        string NationalityCode,
        string? Email = null,
        string? SponsorName = null);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("drivers", async (
            Request request,
            ICommandHandler<CreateDriverCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateDriverCommand(
                request.EmployeeNumber,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.LicenceNumber,
                request.LicenceExpiryDate,
                request.NationalityCode,
                request.Email,
                request.SponsorName);

            Result<Guid> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Drivers);
    }
}
"@
Write-Endpoint "Drivers" "Create.cs" $createDriver

$updateDriver = @"
using Application.Abstractions.Messaging;
using Application.Drivers.UpdateDriver;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Drivers;

internal sealed class Update : IEndpoint
{
    public sealed record Request(
        string FirstName,
        string LastName,
        string PhoneNumber,
        string LicenceNumber,
        DateOnly LicenceExpiryDate,
        string NationalityCode,
        string? Email = null,
        string? SponsorName = null);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("drivers/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateDriverCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateDriverCommand(
                id,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.LicenceNumber,
                request.LicenceExpiryDate,
                request.NationalityCode,
                request.Email,
                request.SponsorName);

            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Drivers);
    }
}
"@
Write-Endpoint "Drivers" "Update.cs" $updateDriver

$getDriverById = @"
using Application.Abstractions.Messaging;
using Application.Drivers.GetDriverById;
using Application.Drivers.GetDrivers;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Drivers;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("drivers/{id:guid}", async (
            Guid id,
            IQueryHandler<GetDriverByIdQuery, DriverResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<DriverResponse> result = await handler.Handle(new GetDriverByIdQuery(id), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Drivers);
    }
}
"@
Write-Endpoint "Drivers" "GetById.cs" $getDriverById

$getAllDrivers = @"
using Application.Abstractions.Messaging;
using Application.Drivers.GetDrivers;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Drivers;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("drivers", async (
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            IQueryHandler<GetDriversQuery, PagedList<DriverResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDriversQuery(searchTerm, sortColumn, sortOrder, page, pageSize);
            Result<PagedList<DriverResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Drivers);
    }
}
"@
Write-Endpoint "Drivers" "GetAll.cs" $getAllDrivers

# --- Driver Auth ---

$createCredentials = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Auth.CreateCredentials;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverAuth;

internal sealed class CreateCredentials : IEndpoint
{
    public sealed record Request(Guid DriverId, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("drivers/{driverId:guid}/credentials", async (
            Guid driverId,
            Request request,
            ICommandHandler<CreateCredentialsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateCredentialsCommand(driverId, request.Password);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverAuth);
    }
}
"@
Write-Endpoint "DriverAuth" "CreateCredentials.cs" $createCredentials

$driverLogin = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Auth.Login;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverAuth;

internal sealed class Login : IEndpoint
{
    public sealed record Request(string Username, string Password, string? Platform = null, string? DeviceToken = null);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("drivers/auth/login", async (
            Request request,
            ICommandHandler<LoginCommand, LoginResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginCommand(request.Username, request.Password, request.Platform, request.DeviceToken);
            Result<LoginResponse> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverAuth);
    }
}
"@
Write-Endpoint "DriverAuth" "Login.cs" $driverLogin

$driverRefresh = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Auth.Refresh;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverAuth;

internal sealed class Refresh : IEndpoint
{
    public sealed record Request(Guid DriverId, string RefreshToken);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("drivers/auth/refresh", async (
            Request request,
            ICommandHandler<RefreshCommand, RefreshResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RefreshCommand(request.DriverId, request.RefreshToken);
            Result<RefreshResponse> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverAuth);
    }
}
"@
Write-Endpoint "DriverAuth" "Refresh.cs" $driverRefresh

$driverLogout = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Auth.Logout;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverAuth;

internal sealed class Logout : IEndpoint
{
    public sealed record Request(Guid DriverId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("drivers/auth/logout", async (
            Request request,
            ICommandHandler<LogoutCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LogoutCommand(request.DriverId);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.DriverAuth);
    }
}
"@
Write-Endpoint "DriverAuth" "Logout.cs" $driverLogout

# --- Driver Attendance ---

$checkIn = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Attendance.CheckIn;
using Domain.Drivers.Enums;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverAttendance;

internal sealed class CheckIn : IEndpoint
{
    public sealed record Request(double? Latitude, double? Longitude, string? Notes, AttendanceSource? Source);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("drivers/{driverId:guid}/attendance/check-in", async (
            Guid driverId,
            Request request,
            ICommandHandler<CheckInCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CheckInCommand(driverId, request.Latitude, request.Longitude, request.Notes, request.Source);
            Result<Guid> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverAttendance);
    }
}
"@
Write-Endpoint "DriverAttendance" "CheckIn.cs" $checkIn

$checkOut = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Attendance.CheckOut;
using Domain.Drivers.Enums;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverAttendance;

internal sealed class CheckOut : IEndpoint
{
    public sealed record Request(double? Latitude, double? Longitude, string? Notes, AttendanceSource? Source);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("drivers/{driverId:guid}/attendance/check-out", async (
            Guid driverId,
            Request request,
            ICommandHandler<CheckOutCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CheckOutCommand(driverId, request.Latitude, request.Longitude, request.Notes, request.Source);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.DriverAttendance);
    }
}
"@
Write-Endpoint "DriverAttendance" "CheckOut.cs" $checkOut

$getAttendance = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Attendance.GetAttendance;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverAttendance;

internal sealed class GetAttendanceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("drivers/{driverId:guid}/attendance", async (
            Guid driverId,
            [FromQuery] DateOnly? startDate,
            [FromQuery] DateOnly? endDate,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            IQueryHandler<GetAttendanceQuery, PagedList<AttendanceResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAttendanceQuery(driverId, startDate, endDate, page, pageSize);
            Result<PagedList<AttendanceResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverAttendance);
    }
}
"@
Write-Endpoint "DriverAttendance" "GetAttendance.cs" $getAttendance

# --- Expenses ---

$submitExpense = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Expenses.SubmitExpense;
using Domain.Drivers.Enums;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverExpenses;

internal sealed class Submit : IEndpoint
{
    public sealed record Request(Guid? TripId, ExpenseType ExpenseType, decimal Amount, DateOnly ExpenseDate, string Description, Uri? ReceiptUrl, decimal? FuelLitres, string? FuelStation, decimal? OdometerReading);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("drivers/{driverId:guid}/expenses", async (
            Guid driverId,
            Request request,
            ICommandHandler<SubmitExpenseCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SubmitExpenseCommand(
                driverId,
                request.TripId,
                request.ExpenseType,
                request.Amount,
                request.ExpenseDate,
                request.Description,
                request.ReceiptUrl,
                request.FuelLitres,
                request.FuelStation,
                request.OdometerReading);

            Result<Guid> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverExpenses);
    }
}
"@
Write-Endpoint "DriverExpenses" "Submit.cs" $submitExpense

$updateExpense = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Expenses.UpdateExpense;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverExpenses;

internal sealed class Update : IEndpoint
{
    public sealed record Request(decimal Amount, DateOnly ExpenseDate, string Description, Uri? ReceiptUrl);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("drivers/expenses/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateExpenseCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateExpenseCommand(id, request.Amount, request.ExpenseDate, request.Description, request.ReceiptUrl);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.DriverExpenses);
    }
}
"@
Write-Endpoint "DriverExpenses" "Update.cs" $updateExpense

$reviewExpense = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Expenses.ReviewExpense;
using Domain.Drivers.Enums;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverExpenses;

internal sealed class Review : IEndpoint
{
    public sealed record Request(ExpenseStatus Status, string? ReviewNotes);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("drivers/expenses/{id:guid}/review", async (
            Guid id,
            Request request,
            ICommandHandler<ReviewExpenseCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ReviewExpenseCommand(id, request.Status, request.ReviewNotes);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.DriverExpenses);
    }
}
"@
Write-Endpoint "DriverExpenses" "Review.cs" $reviewExpense

$getExpenses = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Expenses.GetExpenses;
using Domain.Drivers.Enums;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverExpenses;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("drivers/{driverId:guid}/expenses", async (
            Guid driverId,
            [FromQuery] ExpenseType? type,
            [FromQuery] ExpenseStatus? status,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            IQueryHandler<GetExpensesQuery, PagedList<ExpenseResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetExpensesQuery(driverId, type, status, page, pageSize);
            Result<PagedList<ExpenseResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverExpenses);
    }
}
"@
Write-Endpoint "DriverExpenses" "GetAll.cs" $getExpenses

# --- Driver Assignments ---

$acceptAssignment = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Assignments.AcceptAssignment;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverAssignments;

internal sealed class Accept : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("drivers/assignments/{id:guid}/accept", async (
            Guid id,
            ICommandHandler<AcceptAssignmentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AcceptAssignmentCommand(id);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.DriverAssignments);
    }
}
"@
Write-Endpoint "DriverAssignments" "Accept.cs" $acceptAssignment

$rejectAssignment = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Assignments.RejectAssignment;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverAssignments;

internal sealed class Reject : IEndpoint
{
    public sealed record Request(string? RejectionReason);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("drivers/assignments/{id:guid}/reject", async (
            Guid id,
            Request request,
            ICommandHandler<RejectAssignmentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RejectAssignmentCommand(id, request.RejectionReason);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.DriverAssignments);
    }
}
"@
Write-Endpoint "DriverAssignments" "Reject.cs" $rejectAssignment

$getAssignments = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Assignments.GetAssignments;
using Domain.Drivers.Enums;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverAssignments;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("drivers/{driverId:guid}/assignments", async (
            Guid driverId,
            [FromQuery] AssignmentStatus? status,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            IQueryHandler<GetAssignmentsQuery, PagedList<AssignmentResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAssignmentsQuery(driverId, status, page, pageSize);
            Result<PagedList<AssignmentResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverAssignments);
    }
}
"@
Write-Endpoint "DriverAssignments" "GetAll.cs" $getAssignments

# --- Location & GPS ---

$recordLocation = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Location.RecordLocation;
using Domain.Drivers.Enums;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverLocation;

internal sealed class Record : IEndpoint
{
    public sealed record Request(Guid? TripId, double Latitude, double Longitude, float? Accuracy, float? SpeedKmh, float? Bearing, LocationSource Source);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("drivers/{driverId:guid}/location", async (
            Guid driverId,
            Request request,
            ICommandHandler<RecordLocationCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RecordLocationCommand(
                driverId,
                request.TripId,
                request.Latitude,
                request.Longitude,
                request.Accuracy,
                request.SpeedKmh,
                request.Bearing,
                request.Source);

            Result<Guid> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverLocation);
    }
}
"@
Write-Endpoint "DriverLocation" "Record.cs" $recordLocation

$getLatestLocation = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Location.GetLatestLocation;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverLocation;

internal sealed class GetLatest : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("drivers/{driverId:guid}/location/latest", async (
            Guid driverId,
            IQueryHandler<GetLatestLocationQuery, LocationResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<LocationResponse> result = await handler.Handle(new GetLatestLocationQuery(driverId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverLocation);
    }
}
"@
Write-Endpoint "DriverLocation" "GetLatest.cs" $getLatestLocation

$getGpsLogs = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Location.GetGpsLogs;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverLocation;

internal sealed class GetGpsLogsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("drivers/{driverId:guid}/gps-logs", async (
            Guid driverId,
            [FromQuery] Guid? tripId,
            [FromQuery] DateOnly? date,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            IQueryHandler<GetGpsLogsQuery, PagedList<GpsLogResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGpsLogsQuery(driverId, tripId, date, page, pageSize);
            Result<PagedList<GpsLogResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverLocation);
    }
}
"@
Write-Endpoint "DriverLocation" "GetGpsLogs.cs" $getGpsLogs

$getGpsLogById = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Location.GetGpsLogById;
using Application.Drivers.Location.GetGpsLogs;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverLocation;

internal sealed class GetGpsLogByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("drivers/gps-logs/{id:guid}", async (
            Guid id,
            IQueryHandler<GetGpsLogByIdQuery, GpsLogResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<GpsLogResponse> result = await handler.Handle(new GetGpsLogByIdQuery(id), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverLocation);
    }
}
"@
Write-Endpoint "DriverLocation" "GetGpsLogById.cs" $getGpsLogById

# --- Documents ---

$uploadDocument = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Documents.UploadDocument;
using Domain.Drivers.Enums;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverDocuments;

internal sealed class Upload : IEndpoint
{
    public sealed record Request(Guid? TripId, DriverDocumentType DocumentType, string Title, Uri FileUrl, bool SubmittedFromApp, double? Latitude, double? Longitude);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("drivers/{driverId:guid}/documents", async (
            Guid driverId,
            Request request,
            ICommandHandler<UploadDocumentCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UploadDocumentCommand(
                driverId,
                request.TripId,
                request.DocumentType,
                request.Title,
                request.FileUrl,
                request.SubmittedFromApp,
                request.Latitude,
                request.Longitude);

            Result<Guid> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverDocuments);
    }
}
"@
Write-Endpoint "DriverDocuments" "Upload.cs" $uploadDocument

$getDocuments = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Documents.GetDocuments;
using Domain.Drivers.Enums;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverDocuments;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("drivers/{driverId:guid}/documents", async (
            Guid driverId,
            [FromQuery] DriverDocumentType? type,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            IQueryHandler<GetDocumentsQuery, PagedList<DocumentResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDocumentsQuery(driverId, type, page, pageSize);
            Result<PagedList<DocumentResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverDocuments);
    }
}
"@
Write-Endpoint "DriverDocuments" "GetAll.cs" $getDocuments

# --- Notifications ---

$sendNotification = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Notifications.SendNotification;
using Domain.Drivers.Enums;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverNotifications;

internal sealed class Send : IEndpoint
{
    public sealed record Request(NotificationType NotificationType, NotificationChannel Channel, string Title, string MessageBody, Guid? RelatedEntityId, string? MetaDataJson);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("drivers/{driverId:guid}/notifications", async (
            Guid driverId,
            Request request,
            ICommandHandler<SendNotificationCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SendNotificationCommand(
                driverId,
                request.NotificationType,
                request.Channel,
                request.Title,
                request.MessageBody,
                request.RelatedEntityId,
                request.MetaDataJson);

            Result<Guid> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverNotifications);
    }
}
"@
Write-Endpoint "DriverNotifications" "Send.cs" $sendNotification

$markNotificationRead = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Notifications.MarkNotificationRead;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverNotifications;

internal sealed class MarkRead : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("drivers/notifications/{id:guid}/read", async (
            Guid id,
            ICommandHandler<MarkNotificationReadCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new MarkNotificationReadCommand(id);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.DriverNotifications);
    }
}
"@
Write-Endpoint "DriverNotifications" "MarkRead.cs" $markNotificationRead

$getNotifications = @"
using Application.Abstractions.Messaging;
using Application.Drivers.Notifications.GetNotifications;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.DriverNotifications;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("drivers/{driverId:guid}/notifications", async (
            Guid driverId,
            [FromQuery] bool unreadOnly,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            IQueryHandler<GetNotificationsQuery, PagedList<NotificationResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetNotificationsQuery(driverId, unreadOnly, page, pageSize);
            Result<PagedList<NotificationResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.DriverNotifications);
    }
}
"@
Write-Endpoint "DriverNotifications" "GetAll.cs" $getNotifications

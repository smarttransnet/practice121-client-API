using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Appointments.Commands;
using Domain.PatientQueue;
using Domain.PracticeCentres;
using Infrastructure.Database;
using Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Xunit;

namespace Application.UnitTests;

public class SessionAndQueueBookingTests
{
    private sealed class TestDomainEventsDispatcher : IDomainEventsDispatcher
    {
        public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options, new TestDomainEventsDispatcher());
    }

    [Fact]
    public async Task BookAppointment_MorningSession_AssignsMorningSessionAndQueue()
    {
        using var context = CreateDbContext();
        var doctorId = Guid.NewGuid();
        var placeId = Guid.NewGuid();

        var centre = PracticeCentre.Create(doctorId, placeId, "Test Clinic", 20);
        var sessionGroup = SessionGroup.Create(centre.Id, new List<string> { "MON" });
        var morningTimeBlock = TimeBlock.Create(sessionGroup.Id, "Morning", new TimeSpan(7, 0, 0), new TimeSpan(10, 0, 0));
        sessionGroup.AddTimeBlock(morningTimeBlock);

        centre.AddSessionGroup(sessionGroup);
        context.PracticeCentres.Add(centre);
        await context.SaveChangesAsync();

        var handler = new BookAppointmentCommandHandler(context);
        var command = new BookAppointmentCommand(
            "0710000001",
            doctorId,
            centre.Id,
            new DateOnly(2026, 8, 31), // Monday
            null,
            morningTimeBlock.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.QueueNumber);

        var ticket = await context.PatientQueueTickets.FirstAsync(t => t.Id == result.Value.TicketId);
        Assert.Equal(morningTimeBlock.Id, ticket.SessionId);
    }

    [Fact]
    public async Task BookAppointment_IndependentQueueNumberingPerSession()
    {
        using var context = CreateDbContext();
        var doctorId = Guid.NewGuid();
        var placeId = Guid.NewGuid();

        var centre = PracticeCentre.Create(doctorId, placeId, "Test Clinic", 20);
        var sessionGroup = SessionGroup.Create(centre.Id, new List<string> { "MON" });
        var morningBlock = TimeBlock.Create(sessionGroup.Id, "Morning", new TimeSpan(7, 0, 0), new TimeSpan(10, 0, 0));
        var eveningBlock = TimeBlock.Create(sessionGroup.Id, "Evening", new TimeSpan(17, 0, 0), new TimeSpan(20, 0, 0));
        sessionGroup.AddTimeBlock(morningBlock);
        sessionGroup.AddTimeBlock(eveningBlock);

        centre.AddSessionGroup(sessionGroup);
        context.PracticeCentres.Add(centre);
        await context.SaveChangesAsync();

        var handler = new BookAppointmentCommandHandler(context);
        var date = new DateOnly(2026, 8, 31);

        // Book 1 Morning ticket
        var resMorning1 = await handler.Handle(new BookAppointmentCommand("0710000001", doctorId, centre.Id, date, null, morningBlock.Id), CancellationToken.None);
        // Book 1 Evening ticket
        var resEvening1 = await handler.Handle(new BookAppointmentCommand("0710000002", doctorId, centre.Id, date, null, eveningBlock.Id), CancellationToken.None);

        Assert.True(resMorning1.IsSuccess);
        Assert.True(resEvening1.IsSuccess);
        Assert.Equal(1, resMorning1.Value.QueueNumber);
        Assert.Equal(1, resEvening1.Value.QueueNumber);

        var morningTicket = await context.PatientQueueTickets.FirstAsync(t => t.Id == resMorning1.Value.TicketId);
        var eveningTicket = await context.PatientQueueTickets.FirstAsync(t => t.Id == resEvening1.Value.TicketId);

        Assert.Equal(morningBlock.Id, morningTicket.SessionId);
        Assert.Equal(eveningBlock.Id, eveningTicket.SessionId);
    }

    [Fact]
    public async Task MaxPatientPerSession_MorningFull_BlocksMorning_AllowsEvening()
    {
        using var context = CreateDbContext();
        var doctorId = Guid.NewGuid();
        var placeId = Guid.NewGuid();

        // Max 2 patients per session
        var centre = PracticeCentre.Create(doctorId, placeId, "Test Clinic", 2);
        var sessionGroup = SessionGroup.Create(centre.Id, new List<string> { "MON" });
        var morningBlock = TimeBlock.Create(sessionGroup.Id, "Morning", new TimeSpan(7, 0, 0), new TimeSpan(10, 0, 0));
        var eveningBlock = TimeBlock.Create(sessionGroup.Id, "Evening", new TimeSpan(17, 0, 0), new TimeSpan(20, 0, 0));
        sessionGroup.AddTimeBlock(morningBlock);
        sessionGroup.AddTimeBlock(eveningBlock);

        centre.AddSessionGroup(sessionGroup);
        context.PracticeCentres.Add(centre);

        var visitDate = new DateTime(2026, 8, 31, 0, 0, 0, DateTimeKind.Utc);

        // Fill Morning session with 2 patients (limit reached)
        context.PatientQueueTickets.Add(new PatientQueueTicket
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            PracticeCentreId = centre.Id,
            VisitDate = visitDate,
            SessionId = morningBlock.Id,
            QueueNumber = 1,
            QueueOrder = 1,
            PatientMobile = "0710000001",
            Status = PatientQueueStatus.Waiting
        });
        context.PatientQueueTickets.Add(new PatientQueueTicket
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            PracticeCentreId = centre.Id,
            VisitDate = visitDate,
            SessionId = morningBlock.Id,
            QueueNumber = 2,
            QueueOrder = 2,
            PatientMobile = "0710000002",
            Status = PatientQueueStatus.Waiting
        });
        // 1 Evening ticket
        context.PatientQueueTickets.Add(new PatientQueueTicket
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            PracticeCentreId = centre.Id,
            VisitDate = visitDate,
            SessionId = eveningBlock.Id,
            QueueNumber = 1,
            QueueOrder = 3,
            PatientMobile = "0710000003",
            Status = PatientQueueStatus.Waiting
        });
        await context.SaveChangesAsync();

        var handler = new BookAppointmentCommandHandler(context);
        var dateOnly = new DateOnly(2026, 8, 31);

        // Attempting to book 3rd Morning ticket should fail (limit 2 reached for Morning)
        var morningResult = await handler.Handle(new BookAppointmentCommand("0710000004", doctorId, centre.Id, dateOnly, null, morningBlock.Id), CancellationToken.None);
        Assert.False(morningResult.IsSuccess);
        Assert.Equal("Appointment.NoAvailability", morningResult.Error.Code);

        // Attempting to book 2nd Evening ticket should succeed (limit 2 not reached for Evening)
        var eveningResult = await handler.Handle(new BookAppointmentCommand("0710000005", doctorId, centre.Id, dateOnly, null, eveningBlock.Id), CancellationToken.None);
        Assert.True(eveningResult.IsSuccess);
        Assert.Equal(2, eveningResult.Value.QueueNumber);
    }
}

using Domain.Patients;

namespace Domain.PatientQueue;

public class PatientQueueTicket
{
    public Guid Id { get; set; }
    public int QueueNumber { get; set; }
    public int QueueOrder { get; set; }
    public string PatientMobile { get; set; } = null!; // Contact mobile number
    public Guid? PatientId { get; set; } // Specific patient ID (Parent or Child)
    public PatientAccount? Patient { get; set; }
    public Guid DoctorId { get; set; }
    public Guid PracticeCentreId { get; set; }
    public DateTime VisitDate { get; set; }
    public PatientQueueStatus Status { get; set; }
    public PatientQueuePriority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CalledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

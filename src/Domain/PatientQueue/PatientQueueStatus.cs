namespace Domain.PatientQueue;

public enum PatientQueueStatus
{
    Waiting,
    Ready,
    Called,
    InConsultation,
    Completed,
    Cancelled,
    NoShow
}

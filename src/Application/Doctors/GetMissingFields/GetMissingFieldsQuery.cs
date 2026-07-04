using Application.Abstractions.Messaging;

namespace Application.Doctors.GetMissingFields;

public sealed record MissingFieldsResponse(List<string> MissingFields, string CompletionStatus);

public sealed record GetMissingFieldsQuery : IQuery<MissingFieldsResponse>;

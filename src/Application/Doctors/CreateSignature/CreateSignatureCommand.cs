using Application.Abstractions.Messaging;

namespace Application.Doctors.CreateSignature;

public sealed record CreateSignatureCommand(string SignatureDataUrl, string IpAddress)
    : ICommand;

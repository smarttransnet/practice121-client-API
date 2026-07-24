using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Patients.GetByMobile; // reuse PatientResponse
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Patients.Search;

public sealed record SearchPatientsQuery(
    string? FirstName,
    string? LastName,
    string? NicNumber) : IQuery<List<PatientResponse>>;

internal sealed class SearchPatientsQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<SearchPatientsQuery, List<PatientResponse>>
{
    public async Task<Result<List<PatientResponse>>> Handle(SearchPatientsQuery request, CancellationToken cancellationToken)
    {
        // If all parameters are null or whitespace, return empty list
        if (string.IsNullOrWhiteSpace(request.FirstName) &&
            string.IsNullOrWhiteSpace(request.LastName) &&
            string.IsNullOrWhiteSpace(request.NicNumber))
        {
            return Result.Success(new List<PatientResponse>());
        }

        var query = dbContext.PatientAccounts.AsNoTracking();

#pragma warning disable CA1862, CA1304, CA1311, CA1307
        var firstName = request.FirstName?.Trim().ToLower();
        var lastName = request.LastName?.Trim().ToLower();
        var nicNumber = request.NicNumber?.Trim().ToLower();

        query = query.Where(p => 
            !string.IsNullOrEmpty(firstName) && p.FirstName.ToLower().Contains(firstName) ||
            !string.IsNullOrEmpty(lastName) && p.LastName != null && p.LastName.ToLower().Contains(lastName) ||
            !string.IsNullOrEmpty(nicNumber) && p.NicNumber != null && p.NicNumber.ToLower().Contains(nicNumber)
        );
#pragma warning restore CA1862, CA1304, CA1311, CA1307

        var patients = await query
            .Take(50) // limit results for safety
            .ToListAsync(cancellationToken);

        var response = patients.Select(p => new PatientResponse(
            p.Id,
            p.NicNumber,
            p.FirstName,
            p.LastName,
            p.DateOfBirth,
            p.Gender,
            p.MobileNumber
        )).ToList();

        return Result.Success(response);
    }
}

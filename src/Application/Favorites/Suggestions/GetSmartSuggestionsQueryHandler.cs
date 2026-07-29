using System.Globalization;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Favorites.Suggestions;

internal sealed class GetSmartSuggestionsQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext)
    : IQueryHandler<GetSmartSuggestionsQuery, List<FavoriteSuggestionResponse>>
{
    public async Task<Result<List<FavoriteSuggestionResponse>>> Handle(GetSmartSuggestionsQuery request, CancellationToken cancellationToken)
    {
        DoctorAccount? doctor = await context.DoctorAccounts.AsNoTracking()
            .SingleOrDefaultAsync(d => d.Id == userContext.UserId, cancellationToken);

        if (doctor is null)
        {
            return Result.Failure<List<FavoriteSuggestionResponse>>(Error.NotFound("DoctorAccount.NotFound", "Doctor account not found"));
        }

        DoctorProfile? profile = await context.DoctorProfiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.AccountId == doctor.Id, cancellationToken);

        string? doctorSpecialty = profile?.Specialty;

        var queryable = context.FavoriteMedicines.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(doctorSpecialty))
        {
            queryable = queryable.Where(f => f.DoctorSpecialty == doctorSpecialty);
        }

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            string q = request.Query.Trim().ToLower(CultureInfo.InvariantCulture);
#pragma warning disable CA1862, CA1304, CA1311, IDE0047
            queryable = queryable.Where(f => (f.GenericName != null && f.GenericName.ToLower().Contains(q)) || (f.BrandName != null && f.BrandName.ToLower().Contains(q)));
#pragma warning restore CA1862, CA1304, CA1311, IDE0047
        }

        var suggestions = await queryable
            .GroupBy(f => new
            {
                GenericName = f.GenericName == null ? "" : f.GenericName.Trim(),
                BrandName = f.BrandName == null ? "" : f.BrandName.Trim(),
                Category = f.Category == null ? "" : f.Category.Trim(),
                Dose = f.Dose == null ? "" : f.Dose.Trim(),
                Frequency = f.Frequency == null ? "" : f.Frequency.Trim(),
                Duration = f.Duration == null ? "" : f.Duration.Trim()
            })
            .Select(g => new FavoriteSuggestionResponse
            {
                GenericName = g.Key.GenericName,
                BrandName = string.IsNullOrEmpty(g.Key.BrandName) ? null : g.Key.BrandName,
                Category = g.Key.Category,
                Dose = string.IsNullOrEmpty(g.Key.Dose) ? null : g.Key.Dose,
                Frequency = string.IsNullOrEmpty(g.Key.Frequency) ? null : g.Key.Frequency,
                Duration = string.IsNullOrEmpty(g.Key.Duration) ? null : g.Key.Duration,
                DoctorSpecialty = doctorSpecialty,
                UsageCount = g.Count()
            })
            .OrderByDescending(s => s.UsageCount)
            .ThenBy(s => s.GenericName)
            .Take(20)
            .ToListAsync(cancellationToken);

        return suggestions;
    }
}

using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Api.DTOs;

public record TimeEntryDto(
    int Id,
    int ProjectId,
    string ProjectName,
    DateOnly Date,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    bool DeductLunchBreak,
    decimal? Hours,
    decimal? HourlyRateOverride,
    decimal EffectiveHourlyRate,
    string? Description
);

public record CreateTimeEntryDto(
    [Required] int ProjectId,
    [Required] DateOnly Date,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    bool DeductLunchBreak,
    [Range(0.01, 24)] decimal? Hours,
    [Range(0, 100000)] decimal? HourlyRateOverride,
    string? Description
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var hasHours = Hours.HasValue;
        var hasStart = StartTime.HasValue;
        var hasEnd = EndTime.HasValue;

        if (!hasHours && !hasStart)
        {
            yield return new ValidationResult(
                "You must provide either Hours, or at least a StartTime (EndTime is optional for an in-progress entry).",
                [nameof(Hours), nameof(StartTime)]);
        }

        if (hasEnd && !hasStart)
        {
            yield return new ValidationResult("EndTime cannot be set without a StartTime.", [nameof(StartTime)]);
        }

        if (!hasHours && hasStart && hasEnd)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult("EndTime must be after StartTime.", [nameof(EndTime)]);
            }
            else
            {
                var duration = EndTime!.Value.ToTimeSpan() - StartTime!.Value.ToTimeSpan();
                if (DeductLunchBreak) duration -= TimeSpan.FromMinutes(30);

                if (duration <= TimeSpan.Zero)
                {
                    yield return new ValidationResult(
                        "After deducting the lunch break, the resulting duration is zero or negative.",
                        [nameof(DeductLunchBreak)]);
                }
            }
        }
    }
}

public record UpdateTimeEntryDto(
    [Required] DateOnly Date,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    bool DeductLunchBreak,
    [Range(0.01, 24)] decimal? Hours,
    [Range(0, 100000)] decimal? HourlyRateOverride,
    string? Description
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var hasHours = Hours.HasValue;
        var hasStart = StartTime.HasValue;
        var hasEnd = EndTime.HasValue;

        if (!hasHours && !hasStart)
        {
            yield return new ValidationResult(
                "You must provide either Hours, or at least a StartTime (EndTime is optional for an in-progress entry).",
                [nameof(Hours), nameof(StartTime)]);
        }

        if (hasEnd && !hasStart)
        {
            yield return new ValidationResult("EndTime cannot be set without a StartTime.", [nameof(StartTime)]);
        }

        if (!hasHours && hasStart && hasEnd)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult("EndTime must be after StartTime.", [nameof(EndTime)]);
            }
            else
            {
                var duration = EndTime!.Value.ToTimeSpan() - StartTime!.Value.ToTimeSpan();
                if (DeductLunchBreak) duration -= TimeSpan.FromMinutes(30);

                if (duration <= TimeSpan.Zero)
                {
                    yield return new ValidationResult(
                        "After deducting the lunch break, the resulting duration is zero or negative.",
                        [nameof(DeductLunchBreak)]);
                }
            }
        }
    }
}
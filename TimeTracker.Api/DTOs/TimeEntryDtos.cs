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
    decimal Hours,
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
        var hasStartEnd = StartTime.HasValue && EndTime.HasValue;

        if (!hasHours && !hasStartEnd)
        {
            yield return new ValidationResult(
                "You must provide either Hours, or both StartTime and EndTime.",
                [nameof(Hours), nameof(StartTime), nameof(EndTime)]);
        }

        if (!hasHours && hasStartEnd)
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
        var hasStartEnd = StartTime.HasValue && EndTime.HasValue;

        if (!hasHours && !hasStartEnd)
        {
            yield return new ValidationResult(
                "You must provide either Hours, or both StartTime and EndTime.",
                [nameof(Hours), nameof(StartTime), nameof(EndTime)]);
        }

        if (!hasHours && hasStartEnd && EndTime <= StartTime)
        {
            yield return new ValidationResult("EndTime must be after StartTime.", [nameof(EndTime)]);
        }
    }
}
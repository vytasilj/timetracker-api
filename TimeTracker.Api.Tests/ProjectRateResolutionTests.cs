using TimeTracker.Api.Models;

namespace TimeTracker.Api.Tests;

public class ProjectRateResolutionTests
{
    [Fact]
    public void GetRateForDate_WithNoRates_ReturnsNull()
    {
        var project = new Project { Name = "Test" };

        var result = project.GetRateForDate(new DateOnly(2026, 7, 1));

        Assert.Null(result);
    }

    [Fact]
    public void GetRateForDate_WithSingleRate_AfterEffectiveDate_ReturnsThatRate()
    {
        var project = new Project
        {
            Name = "Test",
            Rates = [new ProjectRate { HourlyRate = 800, EffectiveFrom = new DateOnly(2026, 1, 1) }]
        };

        var result = project.GetRateForDate(new DateOnly(2026, 6, 1));

        Assert.Equal(800, result);
    }

    [Fact]
    public void GetRateForDate_WithSingleRate_BeforeEffectiveDate_FallsBackToOldestKnownRate()
    {
        // An entry predating every recorded rate should still resolve to something
        // sensible (the oldest known rate) rather than null/zero.
        var project = new Project
        {
            Name = "Test",
            Rates = [new ProjectRate { HourlyRate = 800, EffectiveFrom = new DateOnly(2026, 6, 1) }]
        };

        var result = project.GetRateForDate(new DateOnly(2026, 1, 1));

        Assert.Equal(800, result);
    }

    [Fact]
    public void GetRateForDate_WithMultipleRates_PicksTheLatestApplicableOne()
    {
        var project = new Project
        {
            Name = "Test",
            Rates =
            [
                new ProjectRate { HourlyRate = 700, EffectiveFrom = new DateOnly(2026, 1, 1) },
                new ProjectRate { HourlyRate = 800, EffectiveFrom = new DateOnly(2026, 6, 1) },
                new ProjectRate { HourlyRate = 900, EffectiveFrom = new DateOnly(2026, 9, 1) },
            ]
        };

        Assert.Equal(700, project.GetRateForDate(new DateOnly(2026, 3, 1)));
        Assert.Equal(800, project.GetRateForDate(new DateOnly(2026, 7, 1)));
        Assert.Equal(900, project.GetRateForDate(new DateOnly(2026, 12, 1)));
    }

    [Fact]
    public void GetRateForDate_OnExactEffectiveDate_IncludesThatRate()
    {
        var project = new Project
        {
            Name = "Test",
            Rates =
            [
                new ProjectRate { HourlyRate = 700, EffectiveFrom = new DateOnly(2026, 1, 1) },
                new ProjectRate { HourlyRate = 900, EffectiveFrom = new DateOnly(2026, 9, 1) },
            ]
        };

        var result = project.GetRateForDate(new DateOnly(2026, 9, 1));

        Assert.Equal(900, result);
    }
}
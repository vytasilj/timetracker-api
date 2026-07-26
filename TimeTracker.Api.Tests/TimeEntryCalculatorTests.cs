using TimeTracker.Api.Services;

namespace TimeTracker.Api.Tests;

public class TimeEntryCalculatorTests
{
    [Fact]
    public void CalculateHours_WithExplicitHours_ReturnsThatValue()
    {
        var result = TimeEntryCalculator.CalculateHours(
            hoursOverride: 4.5m,
            startTime: null,
            endTime: null,
            deductLunchBreak: false);

        Assert.Equal(4.5m, result);
    }

    [Fact]
    public void CalculateHours_WithExplicitHours_IgnoresStartEndAndLunchBreak()
    {
        // Even if StartTime/EndTime are also provided, Hours should take priority
        var result = TimeEntryCalculator.CalculateHours(
            hoursOverride: 3.0m,
            startTime: new TimeOnly(8, 0),
            endTime: new TimeOnly(16, 0),
            deductLunchBreak: true);

        Assert.Equal(3.0m, result);
    }

    [Fact]
    public void CalculateHours_WithStartEnd_NoLunchBreak_ReturnsCorrectDuration()
    {
        var result = TimeEntryCalculator.CalculateHours(
            hoursOverride: null,
            startTime: new TimeOnly(8, 0),
            endTime: new TimeOnly(16, 0),
            deductLunchBreak: false);

        Assert.Equal(8.0m, result);
    }

    [Fact]
    public void CalculateHours_WithStartEnd_DeductsLunchBreak()
    {
        var result = TimeEntryCalculator.CalculateHours(
            hoursOverride: null,
            startTime: new TimeOnly(8, 0),
            endTime: new TimeOnly(16, 0),
            deductLunchBreak: true);

        Assert.Equal(7.5m, result);
    }

    [Fact]
    public void CalculateHours_WithMultipleBlocksInOneDay_WorksIndependently()
    {
        // Simulates two separate TimeEntry records on the same day (e.g. 8-11 and 13-16)
        var morning = TimeEntryCalculator.CalculateHours(null, new TimeOnly(8, 0), new TimeOnly(11, 0), false);
        var afternoon = TimeEntryCalculator.CalculateHours(null, new TimeOnly(13, 0), new TimeOnly(16, 0), false);

        Assert.Equal(3.0m, morning);
        Assert.Equal(3.0m, afternoon);
        Assert.Equal(6.0m, morning + afternoon);
    }

    [Fact]
    public void CalculateHours_WithoutHoursOrStartEnd_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            TimeEntryCalculator.CalculateHours(
                hoursOverride: null,
                startTime: null,
                endTime: null,
                deductLunchBreak: false));
    }
}
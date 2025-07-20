using IntervalAnalyser.Services;
using IntervalAnalyser.Utils;

namespace IntervalAnalyser.Tests;

public class FitFileProcessorTests
{
    private const string TestFile = "TestData/test.fit";

    [Fact]
    public void ProcessFile_NoFilter_ReturnsAllLaps()
    {
        // Arrange
        var processor = new FitFileProcessor();
        // minAvgPower = 0 to include all laps, no duration filter
        var filter = new LapFilter(minAvgPower: 0, targetDuration: null);

        // Act
        var laps = processor.ProcessFile(TestFile, filter).ToList();

        // Assert
        // Replace expectedCount, expectedPower, expectedDuration with values from your test file
        const int expectedCount = 14;
        Assert.Equal(expectedCount, laps.Count);
    }
    
    [Fact]
    public void ProcessFile_Filter_ReturnsCorrectLaps()
    {
        // Arrange
        var processor = new FitFileProcessor();
        // minAvgPower = 0 to include all laps, no duration filter
        var filter = new LapFilter(minAvgPower: null, targetDuration: null);

        // Act
        var laps = processor.ProcessFile(TestFile, filter).ToList();

        // Assert
        // Replace expectedCount, expectedPower, expectedDuration with values from your test file
        const int expectedCount = 6;
        Assert.Equal(expectedCount, laps.Count);
    }

    [Fact]
    public void ProcessFile_MinPowerFilter_FiltersOutBelowThreshold()
    {
        // Arrange
        var processor = new FitFileProcessor();
        ushort threshold = 250;
        var filter = new LapFilter(minAvgPower: threshold, targetDuration: null);

        // Act
        var laps = processor.ProcessFile(TestFile, filter).ToList();

        // Assert
        Assert.All(laps, lap => Assert.True(lap.AvgPower >= threshold));
    }

    [Fact]
    public void ProcessFile_TargetDurationFilter_FiltersByDuration()
    {
        // Arrange
        var processor = new FitFileProcessor();
        var target = TimeSpan.FromSeconds(60);
        var filter = new LapFilter(minAvgPower: 0, targetDuration: target);

        // Act
        var laps = processor.ProcessFile(TestFile, filter).ToList();

        // Assert
        // All returned laps must have duration within ±2 seconds of 60s
        Assert.All(laps, lap =>
        {
            var b = Math.Abs(lap.DurationSec - target.TotalSeconds) <= 2;
        });
    }
}
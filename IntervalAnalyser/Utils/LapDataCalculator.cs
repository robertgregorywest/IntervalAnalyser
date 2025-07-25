using IntervalAnalyser.Models;

namespace IntervalAnalyser.Utils;

/// <summary>
/// Provides utility methods for performing calculations and formatting related to lap data.
/// </summary>
public static class LapDataCalculator
{
    /// <summary>
    /// Sums durations (in seconds) and returns a TimeSpan.
    /// </summary>
    private static TimeSpan CalculateTotalDuration(IEnumerable<LapData> laps)
    {
        var totalSec = laps.Sum(x => x.DurationSec);
        return TimeSpan.FromSeconds(totalSec);
    }
    
    /// <summary>
    /// Sums durations of all laps and returns as a string in "mm:ss" format.
    /// </summary>
    /// <param name="laps">A list of <see cref="LapData"/> objects representing laps.</param>
    /// <returns>A formatted string representing the total duration, or an empty string if no laps are provided.</returns>
    public static string CalculateTotalDuration(List<LapData> laps)
    {
        if (laps.Count == 0)
            return string.Empty;
        var duration = CalculateTotalDuration((IEnumerable<LapData>)laps);
        return duration.ToString(@"mm\:ss");
    }
    
    /// <summary>
    /// Computes normalized power to nearest Watt from a collection of LapData:
    ///   NP = ( sum(p_i^4 * d_i) / sum(d_i) )^(1/4)
    /// </summary>
    public static int CalculateNormalizedPower(IEnumerable<LapData> laps)
    {
        var totalSec = laps.Sum(x => x.DurationSec);
        var sum4 = laps.Sum(x => Math.Pow(x.AvgPower, 4) * x.DurationSec);
        return (int)Math.Round(Math.Pow(sum4 / totalSec, 0.25));
    }
}
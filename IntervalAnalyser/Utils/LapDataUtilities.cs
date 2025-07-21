using IntervalAnalyser.Models;

namespace IntervalAnalyser.Utils;

public static class LapDataUtilities
{
    /// <summary>
    /// Sums durations (in seconds) and returns a TimeSpan.
    /// </summary>
    private static TimeSpan CalculateTotalDuration(IEnumerable<LapData> laps)
    {
        var totalSec = laps.Sum(x => x.DurationSec);
        return TimeSpan.FromSeconds(totalSec);
    }
    
    public static string FormatTotalDuration(List<LapData> laps)
    {
        if (laps.Count == 0)
            return string.Empty;
        var duration = CalculateTotalDuration(laps);
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
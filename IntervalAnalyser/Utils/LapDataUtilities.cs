using IntervalAnalyser.Models;

namespace IntervalAnalyser.Utils;

public class LapDataUtilities
{
    /// <summary>
    /// Sums durations (in seconds) and returns a TimeSpan.
    /// </summary>
    public static TimeSpan CalculateTotalDuration(IEnumerable<LapData> laps)
    {
        var totalSec = laps.Sum(x => x.DurationSec);
        return TimeSpan.FromSeconds(totalSec);
    }
    
    /// <summary>
    /// Computes normalized power from a collection of LapData:
    ///   NP = ( sum(p_i^4 * d_i) / sum(d_i) )^(1/4)
    /// </summary>
    public static double CalculateNormalizedPower(IEnumerable<LapData> laps)
    {
        double totalSec = laps.Sum(x => x.DurationSec);
        double sum4 = laps.Sum(x => Math.Pow(x.AvgPower, 4) * x.DurationSec);
        return Math.Pow(sum4 / totalSec, 0.25);
    }

}
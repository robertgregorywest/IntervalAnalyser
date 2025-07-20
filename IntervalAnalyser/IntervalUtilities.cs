namespace IntervalAnalyser;

public class IntervalUtilities
{
    /// <summary>
    /// Sums durations (in seconds) and returns a TimeSpan.
    /// </summary>
    public static TimeSpan CalculateTotalDuration(IEnumerable<(double power, double durSec)> laps)
    {
        var totalSec = laps.Sum(x => x.durSec);
        return TimeSpan.FromSeconds(totalSec);
    }

    /// <summary>
    /// Computes normalized power:
    ///   NP = ( sum(p_i^4 * d_i) / sum(d_i) )^(1/4)
    /// </summary>
    public static double CalculateNormalizedPower(IEnumerable<(double power, double durSec)> laps)
    {
        double totalSec = laps.Sum(x => x.durSec);
        double sum4 = laps.Sum(x => Math.Pow(x.power, 4) * x.durSec);
        return Math.Pow(sum4 / totalSec, 0.25);
    }
}
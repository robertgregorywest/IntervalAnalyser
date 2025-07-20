using Dynastream.Fit;

namespace IntervalAnalyser;

public class LapFilter(ushort? minAvgPower = null, TimeSpan? targetDuration = null)
{
    private ushort? MinAvgPower { get; } = minAvgPower;
    private TimeSpan? TargetDuration { get; } = targetDuration;

    private const ushort DefaultPowerThreshold = 250;
    private static readonly TimeSpan DurationTolerance = TimeSpan.FromSeconds(2);

    public bool IsMatch(LapMesg lap)
    {
        // Check average power
        var p = lap.GetAvgPower();
        if (!p.HasValue)
            return false;

        if (MinAvgPower.HasValue)
        {
            // explicit minimum: require ≥ MinAvgPower
            if (p.Value < MinAvgPower.Value)
                return false;
        }
        else
        {
            // no MinAvgPower given: require > 250 W
            if (p.Value <= DefaultPowerThreshold)
                return false;
        }

        // Check duration
        if (!TargetDuration.HasValue) return true;
        
        var t = lap.GetTotalElapsedTime();
        if (!t.HasValue)
            return false;

        var lapDuration = TimeSpan.FromSeconds(t.Value);
        var delta = (lapDuration - TargetDuration.Value).Duration();
        return delta <= DurationTolerance;
    }
}
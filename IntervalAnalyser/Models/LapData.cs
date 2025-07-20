namespace IntervalAnalyser.Models;

public class LapData
{
    public string? FileName   { get; init; }
    public int    LapIndex   { get; init; }
    public double AvgPower   { get; init; }
    public double DurationSec{ get; init; }
}
namespace IntervalAnalyser.Models;

public class ArgumentParserResult
{
    public List<string> FilePaths { get; } = [];
    public ushort? MinPower { get; set; }
    public TimeSpan? TargetDuration { get; set; }
    public bool ShowHelp { get; set; }
    public List<string> Warnings { get; } = [];
}
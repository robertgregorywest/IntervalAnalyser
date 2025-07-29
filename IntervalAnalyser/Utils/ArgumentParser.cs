using IntervalAnalyser.Models;

namespace IntervalAnalyser.Utils;

public static class ArgumentParser
{
    public static ArgumentParserResult Parse(string[] args)
    {
        var result = new ArgumentParserResult();

        if (args.Length == 0)
        {
            result.ShowHelp = true;
            return result;
        }
        
        for (var i = 0; i < args.Length; i++)
        {
            var a = args[i];
            if (a is "--help" or "-h")
            {
                result.ShowHelp = true;
            }
            else if (a.StartsWith("--minPower", StringComparison.OrdinalIgnoreCase))
            {
                var val = a.Contains('=') ? a.Split('=', 2)[1] : args[++i];
                if (ushort.TryParse(val, out var p)) result.MinPower = p;
            }
            else if (a.StartsWith("--targetDuration", StringComparison.OrdinalIgnoreCase))
            {
                var val = a.Contains('=') ? a.Split('=', 2)[1] : args[++i];
                if (TimeSpan.TryParse(val, out var d)) result.TargetDuration = d;
            }
            else if (!a.StartsWith("--"))
            {
                if (File.Exists(a))
                {
                    result.FilePaths.Add(a);
                }
                else if (Directory.Exists(a))
                {
                    var fitFiles = Directory.GetFiles(a, "*.fit", SearchOption.TopDirectoryOnly);
                    result.FilePaths.AddRange(fitFiles);
                }
                else
                {
                    result.Warnings.Add($"Argument not a valid path or file: {a}");
                }
            }
        }

        return result;
    }
}
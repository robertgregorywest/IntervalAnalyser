using System.Text;
using ConsoleTables;
using Dynastream.Fit;
using IntervalAnalyser;
using File = System.IO.File;

// if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
// {
//     Console.WriteLine("Usage: FitLapViewer <file1.fit> [file2.fit ...] [--minPower <watts>] [--targetDuration <hh:mm:ss>]");
//     return;
// }

string? fitFilePath = null;
ushort? minPower = null;
TimeSpan? targetDuration = null;

var filePaths = new List<string>();

for (int i = 0; i < args.Length; i++)
{
    var a = args[i];
    if (a.StartsWith("--minPower", StringComparison.OrdinalIgnoreCase))
    {
        var val = a.Contains('=') ? a.Split('=', 2)[1] : args[++i];
        if (ushort.TryParse(val, out var p)) minPower = p;
    }
    else if (a.StartsWith("--targetDuration", StringComparison.OrdinalIgnoreCase))
    {
        var val = a.Contains('=') ? a.Split('=', 2)[1] : args[++i];
        if (TimeSpan.TryParse(val, out var d)) targetDuration = d;
    }
    else if (!a.StartsWith("--"))
    {
        filePaths.Add(a);
    }
}

if (filePaths.Count == 0)
{
    // Console.WriteLine("Error: No FIT files specified.");
    // return;
    filePaths.Add("../../../../test.fit");
}

// --- Process each file ---
var results = new Dictionary<string, List<double>>();
var filter  = new LapFilter(minPower, targetDuration);

foreach (var path in filePaths)
{
    if (!File.Exists(path))
    {
        Console.WriteLine($"File not found: {path}");
        return;
    }
    using var fitSource = new FileStream(path, FileMode.Open);
    var decoder = new Decode();
    var fitListener = new FitListener();
    decoder.MesgEvent += fitListener.OnMesg;
    
    decoder.Read(fitSource);
    
    var fitMessages = fitListener.FitMessages;
    
    var filtered = fitMessages.LapMesgs.Where(lap => filter.IsMatch(lap)).ToList();
    
    var powers = fitMessages.LapMesgs
        .Where(l => filter.IsMatch(l))
        .Select(l => (double)l.GetAvgPower().Value)
        .ToList();

    results[Path.GetFileName(path)] = powers;
}

// --- Build table ---
var allFiles = results.Keys.ToList();
int maxRows = results.Values.Max(list => list.Count);

var table = new ConsoleTable();
table.AddColumn(new[] { "Interval" }.Concat(allFiles).ToArray());

for (int row = 0; row < maxRows; row++)
{
    var rowVals = new object[1 + allFiles.Count];
    rowVals[0] = (row + 1).ToString();
    for (int c = 0; c < allFiles.Count; c++)
    {
        var vals = results[allFiles[c]];
        rowVals[c + 1] = row < vals.Count
            ? $"{vals[row]:F0} W"
            : "";
    }
    table.AddRow(rowVals);
}

// --- Add average row ---
var avgRow = new object[1 + allFiles.Count];
avgRow[0] = "Avg";
for (int c = 0; c < allFiles.Count; c++)
{
    var vals = results[allFiles[c]];
    avgRow[c + 1] = vals.Count > 0
        ? $"{vals.Average():F0} W"
        : "";
}
table.AddRow(avgRow);

table.Write(Format.Alternative);
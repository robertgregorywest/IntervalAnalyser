using ConsoleTables;
using IntervalAnalyser.Models;
using IntervalAnalyser.Services;
using IntervalAnalyser.Utils;

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    Console.WriteLine("Usage: FitLapViewer <file1.fit> [file2.fit ...] [--minPower <watts>] [--targetDuration <hh:mm:ss>]");
    return;
}

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
    Console.WriteLine("Error: No FIT files specified.");
    return;
}

var filter  = new LapFilter(minPower, targetDuration);
IFitFileProcessor processor = new FitFileProcessor();

// process each file
var allData = new List<LapData>();
foreach (var f in filePaths)
    allData.AddRange(processor.ProcessFile(f, filter));

// 1) Group by file and sort
var grouped = allData
    .GroupBy(ld => ld.FileName)
    .ToDictionary(
        g => g.Key,
        g => g.OrderBy(ld => ld.LapIndex).ToList()
    );

// 2) Determine how many rows we need
int maxLap = grouped.Values.Max(list => list.Count);

// 3) Create table with header: "Lap", then each file name
var table = new ConsoleTable();
table.AddColumn(new[] { "Interval" }.Concat(grouped.Keys).ToArray());

int col;

// 4) Populate per-lap rows
for (int lap = 1; lap <= maxLap; lap++)
{
    var row = new object[1 + grouped.Count];
    row[0] = lap.ToString();
    col = 1;
    foreach (var file in grouped.Keys)
    {
        var entry = grouped[file].FirstOrDefault(ld => ld.LapIndex == lap);
        row[col++] = entry != null
            ? $"{entry.AvgPower:F0} W"
            : "";
    }
    table.AddRow(row);
}

// 5) Summary row: Average Power
var avgRow = new object[1 + grouped.Count];
avgRow[0] = "Avg";
col = 1;
foreach (var list in grouped.Values)
{
    avgRow[col++] = list.Any()
        ? $"{list.Average(ld => ld.AvgPower):F0} W"
        : "";
}
table.AddRow(avgRow);

// 6) Summary row: Total Duration
var durRow = new object[1 + grouped.Count];
durRow[0] = "Total Dur";
col = 1;
foreach (var list in grouped.Values)
{
    if (list.Any())
    {
        durRow[col++] = LapDataUtilities.CalculateTotalDuration(list);
    }
    else durRow[col++] = "";
}
table.AddRow(durRow);

// 7) Summary row: Normalized Power
var npRow = new object[1 + grouped.Count];
npRow[0] = "Norm Pwr";
col = 1;
foreach (var list in grouped.Values)
{
    if (list.Any())
    {
        var np = LapDataUtilities.CalculateNormalizedPower(list);
        npRow[col++] = $"{(int)Math.Round(np)} W";
    }
    else npRow[col++] = "";
}
table.AddRow(npRow);

// 8) Render to console
table.Write(Format.Alternative);
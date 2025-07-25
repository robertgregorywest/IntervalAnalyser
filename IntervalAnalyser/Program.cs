using Spectre.Console;
using IntervalAnalyser.Models;
using IntervalAnalyser.Services;
using IntervalAnalyser.Utils;

try
{
    var parseResult = ArgumentParser.Parse(args);

    foreach (var warning in parseResult.Warnings)
        AnsiConsole.MarkupLine($"[yellow]{warning}[/]");

    if (args.Length == 0 || parseResult.ShowHelp)
    {
        AnsiConsole.MarkupLine("[bold]Usage:[/] IntervalAnalyser <file1.fit> [file2.fit ...] [--minPower <watts>] [--targetDuration <hh:mm:ss>]");
        Environment.Exit(0);
    }

    var filePaths = new HashSet<string>(parseResult.FilePaths, StringComparer.OrdinalIgnoreCase);
    
    if (filePaths.Count == 0)
    {
        AnsiConsole.MarkupLine("[red]Error: No FIT files specified.[/]");
        Environment.Exit(0);
    }

    var minPower = parseResult.MinPower;
    var targetDuration = parseResult.TargetDuration;

    var filter = new LapFilter(minPower, targetDuration);
    var processor = new FitFileProcessor();

    // Process each file
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
    var maxLap = grouped.Values.Max(list => list.Count);

    // 3) Create table with header: "Lap", then each file name
    var table = new Table();
    table.Border(TableBorder.Rounded);
    table.AddColumn("Interval");
    foreach (var file in grouped.Keys)
        table.AddColumn(file);

    // 4) Populate per-lap rows
    for (var lap = 1; lap <= maxLap; lap++)
    {
        var row = new List<string> { lap.ToString() };
        foreach (var file in grouped.Keys)
        {
            var entry = grouped[file].FirstOrDefault(ld => ld.LapIndex == lap);
            row.Add(entry != null ? $"{entry.AvgPower:F0} W" : "");
        }
        table.AddRow(row.ToArray());
    }
    
    table.AddEmptyRow();

    // 5) Summary row: Average Power
    var avgRow = new List<string>();
    foreach (var list in grouped.Values)
        avgRow.Add(list.Count != 0 ? $"{list.Average(ld => ld.AvgPower):F0} W" : "");
    table.AddSummaryRow("Avg", avgRow.ToArray());

    // 6) Summary row: Total Duration
    var durRow = new List<string>();
    foreach (var list in grouped.Values)
        durRow.Add(LapDataCalculator.CalculateTotalDuration(list));
    table.AddSummaryRow("Total Dur", durRow.ToArray());
    
    // 7) Render to console
    AnsiConsole.Write(table);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Fatal error: {ex.Message}");
    Environment.Exit(1);
}
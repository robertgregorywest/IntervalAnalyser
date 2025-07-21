using Spectre.Console;
using Spectre.Console.Rendering;

namespace IntervalAnalyser.Utils;

public static class TableExtensions
{
    /// <summary>
    /// Adds a results row (e.g. total, average, etc.) to the bottom of a Spectre.Console Table.
    /// Automatically formats values with optional bold styling.
    /// </summary>
    /// <param name="table">The table to add the results row to.</param>
    /// <param name="label">The label for the summary row (e.g. "Average", "Total").</param>
    /// <param name="values">The values to display, one per column, excluding the label column.</param>
    public static void AddSummaryRow(this Table table, string label, params string[] values)
    {
        var row = new List<IRenderable>
        {
            new Markup($"[bold green]{label}[/]")
        };

        foreach (var value in values)
        {
            row.Add(new Markup($"[bold green]{value}[/]"));
        }

        // Add the formatted summary row
        table.AddRow(row);
    }
}
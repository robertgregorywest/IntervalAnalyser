# IntervalAnalyser

A .NET 9 console application for processing Garmin FIT files to extract and display lap data with average power and durations, supporting filtering, multi-file comparison, and summary statistics.

## Features

- **FIT File Parsing**: Uses the [Garmin.FIT.sdk](https://www.nuget.org/packages/Garmin.FIT.sdk) to decode `.fit` files and extract `LapMesg` records.
- **Filtering**: Filter laps by minimum average power (default > 250 W if not specified) and/or target duration (within ± 2 seconds).
- **Multi-File Support**: Pass multiple FIT files, each displayed as its own column in the output table.
- **ConsoleTable Output**: Neatly formats per-lap data and summary rows (average power, total duration, normalized power) using [ConsoleTables](https://www.nuget.org/packages/ConsoleTables).

## Requirements

- .NET 9 SDK

## Solution Structure

```
IntervalAnalyser/
├─ IntervalAnalyser.csproj # Main console app
├─ Program.cs              # CLI entry point (argument parsing, wiring)
├─ Models/
│   └─ LapData.cs          # DTO: FileName, LapIndex, AvgPower, DurationSec
├─ Utils/
│   └─ LapFilter.cs        # Lap filtering logic
├─ Services/
│   ├─ IFitFileProcessor.cs
│   └─ FitFileProcessor.cs # Decodes FIT, applies filter, returns LapData
└─ Tests/
   ├─ IntervalAnalyser.Tests.csproj
   ├─ TestData/
   │   └─ test.fit # Sample FIT file
   └─ FitFileProcessorTests.cs
   └─ LapFilterTests.cs
```

## Getting Started

1. **Clone the repository**

   ```bash
   git clone <repo-url>
   cd IntervalAnalyser
   ```

2. **Build the application**

   ```bash
   dotnet build
   ```

3. **Run the app**

   ```bash
   dotnet run --project IntervalAnalyser <file1.fit> [file2.fit ...] [--minPower <watts>] [--targetDuration <hh:mm:ss>]
   ```

   **Examples**:

   ```bash
   # Single file, default filter (>250 W)
   dotnet run --project IntervalAnalyser ride1.fit

   # Multiple files with explicit minPower and duration
   dotnet run --project IntervalAnalyser ride1.fit ride2.fit --minPower 200 --targetDuration 00:03:30
   ```

## Testing

1. **Navigate to test project**

   ```bash
   cd IntervalAnalyser.Tests
   ```

2. **Ensure test FIT file is copied**

   In `IntervalAnalyser.Tests.csproj`, verify:

   ```xml
   <ItemGroup>
     <None Include="TestData\test.fit">
       <CopyToOutputDirectory>Always</CopyToOutputDirectory>
     </None>
   </ItemGroup>
   ```

3. **Run tests**

   ```bash
   dotnet test
   ```

## License

MIT © Rob West

using Dynastream.Fit;
using IntervalAnalyser.Models;
using IntervalAnalyser.Utils;
using File = System.IO.File;

namespace IntervalAnalyser.Services;

public class FitFileProcessor : IFitFileProcessor
{
    public IEnumerable<LapData> ProcessFile(string path, LapFilter filter)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException(path);
        
        using var fitSource = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        var decoder = new Decode();

        // Check if file is a FIT file
        if (!decoder.IsFIT(fitSource))
            throw new InvalidDataException($"File '{path}' is not a valid FIT file.");
        fitSource.Position = 0;
        
        // Check file integrity (CRC)
        if (!decoder.CheckIntegrity(fitSource))
            throw new InvalidDataException($"File '{path}' failed FIT integrity check.");
        fitSource.Position = 0;
        
        var laps = new List<LapData>();
        var idx = 0;
        
        var fitListener = new FitListener();
        decoder.MesgEvent += fitListener.OnMesg;
    
        decoder.Read(fitSource);
    
        foreach (var lapMesg in fitListener.FitMessages.LapMesgs)
        {
            if (!filter.IsMatch(lapMesg)) continue;
            
            var avgPower = lapMesg.GetAvgPower();
            var duration = lapMesg.GetTotalTimerTime();
            if (avgPower == null || duration == null)
                continue;
            
            idx++;
            laps.Add(new LapData {
                FileName    = Path.GetFileName(path),
                LapIndex    = idx,
                AvgPower    = avgPower.Value,
                DurationSec = duration.Value
            });
        }

        return laps;
    }
}
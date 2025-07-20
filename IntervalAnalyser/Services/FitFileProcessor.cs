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

        var laps = new List<LapData>();
        int idx = 0;
        
        using var fitSource = new FileStream(path, FileMode.Open);
        var decoder = new Decode();
        var fitListener = new FitListener();
        decoder.MesgEvent += fitListener.OnMesg;
    
        decoder.Read(fitSource);
    
        foreach (var lapMesg in fitListener.FitMessages.LapMesgs)
        {
            if (!filter.IsMatch(lapMesg)) continue;
            idx++;
            laps.Add(new LapData {
                FileName    = Path.GetFileName(path),
                LapIndex    = idx,
                AvgPower    = lapMesg.GetAvgPower()!.Value,
                DurationSec = lapMesg.GetTotalElapsedTime()!.Value
            });
        }

        return laps;
    }
}
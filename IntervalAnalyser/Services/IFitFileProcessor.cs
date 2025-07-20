using IntervalAnalyser.Utils;

namespace IntervalAnalyser.Services;

public interface IFitFileProcessor
{
    /// <summary>
    /// Reads one FIT file, applies the given filter, 
    /// and returns a list of LapData for that file.
    /// </summary>
    IEnumerable<Models.LapData> ProcessFile(string path, LapFilter filter);
}
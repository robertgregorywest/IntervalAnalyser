using Dynastream.Fit;
using IntervalAnalyser.Utils;

namespace IntervalAnalyser.Tests;

public class LapFilterTests
{
   // helper to build a LapMesg with given avgPower (W) and elapsedTime (s)
        private LapMesg MakeLap(int avgPower, float elapsedSec)
        {
            // this calls the base ctor with MesgNum.Lap for you
            var lap = new LapMesg();

            // these setter methods were generated for you
            lap.SetAvgPower((ushort)avgPower);
            lap.SetTotalElapsedTime(elapsedSec);
            
            return lap;
        }

        [Theory]
        [InlineData(300, true)]   // above default threshold 250
        [InlineData(251, true)]   // just above
        [InlineData(250, false)]  // equal → fails default >250
        [InlineData(200, false)]  // below
        public void DefaultPowerThreshold_IsApplied(int power, bool shouldMatch)
        {
            var filter = new LapFilter(minAvgPower: null, targetDuration: null);
            var lap = MakeLap(power, 60f);
            Assert.Equal(shouldMatch, filter.IsMatch(lap));
        }

        [Theory]
        [InlineData(180, 200, false)]
        [InlineData(200, 200, true)]
        [InlineData(250, 200, true)]
        public void ExplicitMinPower_IsApplied(int power, ushort minPower, bool shouldMatch)
        {
            var filter = new LapFilter(minAvgPower: minPower, targetDuration: null);
            var lap = MakeLap(power, 60f);
            Assert.Equal(shouldMatch, filter.IsMatch(lap));
        }

        [Theory]
        // within ±2s
        [InlineData(60, 60, true)]
        [InlineData(58, 60, true)]
        [InlineData(62, 60, true)]
        // outside tolerance
        [InlineData(57, 60, false)]
        [InlineData(63, 60, false)]
        public void TargetDuration_ToleranceIsApplied(int elapsedSec, int targetSec, bool shouldMatch)
        {
            var filter = new LapFilter(minAvgPower: 0, targetDuration: TimeSpan.FromSeconds(targetSec));
            var lap = MakeLap(300, elapsedSec);
            Assert.Equal(shouldMatch, filter.IsMatch(lap));
        }

        [Fact]
        public void Combined_CriteriaMustBothPass()
        {
            // minPower = 200, targetDur = 30s±2s
            var filter = new LapFilter(minAvgPower: 200, targetDuration: TimeSpan.FromSeconds(30));
            var goodLap = MakeLap(210, 29f);
            var badPower = MakeLap(190, 29f);
            var badDuration = MakeLap(210, 27f);
            Assert.True(filter.IsMatch(goodLap));
            Assert.False(filter.IsMatch(badPower));
            Assert.False(filter.IsMatch(badDuration));
        }
}
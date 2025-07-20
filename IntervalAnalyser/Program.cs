using Dynastream.Fit;

var fitFilePath = args.Length != 1 ? "../../../../test.fit" : args[0];

FileStream? fitSource = null;
    try
    {
        fitSource = new FileStream(fitFilePath, FileMode.Open);

        var decoder = new Decode();
        var fitListener = new FitListener();
        decoder.MesgEvent += fitListener.OnMesg;

        decoder.Read(fitSource);

        var fitMessages = fitListener.FitMessages;

        for (var i = 0; i < fitMessages.LapMesgs.Count; i++)
        {
            var lap = fitMessages.LapMesgs[i];
            
            var avgPower = lap.GetAvgPower();
            
            var elapsed = lap.GetTotalElapsedTime();
            var duration = elapsed.HasValue
                ? TimeSpan.FromSeconds(elapsed.Value)
                : TimeSpan.Zero;

            Console.WriteLine(
                $@"  Lap {i + 1}: Avg Power = {(avgPower.HasValue ? avgPower.Value.ToString() : "N/A")} W, Duration = {duration:hh\:mm\:ss}"
            );
        }

        Console.Write("Press any key to continue...");
        Console.ReadKey();
    }
    catch (FitException ex)
    {
        Console.WriteLine("A FitException occurred when trying to decode the FIT file. Message: " + ex.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Exception occurred when trying to decode the FIT file. Message: " + ex.Message);
    }
    finally
    {
        fitSource?.Close();
    }
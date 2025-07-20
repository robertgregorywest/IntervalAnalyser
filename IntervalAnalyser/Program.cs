// See https://aka.ms/new-console-template for more information

using Dynastream.Fit;
using static IntervalAnalyser.Decoder;

var fileName = args.Length != 1 ? "../../../../test.fit" : args[0];

FileStream fitSource = null;
    try
    {
        fitSource = new FileStream(fileName, FileMode.Open);
        //Console.WriteLine("Opening {0}", fileName);

        var decodeDemo = new Decode();

        // Use a FitListener to capture all decoded messages in a FitMessages object
        var fitListener = new FitListener();
        decodeDemo.MesgEvent += fitListener.OnMesg;

        // Use a custom event handlers to process messages as they are being decoded, and to
        // capture message definitions and developer field definitions
        decodeDemo.MesgEvent += OnMesgCustom;
        decodeDemo.MesgDefinitionEvent += OnMesgDefinitionCustom;
        decodeDemo.DeveloperFieldDescriptionEvent += OnDeveloperFieldDescriptionCustom;

        // Use a MesgBroadcaster for easy integration with existing projects
        //MesgBroadcaster mesgBroadcaster = new MesgBroadcaster();
        //mesgBroadcaster.MesgEvent += OnMesgCustom;
        //mesgBroadcaster.MesgDefinitionEvent += OnMesgDefinitionCustom;
        //decodeDemo.MesgEvent += mesgBroadcaster.OnMesg;
        //decodeDemo.MesgDefinitionEvent += mesgBroadcaster.OnMesgDefinition;

        Console.WriteLine("Decoding...");
        decodeDemo.Read(fitSource);

        FitMessages fitMessages = fitListener.FitMessages;

        foreach (FileIdMesg mesg in fitMessages.FileIdMesgs)
        {
            PrintFileIdMesg(mesg);
        }
        foreach (UserProfileMesg mesg in fitMessages.UserProfileMesgs)
        {
            PrintUserProfileMesg(mesg);
        }
        foreach (DeviceInfoMesg mesg in fitMessages.DeviceInfoMesgs)
        {
            PrintDeviceInfoMesg(mesg);
        }
        foreach (MonitoringMesg mesg in fitMessages.MonitoringMesgs)
        {
            PrintMonitoringMesg(mesg);
        }
        foreach (RecordMesg mesg in fitMessages.RecordMesgs)
        {
            PrintRecordMesg(mesg);
        }
        Console.WriteLine("Decoded FIT file {0}", args[0]);

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
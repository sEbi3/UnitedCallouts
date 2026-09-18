namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of a Cyclist on the Highway", CalloutProbability.Medium)]
public class CyclistOnTheHighway : Callout
{
    private static readonly string[] PedList =
    {
        "A_F_Y_Hippie_01", "A_M_Y_Skater_01", "A_M_M_FatLatin_01", "A_M_M_EastSA_01", "A_M_Y_Latino_01",
        "G_M_Y_FamDNF_01", "G_M_Y_FamCA_01", "G_M_Y_BallaSout_01", "G_M_Y_BallaOrig_01", "G_M_Y_BallaEast_01",
        "G_M_Y_StrPunk_02", "S_M_Y_Dealer_01", "A_M_M_RurMeth_01", "A_M_Y_MethHead_01", "A_M_M_Skidrow_01",
        "S_M_Y_Dealer_01", "a_m_y_mexthug_01", "G_M_Y_MexGoon_03", "G_M_Y_MexGoon_02", "G_M_Y_MexGoon_01",
        "G_M_Y_SalvaGoon_01", "G_M_Y_SalvaGoon_02", "G_M_Y_SalvaGoon_03", "G_M_Y_Korean_01", "G_M_Y_Korean_02",
        "G_M_Y_StrPunk_01"
    };
    private static readonly string[] Bicycles = { "bmx", "Cruiser", "Fixter", "Scorcher", "tribike3", "tribike2", "tribike" };

    private Ped _subject;
    private Vehicle _bike;
    private Vector3 _spawnPoint;
    private Blip _blip;
    private LHandle _pursuit;
    private bool _isStolen;
    private bool _startedPursuit;
    private bool _alreadySubtitleIntrod;

    public override bool OnBeforeCalloutDisplayed()
    {
        List<Vector3> list = new List<Vector3>
        {
            new(1720.068f, 1535.201f, 84.72424f),
            new(2563.921f, 5393.056f, 44.55834f),
            new(-1826.79f, 4697.899f, 56.58701f),
            new(-1344.75f, -757.6135f, 11.10569f),
            new(1163.919f, 449.0514f, 82.59987f),

        };
        _spawnPoint = LocationChooser.ChooseNearestLocation(list);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 100f);
        switch (Rndm.Next(1, 3))
        {
            case 1:
                _isStolen = true;
                break;
            case 2:
                break;
        }

        CalloutMessage = "Reports of a Cyclist on the Highway";
        CalloutAdvisory = "Make contact with the cyclist and have him safely exit the highway.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS SUSPICIOUS_PERSON IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Reports of a Cyclist on the Highway callout accepted.");
        }
        else { Settings.DetailedLogging = false; }

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Reports of a Cyclist on the Highway",
            "~b~Dispatch: ~w~Respond to reports of a cyclist on the highway. Use caution.");

        _subject = new(PedList[Rndm.Next(PedList.Length)], _spawnPoint, 0f);
        _bike = new(Bicycles[Rndm.Next(Bicycles.Length)], _spawnPoint, 0f);
        _subject.WarpIntoVehicle(_bike, -1);

        _blip = _bike.AttachBlip();
        _blip.Name = "Cyclist";
        _blip.Color = Color.LightBlue;
        _blip.EnableRoute(Color.Yellow);

        _subject.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.FollowTraffic);
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_subject != null && _subject.Exists()) _subject.Delete();
        if (_bike != null && _bike.Exists()) _bike.Delete();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (_subject != null && _subject.Exists() && _subject.DistanceTo(MainPlayer) < 20f)
        {
            if (_isStolen && !_startedPursuit)
            {
                _startedPursuit = true;
                GameFiber.StartNew(() =>
                {
                    if (_blip != null && _blip.Exists()) _blip.Delete();
                    _pursuit = Functions.CreatePursuit();
                    Functions.AddPedToPursuit(_pursuit, _subject);
                    Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                    if (_bike != null && _bike.Exists()) _bike.IsStolen = true;
                    if (Settings.DetailedLogging)
                    {
                        Game.LogTrivial("[UnitedCallouts LOG:] Pursuit created in Cyclist on the Highway callout.");
                    }
                    else { Settings.DetailedLogging = false; }
                }, "Reports of a Cyclist on the Highway [UnitedCallouts]");
            }

            if (_subject.DistanceTo(MainPlayer) < 30f && !_alreadySubtitleIntrod)
            {
                if (Settings.HelpMessages)
                {
                    Game.DisplayHelp("~b~Dispatch:~w~ Make contact with the cyclist and have him safely exit the roadway.", 5000);
                }
                else { Settings.HelpMessages = false; }
                _alreadySubtitleIntrod = true;
            }
        }
        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (_subject != null && _subject.IsDead) End();
        if (_subject != null && Functions.IsPedArrested(_subject)) End();
        base.Process();
    } 

    public override void End()
    {
        if (_subject != null && _subject.Exists()) _subject.Dismiss();
        if (_bike != null && _bike.Exists()) _bike.Dismiss();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Reports of a Cyclist on the Highway", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        if (Functions.IsPursuitStillRunning(_pursuit)) { Functions.ForceEndPursuit(_pursuit); }
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Reports of a Cyclist on the Highway callout ended.");
        }
        else { Settings.DetailedLogging = false; }
        base.End();
    }
}
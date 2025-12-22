namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Bicycle on the Freeway", CalloutProbability.Medium)]
public class BicycleOnTheFreeway : Callout
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

    private static readonly string[] Bicycles =
        { "bmx", "Cruiser", "Fixter", "Scorcher", "tribike3", "tribike2", "tribike" };

    // FIXED: Removed static from all instance fields
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

        CalloutMessage = "[UC]~w~ Reports of a Bicycle on the Freeway";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS SUSPICIOUS_PERSON IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Bicycle On the Freeway callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Bicycle on the Freeway",
            "~b~Dispatch:~w~ Someone called the police because there is someone with a ~g~bicycle~w~ on the ~o~freeway~w~. Respond with ~y~Code 2");

        _subject = new(PedList[Rndm.Next(PedList.Length)], _spawnPoint, 0f);
        _bike = new(Bicycles[Rndm.Next(Bicycles.Length)], _spawnPoint, 0f);
        _subject.WarpIntoVehicle(_bike, -1);

        _blip = _bike.AttachBlip();
        _blip.Color = Color.LightBlue;
        _blip.EnableRoute(Color.Yellow);

        _subject.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.FollowTraffic);
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        // FIXED: Added exists checks before deletion
        if (_subject != null && _subject.Exists()) _subject.Delete();
        if (_bike != null && _bike.Exists()) _bike.Delete();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        // FIXED: Added null and exists checks before distance calculation
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
                    Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
                        "~y~Dispatch Information",
                        "The ~g~bicycle~w~ from the suspect is a ~o~" + _bike.Model.Name +
                        "~w~. The ~g~bicycle~w~ was ~r~stolen~w~.");
                    GameFiber.Wait(2000);
                }, "Bicycle on the Freeway [UnitedCallouts]");
            }

            // FIXED: Added null checks
            if (_subject.DistanceTo(MainPlayer) < 25f && MainPlayer.IsOnFoot && !_alreadySubtitleIntrod &&
                _pursuit == null)
            {
                _alreadySubtitleIntrod = true;
                GameFiber.StartNew(() =>
                {
                    Game.DisplayNotification("Perform a normal traffic stop with the ~o~suspect~w~.");
                    Game.DisplayNotification("~b~Dispatch:~w~ Checking the serial number of the bike...");
                    GameFiber.Wait(2000);
                    if (_bike != null && _bike.Exists())
                    {
                        Game.DisplayNotification("~b~Dispatch~w~ We checked the serial number of the bike.<br>Model: ~o~" +
                                                 _bike.Model.Name + "<br>~w~Serial number: ~o~" + _bike.LicensePlate);
                    }
                    return;
                }, "Bicycle on the Freeway [UnitedCallouts]");
            }
        }

        // FIXED: Added null and exists checks
        if (_subject != null && _subject.Exists() && Functions.IsPedArrested(_subject) && _isStolen &&
            _subject.DistanceTo(MainPlayer) < 15f)
        {
            Game.DisplaySubtitle("~y~Suspect: ~w~Please let me go! I bring the bike back.", 4000);
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();

        // FIXED: Added null checks
        if (_subject != null && _subject.IsDead) End();
        if (_subject != null && Functions.IsPedArrested(_subject)) End();

        base.Process();
    }

    public override void End()
    {
        // FIXED: Added exists checks before cleanup
        if (_subject != null && _subject.Exists()) _subject.Dismiss();
        if (_bike != null && _bike.Exists()) _bike.Dismiss();
        if (_blip != null && _blip.Exists()) _blip.Delete();

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Bicycle on the Freeway", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}

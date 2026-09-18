using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of a Stolen Commercial Vehicle", CalloutProbability.Medium)]
class StolenCommercialVehicle : Callout
{
    private static readonly string[] CommercialVehicles =
    {
        "Biff", "Mixer", "Hauler", "Mule", "Flatbed", "Packer", "Pounder"
    };

    private Ped _Suspect;
    private Vehicle _SuspectCar;
    private Vector3 _spawnPoint;
    private Blip _blip;
    private LHandle _pursuit;
    private bool _pursuitCreated;
    private bool _hasBackupBeenCalled;

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 50f);
        CalloutMessage = "Reports of a Stolen Commercial Vehicle";
        CalloutAdvisory = "Reports received of a stolen commercial vehicle, possibly used for unauthorized transport.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "CRIME_OFFICER_IN_NEED_OF_ASSISTANCE_01 FOR CRIME_GRAND_THEFT_AUTO IN_OR_ON_POSITION UNITS_RESPOND_CODE_3", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Stolen Commercial Vehicle callout accepted.");
        }
        else { Settings.DetailedLogging = false; }

        _SuspectCar = new(CommercialVehicles[Rndm.Next(CommercialVehicles.Length)], _spawnPoint);
        _SuspectCar.IsStolen = true;
        _SuspectCar.IsPersistent = true;

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Stolen Commercial Vehicle", "~b~Dispatch: ~w~Respond to a reported stolen commercial vehicle. Vehicle description pending. Use caution and advise upon visual contact. Respond with ~r~Code 3~w~.");
        GameFiber.Wait(500);
        Functions.DisplayVehicleRecord(_SuspectCar, true);
        _Suspect = new Ped(_spawnPoint);
        _Suspect.WarpIntoVehicle(_SuspectCar, -1);
        _Suspect.BlockPermanentEvents = true;
        _Suspect.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.Emergency);

        _blip = _Suspect.AttachBlip();
        _blip.Name = "Stolen Commercial Vehicle";
        _blip.Sprite = BlipSprite.GangVehicle;
        _blip.Color = Color.LightBlue;
        _blip.EnableRoute(Color.Yellow);
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_Suspect) _Suspect.Delete();
        if (_SuspectCar) _SuspectCar.Delete();
        if (_blip) _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (!_pursuitCreated && _Suspect != null && _Suspect.Exists() && MainPlayer.DistanceTo(_Suspect.Position) < 50f)
        {
            if (Settings.DetailedLogging)
            {
                Game.LogTrivial("[UnitedCallouts LOG:] Stolen Commercial Vehicle: Pursuit has been started as the Player is now at the stolen vehicle.");
            }
            else { Settings.DetailedLogging = false; }
            _pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(_pursuit, _Suspect);
            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);

            if (!_hasBackupBeenCalled && Settings.ActivateAiBackup)
            {
                _hasBackupBeenCalled = true;
                Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit,
                    LSPD_First_Response.EBackupUnitType.LocalUnit);
                Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit,
                    LSPD_First_Response.EBackupUnitType.LocalUnit);
                Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit,
                    LSPD_First_Response.EBackupUnitType.AirUnit);
                if (Settings.DetailedLogging)
                {
                    Game.LogTrivial("[UnitedCallouts LOG:] Stolen Commercial Vehicle: AI Backup has been spawned.");
                }
                else { Settings.DetailedLogging = false; }
            }
            else
            {
                Settings.ActivateAiBackup = false;
            }
            _pursuitCreated = true;
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (_Suspect != null && _Suspect.IsDead) End();
        if (_Suspect != null && Functions.IsPedArrested(_Suspect)) End();
        base.Process();
    }

    public override void End()
    {
        if (_Suspect != null && _Suspect.Exists()) _Suspect.Dismiss();
        if (_SuspectCar != null && _SuspectCar.Exists()) _SuspectCar.Dismiss();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Stolen Commercial Vehicle", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        Functions.ForceEndPursuit(_pursuit);
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Stolen Commercial Vehicle callout ended.");
        }
        else { Settings.DetailedLogging = false; }
        base.End();
    }
}
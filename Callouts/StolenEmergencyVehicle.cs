using Rage;

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of a Stolen Emergency Vehicle", CalloutProbability.Medium)]
class StolenEmergencyVehicle : Callout
{
    private static readonly string[] EmergencyVehicles =
    {
        "POLICE", "POLICE2", "POLICE3", "POLICE4", "FBI", "FBI2", "POLICEB", "SHERIFF", "SHERIFF2", "pbus", "pranger",
        "policet", "AMBULANCE", "FIRETRUK", "lguard", "riot", "riot2"
    };

    private Vehicle _SuspectCar;
    private Ped _Suspect;
    private Vector3 _spawnPoint;
    private Blip _blip;
    private LHandle _pursuit;

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 40f);
        CalloutMessage = "Reports of a Stolen Emergency Vehicle";
        CalloutAdvisory = "Reports received of a stolen emergency vehicle, status and occupants currently unknown.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "CRIME_OFFICER_IN_NEED_OF_ASSISTANCE_01 FOR CRIME_STOLEN_POLICE_VEHICLE UNITS_RESPOND_CODE_3", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Stolen Emergency Vehicle callout accepted.");
        } else { Settings.DetailedLogging = false; }

        _SuspectCar = new(EmergencyVehicles[Rndm.Next(EmergencyVehicles.Length)], _spawnPoint);
        _SuspectCar.IsSirenOn = true;
        _SuspectCar.IsStolen = true;

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Stolen Emergency Vehicle", "~b~Dispatch: ~w~Respond to a reported stolen emergency vehicle. Vehicle status and suspect information unknown at this time. Respond with ~r~Code 3~w~.");
        _Suspect = new Ped(_spawnPoint);
        _Suspect.WarpIntoVehicle(_SuspectCar, -1);
        _Suspect.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
        _Suspect.BlockPermanentEvents = true;
        _blip = _Suspect.AttachBlip();
        _blip.EnableRoute(Color.Yellow);
        _blip.Sprite = BlipSprite.GangVehicle;
        _blip.Color = Color.LightBlue;
        _blip.Name = "Stolen Emergency Vehicle";
        _pursuit = Functions.CreatePursuit();
        Functions.AddPedToPursuit(_pursuit, _Suspect);
        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);

        if (Settings.ActivateAiBackup)
        {
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit,
                LSPD_First_Response.EBackupUnitType.LocalUnit);
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit,
                LSPD_First_Response.EBackupUnitType.LocalUnit);
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit,
                LSPD_First_Response.EBackupUnitType.AirUnit);
            if (Settings.DetailedLogging)
            {
                Game.LogTrivial("[UnitedCallouts LOG:] Stolen Emergency Vehicle Callout: AI Backup has been spawned.");
            }
            else { Settings.DetailedLogging = false; }
        } 
        else
        {
            Settings.ActivateAiBackup = false;
        }

        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_Suspect != null && _Suspect.Exists()) _Suspect.Dismiss();
        if (_SuspectCar != null && _SuspectCar.Exists()) _SuspectCar.Dismiss();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (_Suspect != null && _Suspect.Exists() && MainPlayer.DistanceTo(_Suspect.Position) < 30f)
        {
            if (_blip != null && _blip.Exists())
            {
                _blip.Delete();
            }
            GameFiber.Wait(200);
            if (!_blip.Exists())
            {
                _blip = _Suspect.AttachBlip();
            }
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
            "~y~Stolen Emergency Vehicle", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        Functions.ForceEndPursuit(_pursuit);
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Stolen Emergency Vehicle callout ended.");
        } else { Settings.DetailedLogging = false; }
        base.End();
    }
}
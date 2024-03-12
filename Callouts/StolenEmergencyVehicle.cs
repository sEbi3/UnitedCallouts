namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of a Stolen Emergency Vehicle (1)", CalloutProbability.Medium)]
class StolenEmergencyVehicle : Callout
{
    private static readonly string[] CopVehicles =
    {
        "POLICE", "POLICE2", "POLICE3", "POLICE4", "FBI", "FBI2", "POLICEB", "SHERIFF", "SHERIFF2", "pbus", "pranger",
        "policet"
    };

    private static Vehicle _policeCar;
    private static Ped _subject;
    private static Vector3 _spawnPoint;
    private static Blip _blip;
    private static LHandle _pursuit;

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 15f);
        CalloutMessage = "[UC]~w~ Reports of a Stolen Emergency Vehicle.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "CRIME_OFFICER_IN_NEED_OF_ASSISTANCE_01 FOR CRIME_STOLEN_POLICE_VEHICLE UNITS_RESPOND_CODE_3", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Stolen Emergency Vehicle callout accepted.");

        _policeCar = new(CopVehicles[Rndm.Next(CopVehicles.Length)], _spawnPoint)
        {
            IsSirenOn = true
        };

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Dispatch", "Loading ~g~Information~w~ of the ~y~LSPD Database~w~...");
        Functions.DisplayVehicleRecord(_policeCar, true);
        _subject = new Ped(_spawnPoint);
        _subject.WarpIntoVehicle(_policeCar, -1);
        _subject.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
        _subject.BlockPermanentEvents = true;

        _blip = _subject.AttachBlip();

        _pursuit = Functions.CreatePursuit();
        Functions.AddPedToPursuit(_pursuit, _subject);
        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);

        if (Settings.ActivateAiBackup)
        {
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit,
                LSPD_First_Response.EBackupUnitType.LocalUnit);
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit,
                LSPD_First_Response.EBackupUnitType.LocalUnit);
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit,
                LSPD_First_Response.EBackupUnitType.AirUnit);
        }
        else
        {
            Settings.ActivateAiBackup = false;
        }

        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_subject) _subject.Delete();
        if (_policeCar) _policeCar.Delete();
        if (_blip) _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (_subject && _subject.IsDead) End();
        if (_subject && Functions.IsPedArrested(_subject)) End();
        base.Process();
    }

    public override void End()
    {
        if (_blip) _blip.Delete();
        if (_policeCar) _policeCar.Dismiss();
        if (_subject) _subject.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Stolen Emergency Vehicle", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
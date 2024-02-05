namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Stolen Bus Incident", CalloutProbability.Medium)]
public class StolenBusIncident : Callout
{
    private static readonly string[] CivVehicles = { "bus", "coach", "airbus" };
    private static Vehicle _bus;
    private static Ped _suspect;
    private static Ped _v1;
    private static Ped _v2;
    private static Ped _v3;
    private static Vector3 _spawnPoint;
    private static Blip _blip;
    private static LHandle _pursuit;
    private static bool _pursuitCreated;

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 60f);
        CalloutMessage = "[UC]~w~ Reports of a Stolen Bus.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_GRAND_THEFT_AUTO IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Stolen Bus Incident callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Stolen Bus Incident",
            "~b~Dispatch: ~w~Try to arrest the driver to rescue the hostages. Respond with ~r~Code 3");

        _bus = new(CivVehicles[Rndm.Next(CivVehicles.Length)], _spawnPoint)
        {
            IsPersistent = true
        };

        _suspect = _bus.CreateRandomDriver();
        _suspect.IsPersistent = true;
        _suspect.BlockPermanentEvents = true;
        _suspect.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.Emergency);

        _blip = _suspect.AttachBlip();
        _blip.IsFriendly = false;

        _v1 = new Ped(_spawnPoint);
        _v2 = new Ped(_spawnPoint);
        _v3 = new Ped(_spawnPoint);
        _v1.WarpIntoVehicle(_bus, 4);
        _v2.WarpIntoVehicle(_bus, 2);
        _v3.WarpIntoVehicle(_bus, 3);

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

    public override void Process()
    {
        if (!_pursuitCreated && MainPlayer.DistanceTo(_suspect.Position) < 60f)
        {
            _pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(_pursuit, _suspect);
            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
            _pursuitCreated = true;
        }

        if (_pursuitCreated && !Functions.IsPursuitStillRunning(_pursuit))
        {
            End();
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (_suspect && _suspect.IsDead) End();
        if (_suspect && Functions.IsPedArrested(_suspect)) End();
        base.Process();
    }

    public override void End()
    {
        if (_suspect) _suspect.Dismiss();
        if (_bus) _bus.Dismiss();
        if (_blip) _blip.Delete();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Stolen Bus Incident", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
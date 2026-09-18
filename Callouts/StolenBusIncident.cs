using Rage;

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Stolen Bus Incident", CalloutProbability.Medium)]
public class StolenBusIncident : Callout
{
    private static readonly string[] CivVehicles = { "bus", "coach", "airbus" };
    private Vehicle _bus;
    private Ped _suspect;
    private Ped _v1;
    private Ped _v2;
    private Ped _v3;
    private Vector3 _spawnPoint;
    private Blip _blip;
    private LHandle _pursuit;
    private bool _pursuitCreated;

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 60f);
        CalloutMessage = "Reports of a Stolen Bus";
        CalloutAdvisory = "Reports received of a stolen public transit bus with uninvolved passengers still on board.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_GRAND_THEFT_AUTO IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Stolen Bus Incident callout accepted.");
        }
        else { Settings.DetailedLogging = false; }
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Stolen Bus Incident",
            "~b~Dispatch: ~w~Respond to a reported stolen bus. Callers indicate multiple uninvolved passengers on board. Respond with ~r~Code 3~w~.");

        _bus = new(CivVehicles[Rndm.Next(CivVehicles.Length)], _spawnPoint)
        {
            IsPersistent = true
        };

        _suspect = _bus.CreateRandomDriver();
        _suspect.IsPersistent = true;
        _suspect.BlockPermanentEvents = true;
        _suspect.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.Emergency);

        _blip = _suspect.AttachBlip();
        _blip.EnableRoute(Color.Yellow);
        _blip.Sprite = BlipSprite.GangVehicle;
        _blip.Color = Color.LightBlue;
        _blip.Name = "Stolen Bus";

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
            if (Settings.DetailedLogging)
            {
                Game.LogTrivial("[UnitedCallouts LOG:] Stolen Bus Incident Callout: AI Backup has been spawned.");
            }
            else { Settings.DetailedLogging = false; }
        }
        else
        {
            Settings.ActivateAiBackup = false;
        }

        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        if (!_pursuitCreated && _suspect != null && _suspect.Exists() && MainPlayer.DistanceTo(_suspect.Position) < 60f)
        {
            if (_blip != null && _blip.Exists())
            {
                _blip.Delete();
            }
            GameFiber.Wait(200);
            if (!_blip.Exists())
            {
                _blip = _suspect.AttachBlip();
            }

            _pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(_pursuit, _suspect);
            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
            _pursuitCreated = true;
            if (Settings.DetailedLogging)
            {
                Game.LogTrivial("[UnitedCallouts LOG:] Stolen Bus Incident Callout: Pursuit has started.");
            }
            else { Settings.DetailedLogging = false; }
        }
        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (_suspect != null && _suspect.IsDead) End();
        if (_suspect != null && Functions.IsPedArrested(_suspect)) End();
        base.Process();
    }

    public override void End()
    {
        if (_suspect != null && _suspect.Exists()) _suspect.Dismiss();
        if (_bus != null && _bus.Exists()) _bus.Dismiss();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Stolen Bus Incident", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        if (Functions.IsPursuitStillRunning(_pursuit)) { Functions.ForceEndPursuit(_pursuit); }
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Stolen Bus Incident callout ended.");
        }
        else { Settings.DetailedLogging = false; }
        base.End();
    }
}
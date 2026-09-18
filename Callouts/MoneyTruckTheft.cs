using Rage;

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Money Truck Theft", CalloutProbability.Medium)]
public class MoneyTruckTheft : Callout
{
    private static readonly string[] WepList =
{
        "WEAPON_SMG", "WEAPON_PUMPSHOTGUN", "weapon_microsmg", "weapon_machinepistol", "weapon_compactrifle", "WEAPON_CARBINERIFLE", "weapon_specialcarbine"
    };

    private Vehicle _stockade;
    private Ped Aggressor1 => _aggressors[0];
    private Ped Aggressor2 => _aggressors[1];
    private Ped Aggressor3 => _aggressors[2];
    private Ped Aggressor4 => _aggressors[3];
    private Vector3 _spawnPoint;
    private Vector3 _vehicleSpawnPoint;
    private Blip _blip1;
    private Blip _blip2;
    private Blip _blip3;
    private Blip _blip4;
    private Blip _blipTruck;
    private LHandle _pursuit;
    private bool _pursuitCreated;
    private Ped[] _aggressors = new Ped[4];
    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
        _vehicleSpawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
        CalloutMessage = "Reports of a Money Truck Theft";
        CalloutAdvisory = "Reports received of a stolen armored cash transport. Multiple armed suspects observed.";
        CalloutPosition = _vehicleSpawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_01 OFFICERS_REPORT_01 BANK_CAR UNITS_RESPOND_CODE_99_01",
            _vehicleSpawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Money Truck Theft callout accepted.");
        }
        else { Settings.DetailedLogging = false; }
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Money Truck Theft", "~b~Dispatch: ~w~Respond to a reported stolen armored cash transport. Multiple armed suspects observed. Exercise extreme caution, do not engage alone, and advise on suspect movements. Respond with ~r~Code 3");

        _stockade = new Vehicle("STOCKADE", _vehicleSpawnPoint);
        _blipTruck = _stockade.AttachBlip();
        _blipTruck.Sprite = BlipSprite.ArmoredVan;
        _blipTruck.Color = Color.LightBlue;
        _blipTruck.Name = "Stolen Money Truck";

        _pursuit = Functions.CreatePursuit();
        for (int i = 0; i < _aggressors.Length; i++)
        {
            _aggressors[i] = new("g_m_m_chemwork_01", _spawnPoint, 0f);
            _aggressors[i].Armor = 100;
            NativeFunction.Natives.TASK_COMBAT_PED(_aggressors[i], MainPlayer, 0, 1);
            _aggressors[i].WarpIntoVehicle(_stockade, (i == 0) ? -1 : -2);
            Functions.AddPedToPursuit(_pursuit, _aggressors[i]);
        }

        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
        _pursuitCreated = true;

        Aggressor1.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
        Aggressor2.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
        Aggressor3.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
        Aggressor4.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);

        _blip1 = Aggressor1.AttachBlip();
        _blip2 = Aggressor2.AttachBlip();
        _blip3 = Aggressor3.AttachBlip();
        _blip4 = Aggressor4.AttachBlip();

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
                Game.LogTrivial("[UnitedCallouts LOG:] Money Truck Theft: AI Backup has been spawned.");
            } else { Settings.DetailedLogging = false; }
        }
        else
        {
            Settings.ActivateAiBackup = false;
        }

        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        foreach (var ped in _aggressors)
        {
            if (ped != null && ped.Exists()) ped.Delete();
        }

        if (_stockade != null && _stockade.Exists()) _stockade.Delete();
        if (_blip1 != null && _blip1.Exists()) _blip1.Delete();
        if (_blip2 != null && _blip2.Exists()) _blip2.Delete();
        if (_blip3 != null && _blip3.Exists()) _blip3.Delete();
        if (_blip4 != null && _blip4.Exists()) _blip4.Delete();
        if (_blipTruck != null && _blipTruck.Exists()) _blipTruck.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (_stockade != null && _stockade.Exists() && MainPlayer.DistanceTo(_stockade) < 50f)
        {
            if (Aggressor1 != null && Aggressor1.Exists()) Aggressor1.Tasks.FightAgainst(MainPlayer);
            if (Aggressor2 != null && Aggressor2.Exists()) Aggressor2.Tasks.FightAgainst(MainPlayer);
            if (Aggressor3 != null && Aggressor3.Exists()) Aggressor3.Tasks.FightAgainst(MainPlayer);
            if (Aggressor4 != null && Aggressor4.Exists()) Aggressor4.Tasks.FightAgainst(MainPlayer);

            if (Settings.DetailedLogging)
            {
                Game.LogTrivial("[UnitedCallouts LOG:] Money Truck Theft: Aggressors start shooting against the Player.");
            }
            else { Settings.DetailedLogging = false; }
        }

        if (Aggressor1 != null && (Aggressor1.IsDead || Functions.IsPedArrested(Aggressor1)))
        {
            if (_blip1 != null && _blip1.Exists()) _blip1.Delete();
        }

        if (Aggressor2 != null && (Aggressor2.IsDead || Functions.IsPedArrested(Aggressor2)))
        {
            if (_blip2 != null && _blip2.Exists()) _blip2.Delete();
        }

        if (Aggressor3 != null && (Aggressor3.IsDead || Functions.IsPedArrested(Aggressor3)))
        {
            if (_blip3 != null && _blip3.Exists()) _blip3.Delete();
        }

        if (Aggressor4 != null && (Aggressor4.IsDead || Functions.IsPedArrested(Aggressor4)))
        {
            if (_blip4 != null && _blip4.Exists()) _blip4.Delete();
        }
        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (Aggressor1 != null && Aggressor1.Exists() && Aggressor1.IsDead &&
            Aggressor2 != null && Aggressor2.Exists() &&  Aggressor2.IsDead &&
            Aggressor3 != null && Aggressor3.Exists() &&  Aggressor3.IsDead &&
            Aggressor4 != null && Aggressor4.Exists() &&  Aggressor4.IsDead) End();

        if (Aggressor1 != null && Aggressor1.Exists() && Functions.IsPedArrested(Aggressor1) &&
            Aggressor2 != null && Aggressor2.Exists() && Functions.IsPedArrested(Aggressor2) &&
            Aggressor3 != null && Aggressor3.Exists() && Functions.IsPedArrested(Aggressor3) &&
            Aggressor4 != null && Aggressor4.Exists() && Functions.IsPedArrested(Aggressor4)) End();
        base.Process();
    }

    public override void End()
    {
        if (_blip1 != null && _blip1.Exists()) _blip1.Delete();
        if (_blip2 != null && _blip2.Exists()) _blip2.Delete();
        if (_blip3 != null && _blip3.Exists()) _blip3.Delete();
        if (_blip4 != null && _blip4.Exists()) _blip4.Delete();
        if (_blipTruck != null && _blipTruck.Exists()) _blipTruck.Delete();

        foreach (var ped in _aggressors)
        {
            if (ped != null && ped.Exists()) ped.Dismiss();
        }

        if (_stockade != null && _stockade.Exists()) _stockade.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Money Truck Theft", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        if (Functions.IsPursuitStillRunning(_pursuit)) { Functions.ForceEndPursuit(_pursuit); }
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Money Truck Theft callout ended.");
        }
        else { Settings.DetailedLogging = false; }
        base.End();
    }
}
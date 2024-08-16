namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Money Truck Theft", CalloutProbability.Medium)]
public class MoneyTruckTheft : Callout
{
    private static Vehicle _stockade;
    private static Ped Aggressor1 => _aggressors[0];
    private static Ped Aggressor2 => _aggressors[1];
    private static Ped Aggressor3 => _aggressors[2];
    private static Ped Aggressor4 => _aggressors[3];
    private static Vector3 _spawnPoint;
    private static Vector3 _vehicleSpawnPoint;
    private static Blip _blip1;
    private static Blip _blip2;
    private static Blip _blip3;
    private static Blip _blip4;
    private static LHandle _pursuit;
    private static bool _pursuitCreated;

    private static Ped[] _aggressors = new Ped[3];

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
        _vehicleSpawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
        CalloutMessage = "[UC]~w~ Reports of a Money Truck Theft.";
        CalloutPosition = _vehicleSpawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_01 OFFICERS_REPORT_01 BANK_CAR UNITS_RESPOND_CODE_99_01",
            _vehicleSpawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Money Truck Theft callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Money Truck Theft", "~b~Dispatch: ~w~Get the MoneyTruck back. Respond with ~r~Code3");

        _stockade = new Vehicle("STOCKADE", _vehicleSpawnPoint);

        _pursuit = Functions.CreatePursuit();
        for (int i = 0; i < _aggressors.Length; i++)
        {
            _aggressors[i] = new("g_m_m_chemwork_01", _spawnPoint, 0f);
            _aggressors[i].Armor = 100;
            NativeFunction.Natives.TASK_COMBAT_PED(_aggressors[i], MainPlayer, 0, 1);
            _aggressors[i].WarpIntoVehicle(_stockade, (i == 1) ? -1 : -2);
            Functions.AddPedToPursuit(_pursuit, _aggressors[i]);
        }

        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
        _pursuitCreated = true;

        Aggressor1.Inventory.GiveNewWeapon("weapon_gusenberg", 5000, true);
        Aggressor2.Inventory.GiveNewWeapon("weapon_assaultrifle", 5000, true);
        Aggressor3.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);
        Aggressor4.Inventory.GiveNewWeapon("weapon_specialcarbine", 5000, true);

        _blip1 = Aggressor1.AttachBlip();
        _blip2 = Aggressor2.AttachBlip();
        _blip3 = Aggressor3.AttachBlip();
        _blip4 = Aggressor4.AttachBlip();

        if (Settings.ActivateAiBackup)
        {
            Functions.RequestBackup(_vehicleSpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit,
                LSPD_First_Response.EBackupUnitType.LocalUnit);
            Functions.RequestBackup(_vehicleSpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit,
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
        foreach (var ped in _aggressors)
        {
            if (ped) ped.Delete();
        }

        if (_stockade) _stockade.Delete();
        if (_blip1) _blip1.Delete();
        if (_blip2) _blip2.Delete();
        if (_blip3) _blip3.Delete();
        if (_blip4) _blip4.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (MainPlayer.DistanceTo(_stockade) < 15f)
        {
            Aggressor1.Tasks.FightAgainst(MainPlayer);
            Aggressor2.Tasks.FightAgainst(MainPlayer);
            Aggressor3.Tasks.FightAgainst(MainPlayer);
            Aggressor4.Tasks.FightAgainst(MainPlayer);
        }

        if (Aggressor1.IsDead || Functions.IsPedArrested(Aggressor1))
        {
            if (_blip1) _blip1.Delete();
        }

        if (Aggressor2.IsDead || Functions.IsPedArrested(Aggressor2))
        {
            if (_blip2) _blip2.Delete();
        }

        if (Aggressor3.IsDead || Functions.IsPedArrested(Aggressor3))
        {
            if (_blip3) _blip3.Delete();
        }

        if (Aggressor4.IsDead || Functions.IsPedArrested(Aggressor4))
        {
            if (_blip4) _blip4.Delete();
        }

        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (Aggressor1 && Aggressor1.IsDead && Aggressor2 && Aggressor2.IsDead && Aggressor3 && Aggressor3.IsDead &&
            Aggressor4 && Aggressor4.IsDead) End();
        if (Aggressor1 && Functions.IsPedArrested(Aggressor1) && Aggressor2 && Functions.IsPedArrested(Aggressor2) &&
            Aggressor3 && Functions.IsPedArrested(Aggressor3) && Aggressor4 &&
            Functions.IsPedArrested(Aggressor4)) End();
        base.Process();
    }

    public override void End()
    {
        if (_blip1) _blip1.Delete();
        if (_blip2) _blip2.Delete();
        if (_blip3) _blip3.Delete();
        if (_blip4) _blip4.Delete();
        foreach (var ped in _aggressors)
        {
            if (ped) ped.Dismiss();
        }

        if (_stockade) _stockade.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Money Truck Theft", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}

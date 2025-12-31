namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Money Truck Theft", CalloutProbability.Medium)]
public class MoneyTruckTheft : Callout
{
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
    private LHandle _pursuit;
    private bool _pursuitCreated;

    // FIXED: Changed array size from 3 to 4 to prevent out-of-bounds crash
    // FIXED: Removed static modifier to prevent state conflicts between multiple callouts
    private Ped[] _aggressors = new Ped[4];

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
            _aggressors[i].WarpIntoVehicle(_stockade, (i == 0) ? -1 : -2);
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
        // FIXED: Added null and exists checks before deletion
        foreach (var ped in _aggressors)
        {
            if (ped != null && ped.Exists()) ped.Delete();
        }

        if (_stockade != null && _stockade.Exists()) _stockade.Delete();
        if (_blip1 != null && _blip1.Exists()) _blip1.Delete();
        if (_blip2 != null && _blip2.Exists()) _blip2.Delete();
        if (_blip3 != null && _blip3.Exists()) _blip3.Delete();
        if (_blip4 != null && _blip4.Exists()) _blip4.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        // FIXED: Added null and exists checks before distance calculation
        if (_stockade != null && _stockade.Exists() && MainPlayer.DistanceTo(_stockade) < 15f)
        {
            if (Aggressor1 != null && Aggressor1.Exists()) Aggressor1.Tasks.FightAgainst(MainPlayer);
            if (Aggressor2 != null && Aggressor2.Exists()) Aggressor2.Tasks.FightAgainst(MainPlayer);
            if (Aggressor3 != null && Aggressor3.Exists()) Aggressor3.Tasks.FightAgainst(MainPlayer);
            if (Aggressor4 != null && Aggressor4.Exists()) Aggressor4.Tasks.FightAgainst(MainPlayer);
        }

        // FIXED: Added null checks before checking status and deleting blips
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

        if (Game.IsKeyDown(Settings.EndCall)) End();

        // FIXED: Added null checks before checking IsDead/Arrested
        if (Aggressor1 != null && Aggressor1.IsDead &&
            Aggressor2 != null && Aggressor2.IsDead &&
            Aggressor3 != null && Aggressor3.IsDead &&
            Aggressor4 != null && Aggressor4.IsDead) End();

        if (Aggressor1 != null && Functions.IsPedArrested(Aggressor1) &&
            Aggressor2 != null && Functions.IsPedArrested(Aggressor2) &&
            Aggressor3 != null && Functions.IsPedArrested(Aggressor3) &&
            Aggressor4 != null && Functions.IsPedArrested(Aggressor4)) End();

        base.Process();
    }

    public override void End()
    {
        // FIXED: Added exists checks before cleanup
        if (_blip1 != null && _blip1.Exists()) _blip1.Delete();
        if (_blip2 != null && _blip2.Exists()) _blip2.Delete();
        if (_blip3 != null && _blip3.Exists()) _blip3.Delete();
        if (_blip4 != null && _blip4.Exists()) _blip4.Delete();

        foreach (var ped in _aggressors)
        {
            if (ped != null && ped.Exists()) ped.Dismiss();
        }

        if (_stockade != null && _stockade.Exists()) _stockade.Dismiss();

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Money Truck Theft", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}

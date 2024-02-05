using LSPD_First_Response.Engine;

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Robbery at Humane Labs and Research Facility", CalloutProbability.Medium)]
public class RobberyHl : Callout
{
    private Ped _a1;
    private Ped _a2;
    private Ped _a3;
    private Ped _a6;
    private Ped _a7;
    private Ped _a8;
    private Ped _a9;
    private Blip _b1;
    private Blip _b2;
    private Blip _b3;
    private Blip _b6;
    private Blip _b7;
    private Blip _b8;
    private Blip _b9;
    private Ped _swat1;
    private Ped _swat2;
    private Ped _swat3;
    private Vehicle _policeRiot;
    private Vector3 _policeRiotSpawn = new Vector3(3635.675f, 3774.846f, 28.51558f);
    private Vector3 _spawnPoint;
    private Vector3 _swat1Spawn = new Vector3(3631.826f, 3774.041f, 28.51571f);
    private Vector3 _swat2Spawn = new Vector3(3632.776f, 3772.018f, 28.51571f);
    private Vector3 _swat3Spawn = new Vector3(3633.925f, 3768.781f, 28.51571f);
    private Vector3 _joinSwatVector;
    private bool _joinSwat = false;
    private bool _noticed = false;
    private bool _pursuitCreated = false;
    private LHandle _pursuit;

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = new Vector3(3616.228f, 3737.619f, 28.6901f);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 70f);
        AddMinimumDistanceCheck(10f, _spawnPoint);
        CalloutMessage = "[UC]~w~ Robbery at Humane Labs and Research Facility.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_01 crime_robbery_with_a_firearm IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Robbery at Human Labs and Research", "~b~Dispatch: ~w~The wanted robbers are marked on your ~y~GPS~w~! Respond with ~r~Code 3");

        _a1 = new Ped("g_m_m_chemwork_01", _spawnPoint, 0f);
        _a2 = new Ped("g_m_m_chemwork_01", _spawnPoint, 0f);
        _a3 = new Ped("g_m_m_chemwork_01", _spawnPoint, 0f);
        _a6 = new Ped("g_m_m_chemwork_01", _spawnPoint, 0f);
        _a7 = new Ped("g_m_m_chemwork_01", _spawnPoint, 0f);
        _a8 = new Ped("u_m_y_juggernaut_01", _spawnPoint, 0f);
        _a9 = new Ped("g_m_m_chemwork_01", _spawnPoint, 0f);

        _a8.CanRagdoll = false;
        NativeFunction.Natives.SET_PED_SUFFERS_CRITICAL_HITS(_a8, false);
        NativeFunction.Natives.SetPedPathCanUseClimbovers(_a8, true);
        Functions.SetPedCantBeArrestedByPlayer(_a8, true);

        _policeRiot = new Vehicle("RIOT", _policeRiotSpawn, 0f);
        _policeRiot.Heading = -174f;
        _policeRiot.IsSirenOn = true;
        _policeRiot.IsSirenSilent = true;

        _swat1 = new Ped("s_m_y_swat_01", _swat1Spawn, 94f);
        _swat2 = new Ped("s_m_y_swat_01", _swat2Spawn, 94f);
        _swat3 = new Ped("s_m_y_swat_01", _swat3Spawn, 94f);

        _swat1.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 500, true);
        _swat2.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 500, true);
        _swat3.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 500, true);

        NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _swat1, _a1, -1, true);
        NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _swat2, _a1, -1, true);
        NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _swat3, _a1, -1, true);

        _a1.IsPersistent = true;
        _a2.IsPersistent = true;
        _a3.IsPersistent = true;
        _a6.IsPersistent = true;
        _a7.IsPersistent = true;
        _a8.IsPersistent = true;
        _a9.IsPersistent = true;

        _a1.BlockPermanentEvents = true;
        _a2.BlockPermanentEvents = true;
        _a3.BlockPermanentEvents = true;
        _a6.BlockPermanentEvents = true;
        _a7.BlockPermanentEvents = true;
        _a8.BlockPermanentEvents = true;
        _a9.BlockPermanentEvents = true;

        _a1.RelationshipGroup = RelationshipGroup.AggressiveInvestigate;
        _a2.RelationshipGroup = RelationshipGroup.AggressiveInvestigate;
        _a3.RelationshipGroup = RelationshipGroup.AggressiveInvestigate;
        _a6.RelationshipGroup = RelationshipGroup.AggressiveInvestigate;
        _a7.RelationshipGroup = RelationshipGroup.AggressiveInvestigate;
        _a8.RelationshipGroup = RelationshipGroup.AggressiveInvestigate;
        _a9.RelationshipGroup = RelationshipGroup.AggressiveInvestigate;

        _a1.Inventory.GiveNewWeapon("WEAPON_SMG", 5000, true);
        _a2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
        _a3.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);
        _a6.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);
        _a7.Inventory.GiveNewWeapon("WEAPON_SMG", 5000, true);
        _a8.Inventory.GiveNewWeapon("WEAPON_MINIGUN", 5000, true);
        _a9.Inventory.GiveNewWeapon("WEAPON_SMG", 5000, true);

        _a1.Armor = 400;
        _a2.Armor = 300;
        _a8.Armor = 1000;
        _swat1.Armor = 200;
        _swat2.Armor = 200;
        _swat3.Armor = 200;

        _b1 = _a1.AttachBlip();
        _b2 = _a2.AttachBlip();
        _b3 = _a3.AttachBlip();
        _b6 = _a6.AttachBlip();
        _b7 = _a7.AttachBlip();
        _b8 = _a8.AttachBlip();
        _b9 = _a9.AttachBlip();
        _b1.EnableRoute(Color.Yellow);

        if (Settings.ActivateAiBackup)
        {
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.SwatTeam);
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.SwatTeam);
        } else { Settings.ActivateAiBackup = false; }
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_b1) _b1.Delete();
        if (_b2) _b2.Delete();
        if (_b3) _b3.Delete();
        if (_b6) _b6.Delete();
        if (_b7) _b7.Delete();
        if (_b8) _b8.Delete();
        if (_b9) _b9.Delete();
        if (_a1) _a1.Delete();
        if (_a2) _a2.Delete();
        if (_a3) _a3.Delete();
        if (_a6) _a6.Delete();
        if (_a7) _a7.Delete();
        if (_a8) _a8.Delete();
        if (_a9) _a9.Delete();
        if (_policeRiot) _policeRiot.Delete();
        if (_swat1) _swat1.Delete();
        if (_swat2) _swat2.Delete();
        if (_swat3) _swat3.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        GameFiber.StartNew(delegate
        {
            if (MainPlayer.DistanceTo(_spawnPoint) < 22f)
            {
                NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _a1, MainPlayer, 0, 1);
                NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _a2, MainPlayer, 0, 1);
                NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _a3, MainPlayer, 0, 1);
                NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _a6, MainPlayer, 0, 1);
                NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _a7, MainPlayer, 0, 1);
                NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _a8, MainPlayer, 0, 1);
                NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _a9, MainPlayer, 0, 1);

                _a1.Tasks.FightAgainst(MainPlayer);
                _a2.Tasks.FightAgainst(MainPlayer);
                _a3.Tasks.FightAgainst(MainPlayer);
                _a6.Tasks.FightAgainst(MainPlayer);
                _a7.Tasks.FightAgainst(MainPlayer);
                _a8.Tasks.FightAgainst(MainPlayer);
                _a9.Tasks.FightAgainst(MainPlayer);
            }
            if (MainPlayer.IsDead) End();
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (_a1 && _a1.IsDead && _a2 && _a2.IsDead && _a3 && _a3.IsDead && _a6 && _a6.IsDead && _a7 && _a7.IsDead && _a8 && _a8.IsDead && _a9 && _a9.IsDead) End();
            if (_a1 && Functions.IsPedArrested(_a1) && _a2 && Functions.IsPedArrested(_a2) && _a3 && Functions.IsPedArrested(_a3) && _a6 && Functions.IsPedArrested(_a6) && _a7 && Functions.IsPedArrested(_a7) && _a8 && Functions.IsPedArrested(_a8) && _a9 && Functions.IsPedArrested(_a9)) End();
        }, "Robbery at Human Labs and Research [UnitedCallouts]");
        base.Process();
    }

    public override void End()
    {
        if (_b1) _b1.Delete();
        if (_b2) _b2.Delete();
        if (_b3) _b3.Delete();
        if (_b6) _b6.Delete();
        if (_b7) _b7.Delete();
        if (_b8) _b8.Delete();
        if (_b9) _b9.Delete();
        if (_a1) _a1.Dismiss();
        if (_a2) _a2.Dismiss();
        if (_a3) _a3.Dismiss();
        if (_a6) _a6.Dismiss();
        if (_a7) _a7.Dismiss();
        if (_a8) _a8.Dismiss();
        if (_a9) _a9.Dismiss();
        if (_swat1) _swat1.Dismiss(); 
        if (_swat2) _swat2.Dismiss(); 
        if (_swat3) _swat3.Dismiss(); 
        if (_policeRiot) _policeRiot.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Robbery at Human Labs and Research", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
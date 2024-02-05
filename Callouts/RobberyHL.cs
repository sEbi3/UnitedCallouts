namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Robbery at Humane Labs and Research Facility", CalloutProbability.Medium)]
public class RobberyHl : Callout
{
    // Aggressors
    private static Ped _a1;
    private static Ped _a2;
    private static Ped _a3;
    private static Ped _a4;
    private static Ped _a5;
    private static Ped _a6;
    private static Ped _a7;

    // Aggressor Blips
    private static Blip _b1;
    private static Blip _b2;
    private static Blip _b3;
    private static Blip _b4;
    private static Blip _b5;
    private static Blip _b6;
    private static Blip _b7;

    // Swat Peds
    private static Ped _swat1;
    private static Ped _swat2;
    private static Ped _swat3;
    private static Vehicle _policeRiot;
    private static readonly Vector3 PoliceRiotSpawn = new(3635.675f, 3774.846f, 28.51558f);
    private static Vector3 _spawnPoint;
    private static readonly Vector3 Swat1Spawn = new(3631.826f, 3774.041f, 28.51571f);
    private static readonly Vector3 Swat2Spawn = new(3632.776f, 3772.018f, 28.51571f);

    private static readonly Vector3 Swat3Spawn = new(3633.925f, 3768.781f, 28.51571f);
    // private static Vector3 _joinSwatVector;
    // private static bool _joinSwat = false;
    // private static bool _noticed = false;
    // private static bool _pursuitCreated = false;
    // private static LHandle _pursuit;

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = new Vector3(3616.228f, 3737.619f, 28.6901f);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 70f);
        AddMinimumDistanceCheck(10f, _spawnPoint);
        CalloutMessage = "[UC]~w~ Robbery at Humane Labs and Research Facility.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_01 crime_robbery_with_a_firearm IN_OR_ON_POSITION",
            _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Robbery at Human Labs and Research",
            "~b~Dispatch: ~w~The wanted robbers are marked on your ~y~GPS~w~! Respond with ~r~Code 3");

        // Ped 1
        _a1 = new Ped("g_m_m_chemwork_01", _spawnPoint, 0f)
        {
            IsPersistent = true,
            BlockPermanentEvents = true,
            Armor = 400,
            RelationshipGroup = RelationshipGroup.AggressiveInvestigate
        };
        _a1.Inventory.GiveNewWeapon("WEAPON_SMG", 5000, true);
        _b1 = _a1.AttachBlip();

        // Ped 2
        _a2 = new Ped("g_m_m_chemwork_01", _spawnPoint, 0f)
        {
            IsPersistent = true,
            BlockPermanentEvents = true,
            Armor = 400,
            RelationshipGroup = RelationshipGroup.AggressiveInvestigate
        };
        _a2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
        _b2 = _a2.AttachBlip();

        // Ped 3
        _a3 = new Ped("g_m_m_chemwork_01", _spawnPoint, 0f)
        {
            IsPersistent = true,
            BlockPermanentEvents = true,
            Armor = 400,
            RelationshipGroup = RelationshipGroup.AggressiveInvestigate
        };
        _a3.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);
        _b3 = _a3.AttachBlip();

        // Ped 4
        _a4 = new Ped("g_m_m_chemwork_01", _spawnPoint, 0f)
        {
            IsPersistent = true,
            BlockPermanentEvents = true,
            RelationshipGroup = RelationshipGroup.AggressiveInvestigate
        };
        _a4.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);
        _b4 = _a4.AttachBlip();

        // Ped 5
        _a5 = new Ped("g_m_m_chemwork_01", _spawnPoint, 0f)
        {
            IsPersistent = true,
            BlockPermanentEvents = true,
            RelationshipGroup = RelationshipGroup.AggressiveInvestigate
        };
        _b5 = _a5.AttachBlip();
        _a5.Inventory.GiveNewWeapon("WEAPON_SMG", 5000, true);

        // Ped 6
        _a6 = new Ped("u_m_y_juggernaut_01", _spawnPoint, 0f)
        {
            CanRagdoll = false,
            IsPersistent = true,
            BlockPermanentEvents = true,
            RelationshipGroup = RelationshipGroup.AggressiveInvestigate,
            Armor = 1000
        };
        NativeFunction.Natives.SET_PED_SUFFERS_CRITICAL_HITS(_a6, false);
        NativeFunction.Natives.SetPedPathCanUseClimbovers(_a6, true);
        _a6.Inventory.GiveNewWeapon("WEAPON_MINIGUN", 5000, true);
        _b6 = _a6.AttachBlip();
        Functions.SetPedCantBeArrestedByPlayer(_a6, true);

        // Ped 7
        _a7 = new Ped("g_m_m_chemwork_01", _spawnPoint, 0f)
        {
            IsPersistent = true,
            BlockPermanentEvents = true,
            RelationshipGroup = RelationshipGroup.AggressiveInvestigate

        };
        _a7.Inventory.GiveNewWeapon("WEAPON_SMG", 5000, true);
        _b7 = _a7.AttachBlip();

        // Vehicle
        _policeRiot = new Vehicle("RIOT", PoliceRiotSpawn, 0f)
        {
            Heading = -174f,
            IsSirenOn = true,
            IsSirenSilent = true
        };

        // Swat
        _swat1 = new("s_m_y_swat_01", Swat1Spawn, 94f);
        _swat1.Armor = 200;
        _swat1.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 500, true);
        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_swat1, _a1, -1, true);

        _swat2 = new("s_m_y_swat_01", Swat2Spawn, 94f);
        _swat2.Armor = 200;
        _swat2.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 500, true);
        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_swat2, _a1, -1, true);

        _swat3 = new("s_m_y_swat_01", Swat3Spawn, 94f);
        _swat3.Armor = 200;
        _swat3.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 500, true);
        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_swat3, _a1, -1, true);

        _b1.EnableRoute(Color.Yellow);

        if (Settings.ActivateAiBackup)
        {
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Code3,
                LSPD_First_Response.EBackupUnitType.SwatTeam);
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Code3,
                LSPD_First_Response.EBackupUnitType.SwatTeam);
        }
        else
        {
            Settings.ActivateAiBackup = false;
        }

        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_b1) _b1.Delete();
        if (_b2) _b2.Delete();
        if (_b3) _b3.Delete();
        if (_b4) _b4.Delete();
        if (_b5) _b5.Delete();
        if (_b6) _b6.Delete();
        if (_b7) _b7.Delete();
        if (_a1) _a1.Delete();
        if (_a2) _a2.Delete();
        if (_a3) _a3.Delete();
        if (_a4) _a4.Delete();
        if (_a5) _a5.Delete();
        if (_a6) _a6.Delete();
        if (_a7) _a7.Delete();
        if (_policeRiot) _policeRiot.Delete();
        if (_swat1) _swat1.Delete();
        if (_swat2) _swat2.Delete();
        if (_swat3) _swat3.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (MainPlayer.DistanceTo(_spawnPoint) < 22f)
        {
            _a1.Tasks.FightAgainst(MainPlayer);
            _a2.Tasks.FightAgainst(MainPlayer);
            _a3.Tasks.FightAgainst(MainPlayer);
            _a4.Tasks.FightAgainst(MainPlayer);
            _a5.Tasks.FightAgainst(MainPlayer);
            _a6.Tasks.FightAgainst(MainPlayer);
            _a7.Tasks.FightAgainst(MainPlayer);
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (_a1 && _a1.IsDead && _a2 && _a2.IsDead && _a3 && _a3.IsDead && _a4 && _a4.IsDead && _a5 && _a5.IsDead &&
            _a6 && _a6.IsDead && _a7 && _a7.IsDead) End();
        if (_a1 && Functions.IsPedArrested(_a1) && _a2 && Functions.IsPedArrested(_a2) && _a3 &&
            Functions.IsPedArrested(_a3) && _a4 && Functions.IsPedArrested(_a4) && _a5 &&
            Functions.IsPedArrested(_a5) && _a6 && Functions.IsPedArrested(_a6) && _a7 &&
            Functions.IsPedArrested(_a7)) End();
        base.Process();
    }

    public override void End()
    {
        if (_b1) _b1.Delete();
        if (_b2) _b2.Delete();
        if (_b3) _b3.Delete();
        if (_b4) _b4.Delete();
        if (_b5) _b5.Delete();
        if (_b6) _b6.Delete();
        if (_b7) _b7.Delete();
        if (_a1) _a1.Dismiss();
        if (_a2) _a2.Dismiss();
        if (_a3) _a3.Dismiss();
        if (_a4) _a4.Dismiss();
        if (_a5) _a5.Dismiss();
        if (_a6) _a6.Dismiss();
        if (_a7) _a7.Dismiss();
        if (_swat1) _swat1.Dismiss();
        if (_swat2) _swat2.Dismiss();
        if (_swat3) _swat3.Dismiss();
        if (_policeRiot) _policeRiot.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Robbery at Human Labs and Research", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
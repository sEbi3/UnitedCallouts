using System.Linq;

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Robbery at Humane Labs and Research Facility", CalloutProbability.Medium)]
public class RobberyHl : Callout
{
    // Aggressors
    private static Ped A1 => _aggressorPeds[0];
    private static Ped A2 => _aggressorPeds[1];
    private static Ped A3 => _aggressorPeds[2];
    private static Ped A4 => _aggressorPeds[3];
    private static Ped A5 => _aggressorPeds[4];
    private static Ped A6 => _aggressorPeds[5];
    private static Ped A7 => _aggressorPeds[6];

    // Aggressor Blips
    private static Blip B1 => _aggressorBlips[0];
    private static Blip B2 => _aggressorBlips[1];
    private static Blip B3 => _aggressorBlips[2];
    private static Blip B4 => _aggressorBlips[3];
    private static Blip B5 => _aggressorBlips[4];
    private static Blip B6 => _aggressorBlips[5];
    private static Blip B7 => _aggressorBlips[6];

    // Swat Peds
    private static Ped _swat1;
    private static Ped _swat2;
    private static Ped _swat3;
    private static Vehicle _policeRiot;

    // Spawn Points
    private static readonly Vector3 PoliceRiotSpawn = new(3635.675f, 3774.846f, 28.51558f);
    private static Vector3 _spawnPoint;
    private static readonly Vector3 Swat1Spawn = new(3631.826f, 3774.041f, 28.51571f);
    private static readonly Vector3 Swat2Spawn = new(3632.776f, 3772.018f, 28.51571f);
    private static readonly Vector3 Swat3Spawn = new(3633.925f, 3768.781f, 28.51571f);

    // Lists
    private static List<Ped> _aggressorPeds = new()
    {
        A1,
        A2,
        A3,
        A4,
        A5,
        A6,
        A7
    };

    private static List<Blip> _aggressorBlips = new()
    {
        B1,
        B2,
        B3,
        B4,
        B5,
        B6,
        B7
    };

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

        for (int i = 0; i < _aggressorPeds.Count; i++)
        {
            //_aggressorPeds[i]

            _aggressorPeds[i] = new((i == 5) ? "u_m_y_juggernaut_01" : "g_m_m_chemwork_01", _spawnPoint, 0f)
            {
                IsPersistent = true,
                BlockPermanentEvents = true,
                Armor = (i == 5) ? 1000 : 400,
                CanRagdoll = (i != 5),
                RelationshipGroup = RelationshipGroup.AggressiveInvestigate
            };

            _aggressorBlips[i] = _aggressorPeds[i].AttachBlip();

        }

        // Ped 1 (Reg)
        A1.Inventory.GiveNewWeapon("WEAPON_SMG", 5000, true);

        // Ped 2 (Reg)
        A2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);

        // Ped 3 (Reg)
        A3.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);

        // Ped 4 (Reg)
        A4.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);

        // Ped 5 (Reg)
        A5.Inventory.GiveNewWeapon("WEAPON_SMG", 5000, true);

        // Ped 6 (Jugg)
        NativeFunction.Natives.SET_PED_SUFFERS_CRITICAL_HITS(A6, false);
        NativeFunction.Natives.SetPedPathCanUseClimbovers(A6, true);
        A6.Inventory.GiveNewWeapon("WEAPON_MINIGUN", 5000, true);
        Functions.SetPedCantBeArrestedByPlayer(A6, true);

        // Ped 7 (Reg)
        A7.Inventory.GiveNewWeapon("WEAPON_SMG", 5000, true);

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
        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_swat1, A1, -1, true);

        _swat2 = new("s_m_y_swat_01", Swat2Spawn, 94f);
        _swat2.Armor = 200;
        _swat2.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 500, true);
        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_swat2, A1, -1, true);

        _swat3 = new("s_m_y_swat_01", Swat3Spawn, 94f);
        _swat3.Armor = 200;
        _swat3.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 500, true);
        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_swat3, A1, -1, true);

        B1.EnableRoute(Color.Yellow);

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
        foreach (var blip in _aggressorBlips)
        {
            if (blip) blip.Delete();
        }

        foreach (var ped in _aggressorPeds)
        {
            if (ped) ped.Delete();
        }

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
            foreach (var ped in _aggressorPeds)
            {
                ped.Tasks.FightAgainst(MainPlayer);
            }
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (A1 && A1.IsDead && A2 && A2.IsDead && A3 && A3.IsDead && A4 && A4.IsDead && A5 && A5.IsDead &&
            A6 && A6.IsDead && A7 && A7.IsDead) End();
        if (A1 && Functions.IsPedArrested(A1) && A2 && Functions.IsPedArrested(A2) && A3 &&
            Functions.IsPedArrested(A3) && A4 && Functions.IsPedArrested(A4) && A5 &&
            Functions.IsPedArrested(A5) && A6 && Functions.IsPedArrested(A6) && A7 &&
            Functions.IsPedArrested(A7)) End();
        base.Process();
    }

    public override void End()
    {
        foreach (var blip in _aggressorBlips.Where(blip => blip))
        {
            blip.Delete();
        }

        foreach (var ped in _aggressorPeds.Where(ped => ped))
        {
            ped.Dismiss();
        }

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
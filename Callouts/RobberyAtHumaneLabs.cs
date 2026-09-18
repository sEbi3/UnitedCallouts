using System.Linq;

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Robbery at Humane Labs and Research Facility", CalloutProbability.Medium)]
public class RobberyAtHumaneLabs : Callout
{
    private Ped A1 => _aggressorPeds[0];
    private Ped A2 => _aggressorPeds[1];
    private Ped A3 => _aggressorPeds[2];
    private Ped A4 => _aggressorPeds[3];
    private Ped A5 => _aggressorPeds[4];
    private Ped A6 => _aggressorPeds[5];
    private Ped A7 => _aggressorPeds[6];

    private Blip B1 => _aggressorBlips[0];
    private Blip B2 => _aggressorBlips[1];
    private Blip B3 => _aggressorBlips[2];
    private Blip B4 => _aggressorBlips[3];
    private Blip B5 => _aggressorBlips[4];
    private Blip B6 => _aggressorBlips[5];
    private Blip B7 => _aggressorBlips[6];

    private Ped _swat1;
    private Ped _swat2;
    private Ped _swat3;
    private Vehicle _policeRiot;

    private readonly Vector3 PoliceRiotSpawn = new(3635.675f, 3774.846f, 28.51558f);
    private Vector3 _spawnPoint;
    private readonly Vector3 Swat1Spawn = new(3631.826f, 3774.041f, 28.51571f);
    private readonly Vector3 Swat2Spawn = new(3632.776f, 3772.018f, 28.51571f);
    private readonly Vector3 Swat3Spawn = new(3633.925f, 3768.781f, 28.51571f);

    private Ped[] _aggressorPeds = new Ped[7];
    private Blip[] _aggressorBlips = new Blip[7];

    private bool _noticed = false;

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = new Vector3(3616.228f, 3737.619f, 28.6901f);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 70f);
        AddMinimumDistanceCheck(10f, _spawnPoint);
        CalloutMessage = "Robbery at Humane Labs and Research Facility";
        CalloutAdvisory = "Reports received of an armed robbery in progress at the Humane Labs and Research Facility, with heavily armed suspects observed by security.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_01 crime_robbery_with_a_firearm IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Robbery at Humane Labs and Research callout accepted.");
        }
        else { Settings.DetailedLogging = false; }
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Robbery at Human Labs and Research",
            "~b~Dispatch: ~w~Respond to an armed robbery at the Humane Labs and Research Facility. Security reports multiple heavily armed suspects on site. Respond with ~r~Code 3~w~.");

        for (int i = 0; i < _aggressorPeds.Length; i++)
        {
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

        A1.Inventory.GiveNewWeapon("WEAPON_SMG", 5000, true);
        A2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
        A3.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);
        A4.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);
        A5.Inventory.GiveNewWeapon("WEAPON_SMG", 5000, true);

        NativeFunction.Natives.SET_PED_SUFFERS_CRITICAL_HITS(A6, false);
        NativeFunction.Natives.SetPedPathCanUseClimbovers(A6, true);
        A6.Inventory.GiveNewWeapon("WEAPON_MINIGUN", 5000, true);
        Functions.SetPedCantBeArrestedByPlayer(A6, true);

        A7.Inventory.GiveNewWeapon("WEAPON_SMG", 5000, true);

        _policeRiot = new Vehicle("RIOT", PoliceRiotSpawn, 0f)
        {
            Heading = -174f,
            IsSirenOn = true,
            IsSirenSilent = true
        };

        _swat1 = new("s_m_y_swat_01", Swat1Spawn, 94f)
        {
            Armor = 200
        };
        _swat1.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 500, true);
        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_swat1, A1, -1, true);
        Functions.SetPedAsCop(_swat1);

        _swat2 = new("s_m_y_swat_01", Swat2Spawn, 94f)
        {
            Armor = 200
        };
        _swat2.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 500, true);
        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_swat2, A1, -1, true);
        Functions.SetPedAsCop(_swat2);

        _swat3 = new("s_m_y_swat_01", Swat3Spawn, 94f)
        {
            Armor = 200
        };
        _swat3.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 500, true);
        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_swat3, A1, -1, true);
        Functions.SetPedAsCop(_swat3);

        B1.EnableRoute(Color.Yellow);

        if (Settings.ActivateAiBackup)
        {
            Functions.RequestBackup(PoliceRiotSpawn, LSPD_First_Response.EBackupResponseType.Code3,
                LSPD_First_Response.EBackupUnitType.SwatTeam);
            Functions.RequestBackup(PoliceRiotSpawn, LSPD_First_Response.EBackupResponseType.Code3,
                LSPD_First_Response.EBackupUnitType.SwatTeam);
            if (Settings.DetailedLogging)
            {
                Game.LogTrivial("[UnitedCallouts LOG:] Robbery at Humane Labs Callout: AI Backup has been spawned.");
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
        foreach (var blip in _aggressorBlips)
        {
            if (blip != null && blip.Exists()) blip.Delete();
        }

        foreach (var ped in _aggressorPeds)
        {
            if (ped != null && ped.Exists()) ped.Delete();
        }

        if (_policeRiot != null && _policeRiot.Exists()) _policeRiot.Delete();
        if (_swat1 != null && _swat1.Exists()) _swat1.Delete();
        if (_swat2 != null && _swat2.Exists()) _swat2.Delete();
        if (_swat3 != null && _swat3.Exists()) _swat3.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (!_noticed && MainPlayer.DistanceTo(_spawnPoint) < 50f)
        {
            foreach (var ped in _aggressorPeds)
            {
                if (ped != null && ped.Exists()) ped.Tasks.FightAgainst(MainPlayer);
            }
            _noticed = true;
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (A1 != null && A1.IsDead && A2 != null && A2.IsDead && A3 != null && A3.IsDead &&
            A4 != null && A4.IsDead && A5 != null && A5.IsDead &&
            A6 != null && A6.IsDead && A7 != null && A7.IsDead) End();

        if (A1 != null && Functions.IsPedArrested(A1) && A2 != null && Functions.IsPedArrested(A2) &&
            A3 != null && Functions.IsPedArrested(A3) && A4 != null && Functions.IsPedArrested(A4) &&
            A5 != null && Functions.IsPedArrested(A5) && A6 != null && Functions.IsPedArrested(A6) &&
            A7 != null && Functions.IsPedArrested(A7)) End();
        base.Process();
    }

    public override void End()
    {
        foreach (var blip in _aggressorBlips.Where(blip => blip != null && blip.Exists()))
        {
            blip.Delete();
        }

        foreach (var ped in _aggressorPeds.Where(ped => ped != null && ped.Exists()))
        {
            ped.Dismiss();
        }

        if (_swat1 != null && _swat1.Exists()) _swat1.Dismiss();
        if (_swat2 != null && _swat2.Exists()) _swat2.Dismiss();
        if (_swat3 != null && _swat3.Exists()) _swat3.Dismiss();
        if (_policeRiot != null && _policeRiot.Exists()) _policeRiot.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Robbery at Human Labs and Research", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Robbery at Humane Labs and Research callout ended.");
        }
        else { Settings.DetailedLogging = false; }
        base.End();
    }
}
using System.Linq;

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Store Robbery In Progress", CalloutProbability.Medium)]
public class StoreRobberyInProgress : Callout
{
    private static readonly string[] AList = { "mp_g_m_pros_01", "g_m_m_chicold_01" };

    private static readonly string[] WepList =
    {
        "WEAPON_PISTOL", "WEAPON_SMG", "WEAPON_MACHINEPISTOL", "WEAPON_PUMPSHOTGUN", "weapon_heavypistol",
        "weapon_minismg"
    };

    private static readonly string[] VList = { "s_m_m_ammucountry", "mp_m_shopkeep_01", "s_f_m_sweatshop_01" };
    private static Ped _a1;
    private static Ped _a2;
    private static Ped _v;
    private static Vector3 _spawnPoint;
    private static Vector3 _searcharea;
    private static Blip _blip;
    private static LHandle _pursuit;
    private static bool _pursuitCreated;
    private static bool _scene1;
    private static bool _scene2;
    private static bool _scene3;
    private static bool _notificationDisplayed = false;
    private static bool _check = false;
    private static bool _hasBegunAttacking;

    public override bool OnBeforeCalloutDisplayed()
    {
        Tuple<Vector3, float, Vector3, float, Vector3, float>[] spawningLocationList =
        {
            Tuple.Create(new Vector3(73.87572f, -1392.849f, 29.37613f), 263.9599f,
                new Vector3(75.91751f, -1391.883f, 29.37615f), 114.3616f, new Vector3(76.16069f, -1389.982f, 29.37615f),
                19.93308f),
            Tuple.Create(new Vector3(427.145f, -806.8593f, 29.49114f), 97.54604f,
                new Vector3(425.0366f, -807.4084f, 29.49113f), 291.1364f, new Vector3(424.7376f, -809.5753f, 29.49224f),
                217.1934f),
            Tuple.Create(new Vector3(-822.653f, -1071.912f, 11.32811f), 208.5814f,
                new Vector3(-820.9958f, -1073.383f, 11.32811f), 43.43138f,
                new Vector3(-819.3976f, -1072.761f, 11.32906f), 335.4849f),
            Tuple.Create(new Vector3(1695.573f, 4822.746f, 42.06311f), 95.36308f,
                new Vector3(1693.365f, 4821.716f, 42.06312f), 289.7472f, new Vector3(1693.838f, 4819.305f, 42.0641f),
                215.6228f),
            Tuple.Create(new Vector3(-1102.171f, 2711.92f, 19.10787f), 244.3523f,
                new Vector3(-1100.294f, 2710.429f, 19.10785f), 48.32264f, new Vector3(-1098.755f, 2712.339f, 19.10868f),
                338.6742f),
            Tuple.Create(new Vector3(1197.127f, 2711.719f, 38.22263f), 155.001f,
                new Vector3(1197.668f, 2709.616f, 38.2226f), 13.32392f, new Vector3(1200.074f, 2709.42f, 38.22372f),
                318.5346f),
            Tuple.Create(new Vector3(5.792656f, 6511.06f, 31.87785f), 53.38403f,
                new Vector3(3.368677f, 6512.238f, 31.87785f), 242.7346f, new Vector3(1.847083f, 6510.697f, 31.87863f),
                164.7863f),
            Tuple.Create(new Vector3(372.285f, 326.7229f, 103.5664f), 245.4788f,
                new Vector3(374.295f, 325.8951f, 103.5664f), 71.65144f, new Vector3(374.8215f, 327.8206f, 103.5664f),
                67.57134f),
            Tuple.Create(new Vector3(1164.891f, -321.9834f, 69.20512f), 109.593f,
                new Vector3(1163.107f, -322.1323f, 69.20507f), 279.6259f, new Vector3(1163.093f, -324.2025f, 69.20507f),
                279.2135f),
            Tuple.Create(new Vector3(1133.908f, -981.8345f, 46.41584f), 277.1535f,
                new Vector3(1135.899f, -981.3351f, 46.41584f), 106.3927f, new Vector3(1136.195f, -982.9218f, 46.41584f),
                84.65549f),
        };
        List<Vector3> list = spawningLocationList.Select(t => t.Item1).ToList();
        int num = LocationChooser.NearestLocationIndex(list);
        _spawnPoint = spawningLocationList[num].Item1;
        _v = new(VList[Rndm.Next(VList.Length)], _spawnPoint, spawningLocationList[num].Item2)
        {
            IsPersistent = true,
            BlockPermanentEvents = true
        };

        _a1 = new(AList[Rndm.Next(AList.Length)], spawningLocationList[num].Item3, spawningLocationList[num].Item4)
        {
            IsPersistent = true,
            BlockPermanentEvents = true,
            Armor = 200
        };
        _a1.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
        _a1.Face(_v);

        _a2 = new(AList[Rndm.Next(AList.Length)], spawningLocationList[num].Item5, spawningLocationList[num].Item6)
        {
            IsPersistent = true,
            BlockPermanentEvents = true,
            Armor = 200
        };
        _a2.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
        _a2.Face(_v);

        switch (Rndm.Next(1, 4))
        {
            case 1:
                _scene1 = true;
                break;
            case 2:
                _scene2 = true;
                break;
            case 3:
                _scene3 = true;
                break;
        }

        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 100f);
        CalloutMessage = "[UC]~w~ Reports of a Store Robbery in Progress.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("DISP_ATTENTION_UNIT WE_HAVE CRIME_ROBBERY IN_OR_ON_POSITION",
            _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Store Robbery In Progress callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Store Robbery In Progress",
            "~b~Dispatch:~w~ Someone called the police because of a store robbery. Respond with ~r~Code 3~w~.");

        _searcharea = _spawnPoint.Around2D(1f, 2f);
        _blip = new(_searcharea, 20f)
        {
            Color = Color.Yellow,
            Alpha = 0.5f
        };
        _blip.EnableRoute(Color.Yellow);
        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_a1, _v, -1, true);
        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_a2, _v, -1, true);

        var aRelationshipGroup = new RelationshipGroup("A");
        var vRelationshipGroup = new RelationshipGroup("V");
        _v.RelationshipGroup = vRelationshipGroup;
        _a1.RelationshipGroup = aRelationshipGroup;
        _a2.RelationshipGroup = aRelationshipGroup;
        _v.Tasks.PutHandsUp(-1, _a1);

        aRelationshipGroup.SetRelationshipWith(vRelationshipGroup, Relationship.Hate);
        aRelationshipGroup.SetRelationshipWith(MainPlayer.RelationshipGroup, Relationship.Hate);
        aRelationshipGroup.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
        vRelationshipGroup.SetRelationshipWith(aRelationshipGroup, Relationship.Hate);

        if (Settings.ActivateAiBackup)
        {
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Code3,
                LSPD_First_Response.EBackupUnitType.SwatTeam);
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Code3,
                LSPD_First_Response.EBackupUnitType.LocalUnit);
        }
        else
        {
            Settings.ActivateAiBackup = false;
        }

        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_a1) _a1.Delete();
        if (_a2) _a2.Delete();
        if (_v) _v.Delete();
        if (_blip) _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (!_hasBegunAttacking && _a1.DistanceTo(MainPlayer) < 25f)
        {
            _hasBegunAttacking = true;
            GameFiber.StartNew(() =>
            {
                if (_scene1)
                {
                    _a1.Tasks.FightAgainstClosestHatedTarget(1000f);
                    _a2.Tasks.FightAgainstClosestHatedTarget(1000f);
                    GameFiber.Wait(2000);
                    _hasBegunAttacking = true;
                }
                else if (_scene2 && !_notificationDisplayed && !_check)
                {
                    _a1.Tasks.FightAgainstClosestHatedTarget(1000f);
                    _a2.Tasks.FightAgainstClosestHatedTarget(1000f);
                    GameFiber.Wait(2000);
                    _hasBegunAttacking = true;
                }
                else if (_scene3 && !_pursuitCreated)
                {
                    _pursuit = Functions.CreatePursuit();
                    Functions.AddPedToPursuit(_pursuit, _a1);
                    Functions.AddPedToPursuit(_pursuit, _a2);
                    Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                    _pursuitCreated = true;
                }
            });
        }

        if (_a1 && _a1.IsDead && _a2 && _a2.IsDead) End();
        if (_a1 && Functions.IsPedArrested(_a1) && _a2 && Functions.IsPedArrested(_a2)) End();
        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        base.Process();
    }

    public override void End()
    {
        if (_a1) _a1.Dismiss();
        if (_a2) _a2.Dismiss();
        if (_v) _v.Dismiss();
        if (_blip) _blip.Delete();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Store Robbery In Progress", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
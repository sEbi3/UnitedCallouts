namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Jewellery Robbery In Progress", CalloutProbability.Medium)]
public class JewelleryRobbery : Callout
{
    private static readonly string[] AList = { "mp_g_m_pros_01", "g_m_m_chicold_01" };

    private static readonly string[] WepList =
    {
        "WEAPON_SMG", "WEAPON_PUMPSHOTGUN", "weapon_microsmg", "weapon_machinepistol", "weapon_compactrifle",
        "weapon_combatpistol", "weapon_pistol"
    };

    // FIXED: Removed static from all instance fields
    private Ped _a1;
    private Ped _a2;
    private Ped _a3;
    private Ped _cop1;
    private Ped _cop2;
    private Vehicle _copCar1;
    private Vehicle _copCar2;
    private Vector3 _spawnPoint;
    private Vector3 _searchArea;
    private Vector3 _ag1Spawnpoint;
    private Vector3 _ag2Spawnpoint;
    private Vector3 _ag3Spawnpoint;
    private Vector3 _cop1Spawnpoint;
    private Vector3 _cop2Spawnpoint;
    private Vector3 _copCar1Spawnpoint;
    private Vector3 _copCar2Spawnpoint;
    private bool _scene1;
    private bool _scene2;
    private bool _scene3;
    private Blip _blip;
    private LHandle _pursuit;
    private bool _pursuitCreated;
    private bool _notificationDisplayed = false;
    private bool _check = false;
    private bool _hasBegunAttacking;

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = new(-625.7944f, -229.267f, 38.05706f);
        _ag1Spawnpoint = new(-625.7944f, -229.267f, 38.05706f);
        _ag2Spawnpoint = new(-621.2017f, -234.7889f, 38.05706f);
        _ag3Spawnpoint = new(-619.4506f, -229.297f, 38.05701f);
        _cop1Spawnpoint = new(-644.9532f, -231.6783f, 37.74535f);
        _cop2Spawnpoint = new(-646.092f, -234.0365f, 37.75615f);
        _copCar1Spawnpoint = new(-645.6291f, -232.7989f, 37.35386f);
        _copCar2Spawnpoint = new(-626.4602f, -257.1244f, 38.29225f);

        _a1 = new(AList[Rndm.Next(AList.Length)], _ag1Spawnpoint, 144.7047f);
        _a2 = new(AList[Rndm.Next(AList.Length)], _ag2Spawnpoint, 105.2843f);
        _a3 = new(AList[Rndm.Next(AList.Length)], _ag3Spawnpoint, 120.7921f);

        _cop1 = new("S_M_Y_COP_01", _cop1Spawnpoint, 247.3842f);
        _cop2 = new("S_M_Y_COP_01", _cop2Spawnpoint, 253.8056f);

        _copCar1 = new("POLICE", _copCar1Spawnpoint, 245.2429f);
        _copCar2 = new("RIOT", _copCar2Spawnpoint, 14.0995f);

        _cop1.WarpIntoVehicle(_copCar1, 1);
        _cop2.WarpIntoVehicle(_copCar1, 2);

        _copCar1.IsSirenOn = true;
        _copCar2.IsSirenOn = true;
        _copCar1.IsSirenSilent = true;
        _copCar2.IsSirenSilent = true;

        _cop1.Tasks.LeaveVehicle(_copCar1, LeaveVehicleFlags.LeaveDoorOpen);
        _cop2.Tasks.LeaveVehicle(_copCar1, LeaveVehicleFlags.LeaveDoorOpen);
        _cop1.Tasks.Clear();
        _cop2.Tasks.Clear();


        switch (Rndm.Next(1, 3))
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
        CalloutMessage = "[UC]~w~ Reports of a Jewellery Robbery in Progress.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("DISP_ATTENTION_UNIT WE_HAVE CRIME_ROBBERY IN_OR_ON_POSITION",
            _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Jewellery Robbery callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Jewellery Robbery",
            "~b~Dispatch:~w~ The units on scene need backup at the jewellery. Respond with ~r~Code 3~w~.");

        _a1.IsPersistent = true;
        _a1.BlockPermanentEvents = true;
        _a1.Armor = 200;
        _a1.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);

        _a2.IsPersistent = true;
        _a2.BlockPermanentEvents = true;
        _a2.Armor = 200;
        _a2.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);

        _a3.IsPersistent = true;
        _a3.BlockPermanentEvents = true;
        _a3.Armor = 200;
        _a3.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);

        _cop1.IsPersistent = true;
        _cop1.BlockPermanentEvents = true;
        _cop1.Armor = 200;
        _cop1.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
        Functions.IsPedACop(_cop1);

        _cop2.IsPersistent = true;
        _cop2.BlockPermanentEvents = true;
        _cop2.Armor = 200;
        _cop2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
        Functions.IsPedACop(_cop2);

        _searchArea = _spawnPoint.Around2D(1f, 2f);
        _blip = new(_searchArea, 20f);
        _blip.EnableRoute(Color.Yellow);
        _blip.Color = Color.Yellow;
        _blip.Alpha = 0.5f;

        var agRelationshipGroup = new RelationshipGroup("A");
        _a1.RelationshipGroup = agRelationshipGroup;
        _a2.RelationshipGroup = agRelationshipGroup;
        _a3.RelationshipGroup = agRelationshipGroup;
        _cop1.RelationshipGroup = RelationshipGroup.Cop;
        _cop2.RelationshipGroup = RelationshipGroup.Cop;

        agRelationshipGroup.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
        agRelationshipGroup.SetRelationshipWith(MainPlayer.RelationshipGroup, Relationship.Hate);
        RelationshipGroup.Cop.SetRelationshipWith(agRelationshipGroup, Relationship.Hate);

        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_cop1, _a3, -1, true);
        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_cop2, _a3, -1, true);

        if (Settings.ActivateAiBackup)
        {
            Functions.RequestBackup(_copCar1Spawnpoint, LSPD_First_Response.EBackupResponseType.Code3,
                LSPD_First_Response.EBackupUnitType.SwatTeam);
            Functions.RequestBackup(_copCar1Spawnpoint, LSPD_First_Response.EBackupResponseType.Code3,
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
        // FIXED: Added exists checks before deletion
        if (_a1 != null && _a1.Exists()) _a1.Delete();
        if (_a2 != null && _a2.Exists()) _a2.Delete();
        if (_a3 != null && _a3.Exists()) _a3.Delete();
        if (_cop1 != null && _cop1.Exists()) _cop1.Delete();
        if (_cop2 != null && _cop2.Exists()) _cop2.Delete();
        if (_copCar1 != null && _copCar1.Exists()) _copCar1.Delete();
        if (_copCar2 != null && _copCar2.Exists()) _copCar2.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        // FIXED: Changed logic from (!_hasBegunAttacking || _pursuitCreated) to proper AND condition
        if (!_hasBegunAttacking && !_pursuitCreated && _spawnPoint.DistanceTo(MainPlayer) < 22f)
        {
            _hasBegunAttacking = true;
            _pursuitCreated = true;

            GameFiber.StartNew(() =>
            {
                if (_scene1 && !_hasBegunAttacking)
                {
                    if (_a1 != null && _a1.Exists()) _a1.Tasks.FightAgainstClosestHatedTarget(1000f);
                    if (_a2 != null && _a2.Exists()) _a2.Tasks.FightAgainstClosestHatedTarget(1000f);
                    if (_a3 != null && _a3.Exists()) _a3.Tasks.FightAgainstClosestHatedTarget(1000f);
                    if (_cop1 != null && _cop1.Exists()) _cop1.Tasks.FightAgainstClosestHatedTarget(1000f);
                    if (_cop2 != null && _cop2.Exists()) _cop2.Tasks.FightAgainstClosestHatedTarget(1000f);
                    if (_cop2 != null && _cop2.Exists() && _a1 != null && _a1.Exists()) _cop2.Tasks.FightAgainst(_a1);
                    if (_cop1 != null && _cop1.Exists() && _a2 != null && _a2.Exists()) _cop1.Tasks.FightAgainst(_a2);
                    if (_a1 != null && _a1.Exists()) _a1.Tasks.FightAgainst(MainPlayer);
                    if (_a2 != null && _a2.Exists()) _a2.Tasks.FightAgainst(MainPlayer);
                    if (_a3 != null && _a3.Exists()) _a3.Tasks.FightAgainst(MainPlayer);
                    GameFiber.Wait(2000);
                }
                else if (_scene2 && !_notificationDisplayed && !_check)
                {
                    if (_a1 != null && _a1.Exists()) _a1.Tasks.FightAgainstClosestHatedTarget(1000f);
                    if (_a2 != null && _a2.Exists()) _a2.Tasks.FightAgainstClosestHatedTarget(1000f);
                    if (_a3 != null && _a3.Exists()) _a3.Tasks.FightAgainstClosestHatedTarget(1000f);
                    if (_cop1 != null && _cop1.Exists()) _cop1.Tasks.FightAgainstClosestHatedTarget(1000f);
                    if (_cop2 != null && _cop2.Exists()) _cop2.Tasks.FightAgainstClosestHatedTarget(1000f);
                    if (_cop2 != null && _cop2.Exists() && _a1 != null && _a1.Exists()) _cop2.Tasks.FightAgainst(_a1);
                    if (_cop1 != null && _cop1.Exists() && _a2 != null && _a2.Exists()) _cop1.Tasks.FightAgainst(_a2);
                    if (_a1 != null && _a1.Exists()) _a1.Tasks.FightAgainst(MainPlayer);
                    if (_a2 != null && _a2.Exists()) _a2.Tasks.FightAgainst(MainPlayer);
                    if (_a3 != null && _a3.Exists()) _a3.Tasks.FightAgainst(MainPlayer);
                    GameFiber.Wait(2000);
                }
                else if (_scene3 && !_pursuitCreated)
                {
                    if (_cop2 != null && _cop2.Exists() && _a1 != null && _a1.Exists()) _cop2.Tasks.FightAgainst(_a1);
                    if (_cop1 != null && _cop1.Exists() && _a2 != null && _a2.Exists()) _cop1.Tasks.FightAgainst(_a2);
                    _pursuit = Functions.CreatePursuit();
                    if (_a1 != null && _a1.Exists()) Functions.AddPedToPursuit(_pursuit, _a1);
                    if (_a2 != null && _a2.Exists()) Functions.AddPedToPursuit(_pursuit, _a2);
                    if (_a3 != null && _a3.Exists()) Functions.AddPedToPursuit(_pursuit, _a3);
                    Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                }
            }, "Jewellery Robbery [UnitedCallouts]");
        }

        // FIXED: Added null checks
        if (_a1 != null && _a1.IsDead && _a2 != null && _a2.IsDead && _a3 != null && _a3.IsDead) End();
        if (_a1 != null && Functions.IsPedArrested(_a1) && _a2 != null && Functions.IsPedArrested(_a2) && _a3 != null &&
            Functions.IsPedArrested(_a3)) End();
        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        base.Process();
    }

    public override void End()
    {
        // FIXED: Added exists checks before cleanup
        if (_a1 != null && _a1.Exists()) _a1.Dismiss();
        if (_a2 != null && _a2.Exists()) _a2.Dismiss();
        if (_a3 != null && _a3.Exists()) _a3.Dismiss();
        if (_copCar1 != null && _copCar1.Exists()) _copCar1.Dismiss();
        if (_copCar2 != null && _copCar2.Exists()) _copCar2.Dismiss();
        if (_blip != null && _blip.Exists()) _blip.Delete();

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Jewellery Robbery", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] K9 Backup Required", CalloutProbability.Medium)]
public class K9BackupRequired : Callout
{
    private static readonly string[] CopList =
        { "S_M_Y_COP_01", "S_F_Y_COP_01", "S_M_Y_SHERIFF_01", "S_F_Y_SHERIFF_01" };

    private static readonly string[] CopCars =
        { "POLICE", "POLICE2", "POLICE3", "SHERIFF", "SHERIFF2" };

    private static readonly string[] VCars =
    {
        "DUKES", "BALLER", "BALLER2", "BISON", "BISON2", "BJXL", "CAVALCADE", "CHEETAH", "COGCABRIO", "ASEA", "ADDER",
        "FELON", "FELON2", "ZENTORNO",
        "WARRENER", "RAPIDGT", "INTRUDER", "FELTZER2", "FQ2", "RANCHERXL", "REBEL", "SCHWARZER", "COQUETTE",
        "CARBONIZZARE", "EMPEROR", "SULTAN", "EXEMPLAR", "MASSACRO",
        "DOMINATOR", "ASTEROPE", "PRAIRIE", "NINEF", "WASHINGTON", "CHINO", "CASCO", "INFERNUS", "ZTYPE", "DILETTANTE",
        "VIRGO", "F620", "PRIMO", "SULTAN", "EXEMPLAR", "F620", "FELON2", "FELON", "SENTINEL", "WINDSOR",
        "DOMINATOR", "DUKES", "GAUNTLET", "VIRGO", "ADDER", "BUFFALO", "ZENTORNO", "MASSACRO"
    };

    private Ped _cop;
    private Ped _v;
    private Vehicle _vV;
    private Vehicle _vCop;
    private Vector3 _spawnPoint;
    private Blip _blip;
    private LHandle _pursuit;
    private bool _pursuitCreated;
    private bool _scene1;
    private bool _scene2;
    private bool _scene3;
    private bool _notificationDisplayed;
    private bool _check;
    private bool _hasBegunAttacking;

    public override bool OnBeforeCalloutDisplayed()
    {
        List<Vector3> list = new();

        Tuple<Vector3, float>[] spawningLocationList =
        {
            Tuple.Create(new Vector3(-452.2763f, 5930.209f, 32.00574f), 141.1158f),
            Tuple.Create(new Vector3(2689.76f, 4379.656f, 46.21445f), 123.7446f),
            Tuple.Create(new Vector3(-2848.013f, 2205.696f, 31.40776f), 117.3819f),
            Tuple.Create(new Vector3(-1079.767f, -2050.001f, 12.78075f), 223.3597f),
            Tuple.Create(new Vector3(1901.965f, -735.1039f, 84.55292f), 125.9702f),
            Tuple.Create(new Vector3(2620.896f, 255.5361f, 97.55639f), 349.3095f),
            Tuple.Create(new Vector3(1524.368f, 820.0878f, 77.10448f), 332.4926f),
            Tuple.Create(new Vector3(2404.46f, 2872.158f, 39.88745f), 307.5641f),
            Tuple.Create(new Vector3(2913.759f, 4148.546f, 50.26934f), 16.63741f),
        };
        for (int i = 0; i < spawningLocationList.Length; i++)
        {
            list.Add(spawningLocationList[i].Item1);
        }

        int num = LocationChooser.NearestLocationIndex(list);
        _spawnPoint = spawningLocationList[num].Item1;
        _vCop = new(CopCars[Rndm.Next(CopCars.Length)], _spawnPoint, spawningLocationList[num].Item2);
        switch (Rndm.Next(1, 5))
        {
            case 1:
                _scene1 = true;
                break;
            case 2:
                _scene2 = true;
                break;
            case 4:
                _scene3 = true;
                break;
        }

        _vV = new(VCars[Rndm.Next(VCars.Length)], _vCop.GetOffsetPosition(Vector3.RelativeFront * 9f), _vCop.Heading);
        _vCop.IsSirenOn = true;
        _vCop.IsSirenSilent = true;

        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS OFFICER_REQUESTING_BACKUP", _spawnPoint);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 100f);

        CalloutMessage = "Request for K9 Unit Assistance";
        CalloutAdvisory = "K9 unit requested for a traffic stop on the Highway.";
        CalloutPosition = _spawnPoint;
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] K9 Backup Required callout accepted.");
        }
        else { Settings.DetailedLogging = false; }
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~K9 Backup Required",
            "~b~Dispatch: ~w~The Officer on scene reports possible narcotics suspicion. Proceed with caution and advise on K9 deployment.");

        _cop = new(CopList[Rndm.Next(CopList.Length)], _spawnPoint, 0f);
        _cop.IsPersistent = true;
        _cop.BlockPermanentEvents = true;
        _cop.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
        _cop.WarpIntoVehicle(_vCop, -1);
        _cop.Tasks.CruiseWithVehicle(0, VehicleDrivingFlags.None);
        Functions.IsPedACop(_cop);

        _v = new(_spawnPoint);
        _v.IsPersistent = true;
        _v.BlockPermanentEvents = true;
        _v.WarpIntoVehicle(_vV, -1);
        _v.Tasks.CruiseWithVehicle(0, VehicleDrivingFlags.None);

        _blip = _cop.AttachBlip();
        _blip.EnableRoute(Color.Yellow);
        _blip.Sprite = BlipSprite.GangVehicle;
        _blip.Color = Color.LightBlue;

        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_cop != null && _cop.Exists()) _cop.Delete();
        if (_v != null && _v.Exists()) _v.Delete();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (_scene1 && _cop != null && _cop.Exists() && _cop.DistanceTo(MainPlayer) < 25f && MainPlayer.IsOnFoot)
        {
            GameFiber.StartNew(() =>
            {
                if (_v != null && _v.Exists() && _vV != null && _vV.Exists())
                {
                    _v.Tasks.LeaveVehicle(_vV, LeaveVehicleFlags.None);
                    _v.Health = 200;
                }
                if (_cop.Exists() && _vCop != null && _vCop.Exists())
                {
                    _cop.Tasks.LeaveVehicle(_vCop, LeaveVehicleFlags.LeaveDoorOpen);
                }
                GameFiber.Wait(200);
                if (_v != null && _v.Exists() && _cop != null && _cop.Exists())
                {
                    var vRelationshipGroup = new RelationshipGroup("V");
                    if (_v != null && _v.Exists()) _v.RelationshipGroup = vRelationshipGroup;
                    if (_cop != null && _cop.Exists()) _cop.RelationshipGroup = RelationshipGroup.Cop;
                    Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.Cop, vRelationshipGroup, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(MainPlayer.RelationshipGroup, vRelationshipGroup, Relationship.Hate);
                    if (_v != null && _v.Exists())
                    {
                        _v.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
                        _v.Tasks.FightAgainstClosestHatedTarget(1000f);
                    }
                    if (_cop != null && _cop.Exists()) _cop.Tasks.FightAgainstClosestHatedTarget(1000f);
                }
            }, "K9 Backup Required [UnitedCallouts]");
        }

        if (_scene2 && _cop != null && _cop.Exists() && _cop.DistanceTo(MainPlayer) < 25f && MainPlayer.IsOnFoot && !_notificationDisplayed && !_check)
        {
            Functions.PlayScannerAudio("ATTENTION_GENERIC_01 OFFICERS_ARRIVED_ON_SCENE");
            _check = true;

            GameFiber.StartNew(() =>
            {
                if (_cop.Exists() && _vCop != null && _vCop.Exists())
                {
                    _cop.Tasks.LeaveVehicle(_vCop, LeaveVehicleFlags.LeaveDoorOpen);
                }
                GameFiber.Wait(600);
                if (_cop.Exists() && _v != null && _v.Exists())
                {
                    NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_cop, _v, -1, true);
                }
                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
                    "~y~K9 Backup Required",
                    "K9 unit, perform a vehicle inspection. Maintain officer safety and report back once search is complete.");
                _notificationDisplayed = true;
                if (Settings.HelpMessages)
                {
                    Game.DisplayHelp("If all operations complete at this location, you are clear to conclude the traffic stop and end the call.");
                }
                else { Settings.HelpMessages = false; }
            }, "K9 Backup Required [UnitedCallouts]");
        }

        if (!_pursuitCreated && _scene3 && _cop != null && _cop.Exists() && _cop.DistanceTo(MainPlayer) < 25f && MainPlayer.IsOnFoot)
        {
            _pursuit = Functions.CreatePursuit();
            if (_v != null && _v.Exists()) Functions.AddPedToPursuit(_pursuit, _v);
            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
            _pursuitCreated = true;
            if (Settings.DetailedLogging)
            {
                Game.LogTrivial("[UnitedCallouts LOG:] K9 Backup Required Callout: Pursuit has started.");
            }
            else { Settings.DetailedLogging = false; }
        }
        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (_v != null && _v.IsDead) End();
        if (_v != null && Functions.IsPedArrested(_v)) 
        base.Process();
    }

    public override void End()
    {
        if (_cop != null && _cop.Exists()) _cop.Dismiss();
        if (_v != null && _v.Exists()) _v.Dismiss();
        if (_vV != null && _vV.Exists()) _vV.Dismiss();
        if (_vCop != null && _vCop.Exists()) _vCop.Dismiss();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~K9-Backup Required", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        if (Functions.IsPursuitStillRunning(_pursuit)) { Functions.ForceEndPursuit(_pursuit); }
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] K9 Backup Required callout ended.");
        }
        else { Settings.DetailedLogging = false; }
        base.End();
    }
}
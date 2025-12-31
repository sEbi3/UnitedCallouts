using System.Linq;

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Traffic Stop Backup Required", CalloutProbability.Medium)]
public class TrafficStopBackupRequired : Callout
{
    private static readonly string[] CopList =
        { "S_M_Y_COP_01", "S_F_Y_COP_01", "S_M_Y_SHERIFF_01", "S_F_Y_SHERIFF_01" };

    private static readonly string[] CopCars =
        { "POLICE", "POLICE2", "POLICE3", "POLICE4", "FBI", "FBI2", "SHERIFF", "SHERIFF2" };

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

    // FIXED: Removed static from all instance fields
    private Ped _cop;
    private Ped _v;
    private Vehicle _vV;
    private Vehicle _vCop;
    private Vector3 _spawnPoint;
    private Blip _blip;
    private int _callOutMessage;
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
        List<Vector3> list = spawningLocationList.Select(t => t.Item1).ToList();
        int num = LocationChooser.NearestLocationIndex(list);
        _spawnPoint = spawningLocationList[num].Item1;
        _vCop = new Vehicle(CopCars[Rndm.Next(CopCars.Length)], _spawnPoint, spawningLocationList[num].Item2);
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

        _vV = new Vehicle(VCars[Rndm.Next(VCars.Length)], _vCop.GetOffsetPosition(Vector3.RelativeFront * 9f),
            _vCop.Heading);
        _vCop.IsSirenOn = true;
        _vCop.IsSirenSilent = true;

        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS OFFICER_REQUESTING_BACKUP", _spawnPoint);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 100f);
        switch (Rndm.Next(1, 4))
        {
            case 1:
                CalloutMessage = "[UC]~w~ Traffic Stop Backup Required";
                _callOutMessage = 1;
                break;
            case 2:
                CalloutMessage = "[UC]~w~ Traffic Stop Backup Required";
                _callOutMessage = 2;
                break;
            case 3:
                CalloutMessage = "[UC]~w~ Traffic Stop Backup Required";
                _callOutMessage = 3;
                break;
        }

        CalloutPosition = _spawnPoint;
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Traffic Stop Backup Required callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Traffic Stop Backup Required",
            "~b~Dispatch:~w~ A cop needs backup for a traffic stop. Respond with ~y~Code 2~w~.");

        _cop = new(CopList[Rndm.Next(CopList.Length)], _spawnPoint, 0f)
        {
            IsPersistent = true,
            BlockPermanentEvents = true
        };
        _cop.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
        _cop.WarpIntoVehicle(_vCop, -1);
        _cop.Tasks.CruiseWithVehicle(0, VehicleDrivingFlags.None);
        Functions.IsPedACop(_cop);

        _v = new(_spawnPoint)
        {
            IsPersistent = true,
            BlockPermanentEvents = true
        };
        _v.WarpIntoVehicle(_vV, -1);
        _v.Tasks.CruiseWithVehicle(0, VehicleDrivingFlags.None);

        _blip = new(_cop)
        {
            Color = Color.LightBlue
        };
        _blip.EnableRoute(Color.Blue);
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        // FIXED: Added exists checks
        if (_cop != null && _cop.Exists()) _cop.Delete();
        if (_v != null && _v.Exists()) _v.Delete();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (_spawnPoint.DistanceTo(MainPlayer) < 25f)
        {
            // FIXED: Added null and exists checks
            if (_scene1 && !_hasBegunAttacking && _cop != null && _cop.Exists() &&
                _cop.DistanceTo(MainPlayer) < 25f && MainPlayer.IsOnFoot)
            {
                _hasBegunAttacking = true;
                GameFiber.StartNew(() =>
                {
                    if (_v != null && _v.Exists() && _vV != null && _vV.Exists())
                    {
                        _v.Tasks.LeaveVehicle(_vV, LeaveVehicleFlags.None);
                        _v.Health = 200;
                    }
                    if (_cop != null && _cop.Exists() && _vCop != null && _vCop.Exists())
                    {
                        _cop.Tasks.LeaveVehicle(_vCop, LeaveVehicleFlags.LeaveDoorOpen);
                    }
                    GameFiber.Wait(200);

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
                    GameFiber.Wait(2600);
                });
            }

            // FIXED: Added null and exists checks
            if (_scene2 && _cop != null && _cop.Exists() &&
                _cop.DistanceTo(MainPlayer) < 25f && MainPlayer.IsOnFoot &&
                !_notificationDisplayed && !_check)
            {
                _check = true;
                GameFiber.StartNew(() =>
                {
                    if (_cop != null && _cop.Exists() && _vCop != null && _vCop.Exists())
                    {
                        _cop.Tasks.LeaveVehicle(_vCop, LeaveVehicleFlags.LeaveDoorOpen);
                    }
                    GameFiber.Wait(600);
                    if (_cop != null && _cop.Exists() && _v != null && _v.Exists())
                    {
                        NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _cop, _v, -1, true);
                    }
                    Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "",
                        "Perform a traffic stop.");
                    _notificationDisplayed = true;
                    Game.DisplayHelp("Press the ~y~END~w~ key to end the Traffic Stop Backup callout.", 5000);
                    GameFiber.Wait(600);
                    Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
                        "~y~Dispatch", "Loading ~g~Informations~w~ of the ~y~LSPD Database~w~...");
                    if (_vV != null && _vV.Exists())
                    {
                        Functions.DisplayVehicleRecord(_vV, true);
                    }
                });
            }

            // FIXED: Added null and exists checks
            if (_scene3 && _cop != null && _cop.Exists() &&
                _cop.DistanceTo(MainPlayer) < 25f && MainPlayer.IsOnFoot && !_pursuitCreated)
            {
                _pursuit = Functions.CreatePursuit();
                if (_v != null && _v.Exists()) Functions.AddPedToPursuit(_pursuit, _v);
                Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                _pursuitCreated = true;
            }
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();

        // FIXED: Added null checks
        if (_v != null && _v.IsDead) End();
        if (_v != null && Functions.IsPedArrested(_v)) End();

        base.Process();
    }

    public override void End()
    {
        // FIXED: Added exists checks
        if (_cop != null && _cop.Exists()) _cop.Dismiss();
        if (_v != null && _v.Exists()) _v.Dismiss();
        if (_vV != null && _vV.Exists()) _vV.Dismiss();
        if (_vCop != null && _vCop.Exists()) _vCop.Dismiss();
        if (_blip != null && _blip.Exists()) _blip.Delete();

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Traffic Stop Backup Required", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}

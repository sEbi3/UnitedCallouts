namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] K9 Backup Required", CalloutProbability.Medium)]
public class K9BackupRequired : Callout
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

    private static Ped _cop;
    private static Ped _v;
    private static Vehicle _vV;
    private static Vehicle _vCop;
    private static Vector3 _spawnPoint;

    private static Blip _blip;

    //private static int _callOutMessage;
    private static LHandle _pursuit;
    private static bool _pursuitCreated;
    private static bool _scene1;
    private static bool _scene2;
    private static bool _scene3;
    private static bool _notificationDisplayed;
    private static bool _check;
    private static bool _hasBegunAttacking;

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

        // Not sure if this was supposed to be something more
        // switch (Rndm.Next(1, 3))
        // {
        //     case 1:
        //         CalloutMessage = "[UC]~w~ K9 Backup Required.";
        //         _callOutMessage = 1;
        //         break;
        //     case 2:
        //         CalloutMessage = "[UC]~w~ K9 Backup Required.";
        //         _callOutMessage = 2;
        //         break;
        //     case 3:
        //         CalloutMessage = "[UC]~w~ K9 Backup Required.";
        //         _callOutMessage = 3;
        //         break;
        // }
        CalloutMessage = "[UC]~w~ K9 Backup Required.";
        CalloutPosition = _spawnPoint;
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: K9BackupRequired callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~K9 Backup Required",
            "~b~Dispatch:~w~ A Cop needs a K9-Unit for a traffic stop. Respond with ~y~Code 2~w~.");

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

        _blip = new(_cop);
        _blip.EnableRoute(Color.Blue);
        _blip.Color = Color.LightBlue;
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_cop) _cop.Delete();
        if (_v) _v.Delete();
        if (_blip) _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (_spawnPoint.DistanceTo(MainPlayer) < 25f)
        {
            if (_scene1 && !_hasBegunAttacking && _cop.DistanceTo(MainPlayer) < 25f && MainPlayer.IsOnFoot)
            {
                _hasBegunAttacking = true;
                GameFiber.StartNew(() =>
                {
                    _v.Tasks.LeaveVehicle(_vV, LeaveVehicleFlags.None);
                    _v.Health = 200;
                    _cop.Tasks.LeaveVehicle(_vCop, LeaveVehicleFlags.LeaveDoorOpen);
                    GameFiber.Wait(200);
                    var viRelationshipGroup = new RelationshipGroup("V");
                    _v.RelationshipGroup = viRelationshipGroup;
                    _cop.RelationshipGroup = RelationshipGroup.Cop;
                    RelationshipGroup.Cop.SetRelationshipWith(viRelationshipGroup, Relationship.Hate);
                    _v.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
                    _v.Tasks.FightAgainstClosestHatedTarget(1000f);
                    _cop.Tasks.FightAgainstClosestHatedTarget(1000f);
                    GameFiber.Wait(2000);
                    _v.Tasks.FightAgainst(MainPlayer);
                    GameFiber.Wait(600);
                }, "K9Backup Required [UnitedCallouts]");
            }

            if (_scene2 && _cop.DistanceTo(MainPlayer) < 25f && MainPlayer.IsOnFoot &&
                !_notificationDisplayed && !_check)
            {
                _check = true;

                GameFiber.StartNew(() =>
                {
                    _cop.Tasks.LeaveVehicle(_vCop, LeaveVehicleFlags.LeaveDoorOpen);
                    GameFiber.Wait(600);
                    NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_cop, _v, -1, true);
                    Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
                        "~y~Dispatch",
                        "Go with your ~y~K9~w~ to the vehicle and let the ~y~K9~o~ search~w~ the vehicle.");
                    _notificationDisplayed = true;
                    Game.DisplayHelp("Press the ~y~END~w~ key to end the K9-Backup callout.");
                    GameFiber.Wait(600);
                    Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
                        "~y~Dispatch", "Loading ~g~Informations~w~ of the ~y~LSPD Database~w~...");
                    Functions.DisplayVehicleRecord(_vV, true);
                }, "K9Backup Required [UnitedCallouts]");
            }

            if (!_pursuitCreated && _scene3 && _cop.DistanceTo(MainPlayer) < 25f && MainPlayer.IsOnFoot)
            {
                _pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(_pursuit, _v);
                Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                _pursuitCreated = true;
            }
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (_v && _v.IsDead) End();
        if (_v && Functions.IsPedArrested(_v)) End();
        base.Process();
    }

    public override void End()
    {
        if (_cop) _cop.Dismiss();
        if (_v) _v.Dismiss();
        if (_vV) _vV.Dismiss();
        if (_vCop) _vCop.Dismiss();
        if (_blip) _blip.Delete();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~K9-Backup Required", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using System;
using System.Drawing;
using System.Collections.Generic;
using UnitedCallouts.Stuff;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] K9 Backup Required", CalloutProbability.Medium)]
    public class K9BackupRequired : Callout
    {
        private string[] CopList = new string[] { "S_M_Y_COP_01", "S_F_Y_COP_01", "S_M_Y_SHERIFF_01", "S_F_Y_SHERIFF_01" };
        private string[] CopCars = new string[] { "POLICE", "POLICE2", "POLICE3", "POLICE4", "FBI", "FBI2", "SHERIFF", "SHERIFF2" };
        private string[] VCars = new string[] {"DUKES", "BALLER", "BALLER2", "BISON", "BISON2", "BJXL", "CAVALCADE", "CHEETAH", "COGCABRIO", "ASEA", "ADDER", "FELON", "FELON2", "ZENTORNO",
                                               "WARRENER", "RAPIDGT", "INTRUDER", "FELTZER2", "FQ2", "RANCHERXL", "REBEL", "SCHWARZER", "COQUETTE", "CARBONIZZARE", "EMPEROR", "SULTAN", "EXEMPLAR", "MASSACRO",
                                               "DOMINATOR", "ASTEROPE", "PRAIRIE", "NINEF", "WASHINGTON", "CHINO", "CASCO", "INFERNUS", "ZTYPE", "DILETTANTE", "VIRGO", "F620", "PRIMO", "SULTAN", "EXEMPLAR", "F620", "FELON2", "FELON", "SENTINEL", "WINDSOR",
                                               "DOMINATOR", "DUKES", "GAUNTLET", "VIRGO", "ADDER", "BUFFALO", "ZENTORNO", "MASSACRO" };
        private Ped _Cop;
        private Ped _V;
        private Vehicle _vV;
        private Vehicle _vCop;
        private Vector3 _SpawnPoint;
        private Blip _Blip;
        private int _callOutMessage = 0;
        private LHandle _pursuit;
        private bool _pursuitCreated = false;
        private bool _Scene1 = false;
        private bool _Scene2 = false;
        private bool _Scene3 = false;
        private bool _notificationDisplayed = false;
        private bool _check = false;
        private bool _hasBegunAttacking = false;

        public override bool OnBeforeCalloutDisplayed()
        {

            Random random = new Random();
            List<Vector3> list = new List<Vector3> ();
            Tuple<Vector3, float> [] SpawningLocationList =
            {
                Tuple.Create(new Vector3(-452.2763f, 5930.209f, 32.00574f),141.1158f),
                Tuple.Create(new Vector3(2689.76f, 4379.656f, 46.21445f),123.7446f),
                Tuple.Create(new Vector3(-2848.013f, 2205.696f, 31.40776f),117.3819f),
                Tuple.Create(new Vector3(-1079.767f, -2050.001f, 12.78075f),223.3597f),
                Tuple.Create(new Vector3(1901.965f, -735.1039f, 84.55292f),125.9702f),
                Tuple.Create(new Vector3(2620.896f, 255.5361f, 97.55639f),349.3095f),
                Tuple.Create(new Vector3(1524.368f, 820.0878f, 77.10448f),332.4926f),
                Tuple.Create(new Vector3(2404.46f, 2872.158f, 39.88745f),307.5641f),
                Tuple.Create(new Vector3(2913.759f, 4148.546f, 50.26934f),16.63741f),
            };
            for(int i = 0; i < SpawningLocationList.Length; i++)
            {
                list.Add(SpawningLocationList[i].Item1);
            }
            int num = LocationChooser.nearestLocationIndex(list);
            _SpawnPoint = SpawningLocationList[num].Item1;
            _vCop = new Vehicle(CopCars[new Random().Next(CopCars.Length)], _SpawnPoint, SpawningLocationList[num].Item2);
            switch (new Random().Next(1, 3))
            {
                case 1:
                    _Scene1 = true;
                    break;
                case 2:
                    _Scene2 = true;
                    break;
                case 3:
                    _Scene3 = true;
                    break;
            }
            _vV = new Vehicle(VCars[new Random().Next((int)VCars.Length)], _vCop.GetOffsetPosition(Vector3.RelativeFront * 9f), _vCop.Heading);
            _vCop.IsSirenOn = true;
            _vCop.IsSirenSilent = true;

            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS OFFICER_REQUESTING_BACKUP", _SpawnPoint);
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 100f);
            switch (new Random().Next(1, 3))
            {
                case 1:
                    CalloutMessage = "[UC]~w~ K9 Backup Required.";
                    _callOutMessage = 1;
                    break;
                case 2:
                    CalloutMessage = "[UC]~w~ K9 Backup Required.";
                    _callOutMessage = 2;
                    break;
                case 3:
                    CalloutMessage = "[UC]~w~ K9 Backup Required.";
                    _callOutMessage = 3;
                    break;
            }
            CalloutPosition = _SpawnPoint;
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: K9BackupRequired callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~K9 Backup Required", "~b~Dispatch:~w~ A Cop needs a K9-Unit for a traffic stop. Respond with ~y~Code 2~w~.");

            _Cop = new Ped(CopList[new Random().Next((int)CopList.Length)], _SpawnPoint, 0f);
            _Cop.IsPersistent = true;
            _Cop.BlockPermanentEvents = true;
            _Cop.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
            _Cop.WarpIntoVehicle(_vCop, -1);
            _Cop.Tasks.CruiseWithVehicle(0, VehicleDrivingFlags.None);
            Functions.IsPedACop(_Cop);

            _V = new Ped(_SpawnPoint);
            _V.IsPersistent = true;
            _V.BlockPermanentEvents = true;
            _V.WarpIntoVehicle(_vV, -1);
            _V.Tasks.CruiseWithVehicle(0, VehicleDrivingFlags.None);

            _Blip = new Blip(_Cop);
            _Blip.EnableRoute(Color.Blue);
            _Blip.Color = Color.LightBlue;
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_Cop) _Cop.Delete();
            if (_V) _V.Delete();
            if (_Blip) _Blip.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_SpawnPoint.DistanceTo(Game.LocalPlayer.Character) < 25f)
                {
                    if (_Scene1 == true && _Scene3 == false && _Scene2 == false && _Cop.DistanceTo(Game.LocalPlayer.Character) < 25f && Game.LocalPlayer.Character.IsOnFoot && !_hasBegunAttacking)
                    {
                        _V.Tasks.LeaveVehicle(_vV, LeaveVehicleFlags.None);
                        _V.Health = 200;
                        _Cop.Tasks.LeaveVehicle(_vCop, LeaveVehicleFlags.LeaveDoorOpen);
                        GameFiber.Wait(200);
                        new RelationshipGroup("Cop");
                        new RelationshipGroup("V");
                        _V.RelationshipGroup = "V";
                        _Cop.RelationshipGroup = "Cop";
                        Game.SetRelationshipBetweenRelationshipGroups("Cop", "V", Relationship.Hate);
                        _V.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
                        _V.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _Cop.Tasks.FightAgainstClosestHatedTarget(1000f);
                        GameFiber.Wait(2000);
                        _V.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        _hasBegunAttacking = true;
                        GameFiber.Wait(600);
                    }
                    if (_Scene2 == true && _Scene3 == false && _Scene1 == false && _Cop.DistanceTo(Game.LocalPlayer.Character) < 25f && Game.LocalPlayer.Character.IsOnFoot && !_notificationDisplayed && !_check)
                    {
                        _Cop.Tasks.LeaveVehicle(_vCop, LeaveVehicleFlags.LeaveDoorOpen);
                        GameFiber.Wait(600);
                        NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _Cop, _V, -1, true);
                        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "Go with your ~y~K9~w~ to the vehicle and let the ~y~K9~o~ search~w~ the vehicle.");
                        _notificationDisplayed = true;
                        Game.DisplayHelp("Press the ~y~END~w~ key to end the K9-Backup callout.");
                        GameFiber.Wait(600);
                        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "Loading ~g~Informations~w~ of the ~y~LSPD Database~w~...");
                        Functions.DisplayVehicleRecord(_vV, true);
                        _check = true;
                    }
                    if (_Scene3 == true && _Scene1 == false && _Scene2 == false && _Cop.DistanceTo(Game.LocalPlayer.Character) < 25f && Game.LocalPlayer.Character.IsOnFoot)
                    {
                        _pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(_pursuit, _V);
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        _pursuitCreated = true;
                    }
                }
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (_V && _V.IsDead) End();
                if (_V && Functions.IsPedArrested(_V)) End();
            }, "K9Backup Required [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_Cop) _Cop.Dismiss();
            if (_V) _V.Dismiss();
            if (_vV) _vV.Dismiss();
            if (_vCop) _vCop.Dismiss();
            if (_Blip) _Blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~K9-Backup Required", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
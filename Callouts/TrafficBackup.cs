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
    [CalloutInfo("TrafficBackup", CalloutProbability.Medium)]
    public class TrafficBackup : Callout
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
        private Vector3 Location1;
        private Vector3 Location2;
        private Vector3 Location3;
        private Vector3 Location4;
        private Vector3 Location5;
        private Vector3 Location6;
        private Vector3 Location7;
        private Vector3 Location8;
        private Vector3 Location9;
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
            Location1 = new Vector3(-452.2763f, 5930.209f, 32.00574f);
            Location2 = new Vector3(2689.76f, 4379.656f, 46.21445f);
            Location3 = new Vector3(-2848.013f, 2205.696f, 31.40776f);
            Location4 = new Vector3(-1079.767f, -2050.001f, 12.78075f);
            Location5 = new Vector3(1901.965f, -735.1039f, 84.55292f);
            Location6 = new Vector3(2620.896f, 255.5361f, 97.55639f);
            Location7 = new Vector3(1524.368f, 820.0878f, 77.10448f);
            Location8 = new Vector3(2404.46f, 2872.158f, 39.88745f);
            Location9 = new Vector3(2913.759f, 4148.546f, 50.26934f);
            Random random = new Random();
            List<string> list = new List<string>
            {
                "Location1",
                "Location2",
                "Location3",
                "Location4",
                "Location5",
                "Location6",
                "Location7",
                "Location8",
                "Location9",
            };
            int num = random.Next(0, 9);

            if (list[num] == "Location1")
            {
                _SpawnPoint = Location1;
                _vCop = new Vehicle(CopCars[new Random().Next((int)CopCars.Length)], Location1, 141.1158f);
            }
            if (list[num] == "Location2")
            {
                _SpawnPoint = Location2;
                _vCop = new Vehicle(CopCars[new Random().Next((int)CopCars.Length)], Location2, 123.7446f);
            }
            if (list[num] == "Location3")
            {
                _SpawnPoint = Location3;
                _vCop = new Vehicle(CopCars[new Random().Next((int)CopCars.Length)], Location3, 117.3819f);
            }
            if (list[num] == "Location4")
            {
                _SpawnPoint = Location4;
                _vCop = new Vehicle(CopCars[new Random().Next((int)CopCars.Length)], Location4, 223.3597f);
            }
            if (list[num] == "Location5")
            {
                _SpawnPoint = Location5;
                _vCop = new Vehicle(CopCars[new Random().Next((int)CopCars.Length)], Location5, 125.9702f);
            }
            if (list[num] == "Location6")
            {
                _SpawnPoint = Location6;
                _vCop = new Vehicle(CopCars[new Random().Next((int)CopCars.Length)], Location6, 349.3095f);
            }
            if (list[num] == "Location7")
            {
                _SpawnPoint = Location7;
                _vCop = new Vehicle(CopCars[new Random().Next((int)CopCars.Length)], Location7, 332.4926f);
            }
            if (list[num] == "Location8")
            {
                _SpawnPoint = Location8;
                _vCop = new Vehicle(CopCars[new Random().Next((int)CopCars.Length)], Location8, 307.5641f);
            }
            if (list[num] == "Location9")
            {
                _SpawnPoint = Location9;
                _vCop = new Vehicle(CopCars[new Random().Next((int)CopCars.Length)], Location9, 16.63741f);
            }
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

            Functions.PlayScannerAudioUsingPosition("UNITS WE_HAVE CRIME_CIVILIAN_NEEDING_ASSISTANCE_02", _SpawnPoint);
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 100f);
            switch (new Random().Next(1, 3))
            {
                case 1:
                    this.CalloutMessage = "~b~Dispatch:~s~ Traffic Stop Backup";
                    _callOutMessage = 1;
                    break;
                case 2:
                    this.CalloutMessage = "~b~Dispatch:~s~ Traffic Stop Backup";
                    _callOutMessage = 2;
                    break;
                case 3:
                    this.CalloutMessage = "~b~Dispatch:~s~ Traffic Stop Backup";
                    _callOutMessage = 3;
                    break;
            }
            this.CalloutPosition = _SpawnPoint;
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: Traffic Stop Backup callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Traffic Stop Backup", "~b~Dispatch:~w~ A Cop needs backup for a traffic stop. Respond with ~y~Code 2~w~.");

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
            if (_Cop.Exists()) _Cop.Delete();
            if (_V.Exists()) _V.Delete();
            if (_Blip.Exists()) _Blip.Delete();
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
                        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "Perform a traffic stop.");
                        _notificationDisplayed = true;
                        Game.DisplayHelp("Press the ~y~END~w~ key to end the Traffic Stop Backup callout.");
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
                if (_V.IsDead || !_V.Exists() || Functions.IsPedArrested(_V)) End();
            }, "Traffic Stop Backup [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_Cop.Exists()) _Cop.Dismiss();
            if (_V.Exists()) _V.Dismiss();
            if (_vV.Exists()) _vV.Dismiss();
            if (_vCop.Exists()) _vCop.Dismiss();
            if (_Blip.Exists()) _Blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Traffic Stop Backup", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
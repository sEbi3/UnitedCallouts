using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using System;
using System.Drawing;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Jewellery Robbery In Progress", CalloutProbability.Medium)]
    public class JewelleryRobbery : Callout
    {
        private string[] AList = new string[] { "mp_g_m_pros_01", "g_m_m_chicold_01" };
        private string[] wepList = new string[] { "WEAPON_SMG", "WEAPON_PUMPSHOTGUN", "weapon_microsmg", "weapon_machinepistol", "weapon_compactrifle", "weapon_combatpistol", "weapon_pistol" };
        private Ped _A1;
        private Ped _A2;
        private Ped _A3;
        private Ped _Cop1;
        private Ped _Cop2;
        private Vehicle _CopCar1;
        private Vehicle _CopCar2;
        private Vector3 _SpawnPoint;
        private Vector3 _searcharea;
        private Vector3 _AG1_Spawnpoint;
        private Vector3 _AG2_Spawnpoint;
        private Vector3 _AG3_Spawnpoint;
        private Vector3 _Cop1_Spawnpoint;
        private Vector3 _Cop2_Spawnpoint;
        private Vector3 _Cop_Car1_Spawnpoint;
        private Vector3 _Cop_Car2_Spawnpoint;
        private bool _Scene1 = false;
        private bool _Scene2 = false;
        private bool _Scene3 = false;
        private Blip _Blip;
        private LHandle _pursuit;
        private bool _pursuitCreated = false;
        private bool _notificationDisplayed = false;
        private bool _check = false;
        private bool _hasBegunAttacking = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _SpawnPoint = new Vector3(-625.7944f, -229.267f, 38.05706f);
            _AG1_Spawnpoint = new Vector3(-625.7944f, -229.267f, 38.05706f);
            _AG2_Spawnpoint = new Vector3(-621.2017f, -234.7889f, 38.05706f);
            _AG3_Spawnpoint = new Vector3(-619.4506f, -229.297f, 38.05701f);
            _Cop1_Spawnpoint = new Vector3(-644.9532f, -231.6783f, 37.74535f);
            _Cop2_Spawnpoint = new Vector3(-646.092f, -234.0365f, 37.75615f);
            _Cop_Car1_Spawnpoint = new Vector3(-645.6291f, -232.7989f, 37.35386f);
            _Cop_Car2_Spawnpoint = new Vector3(-626.4602f, -257.1244f, 38.29225f);

            _A1 = new Ped(AList[new Random().Next((int)AList.Length)], _AG1_Spawnpoint, 144.7047f);
            _A2 = new Ped(AList[new Random().Next((int)AList.Length)], _AG2_Spawnpoint, 105.2843f);
            _A3 = new Ped(AList[new Random().Next((int)AList.Length)], _AG3_Spawnpoint, 120.7921f);
            _Cop1 = new Ped("S_M_Y_COP_01", _Cop1_Spawnpoint, 247.3842f);
            _Cop2 = new Ped("S_M_Y_COP_01", _Cop2_Spawnpoint, 253.8056f);
            _CopCar1 = new Vehicle("POLICE", _Cop_Car1_Spawnpoint, 245.2429f);
            _CopCar2 = new Vehicle("RIOT", _Cop_Car2_Spawnpoint, 14.0995f);
            _Cop1.WarpIntoVehicle(_CopCar1, 1);
            _Cop2.WarpIntoVehicle(_CopCar1, 2);
            _CopCar1.IsSirenOn = true;
            _CopCar2.IsSirenOn = true;
            _CopCar1.IsSirenSilent = true;
            _CopCar2.IsSirenSilent = true;
            _Cop1.Tasks.LeaveVehicle(_CopCar1, LeaveVehicleFlags.LeaveDoorOpen);
            _Cop2.Tasks.LeaveVehicle(_CopCar1, LeaveVehicleFlags.LeaveDoorOpen);
            _Cop1.Tasks.Clear();
            _Cop2.Tasks.Clear();

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

            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 100f);
            CalloutMessage = "[UC]~w~ Reports of a jewellery robbery in progress.";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("DISP_ATTENTION_UNIT WE_HAVE CRIME_ROBBERY IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: JewelryRobbery callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Jewelry Robbery", "~b~Dispatch:~w~ The units on scene needs backup at the jewellery. Respond with ~r~Code 3~w~.");

            _A1.IsPersistent = true;
            _A1.BlockPermanentEvents = true;
            _A1.Armor = 200;
            _A1.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);

            _A2.IsPersistent = true;
            _A2.BlockPermanentEvents = true;
            _A2.Armor = 200;
            _A2.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);

            _A3.IsPersistent = true;
            _A3.BlockPermanentEvents = true;
            _A3.Armor = 200;
            _A3.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);

            _Cop1.IsPersistent = true;
            _Cop1.BlockPermanentEvents = true;
            _Cop1.Armor = 200;
            _Cop1.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
            Functions.IsPedACop(_Cop1);

            _Cop2.IsPersistent = true;
            _Cop2.BlockPermanentEvents = true;
            _Cop2.Armor = 200;
            _Cop2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
            Functions.IsPedACop(_Cop2);

            _searcharea = _SpawnPoint.Around2D(1f, 2f);
            _Blip = new Blip(_searcharea, 20f);
            _Blip.EnableRoute(Color.Yellow);
            _Blip.Color = Color.Yellow;
            _Blip.Alpha = 1f;

            new RelationshipGroup("A");
            new RelationshipGroup("COP");
            _A1.RelationshipGroup = "A";
            _A2.RelationshipGroup = "A";
            _A3.RelationshipGroup = "A";
            _Cop1.RelationshipGroup = "COP";
            _Cop2.RelationshipGroup = "COP";
            Game.LocalPlayer.Character.RelationshipGroup = "COP";
            Game.SetRelationshipBetweenRelationshipGroups("A", "COP", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("COP", "A", Relationship.Hate);

            NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _Cop1, _A3, -1, true);
            NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _Cop2, _A3, -1, true);
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_A1.Exists()) _A1.Delete();
            if (_A2.Exists()) _A2.Delete();
            if (_A3.Exists()) _A3.Delete();
            if (_Cop1.Exists()) _Cop1.Delete();
            if (_Cop2.Exists()) _Cop2.Delete();
            if (_CopCar1.Exists()) _CopCar1.Delete();
            if (_CopCar2.Exists()) _CopCar2.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_SpawnPoint.DistanceTo(Game.LocalPlayer.Character) < 22f)
                {
                    if (_Scene1 == true && !_hasBegunAttacking)
                    {
                        _A1.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _A2.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _A3.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _Cop1.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _Cop2.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _Cop2.Tasks.FightAgainst(_A1);
                        _Cop1.Tasks.FightAgainst(_A2);
                        _A1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        _A2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        _A3.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        GameFiber.Wait(2000);
                        _hasBegunAttacking = true;
                    }
                    else if (_Scene2 == true && !_notificationDisplayed && !_check)
                    {
                        _A1.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _A2.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _A3.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _Cop1.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _Cop2.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _Cop2.Tasks.FightAgainst(_A1);
                        _Cop1.Tasks.FightAgainst(_A2);
                        _A1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        _A2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        _A3.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        GameFiber.Wait(2000);
                        _hasBegunAttacking = true;
                    }
                    else if (_Scene3 == true && !_pursuitCreated)
                    {
                        _Cop2.Tasks.FightAgainst(_A1);
                        _Cop1.Tasks.FightAgainst(_A2);
                        _pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(_pursuit, _A1);
                        Functions.AddPedToPursuit(_pursuit, _A2);
                        Functions.AddPedToPursuit(_pursuit, _A3);
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        _pursuitCreated = true;
                    }
                }
                if (_A1.Exists() && _A1.IsDead && _A2.Exists() && _A2.IsDead && _A3.Exists() && _A3.IsDead) End();
                if (_A1.Exists() && Functions.IsPedArrested(_A1) && _A2.Exists() && Functions.IsPedArrested(_A2) && _A3.Exists() && Functions.IsPedArrested(_A3)) End();
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
            }, "Jewelry Robbery [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_A1.Exists()) _A1.Dismiss();
            if (_A2.Exists()) _A2.Dismiss();
            if (_A3.Exists()) _A3.Dismiss();
            if (_CopCar1.Exists()) _CopCar1.Dismiss();
            if (_CopCar2.Exists()) _CopCar2.Dismiss();
            if (_Blip.Exists()) _Blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Jewelry Robbery", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}

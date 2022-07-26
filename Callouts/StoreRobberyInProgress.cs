using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Store Robbery In Progress", CalloutProbability.Medium)]
    public class StoreRobberyInProgress : Callout
    {
        private string[] AList = new string[] { "mp_g_m_pros_01", "g_m_m_chicold_01" };
        private string[] wepList = new string[] { "WEAPON_PISTOL", "WEAPON_SMG", "WEAPON_MACHINEPISTOL", "WEAPON_PUMPSHOTGUN", "weapon_heavypistol", "weapon_minismg" };
        private string[] VList = new string[] { "s_m_m_ammucountry", "mp_m_shopkeep_01", "s_f_m_sweatshop_01" };
        private Ped _A1;
        private Ped _A2;
        private Ped _V;
        private Vector3 _SpawnPoint;
        private Vector3 _searcharea;
        private Blip _Blip;
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
            Tuple<Vector3, float, Vector3, float, Vector3, float>[] SpawningLocationList = {
                    Tuple.Create(new Vector3(73.87572f, -1392.849f, 29.37613f),263.9599f, new Vector3(75.91751f, -1391.883f, 29.37615f), 114.3616f,new Vector3(76.16069f, -1389.982f, 29.37615f), 19.93308f),
                    Tuple.Create(new Vector3(427.145f, -806.8593f, 29.49114f),97.54604f,new Vector3(425.0366f, -807.4084f, 29.49113f), 291.1364f,new Vector3(424.7376f, -809.5753f, 29.49224f), 217.1934f),
                    Tuple.Create(new Vector3(-822.653f, -1071.912f, 11.32811f),208.5814f, new Vector3(-820.9958f, -1073.383f, 11.32811f), 43.43138f,new Vector3(-819.3976f, -1072.761f, 11.32906f), 335.4849f),
                    Tuple.Create(new Vector3(1695.573f, 4822.746f, 42.06311f),95.36308f,new Vector3(1693.365f, 4821.716f, 42.06312f), 289.7472f,new Vector3(1693.838f, 4819.305f, 42.0641f), 215.6228f),
                    Tuple.Create(new Vector3(-1102.171f, 2711.92f, 19.10787f),244.3523f,new Vector3(-1100.294f, 2710.429f, 19.10785f), 48.32264f,new Vector3(-1098.755f, 2712.339f, 19.10868f), 338.6742f),
                    Tuple.Create(new Vector3(1197.127f, 2711.719f, 38.22263f),155.001f,new Vector3(1197.668f, 2709.616f, 38.2226f), 13.32392f,new Vector3(1200.074f, 2709.42f, 38.22372f), 318.5346f),
                    Tuple.Create(new Vector3(5.792656f, 6511.06f, 31.87785f),53.38403f, new Vector3(3.368677f, 6512.238f, 31.87785f), 242.7346f,new Vector3(1.847083f, 6510.697f, 31.87863f), 164.7863f),
                    Tuple.Create(new Vector3(372.285f, 326.7229f, 103.5664f),245.4788f, new Vector3(374.295f, 325.8951f, 103.5664f), 71.65144f,new Vector3(374.8215f, 327.8206f, 103.5664f), 67.57134f),
                    Tuple.Create(new Vector3(1164.891f, -321.9834f, 69.20512f),109.593f,new Vector3(1163.107f, -322.1323f, 69.20507f), 279.6259f,new Vector3(1163.093f, -324.2025f, 69.20507f), 279.2135f),
                    Tuple.Create(new Vector3(1133.908f, -981.8345f, 46.41584f),277.1535f,new Vector3(1135.899f, -981.3351f, 46.41584f), 106.3927f,new Vector3(1136.195f, -982.9218f, 46.41584f), 84.65549f),
            };
            int num = random.Next(0, SpawningLocationList.Length);
            _SpawnPoint = SpawningLocationList[num].Item1;
            _V = new Ped(VList[new Random().Next(VList.Length)], _SpawnPoint, SpawningLocationList[num].Item2);
            _A1 = new Ped(AList[new Random().Next(AList.Length)], SpawningLocationList[num].Item3, SpawningLocationList[num].Item4);
            _A2 = new Ped(AList[new Random().Next(AList.Length)], SpawningLocationList[num].Item5, SpawningLocationList[num].Item6);
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
            CalloutMessage = "[UC]~w~ Reports of a Store Robbery in Progress.";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("DISP_ATTENTION_UNIT WE_HAVE CRIME_ROBBERY IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: Store Robbery In Progress callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Store Robbery In Progress", "~b~Dispatch:~w~ Someone called the police because of a store robbery. Respond with ~r~Code 3~w~.");
            _V.IsPersistent = true;
            _V.BlockPermanentEvents = true;

            _A1.IsPersistent = true;
            _A1.BlockPermanentEvents = true;
            _A1.Armor = 200;
            _A1.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);
            _A1.Face(_V);

            _A2.IsPersistent = true;
            _A2.BlockPermanentEvents = true;
            _A2.Armor = 200;
            _A2.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);
            _A2.Face(_V);

            _searcharea = _SpawnPoint.Around2D(1f, 2f);
            _Blip = new Blip(_searcharea, 20f);
            _Blip.EnableRoute(Color.Yellow);
            _Blip.Color = Color.Yellow;
            _Blip.Alpha = 0.5f;
            NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _A1, _V, -1, true);
            NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _A2, _V, -1, true);

            new RelationshipGroup("A");
            new RelationshipGroup("V");
            _V.RelationshipGroup = "V";
            _A1.RelationshipGroup = "A";
            _A2.RelationshipGroup = "A";
            _V.Tasks.PutHandsUp(-1, _A1);
            Game.LocalPlayer.Character.RelationshipGroup = "V";
            Game.SetRelationshipBetweenRelationshipGroups("A", "V", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("V", "A", Relationship.Hate);

            if (Settings.ActivateAIBackup)
            {
                Functions.RequestBackup(_SpawnPoint, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.SwatTeam);
                Functions.RequestBackup(_SpawnPoint, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
            }
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_A1) _A1.Delete();
            if (_A2) _A2.Delete();
            if (_V) _V.Delete();
            if (_Blip) _Blip.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_A1.DistanceTo(Game.LocalPlayer.Character) < 25f)
                {
                    if (_Scene1 == true && !_hasBegunAttacking)
                    {
                        _A1.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _A2.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _A1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        _A2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        GameFiber.Wait(2000);
                        _hasBegunAttacking = true;
                    }
                    else if (_Scene2 == true && !_notificationDisplayed && !_check)
                    {
                        _A1.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _A2.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _A1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        _A2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        GameFiber.Wait(2000);
                        _hasBegunAttacking = true;
                    }
                    else if (_Scene3 == true && !_pursuitCreated)
                    {
                        _pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(_pursuit, _A1);
                        Functions.AddPedToPursuit(_pursuit, _A2);
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        _pursuitCreated = true;
                    }
                }
                if (_A1 && _A1.IsDead && _A2 && _A2.IsDead) End();
                if (_A1 && Functions.IsPedArrested(_A1) && _A2 && Functions.IsPedArrested(_A2)) End();
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
            }, "Store Robbery In Progress [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_A1) _A1.Dismiss();
            if (_A2) _A2.Dismiss();
            if (_V) _V.Dismiss();
            if (_Blip) _Blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Store Robbery In Progress", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
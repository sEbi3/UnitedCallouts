using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Hostage Situation Reported", CalloutProbability.Medium)]
    public class HostageSituationReported : Callout
    {
        private string[] wepList = new string[] { "weapon_bullpuprifle_mk2", "WEAPON_SMG", "WEAPON_MACHINEPISTOL", "WEAPON_PUMPSHOTGUN" };
        private Ped _AG1;
        private Ped _AG2;
        private Ped _V1;
        private Ped _V2;
        private Ped _V3;
        private Ped _V4;
        private Vector3 _searcharea;
        private Vector3 _SpawnPoint;
        private Blip _SpawnLocation;
        private bool _Scene1 = false;
        private bool _Scene2 = false;
        private bool _notificationDisplayed = false;

        public override bool OnBeforeCalloutDisplayed()
        {

            Random random = new Random();
            List<Vector3> list = new List<Vector3>
            {
                new Vector3(976.6871f, -96.42852f, 74.84537f),
                new Vector3(1250.002f, -3014.48f, 9.319259f),

        };
            int num = random.Next(0, list.Count);
            _SpawnPoint = list[num];
            _AG1 = new Ped("mp_g_m_pros_01", _SpawnPoint, 0f);
            _AG2 = new Ped("mp_g_m_pros_01", _SpawnPoint, 0f);
            _V1 = new Ped(_SpawnPoint, 0f);
            _V2 = new Ped(_SpawnPoint, 0f);
            _V3 = new Ped(_SpawnPoint, 0f);
            _V4 = new Ped(_SpawnPoint, 0f);
            switch (new Random().Next(1, 3))
            {
                case 1:
                    _Scene1 = true;
                    break;
                case 2:
                    _Scene2 = true;
                    break;
                case 3:
                    _Scene2 = true;
                    break;
            }
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 100f);
            AddMinimumDistanceCheck(10f, _SpawnPoint);
            CalloutMessage = "[UC]~w~ Hostage Situation Reported";
            CalloutPosition = _SpawnPoint;
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            _AG1.IsPersistent = true;
            _AG1.BlockPermanentEvents = true;
            _AG1.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);
            _AG1.Health = 200;
            _AG1.Armor = 300;

            _AG2.IsPersistent = true;
            _AG2.BlockPermanentEvents = true;
            _AG2.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);
            _AG2.Health = 200;
            _AG2.Armor = 300;

            _V1.BlockPermanentEvents = true;
            _V2.BlockPermanentEvents = true;
            _V3.BlockPermanentEvents = true;
            _V4.BlockPermanentEvents = true;

            _V1.IsPersistent = true;
            _V2.IsPersistent = true;
            _V3.IsPersistent = true;
            _V4.IsPersistent = true;

            _V1.Tasks.PlayAnimation("random@arrests@busted", "idle_a", 8.0F, AnimationFlags.Loop);
            _V2.Tasks.PlayAnimation("random@arrests@busted", "idle_a", 8.0F, AnimationFlags.Loop);
            _V3.Tasks.PlayAnimation("random@arrests@busted", "idle_a", 8.0F, AnimationFlags.Loop);
            _V4.Tasks.PlayAnimation("random@arrests@busted", "idle_a", 8.0F, AnimationFlags.Loop);

            NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _AG1, _V1, -1, true);
            NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _AG2, _V2, -1, true);

            new RelationshipGroup("AG");
            new RelationshipGroup("VI");
            _AG1.RelationshipGroup = "AG";
            _AG2.RelationshipGroup = "AG";
            _V1.RelationshipGroup = "VI";
            _V2.RelationshipGroup = "VI";
            _V3.RelationshipGroup = "VI";
            _V4.RelationshipGroup = "VI";
            Game.SetRelationshipBetweenRelationshipGroups("AG", "VI", Relationship.Hate);

            _searcharea = _SpawnPoint.Around2D(1f, 2f);
            _SpawnLocation = new Blip(_searcharea, 40f);
            _SpawnLocation.EnableRoute(Color.Yellow);
            _SpawnLocation.Color = Color.Yellow;
            _SpawnLocation.Alpha = 0.5f;

            Functions.PlayScannerAudioUsingPosition("DISP_ATTENTION_UNIT WE_HAVE CRIME_ROBBERY IN_OR_ON_POSITION", _SpawnPoint);
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~FIB", "The police department called for the ~y~Hostage Rescue Team~w~ to rescue all ~o~hostages~w~.");
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_AG1) _AG1.Delete();
            if (_AG2) _AG2.Delete();
            if (_V1) _V1.Delete();
            if (_V2) _V2.Delete();
            if (_V3) _V3.Delete();
            if (_V4) _V4.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_SpawnPoint.DistanceTo(Game.LocalPlayer.Character) < 25f && Game.LocalPlayer.Character.IsOnFoot && !_notificationDisplayed)
                {
                    if (_SpawnLocation) _SpawnLocation.Delete();

                    Game.DisplayHelp("The ~y~Hostage Rescue Team~w~ is ~g~on scene~w~. ");
                    _notificationDisplayed = true;
                }
                if (_AG1.DistanceTo(Game.LocalPlayer.Character) < 14f)
                {
                    if (_Scene1 == true && _Scene2 == false && _AG1.DistanceTo(Game.LocalPlayer.Character) < 18f)
                    {
                        Game.DisplaySubtitle("~y~Criminal~w~ shhh....I am hearing steps!");

                        _AG1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        _AG2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                    if (_Scene2 == true && _Scene1 == false && Game.LocalPlayer.Character.DistanceTo(_AG1) < 18f)
                    {
                        _AG1.Tasks.FightAgainstClosestHatedTarget(1000f);
                        _AG2.Tasks.FightAgainstClosestHatedTarget(1000f);

                        _AG1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        _AG2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                }
                if (_AG1 && _AG1.IsDead && _AG2 && _AG2.IsDead) End();
                if (_AG1 && Functions.IsPedArrested(_AG1) && _AG2 && Functions.IsPedArrested(_AG2)) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (Game.LocalPlayer.Character.IsDead) End();
            }, "Hostage Situation Reported [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_AG1) _AG1.Dismiss();
            if (_AG2) _AG2.Dismiss();
            if (_V1) _V1.Dismiss();
            if (_V2) _V2.Dismiss();
            if (_V3) _V3.Dismiss();
            if (_V4) _V4.Dismiss();
            if (_SpawnLocation) _SpawnLocation.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Hostage Situation Reported", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}

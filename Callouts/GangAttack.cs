using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using System.Collections.Generic;
using System;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Reports of a Gang Shootout", CalloutProbability.Medium)]
    class GangAttack : Callout
    {
        private Ped _AG1;
        private Ped _AG2;
        private Ped _AG3;
        private Ped _AG4;
        private Ped _AG5;
        private Ped _AG6;
        private Vector3 _SpawnPoint;
        private Blip _Blip;
        private Blip _Blip2;
        private Blip _Blip3;
        private Blip _Blip4;
        private Blip _Blip5;
        private Blip _Blip6;
        private Vector3 _Location1;
        private Vector3 _Location2;
        private Vector3 _Location3;
        private Vector3 _Location4;
        private Vector3 _Location5;
        private bool _hasBegunAttacking = false; 

        public override bool OnBeforeCalloutDisplayed()
        {
            _Location1 = new Vector3(105.1732f, -1937.076f, 20.41693f);
            _Location2 = new Vector3(-183.6035f, -1669.903f, 33.10927f);
            _Location3 = new Vector3(327.7954f, -2034.417f, 20.5504f);
            _Location4 = new Vector3(-743.142f, -923.304f, 18.68627f);
            _Location5 = new Vector3(-133.8996f, -1628.18f, 31.89049f);
            Random random = new Random();
            List<string> list = new List<string>
            {
                "Location1",
                "Location2",
                "Location3",
                "Location4",
                "Location5",
            };
            int num = random.Next(0, 5);
            if (list[num] == "Location1")
            {
                _SpawnPoint = _Location1;
            }
            if (list[num] == "Location2")
            {
                _SpawnPoint = _Location2;
            }
            if (list[num] == "Location3")
            {
                _SpawnPoint = _Location3;
            }
            if (list[num] == "Location4")
            {
                _SpawnPoint = _Location4;
            }
            if (list[num] == "Location5")
            {
                _SpawnPoint = _Location5;
            }
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 70f);
            CalloutMessage = "[UC]~w~ Reports of a gang shootout.";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_01 CITIZENS_REPORT_01 GANG_RELATED_VIOLENCE UNITS_RESPOND_CODE_99_01", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: GangAttack callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Gang Attack", "~b~Dispatch: ~w~The gang members are marked on the map. Respond with ~r~Code 3");

            _AG1 = new Ped("g_m_y_ballasout_01", Stuff.Vector3Extension.ExtensionAround(_SpawnPoint, 20f), 0f);
            _AG2 = new Ped("g_m_y_ballasout_01", Stuff.Vector3Extension.ExtensionAround(_SpawnPoint, 30f), 0f);
            _AG3 = new Ped("g_m_y_ballasout_01", Stuff.Vector3Extension.ExtensionAround(_SpawnPoint, 22f), 0f);
            _AG4 = new Ped("g_m_y_famca_01", Stuff.Vector3Extension.ExtensionAround(_SpawnPoint, 24f), 0f);
            _AG5 = new Ped("g_m_y_famca_01", Stuff.Vector3Extension.ExtensionAround(_SpawnPoint, 26f), 0f);
            _AG6 = new Ped("g_m_y_famca_01", Stuff.Vector3Extension.ExtensionAround(_SpawnPoint, 28f), 0f);

            _AG1.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
            _AG2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
            _AG3.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
            _AG4.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
            _AG5.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
            _AG6.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);

            _Blip = _AG1.AttachBlip();
            _Blip2 = _AG2.AttachBlip();
            _Blip3 = _AG3.AttachBlip();
            _Blip4 = _AG4.AttachBlip();
            _Blip5 = _AG5.AttachBlip();
            _Blip6 = _AG6.AttachBlip();
            _Blip.EnableRoute(Color.Yellow);
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_Blip.Exists()) _Blip.Delete();
            if (_Blip2.Exists()) _Blip2.Delete();
            if (_Blip3.Exists()) _Blip3.Delete();
            if (_Blip4.Exists()) _Blip4.Delete();
            if (_Blip5.Exists()) _Blip5.Delete();
            if (_Blip6.Exists()) _Blip6.Delete();
            if (_AG1.Exists()) _AG1.Delete();
            if (_AG2.Exists()) _AG2.Delete();
            if (_AG3.Exists()) _AG3.Delete();
            if (_AG4.Exists()) _AG4.Delete();
            if (_AG5.Exists()) _AG5.Delete();
            if (_AG6.Exists()) _AG6.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (Game.LocalPlayer.Character.DistanceTo(_SpawnPoint) < 90f && !_hasBegunAttacking)
                {
                    new RelationshipGroup("BALLAS");
                    new RelationshipGroup("GROVE");
                    _AG1.RelationshipGroup = "BALLAS";
                    _AG2.RelationshipGroup = "BALLAS";
                    _AG3.RelationshipGroup = "BALLAS";
                    _AG4.RelationshipGroup = "GROVE";
                    _AG5.RelationshipGroup = "GROVE";
                    _AG6.RelationshipGroup = "GROVE";
                    Game.SetRelationshipBetweenRelationshipGroups("BALLAS", "GROVE", Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups("GROVE", "BALLAS", Relationship.Hate);
                    _AG1.Tasks.FightAgainstClosestHatedTarget(1000f);
                    _AG2.Tasks.FightAgainstClosestHatedTarget(1000f);
                    _AG3.Tasks.FightAgainstClosestHatedTarget(1000f);
                    _AG4.Tasks.FightAgainstClosestHatedTarget(1000f);
                    _AG5.Tasks.FightAgainstClosestHatedTarget(1000f);
                    _AG6.Tasks.FightAgainstClosestHatedTarget(1000f);
                    _hasBegunAttacking = true;
                    GameFiber.Sleep(5000);
                }
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (_AG1.Exists() && _AG1.IsDead && _AG2.Exists() && _AG2.IsDead && _AG3.Exists() && _AG3.IsDead && _AG4.Exists() && _AG4.IsDead && _AG5.Exists() && _AG5.IsDead && _AG6.Exists() && _AG6.IsDead) End();
                if (_AG1.Exists() && Functions.IsPedArrested(_AG1) && _AG2.Exists() && Functions.IsPedArrested(_AG2) && _AG3.Exists() && Functions.IsPedArrested(_AG3) && _AG4.Exists() && Functions.IsPedArrested(_AG4) && _AG5.Exists() && Functions.IsPedArrested(_AG5) && _AG6.Exists() && Functions.IsPedArrested(_AG6)) End();
            }, "GangAttack [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_Blip.Exists()) _Blip.Delete();
            if (_Blip2.Exists()) _Blip2.Delete();
            if (_Blip3.Exists()) _Blip3.Delete();
            if (_Blip4.Exists()) _Blip4.Delete();
            if (_Blip5.Exists()) _Blip5.Delete();
            if (_Blip6.Exists()) _Blip6.Delete();
            if (_AG1.Exists()) _AG1.Dismiss();
            if (_AG2.Exists()) _AG2.Dismiss();
            if (_AG3.Exists()) _AG3.Dismiss();
            if (_AG4.Exists()) _AG4.Dismiss();
            if (_AG5.Exists()) _AG5.Dismiss();
            if (_AG6.Exists()) _AG6.Dismiss();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Gang Attack", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
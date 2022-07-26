using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using System.Collections.Generic;
using System;
using UnitedCallouts.Stuff;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Reports of a Gang Shootout", CalloutProbability.Medium)]
    public class GangShootout : Callout
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
        private bool _hasBegunAttacking = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            Random random = new Random();
            List<Vector3> list = new List<Vector3>
            {
                new Vector3(105.1732f, -1937.076f, 20.41693f),
                new Vector3(-183.6035f, -1669.903f, 33.10927f),
                new Vector3(327.7954f, -2034.417f, 20.5504f),
                new Vector3(-743.142f, -923.304f, 18.68627f),
                new Vector3(1111.735f, -1610.99f, 4.408495f),

            };
            int num = random.Next(0, list.Count);
            _SpawnPoint = list[num];
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 70f);
            CalloutMessage = "[UC]~w~ Reports of a Gang Shootout.";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_01 CITIZENS_REPORT_01 GANG_RELATED_VIOLENCE UNITS_RESPOND_CODE_99_01", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: Gang Shootout callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Gang Shootout", "~b~Dispatch: ~w~The gang members are marked on the map. Respond with ~r~Code 3");

            _AG1 = new Ped("g_m_y_ballasout_01", Vector3Extension.ExtensionAround(_SpawnPoint, 20f), 0f);
            _AG2 = new Ped("g_m_y_ballasout_01", Vector3Extension.ExtensionAround(_SpawnPoint, 30f), 0f);
            _AG3 = new Ped("g_m_y_ballasout_01", Vector3Extension.ExtensionAround(_SpawnPoint, 22f), 0f);
            _AG4 = new Ped("g_m_y_famca_01", Vector3Extension.ExtensionAround(_SpawnPoint, 24f), 0f);
            _AG5 = new Ped("g_m_y_famca_01", Vector3Extension.ExtensionAround(_SpawnPoint, 26f), 0f);
            _AG6 = new Ped("g_m_y_famca_01", Vector3Extension.ExtensionAround(_SpawnPoint, 28f), 0f);

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

            if (Settings.ActivateAIBackup)
            {
                Functions.RequestBackup(_SpawnPoint, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.SwatTeam);
                Functions.RequestBackup(_SpawnPoint, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
            }
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_Blip) _Blip.Delete();
            if (_Blip2) _Blip2.Delete();
            if (_Blip3) _Blip3.Delete();
            if (_Blip4) _Blip4.Delete();
            if (_Blip5) _Blip5.Delete();
            if (_Blip6) _Blip6.Delete();
            if (_AG1) _AG1.Delete();
            if (_AG2) _AG2.Delete();
            if (_AG3) _AG3.Delete();
            if (_AG4) _AG4.Delete();
            if (_AG5) _AG5.Delete();
            if (_AG6) _AG6.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (Game.LocalPlayer.Character.DistanceTo(_SpawnPoint) < 100f && !_hasBegunAttacking)
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
                if (_AG1 && _AG1.IsDead && _AG2 && _AG2.IsDead && _AG3 && _AG3.IsDead && _AG4 && _AG4.IsDead && _AG5 && _AG5.IsDead && _AG6 && _AG6.IsDead) End();
                if (_AG1 && Functions.IsPedArrested(_AG1) && _AG2 && Functions.IsPedArrested(_AG2) && _AG3 && Functions.IsPedArrested(_AG3) && _AG4 && Functions.IsPedArrested(_AG4) && _AG5 && Functions.IsPedArrested(_AG5) && _AG6 && Functions.IsPedArrested(_AG6)) End();
            }, "Gang Shootout [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_Blip) _Blip.Delete();
            if (_Blip2) _Blip2.Delete();
            if (_Blip3) _Blip3.Delete();
            if (_Blip4) _Blip4.Delete();
            if (_Blip5) _Blip5.Delete();
            if (_Blip6) _Blip6.Delete();
            if (_AG1) _AG1.Dismiss();
            if (_AG2) _AG2.Dismiss();
            if (_AG3) _AG3.Dismiss();
            if (_AG4) _AG4.Dismiss();
            if (_AG5) _AG5.Dismiss();
            if (_AG6) _AG6.Dismiss();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Gang Shootout", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
            // Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
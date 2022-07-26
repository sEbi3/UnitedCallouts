using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using System.Collections.Generic;
using System;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Public Peace Disturbance", CalloutProbability.Medium)]
    class PublicPeaceDisturbance : Callout
    {
        private Ped _AG1;
        private Ped _AG2;
        private Vector3 _SpawnPoint;
        private Blip _Blip;
        private Blip _Blip2;
        private bool _hasBegunAttacking = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            Random random = new Random();
            List<Vector3> list = new List<Vector3>
            {
                new Vector3(-227.4819f, 285.4045f, 91.66596f),
                new Vector3(-358.5807f, 265.6691f, 84.38922f),
                new Vector3(-434.0634f, 257.6701f, 82.99319f),
                new Vector3(1986.708f, 3050.588f, 47.2151f),
                new Vector3(99.72031f, 215.0221f, 107.9258f),
                new Vector3(99.72031f, 215.0221f, 107.9258f), // Remove
                new Vector3(-292.416f, -303.66f, 10.06316f),
                new Vector3(-199.3456f, -786.0698f, 30.45403f),
                new Vector3(-708.6214f, -913.0787f, 19.21559f),
                new Vector3(-828.1153f, -1074.215f, 11.32811f),
            };
            int num = random.Next(0, list.Count);
            _SpawnPoint = list[num];
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 70f);
            CalloutMessage = "[UC]~w~ Reports of a Public Peace Disturbance in Progress.";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_CIVILIAN_NEEDING_ASSISTANCE_IN", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: Public Peace Disturbance callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Public Peace Disturbance", "~b~Dispatch: ~w~Stop the two People who are fighting! Respond with ~r~Code 3");

            _AG1 = new Ped(_SpawnPoint);
            _AG2 = new Ped(_SpawnPoint);
            _AG1.Armor = 300;
            _AG2.Armor = 300;
            _AG1.Health = 300;
            _AG2.Health = 300;

            _Blip = _AG1.AttachBlip();
            _Blip2 = _AG2.AttachBlip();
            _Blip.EnableRoute(Color.Yellow);

            if (Settings.ActivateAIBackup)
            {
                Functions.RequestBackup(_SpawnPoint, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
            }
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_Blip) _Blip.Delete();
            if (_Blip2) _Blip2.Delete();
            if (_AG1) _AG1.Delete();
            if (_AG2) _AG2.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (Game.LocalPlayer.Character.DistanceTo(_SpawnPoint) < 80f & !_hasBegunAttacking)
                {
                    new RelationshipGroup("BALLAS");
                    new RelationshipGroup("GROVE");
                    _AG1.RelationshipGroup = "BALLAS";
                    _AG2.RelationshipGroup = "GROVE";
                    Game.SetRelationshipBetweenRelationshipGroups("BALLAS", "GROVE", Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups("GROVE", "BALLAS", Relationship.Hate);
                    _AG1.Tasks.FightAgainstClosestHatedTarget(1000f);
                    _AG2.Tasks.FightAgainstClosestHatedTarget(1000f);
                    _hasBegunAttacking = true;
                    GameFiber.Sleep(5000);
                }
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (_AG1 && _AG1.IsDead && _Blip) _Blip.Delete();
                if (_AG2 && _AG2.IsDead && _Blip2) _Blip2.Delete();
                if (_AG1 && _AG1.IsDead && _AG2 && _AG2.IsDead) End();
                if (Functions.IsPedArrested(_AG1) && Functions.IsPedArrested(_AG2)) End();
            }, "Fighting [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_Blip) _Blip.Delete();
            if (_Blip2) _Blip2.Delete();
            if (_AG1) _AG1.Dismiss();
            if (_AG2) _AG2.Dismiss();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Public Peace Disturbance", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
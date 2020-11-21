using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using System;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("Fighting in progress", CalloutProbability.Medium)]
    class Fighting : Callout
    {
        private Ped _AG1;
        private Ped _AG2;
        private Vector3 _SpawnPoint;
        private Blip _Blip;
        private Blip _Blip2;
        private bool _hasBegunAttacking = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 70f);
            CalloutMessage = "~b~Dispatch:~s~ Fighting in progress";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_CIVILIAN_NEEDING_ASSISTANCE_IN", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: Fighting callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Fighting in progress", "~b~Dispatch: ~w~Stop the two People who are fighting! Respond with ~r~Code 3");

            _AG1 = new Ped(_SpawnPoint);
            _AG2 = new Ped(_SpawnPoint);
            _AG1.Armor = 300;
            _AG2.Armor = 300;
            _AG1.Health = 300;
            _AG2.Health = 300;

            _Blip = _AG1.AttachBlip();
            _Blip2 = _AG2.AttachBlip();
            _Blip.EnableRoute(Color.Yellow);
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_Blip.Exists()) _Blip.Delete();
            if (_Blip2.Exists()) _Blip2.Delete();
            if (_AG1.Exists()) _AG1.Delete();
            if (_AG2.Exists()) _AG2.Delete();
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
                if (_AG1.IsDead && _AG2.IsDead) End();
                if (Functions.IsPedArrested(_AG1) && Functions.IsPedArrested(_AG2)) End();
            }, "Fighting [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_Blip.Exists()) _Blip.Delete();
            if (_Blip2.Exists()) _Blip2.Delete();
            if (_AG1.Exists()) _AG1.Dismiss();
            if (_AG2.Exists()) _AG2.Dismiss();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Fighting in progress", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
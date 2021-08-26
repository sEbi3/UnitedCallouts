using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using System.Collections.Generic;
using System;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Public Peace Disturbance", CalloutProbability.Medium)]
    class Fighting : Callout
    {
        private Ped _AG1;
        private Ped _AG2;
        private Vector3 _SpawnPoint;
        private Blip _Blip;
        private Blip _Blip2;
        private Vector3 _Location1;
        private Vector3 _Location2;
        private Vector3 _Location3;
        private Vector3 _Location4;
        private Vector3 _Location5;
        private Vector3 _Location6;
        private Vector3 _Location7;
        private Vector3 _Location8;
        private Vector3 _Location9;
        private Vector3 _Location10;
        private bool _hasBegunAttacking = false;
        private bool _CalloutFinished = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _Location1 = new Vector3(-227.4819f, 285.4045f, 91.66596f);
            _Location2 = new Vector3(-358.5807f, 265.6691f, 84.38922f);
            _Location3 = new Vector3(-434.0634f, 257.6701f, 82.99319f);
            _Location4 = new Vector3(1986.708f, 3050.588f, 47.2151f);
            _Location5 = new Vector3(99.72031f, 215.0221f, 107.9258f);
            _Location6 = new Vector3(-334.8364f, -138.3821f, 39.0096f);
            _Location7 = new Vector3(-292.416f, -303.66f, 10.06316f);
            _Location8 = new Vector3(-199.3456f, -786.0698f, 30.45403f);
            _Location9 = new Vector3(-708.6214f, -913.0787f, 19.21559f);
            _Location10 = new Vector3(-828.1153f, -1074.215f, 11.32811f);
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
                "Location10",
            };
            int num = random.Next(0, 10);
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
            if (list[num] == "Location6")
            {
                _SpawnPoint = _Location6;
            }
            if (list[num] == "Location7")
            {
                _SpawnPoint = _Location7;
            }
            if (list[num] == "Location8")
            {
                _SpawnPoint = _Location8;
            }
            if (list[num] == "Location9")
            {
                _SpawnPoint = _Location9;
            }
            if (list[num] == "Location10")
            {
                _SpawnPoint = _Location10;
            }
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 70f);
            CalloutMessage = "[UC]~w~ Reports of a public peace disturbance in progress.";
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
                if (_AG1 && Functions.IsPedArrested(_AG1) && _AG2 && Functions.IsPedArrested(_AG2)) End();
            }, "Fighting [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_Blip) _Blip.Delete();
            if (_Blip2) _Blip2.Delete();
            if (_AG1) _AG1.Dismiss();
            if (_AG2) _AG2.Dismiss();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Fighting in progress", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            _CalloutFinished = true;
            base.End();
        }
    }
}
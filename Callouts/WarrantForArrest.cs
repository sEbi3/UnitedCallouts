using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using System.Collections.Generic;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Warrant for Arrest", CalloutProbability.Medium)]
    public class WarrantForArrest : Callout
    {
        private string[] wepList = new string[] { "WEAPON_PISTOL", "WEAPON_SMG", "WEAPON_MACHINEPISTOL", "WEAPON_PUMPSHOTGUN" };
        private Ped _subject;
        private Vector3 _SpawnPoint;
        private Vector3 _searcharea;
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
        private Blip _Blip;
        private int storyLine = 1;
        private int _callOutMessage = 0;
        private bool _attack = false;
        private bool HasWeapon = false;
        private bool _wasClose = false;
        private bool _alreadySubtitleIntrod = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _Location1 = new Vector3(-73.264f, -28.95624f, 65.75121f);
            _Location2 = new Vector3(-1123.261f, 483.8483f, 82.16084f);
            _Location3 = new Vector3(967.7412f, -546.0015f, 59.36506f);
            _Location4 = new Vector3(-109.5984f, -10.19665f, 70.51959f);
            _Location5 = new Vector3(-10.93565f, -1434.329f, 31.11683f);
            _Location6 = new Vector3(-1.838376f, 523.2645f, 174.6274f);
            _Location7 = new Vector3(-801.5516f, 178.7447f, 72.83471f);
            _Location8 = new Vector3(-812.7239f, 178.7438f, 76.74079f);
            _Location9 = new Vector3(3.542758f, 526.8926f, 170.6218f);
            _Location10 = new Vector3(-1155.698f, -1519.297f, 10.63272f);
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
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 30f);
            switch (new Random().Next(1, 3))
            {
                case 1:
                    _attack = true;
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
            switch (new Random().Next(1, 3))
            {
                case 1:
                    CalloutMessage = "[UC]~w~ Warrant for Arrest";
                    _callOutMessage = 1;
                    break;
                case 2:
                    CalloutMessage = "[UC]~w~ Warrant for Arrest";
                    _callOutMessage = 2;
                    break;
                case 3:
                    CalloutMessage = "[UC]~w~ Warrant for Arrest";
                    _callOutMessage = 3;
                    break;
            }
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_SUSPECT_RESISTING_ARREST_01 IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: Warrant for Arrest callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Warrant for Arrest", "~b~Dispatch:~w~ Try to ~o~speak~w~ and ~b~arrest~w~ the wanted person. Respond with ~r~Code 3");

            _subject = new Ped(_SpawnPoint);
            _subject.Position = _SpawnPoint;
            _subject.IsPersistent = true;
            _subject.BlockPermanentEvents = true;
            LSPD_First_Response.Engine.Scripting.Entities.Persona.FromExistingPed(_subject).Wanted = true;

            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "Loading ~g~Information~w~ of the ~y~LSPD Database~w~...");
            Functions.DisplayPedId(_subject, true);

            _searcharea = _SpawnPoint.Around2D(1f, 2f);
            _Blip = new Blip(_searcharea, 30f);
            _Blip.Color = Color.Yellow;
            _Blip.EnableRoute(Color.Yellow);
            _Blip.Alpha = 0.5f;
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_subject) _subject.Delete();
            if (_Blip) _Blip.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_subject.DistanceTo(Game.LocalPlayer.Character) < 20f && !_wasClose)
                {
                    Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH OFFICERS_ARRIVED_ON_SCENE");
                    _wasClose = true;
                    if (_attack == true && !HasWeapon)
                    {
                        _subject.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);
                        HasWeapon = true;
                        _subject.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                    if (_attack == false  && _subject.DistanceTo(Game.LocalPlayer.Character) < 10f && Game.LocalPlayer.Character.IsOnFoot && _alreadySubtitleIntrod == false)
                    {
                        Game.DisplaySubtitle("Press ~y~Y ~w~to speak with the person.", 5000);
                        _alreadySubtitleIntrod = true;
                        _wasClose = true;
                    }
                    if (_attack == false && _subject.DistanceTo(Game.LocalPlayer.Character) < 2f && Game.IsKeyDown(Settings.Dialog))
                    {
                        _subject.Face(Game.LocalPlayer.Character);
                        switch (storyLine)
                        {
                            case 1:
                                Game.DisplaySubtitle("~y~Suspect: ~w~Hello Officer! Can I help you? (1/5)", 5000);
                                storyLine++;
                                break;
                            case 2:
                                Game.DisplaySubtitle("~b~You: ~w~We have a warrant for your arrest. (2/5)", 5000);
                                storyLine++;
                                break;
                            case 3:
                                Game.DisplaySubtitle("~y~Suspect: ~w~...me? Are you sure? (3/5)", 5000);
                                storyLine++;
                                break;
                            case 4:
                                if (_callOutMessage == 1)
                                    Game.DisplaySubtitle("~b~You: ~w~I have to arrest you because we have an arrest warrant against you. You need to come with me. (4/5)", 5000);
                                if (_callOutMessage == 2)
                                    Game.DisplaySubtitle("~b~You: ~w~Tell that to the court. Don't make this hard! (4/5)", 5000);
                                if (_callOutMessage == 3)
                                    Game.DisplaySubtitle("~b~You: ~w~Tell that to the court. Don't make this harder than what it needs to be! (4/5)", 5000);
                                storyLine++;
                                break;
                            case 5:
                                if (_callOutMessage == 1)
                                {
                                    _subject.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                                    Game.DisplaySubtitle("~y~Suspect: ~w~Okay, fine. (5/5)", 5000);
                                }
                                if (_callOutMessage == 2)
                                {
                                    Game.DisplaySubtitle("~y~Suspect: ~w~You're not taking me in, you pig! (5/5)", 5000);
                                    _subject.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
                                    Rage.Native.NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _subject, Game.LocalPlayer.Character, 0, 16);
                                }
                                if (_callOutMessage == 3)
                                {
                                    Game.DisplaySubtitle("~y~Suspect: ~w~I'm not going with you... I'm sorry but I can't go back to prison! (5/5)", 5000);
                                    _subject.Inventory.GiveNewWeapon("WEAPON_KNIFE", 500, true);
                                    Rage.Native.NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _subject, Game.LocalPlayer.Character, 0, 16);
                                }
                                storyLine++;
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (_subject && _subject.IsDead) End();
                if (_subject && Functions.IsPedArrested(_subject)) End();
            }, "Warrant for Arrest [UnitedCallouts]");
            base.Process();
        }
        public override void End()
        {
            if (_subject) _subject.Dismiss();
            if (_Blip) _Blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Warrant for Arrest", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
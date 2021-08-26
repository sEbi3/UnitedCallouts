using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Welfare Check Request", CalloutProbability.Medium)]
    public class WelfareCheck : Callout
    {
        private Ped _subject;
        private string[] Suspects = new string[] { "ig_andreas", "g_m_m_armlieut_01", "a_m_m_bevhills_01", "a_m_y_business_02", "s_m_m_gaffer_01",
                                                   "a_f_y_golfer_01", "a_f_y_bevhills_01", "a_f_y_bevhills_04", "a_f_y_fitness_02"};
        private Vector3 _SpawnPoint;
        private Vector3 _searcharea;
        private Vector3 _Location1;
        private Vector3 _Location2;
        private Vector3 _Location3;
        private Vector3 _Location4;
        private Blip _Blip;
        private int _storyLine = 1;
        private int _callOutMessage = 0;
        private bool _Scene1 = false;
        private bool _Scene2 = false;
        private bool _Scene3 = false;
        private bool _wasClose = false;
        private bool _alreadySubtitleIntrod = false;
        private bool _notificationDisplayed = false;
        private bool _getAmbulance = false;
        private bool _CalloutFinished = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _Location1 = new Vector3(917.1311f, -651.3591f, 57.86318f);
            _Location2 = new Vector3(-1905.715f, 365.4793f, 93.58082f);
            _Location3 = new Vector3(1661.571f, 4767.511f, 42.00745f);
            _Location4 = new Vector3(1878.274f, 3922.46f, 33.06999f);

            Random random = new Random();
            List<string> list = new List<string>
            {
                "Location1",
                "Location2",
                "Location3",
                "Location4",
            };
            int num = random.Next(0, 4);
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
            _subject = new Ped(Suspects[new Random().Next((int)Suspects.Length)], _SpawnPoint, 0f);
            LSPD_First_Response.Mod.API.Functions.GetPersonaForPed(_subject);
            switch (new Random().Next(1, 3))
            {
                case 1:
                    _subject.Kill();
                    _Scene1 = true;
                    break;
                case 2:
                    _Scene3 = true;
                    break;
                case 3:
                    _subject.Dismiss();
                    _Scene2 = true;
                    break;
            }
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 100f);
            switch (new Random().Next(1, 3))
            {
                case 1:
                    CalloutMessage = "[UC]~w~ Welfare Check Request";
                    _callOutMessage = 1;
                    break;
                case 2:
                    CalloutMessage = "[UC]~w~ Welfare Check Request";
                    _callOutMessage = 2;
                    break;
                case 3:
                    CalloutMessage = "[UC]~w~ Welfare Check Request";
                    _callOutMessage = 3;
                    break;
            }
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("UNITS WE_HAVE CRIME_CIVILIAN_NEEDING_ASSISTANCE_02", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: WelfareCheck callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Welfare Check Request", "~b~Dispatch:~w~ Someone called the police for a welfare check. Search the ~y~yellow area~w~ for the person. Respond with ~y~Code 2");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "", "Loading ~g~Information~w~ of the ~y~LSPD Database~w~...");
            Functions.DisplayPedId(_subject, true);

            _searcharea = _SpawnPoint.Around2D(1f, 2f);
            _Blip = new Blip(_searcharea, 40f);
            _Blip.EnableRoute(Color.Yellow);
            _Blip.Color = Color.Yellow;
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
                if (_SpawnPoint.DistanceTo(Game.LocalPlayer.Character) < 25f)
                {
                    if (_Scene1 == true && _subject.DistanceTo(Game.LocalPlayer.Character) < 10f && Game.LocalPlayer.Character.IsOnFoot && !_notificationDisplayed && !_getAmbulance)
                    {
                        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "We are going to call an ~y~ambulance~w~ to your current location, officer. Press the ~y~END~w~ key to end the welfare check callout.");
                        _notificationDisplayed = true;
                        GameFiber.Wait(1000);
                        Game.DisplayHelp("Press the ~y~END~w~ key to end the wellfare check callout. The callout is ~g~CODE 4~w~.");
                        Functions.RequestBackup(Game.LocalPlayer.Character.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.Ambulance);
                        _getAmbulance = true;
                    }
                    if (_Scene2 == true && _SpawnPoint.DistanceTo(Game.LocalPlayer.Character) < 8f && Game.LocalPlayer.Character.IsOnFoot && !_notificationDisplayed)
                    {
                        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "Investigate the area. If you don't find anyone here, then ~g~End~w~ the callout.");
                        _notificationDisplayed = true;
                    }
                    if (_Scene3 == true && _subject.DistanceTo(Game.LocalPlayer.Character) < 25f && Game.LocalPlayer.Character.IsOnFoot && _alreadySubtitleIntrod == false)
                    {
                        Game.DisplaySubtitle("Press ~y~Y ~w~to speak with the suspect.", 5000);
                        Game.DisplayHelp("Press the ~y~END~w~ key to end the ~o~welfare check~w~ callout.", 5000);
                        _alreadySubtitleIntrod = true;
                        _wasClose = true;
                    }
                    if (_Scene3 == true && _Scene1 == false && _Scene2 == false && _subject.DistanceTo(Game.LocalPlayer.Character) < 2f && Game.IsKeyDown(Settings.Dialog))
                    {
                        _subject.Face(Game.LocalPlayer.Character);
                        switch (_storyLine)
                        {
                            case 1:
                                Game.DisplaySubtitle("~y~Suspect: ~w~Hello Officer, how can I help you? Is everything alright? (1/5)", 5000);
                                _storyLine++;
                                break;
                            case 2:
                                Game.DisplaySubtitle("~b~You: ~w~Hi. I'm just checking in on this address as we've had a welfare check request come through. Apparently you weren't answering your phone and someone is concerned. Is everything okay here? (2/5)", 5000);
                                _storyLine++;
                                break;
                            case 3:
                                Game.DisplaySubtitle("~y~Suspect: ~w~Oh dear! I didn't want to worry anyone. (3/5)", 5000);
                                _storyLine++;
                                break;
                            case 4:
                                if (_callOutMessage == 1)
                                    Game.DisplaySubtitle("~y~Suspect: ~w~I lost my phone on the bus today, I was actually just about to head to a payphone to ring the bus depot. (4/5)", 5000);
                                if (_callOutMessage == 2)
                                    Game.DisplaySubtitle("~y~Suspect: ~w~My phone battery died because I forgot to charge it earlier! I did see a missed call but didn't think anything of it. (4/5)", 5000);
                                if (_callOutMessage == 3)
                                    Game.DisplaySubtitle("~y~Suspect: ~w~Let me check... Oops, I had my phone on silent! I'll call them back now. Sorry to cause such trouble! (4/5)", 5000);
                                _storyLine++;
                                break;
                            case 5:
                                if (_callOutMessage == 1)
                                    Game.DisplaySubtitle("~b~You: ~w~Ouch. I'll let dispatch know everything is okay. Good luck finding your phone! (5/5)", 5000);
                                if (_callOutMessage == 2)
                                    Game.DisplaySubtitle("~b~You: ~w~Alright, well as long as everything here is okay, I can leave. You should return that phone call though, the caller was really worried. (5/5)", 5000);
                                if (_callOutMessage == 3)
                                    Game.DisplaySubtitle("~b~You: ~w~No problem, I'm just glad you're okay. I'll let dispatch know everything is fine here. (5/5)", 5000);
                                _storyLine++;
                                break;
                            case 6:
                                if (_callOutMessage == 1)
                                    End();
                                if (_callOutMessage == 2)
                                    End();
                                if (_callOutMessage == 3)
                                    End();
                                _storyLine++;
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (Game.LocalPlayer.Character.IsDead) End();
            }, "WellfareCheck [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_subject) _subject.Dismiss();
            if (_Blip) _Blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~WelfareCheck", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            _CalloutFinished = true;
            base.End();
        }
    }
}
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("WelfareCheck", CalloutProbability.Medium)]
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
                    this.CalloutMessage = "~b~Dispatch:~s~ Welfare check";
                    _callOutMessage = 1;
                    break;
                case 2:
                    this.CalloutMessage = "~b~Dispatch:~s~ Welfare check";
                    _callOutMessage = 2;
                    break;
                case 3:
                    this.CalloutMessage = "~b~Dispatch:~s~ Welfare check";
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
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Welfare Check", "~b~Dispatch:~w~ Someone called the police for a welfare check. Search the ~y~yellow area~w~ for the person. Respond with ~y~Code 2");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "Loading ~g~Informations~w~ of the ~y~LSPD Database~w~...");
            Functions.DisplayPedId(_subject, true);

            _searcharea = _SpawnPoint.Around2D(1f, 2f);
            _Blip = new Blip(_searcharea, 40f);
            _Blip.EnableRoute(Color.Yellow);
            _Blip.Color = Color.Yellow;
            _Blip.Alpha = 2f;   
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_subject.Exists()) _subject.Delete();
            if (_Blip.Exists()) _Blip.Delete();
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
                        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "We are going to call a ~y~ambulance~w~ to you're current location, Officer. Press the ~y~END~w~ key to end the welfare check callout.");
                        _notificationDisplayed = true;
                        GameFiber.Wait(1000);
                        Game.DisplayHelp("Press the ~y~END~w~ key to end the wellfare check callout. The callout is ~g~CODE 4~w~.");
                        Functions.RequestBackup(Game.LocalPlayer.Character.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.Ambulance);
                        _getAmbulance = true;
                    }
                    if (_Scene2 == true && _SpawnPoint.DistanceTo(Game.LocalPlayer.Character) < 8f && Game.LocalPlayer.Character.IsOnFoot && !_notificationDisplayed)
                    {
                        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "No one is at home.<br>You are ~g~code 4~w~.");
                        _notificationDisplayed = true;
                        GameFiber.Wait(1000);
                        End();
                    }
                    if (_Scene3 == true && _subject.DistanceTo(Game.LocalPlayer.Character) < 25f && Game.LocalPlayer.Character.IsOnFoot && _alreadySubtitleIntrod == false)
                    {
                        Game.DisplaySubtitle("Press ~y~Y ~w~to speak with the suspect.", 5000);
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
                                Game.DisplaySubtitle("~b~You: ~w~Hello, we got a call for a welfare check, because you didn't response to the call. (2/5)", 5000);
                                _storyLine++;
                                break;
                            case 3:
                                Game.DisplaySubtitle("~y~Suspect: ~w~Oh no, I forgot that. (3/5)", 5000);
                                _storyLine++;
                                break;
                            case 4:
                                if (_callOutMessage == 1)
                                    Game.DisplaySubtitle("~y~Suspect: ~w~I lost my phone today. I can't find it. (4/5)", 5000);
                                if (_callOutMessage == 2)
                                    Game.DisplaySubtitle("~y~Suspect: ~w~The battery of my phone was empty. (4/5)", 5000);
                                if (_callOutMessage == 3)
                                    Game.DisplaySubtitle("~y~Suspect: ~w~Okay, I'll definitely call back now. Thank you! (4/5)", 5000);
                                _storyLine++;
                                break;
                            case 5:
                                if (_callOutMessage == 1)
                                    Game.DisplaySubtitle("~b~You: ~w~Hm, that is annoying. We will let the caller know, that everything is alright. (5/5)", 5000);
                                    Game.DisplayHelp("Press the ~y~END~w~ key to end the ~o~welfare check~w~ callout. The callout is ~g~ CODE 4~w~.");
                                if (_callOutMessage == 2)
                                    Game.DisplaySubtitle("~b~You: ~w~Okay, everything is alright. If the battery is full now, you can call back. (5/5)", 5000);
                                    Game.DisplayHelp("Press the ~y~END~w~ key to end the ~o~welfare check~w~ callout. The callout is ~g~ CODE 4~w~.");
                                if (_callOutMessage == 3)
                                    Game.DisplaySubtitle("~b~You: ~w~Alright, no problem! Have a nice day. (5/5)", 5000);
                                    Game.DisplayHelp("Press the ~y~END~w~ key to end the ~o~welfare check~w~ callout. The callout is ~g~ CODE 4~w~.");
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
            if (_subject.Exists()) _subject.Dismiss();
            if (_Blip.Exists()) _Blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~WelfareCheck", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using System.Collections.Generic;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Troublemaker at Metro Station", CalloutProbability.Medium)]
    public class troublemaker : Callout
    {
        private Ped _subject;
        private string[] pedList = new string[] { "s_m_y_dealer_01", "u_m_m_jesus_01", "u_m_y_militarybum", "u_m_y_proldriver_01", "a_m_o_soucent_03", "u_m_o_tramp_01",
                                                  "a_m_m_tramp_01", "a_m_o_tramp_01", "a_m_m_trampbeac_01" };
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
        private Blip _Blip;
        private bool _attack = false;
        private bool _startedPursuit = false;
        private bool _wasClose = false;
        private bool _alreadySubtitleIntrod = false;
        private int _storyLine = 1;
        private int _callOutMessage = 0;
        private bool _CalloutFinished = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _Location1 = new Vector3(-292.7569f, -305.3849f, 10.06316f);
            _Location2 = new Vector3(-282.8204f, -326.8933f, 18.28812f);
            _Location3 = new Vector3(262.1429f, -1205.378f, 29.28906f);
            _Location4 = new Vector3(298.8077f, -1206.379f, 38.89511f);
            _Location5 = new Vector3(-854.7909f, -107.829f, 28.18498f);
            _Location6 = new Vector3(-824.4403f, -129.8238f, 28.17533f);
            _Location7 = new Vector3(-1359.522f, -472.9277f, 23.27035f);
            _Location8 = new Vector3(-1346.406f, -474.0514f, 15.04538f);
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
            };
            int num = random.Next(0, 8);
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
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 30f);    
            switch (new Random().Next(1, 3))
            {
                case 1:
                    _attack = true;
                    break;
                case 2:
                    _attack = false;
                    break;
                case 3:
                    _attack = false;
                    break;
            }
            switch (new Random().Next(1, 3))
            {
                case 1:
                    CalloutMessage = "[UC]~w~ Reports of a troublemaker at Metro station";
                    _callOutMessage = 1;
                    break;
                case 2:
                    CalloutMessage = "[UC]~w~ Reports of a troublemaker at Metro station";
                    _callOutMessage = 2;
                    break;
                case 3:
                    CalloutMessage = "[UC]~w~ Reports of a troublemaker at Metro station";
                    _callOutMessage = 3;
                    break;
            }
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_SUSPECT_RESISTING_ARREST_01 IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: Troublemaker callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Troublemaker at Metro station", "~b~Dispatch:~w~ Search the ~y~yellow area in the metro station~w~ and try to contact the troublemaker. Respond with ~y~Code 2");

            _subject = new Ped(pedList[new Random().Next((int)pedList.Length)], _SpawnPoint, 0f);
            _subject.Position = _SpawnPoint;
            _subject.IsPersistent = true;
            _subject.BlockPermanentEvents = true;
            _subject.Tasks.PlayAnimation("amb@world_human_bum_standing@drunk@base", "base", 5, AnimationFlags.None);

            _searcharea = _SpawnPoint.Around2D(1f, 2f);
            _Blip = new Blip(_searcharea, 80f);
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
                if (_subject.DistanceTo(Game.LocalPlayer.Character) < 20f)
                {
                    if (_attack == true && _startedPursuit == false)
                    {
                        _subject.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                    if (_attack == false && _subject.DistanceTo(Game.LocalPlayer.Character) < 6f && Game.LocalPlayer.Character.IsOnFoot && _alreadySubtitleIntrod == false)
                    {
                        Game.DisplaySubtitle("Press ~y~Y ~w~to speak with the troublemaker.", 5000);
                        _subject.Face(Game.LocalPlayer.Character);
                        _alreadySubtitleIntrod = true;
                        _wasClose = true;
                        if (_Blip) _Blip.Delete();
                    }
                    if (_subject.DistanceTo(Game.LocalPlayer.Character) < 5f && Game.LocalPlayer.Character.IsOnFoot && _alreadySubtitleIntrod == false)
                    {
                        _subject.Face(Game.LocalPlayer.Character);
                    }
                    if (_attack == false && _subject.DistanceTo(Game.LocalPlayer.Character) < 2f && Game.IsKeyDown(Settings.Dialog))
                    {
                        _subject.Face(Game.LocalPlayer.Character);
                        switch (_storyLine)
                        {
                            case 1:
                                if (_callOutMessage == 1)
                                    Game.DisplaySubtitle("~r~Troublemaker: ~w~Hello officer, is there a problem? (1/5)", 5000);
                                if (_callOutMessage == 2)
                                    Game.DisplaySubtitle("~r~Troublemaker: ~w~What do you want? (1/5)", 5000);
                                if (_callOutMessage == 3)
                                    Game.DisplaySubtitle("~r~Troublemaker: ~w~I'm not drunk! (1/5)", 5000);
                                _storyLine++;
                                break;
                            case 2:
                                if (_callOutMessage == 1)
                                    Game.DisplaySubtitle("~b~You: ~w~We've had some reports of a male matching your description causing a disturbance here. (2/5)", 5000);
                                if (_callOutMessage == 2)
                                    Game.DisplaySubtitle("~b~You: ~w~Alright, let's stay calm. What are you doing here? (2/5)", 5000);
                                if (_callOutMessage == 3)
                                    Game.DisplaySubtitle("~b~You: ~w~Sure. We've had reports of an intoxicated individual harassing members of the public. That wouldn't be you, would it? (2/5)", 5000);
                                _storyLine++;
                                break;
                            case 3:
                                if (_callOutMessage == 1)
                                    Game.DisplaySubtitle("~r~Troublemaker: ~w~Okay, I may have had an argument down by the tracks. I just lost my temper, I'm sorry! (3/5)", 5000);
                                if (_callOutMessage == 2)
                                    Game.DisplaySubtitle("~r~Troublemaker: ~w~What do you care? I'm not doing anything wrong. Leave me alone. (3/5)", 5000);
                                if (_callOutMessage == 3)
                                    Game.DisplaySubtitle("~r~Troublemaker: ~w~Harassing? Please, I was trying to get the hobo down there to leave the area. I was doing your job! (3/5)", 5000); 
                                _storyLine++;
                                break;
                            case 4:
                                if (_callOutMessage == 1)
                                    Game.DisplaySubtitle("~b~You: ~w~Alright, well you need to leave the area so that I know there won't be any further issues. (4/5)", 5000);
                                if (_callOutMessage == 2)
                                    Game.DisplaySubtitle("~b~You: ~w~We've had reports of someone using vulgar and offensive language. (4/5)", 5000);
                                if (_callOutMessage == 3)
                                    Game.DisplaySubtitle("~b~You: ~w~Alright, well let us deal with that. Please leave the area, you're making people uncomfortable and being a nuisance. (4/5)", 5000);
                                _storyLine++;
                                break;
                            case 5:
                                if (_callOutMessage == 1)
                                {
                                    _subject.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                                    Game.DisplaySubtitle("~r~Troublemaker: ~w~Of course, I'm sorry. I'll wait for my friend elsewhere. (5/5)", 5000);
                                }
                                if (_callOutMessage == 2)
                                {
                                    Game.DisplaySubtitle("~r~Troublemaker: ~w~What the fuck has happened to this country? I have free speech you bitch! You can't silence me! (5/5)", 5000);
                                    _subject.Inventory.GiveNewWeapon("WEAPON_KNIFE", 500, true);
                                    Rage.Native.NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _subject, Game.LocalPlayer.Character, 0, 16);
                                }
                                if (_callOutMessage == 3)
                                {
                                    Game.DisplaySubtitle("~r~Troublemaker: ~w~Oh am I? Well how about now?! (5/5)", 5000);
                                    _subject.Inventory.GiveNewWeapon("WEAPON_MOLOTOV", 500, true);
                                    Rage.Native.NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _subject, Game.LocalPlayer.Character, 0, 16);
                                }
                                _storyLine++;
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
            }, "WantedPerson [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_subject) _subject.Dismiss();
            if (_Blip) _Blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Troublemaker at Metro station", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            _CalloutFinished = true;
            base.End();
        }

    }
}
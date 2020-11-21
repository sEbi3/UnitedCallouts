using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Drawing;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("DrugDealer", CalloutProbability.Medium)]
    public class DrugDealer : Callout
    {
        private string[] DrugCars = new string[] { "Speedo", "Rumpo", "Burrito" };
        private string[] wepList = new string[] { "WEAPON_PISTOL" };
        private string[] pedList = new string[] { "A_F_Y_Hippie_01", "A_M_Y_Skater_01", "A_M_M_FatLatin_01", "A_M_M_EastSA_01", "A_M_Y_Latino_01", "G_M_Y_FamDNF_01",
                                                  "G_M_Y_FamCA_01", "G_M_Y_BallaSout_01", "G_M_Y_BallaOrig_01", "G_M_Y_BallaEast_01", "G_M_Y_StrPunk_02", "S_M_Y_Dealer_01", "A_M_M_RurMeth_01",
                                                  "A_M_Y_MethHead_01", "A_M_M_Skidrow_01", "S_M_Y_Dealer_01", "G_M_Y_MexGoon_03", "G_M_Y_MexGoon_02", "G_M_Y_MexGoon_01",
                                                  "G_M_Y_SalvaGoon_01", "G_M_Y_SalvaGoon_02", "G_M_Y_SalvaGoon_03", "G_M_Y_Korean_01", "G_M_Y_Korean_02", "G_M_Y_StrPunk_01" };
        private Ped _subject;
        private Vehicle _Car;
        private Vector3 _SpawnPoint;
        private Blip _Blip;
        private LHandle _pursuit;
        private int _storyLine = 1;
        private int _callOutMessage = 0;
        private bool _hasDrugs = false;
        private bool _startedPursuit = false;
        private bool _wasClose = false;
        private bool _alreadySubtitleIntrod = false;
        private bool _hasTalkedBack = false;
        private bool _notificationDisplayed = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 100f);
            switch (new Random().Next(1, 3))
            {
                case 1:
                    _hasDrugs = true;
                    break;
                case 2:
                    break;
                case 3:    
                    break;
            }
            switch (new Random().Next(1, 3))
            {
                case 1:
                    CalloutMessage = "~b~Dispatch:~s~ Drug Dealer";
                    _callOutMessage = 1;
                    break;
                case 2:
                    CalloutMessage = "~b~Dispatch:~s~ Drug Dealer";
                    _callOutMessage = 2;
                    break;
                case 3:
                    CalloutMessage = "~b~Dispatch:~s~ Drug Dealer";
                    _callOutMessage = 3;
                    break;
            }
            this.CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS SUSPICIOUS_PERSON IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: DrugDealer callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Drug Dealer", "~b~Dispatch:~w~ Try to perform a ~o~traffic stop~w~, to speak with the Dealer. Respond with ~y~Code 2");

            _subject = new Ped(pedList[new Random().Next((int)pedList.Length)], _SpawnPoint, 0f);
            _Car = new Vehicle(DrugCars[new Random().Next((int)DrugCars.Length)], _SpawnPoint);
            _subject.WarpIntoVehicle(_Car, -1);
            _subject.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.FollowTraffic);

            _Blip = _subject.AttachBlip();
            _Blip.EnableRoute(Color.Yellow);
            _Blip.Color = Color.Red;
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_subject.Exists()) _subject.Delete();
            if (_Car.Exists()) _Car.Delete();
            if (_Blip.Exists()) _Blip.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            if (_Car.DistanceTo(Game.LocalPlayer.Character) < 50f && !_notificationDisplayed && _alreadySubtitleIntrod == false && _pursuit == null)
            {
                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts Computer", "~y~DrugDealer Information", "The car from the dealer is a ~o~" + _Car.Model.Name + "~w~. The plate of the car is ~b~" + _Car.LicensePlate + "~w~.");
                _notificationDisplayed = true;
            }
            GameFiber.StartNew(delegate
            {
                if (_subject.DistanceTo(Game.LocalPlayer.Character) < 25f)
                {
                    if (_hasDrugs == true && _startedPursuit == false)
                    {
                        _pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(_pursuit, _subject);
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        _startedPursuit = true;
                    }
                    if (_subject.DistanceTo(Game.LocalPlayer.Character) < 25f && Game.LocalPlayer.Character.IsOnFoot && !_notificationDisplayed && _alreadySubtitleIntrod == false && _pursuit == null)
                    {
                        Game.DisplaySubtitle("Press ~y~Y ~w~to speak with the Dealer.", 5000);
                        _alreadySubtitleIntrod = true;
                        _wasClose = true;
                        _notificationDisplayed = true;
                    }
                    if (_hasDrugs == false && _subject.DistanceTo(Game.LocalPlayer.Character) < 15f && Game.IsKeyDown(Settings.Dialog))
                    {
                        switch (_storyLine)
                        {
                            case 1:
                                Game.DisplaySubtitle("~y~Suspect: ~w~I was about to pick up furniture!(1/3)", 5000);
                                _storyLine++;
                                break;
                            case 2:
                                Game.DisplaySubtitle("~b~You: ~w~Okay, relax buddy. Where are you coming from?(2/3)", 5000);
                                _storyLine++;
                                break;
                            case 3:
                                Game.DisplaySubtitle("~y~Suspect: ~w~I come from home to help my friend move with the furniture. (3/3)", 5000);
                                _storyLine++;
                                break;
                            case 4:
                                if (_callOutMessage == 1)
                                    Game.DisplaySubtitle("~b~You: ~w~How come we have people saying you've been peering into vehicles?(1/2)", 5000);
                                if (_callOutMessage == 2)
                                    Game.DisplaySubtitle("~b~You: ~w~Why do we have some people saying they saw someone dealing drugs around here?(1/2)", 5000);
                                if (_callOutMessage == 3)
                                    Game.DisplaySubtitle("~b~You: ~w~What if I told you I saw you making a hand to hand transaction with someone back a few blocks?(1/2)", 5000);
                                _storyLine++;
                                break;
                            case 5:
                                if (_callOutMessage == 1)
                                {
                                    Game.DisplaySubtitle("~y~Suspect: ~w~I have nothing to do with that!(2/2)", 5000);
                                    _subject.Inventory.GiveNewWeapon(new WeaponDescriptor("WEAPON_PISTOL"), 56, false);
                                    Rage.Native.NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _subject, Game.LocalPlayer.Character, 0, 16);
                                }
                                if (_callOutMessage == 2)
                                    Game.DisplaySubtitle("~y~Suspect: ~w~I'm not like that, sir. (2/2)", 5000);
                                if (_callOutMessage == 3)
                                    Game.DisplaySubtitle("~y~Suspect: ~w~Yeah, right! And where was that? I know my rights sir! (2/2)", 5000);
                                _storyLine++;
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (_subject.IsDead) End();
                if (Functions.IsPedArrested(_subject)) End();
            }, "DrugDealer [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_subject.Exists()) _subject.Dismiss();
            if (_Car.Exists()) _Car.Dismiss();
            if (_Blip.Exists()) _Blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~DrugDealer", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
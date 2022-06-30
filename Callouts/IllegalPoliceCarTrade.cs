using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using System;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Reports of an Illegal Police Car Trade", CalloutProbability.Medium)]
    public class IllegalPoliceCarTrade : Callout
    {
        private string[] CarList = new string[] { "POLICE", "POLICE2", "POLICE3", "SHERIFF", "POLICE4", "SHERIFF2", "FBI", "FBI2", "POLICEB" };
        private string[] SellerList = new string[] { "ig_andreas", "ig_bankman", "ig_barry", "a_m_m_business_01", "a_m_y_business_02" };
        private Ped _Buyer;
        private Ped _Seller;
        private Vector3 _SpawnPoint;
        private Vector3 _BuyerSpawn;
        private Vector3 _CarSpawn = new Vector3(-30.4387f, -1089.152f, 26.42208f);
        private Vehicle _Car;
        private Blip _Blip;
        private LHandle _pursuit;
        private bool _attack = false;
        private int _storyLine = 1;
        private bool _startedPursuit = false;
        private bool _wasClose = false;
        private bool _alreadySubtitleIntrod = false;
        private bool _hasTalkedBack = false;
        private int _callOutMessage = 0;

        public override bool OnBeforeCalloutDisplayed()
        {
            _SpawnPoint = new Vector3(-34.79253f, -1096.583f, 26.42235f);
            _BuyerSpawn = new Vector3(-33.81899f, -1089.764f, 26.42229f);

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
                    CalloutMessage = "[UC]~w~ Reports of an Illegal Police Car Trade.";
                    _callOutMessage = 1;
                    break;
                case 2:
                    CalloutMessage = "[UC]~w~ Reports of an Illegal Police Car Trade.";
                    _callOutMessage = 2;
                    break;
                case 3:
                    CalloutMessage = "[UC]~w~ Reports of an Illegal Police Car Trade.";
                    _callOutMessage = 3;
                    break;
            }
            CalloutPosition = _SpawnPoint;
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: Illegal Police Car Trade callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Illegal Police Car Trade", "~b~Dispatch:~w~ Try to arrest the buyer and seller from the illegal trade. Respond with ~y~Code 2");

            _Seller = new Ped(SellerList[new Random().Next((int)SellerList.Length)], _SpawnPoint, 0f);
            _Seller.Position = _SpawnPoint;
            _Seller.IsPersistent = true;
            _Seller.BlockPermanentEvents = true;

            _Buyer = new Ped(_BuyerSpawn);
            _Buyer.Position = _BuyerSpawn;
            _Buyer.IsPersistent = true;
            _Buyer.BlockPermanentEvents = true;
            _Buyer.RelationshipGroup = RelationshipGroup.AggressiveInvestigate;
            _Seller.RelationshipGroup = RelationshipGroup.AggressiveInvestigate;

            _Car = new Vehicle(CarList[new Random().Next((int)CarList.Length)], _CarSpawn);
            _Car.IsStolen = true;

            _Blip = _Car.AttachBlip();
            _Blip.Color = Color.LightBlue;
            _Blip.IsFriendly = false;
            _Blip.EnableRoute(System.Drawing.Color.Yellow);
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_SUSPECT_RESISTING_ARREST_01 IN_OR_ON_POSITION CODE2", _SpawnPoint);
            return base.OnCalloutAccepted();
        }
        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
            if (_Buyer) _Buyer.Delete();
            if (_Blip) _Blip.Delete();
            if (_Seller) _Seller.Delete();
            if (_Car) _Car.Delete();
        }
        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_Seller.DistanceTo(Game.LocalPlayer.Character) < 20f)
                {
                    if (_attack == true && _startedPursuit == false)
                    {
                        _pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(_pursuit, _Seller);
                        Functions.AddPedToPursuit(_pursuit, _Buyer);
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        _startedPursuit = true;
                    }
                    if (_Seller.DistanceTo(Game.LocalPlayer.Character) < 15f && Game.LocalPlayer.Character.IsOnFoot && _alreadySubtitleIntrod == false && _pursuit == null)
                    {
                        Game.DisplaySubtitle("Press ~y~Y ~w~to speak with the Seller", 5000);
                        _Buyer.Face(_Car);
                        _Seller.Face(Game.LocalPlayer.Character);
                        Functions.PlayScannerAudio("ATTENTION_GENERIC_01 OFFICERS_ARRIVED_ON_SCENE");
                        _alreadySubtitleIntrod = true;
                        _wasClose = true;
                    }
                    if (_attack == false && _Seller.DistanceTo(Game.LocalPlayer.Character) < 2f && Game.IsKeyDown(Settings.Dialog))
                    {
                        _Seller.Face(Game.LocalPlayer.Character);
                        switch (_storyLine)
                        {
                            case 1:
                                Game.DisplaySubtitle("~y~Seller: ~w~Oh, hello officer, I didn't hear you. How can I help? (1/5)", 5000);
                                _storyLine++;
                                break;
                            case 2:
                                Game.DisplaySubtitle("~b~You: ~w~Are you the owner? (2/5)", 5000);
                                _storyLine++;
                                break;
                            case 3:
                                Game.DisplaySubtitle("~y~Suspect: ~w~Ah...yes I am the owner! Is anything wrong? (3/5)", 5000);
                                _storyLine++;
                                break;
                            case 4:
                                if (_callOutMessage == 1)
                                    Game.DisplaySubtitle("~b~You: ~w~Is there a reason you have a police vehicle in your garage? (4/5)", 5000);
                                if (_callOutMessage == 2)
                                    Game.DisplaySubtitle("~b~You: ~w~That's a police vehicle... what's going on here? (4/5)", 5000);
                                if (_callOutMessage == 3)
                                    Game.DisplaySubtitle("~b~You: ~w~I couldn't help but notice that police vehicle. Care to explain? (4/5)", 5000);
                                _storyLine++;
                                break;
                            case 5:
                                if (_callOutMessage == 1)
                                    Game.DisplaySubtitle("~y~Suspect: ~w~Uh... Yes! It's here because... Ah, forget it. Do what you need to do. (5/5)", 5000);
                                    Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch Information", "The plate of the ~b~" + _Car.Model.Name + "~w~ is ~o~" + _Car.LicensePlate + "~w~. The car was ~r~stolen~w~ from the police station in ~b~Mission Row~w~.");
                                    Game.DisplayHelp("~y~Arrest the owner and the buyer.", 5000);
                                if (_callOutMessage == 2)
                                {
                                    Game.DisplaySubtitle("~y~Suspect: ~w~You weren't meant to see this! (5/5)", 5000);
                                    _Buyer.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
                                    Rage.Native.NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _Buyer, Game.LocalPlayer.Character, 0, 16);
                                }
                                if (_callOutMessage == 3)
                                {
                                    Game.DisplaySubtitle("~y~Suspect: ~w~I could, but there's no point in talking to a corpse! (5/5)", 5000);
                                    _Seller.Inventory.GiveNewWeapon("WEAPON_KNIFE", 500, true);
                                    Rage.Native.NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _Seller, Game.LocalPlayer.Character, 0, 16);
                                    Rage.Native.NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _Buyer, Game.LocalPlayer.Character, 0, 16);
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
                if (_Seller && _Seller.IsDead && _Buyer.Exists() && _Buyer.IsDead) End();
                if (_Seller && Functions.IsPedArrested(_Seller) && _Buyer.Exists() && Functions.IsPedArrested(_Buyer)) End();
            }, "Illegal Police Car Trade [UnitedCallouts]");
            base.Process();
        }
        public override void End()
        {
            if (_Seller) _Seller.Dismiss();
            if (_Blip) _Blip.Delete();
            if (_Buyer) _Buyer.Dismiss();
            if (_Car) _Car.Dismiss();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Illegal Police Car Trade", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
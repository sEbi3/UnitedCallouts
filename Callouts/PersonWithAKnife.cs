using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using System;
using System.Drawing;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("Person With Knife", CalloutProbability.Medium)]
    public class PersonWithKnife : Callout
    {
        private string[] pedList = new string[] { "A_F_M_SouCent_01", "A_F_M_SouCent_02", "A_M_Y_Skater_01", "A_M_M_FatLatin_01", "A_M_M_EastSA_01", "A_M_Y_Latino_01", "G_M_Y_FamDNF_01",
                                                  "G_M_Y_FamCA_01", "G_M_Y_BallaSout_01", "G_M_Y_BallaOrig_01", "G_M_Y_BallaEast_01", "G_M_Y_StrPunk_02", "S_M_Y_Dealer_01", "A_M_M_RurMeth_01",
                                                  "A_M_M_Skidrow_01", "A_M_Y_MexThug_01", "G_M_Y_MexGoon_03", "G_M_Y_MexGoon_02", "G_M_Y_MexGoon_01", "G_M_Y_SalvaGoon_01", "G_M_Y_SalvaGoon_02",
                                                  "G_M_Y_SalvaGoon_03", "G_M_Y_Korean_01", "G_M_Y_Korean_02", "G_M_Y_StrPunk_01" };
        private Ped _subject;
        private Vector3 _SpawnPoint;
        private Vector3 _searcharea;
        private Blip _Blip;
        private LHandle _pursuit;
        private int _callOutMessage = 0;
        private int _scenario = 0;
        private int _storyLine = 1;
        private bool _CalloutRunnig;
        private bool _hasBegunAttacking = false;
        private bool _isArmed = false;
        private bool _alreadySubtitleIntrod = false;
        private bool _hasPursuitBegun = false;
        private bool _hasSpoke = false;
        private bool _pursuitCreated = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _scenario = new Random().Next(0, 100);
            _SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 100f);
            CalloutMessage = "~b~Dispatch:~s~ Person with a knife.";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS ASSAULT_WITH_AN_DEADLY_WEAPON CIV_ASSISTANCE IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Person with a knife", "~b~Dispatch: ~w~Try to arrest the suspect. Respond with ~r~Code 3");

            _subject = new Ped(pedList[new Random().Next((int)pedList.Length)], _SpawnPoint, 0f);
            _subject.BlockPermanentEvents = true;
            _subject.IsPersistent = true;
            _subject.Tasks.Wander();

            _searcharea = _SpawnPoint.Around2D(1f, 2f);
            _Blip = new Blip(_searcharea, 80f);
            _Blip.Color = Color.Yellow;
            _Blip.EnableRoute(Color.Yellow);
            _Blip.Alpha = 5f;
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_Blip.Exists()) _Blip.Delete();
            if (_subject.Exists()) _subject.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_subject.DistanceTo(Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront)) < 18f && !_isArmed)
                {
                    _subject.Inventory.GiveNewWeapon("WEAPON_KNIFE", 500, true);
                    _isArmed = true;
                }
                if (_subject.Exists() && _subject.DistanceTo(Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront)) < 18f && !_hasBegunAttacking)
                {
                    if (_scenario > 40)
                    {
                        _subject.KeepTasks = true;
                        _subject.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        _hasBegunAttacking = true;
                        switch (new Random().Next(1, 3))
                        {
                            case 1:
                                Game.DisplaySubtitle("~r~Suspect: ~w~I do not want to live anymore!", 4000);
                                _hasSpoke = true;
                                break;
                            case 2:
                                Game.DisplaySubtitle("~r~Suspect: ~w~Go away! - I'm not going back to the psychiatric hospital!", 4000);
                                _hasSpoke = true;
                                break;
                            case 3:
                                Game.DisplaySubtitle("~r~Suspect: ~w~I'm not going back to the psychiatric hospital!", 4000);
                                _hasSpoke = true;
                                break;
                            default: break;
                        }
                        GameFiber.Wait(2000);
                    }
                    else
                    {
                        if (!_hasPursuitBegun)
                        {
                            _pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(_pursuit, _subject);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            _hasPursuitBegun = true;
                        }
                    }
                }
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (_subject.IsDead) End();
                if (Functions.IsPedArrested(_subject)) End();
            }, "Person with Knife [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_subject.Exists()) _subject.Dismiss();
            if (_Blip.Exists()) _Blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Person With Knife", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
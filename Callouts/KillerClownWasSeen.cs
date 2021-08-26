using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Drawing;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Reports of an Armed Clown", CalloutProbability.Medium)]
    public class KillerClownWasSeen : Callout
    {
        private string[] pedList = new string[] { "s_m_y_clown_01" };
        private string[] wepList = new string[] { "WEAPON_PISTOL", "WEAPON_BAT", "WEAPON_KNIFE", "WEAPON_BOTTLE", "WEAPON_MUSKET", "WEAPON_MACHETE" };
        private Ped _subject;
        private Vector3 _SpawnPoint;
        private Vector3 _searcharea;
        private Blip _Blip;
        private LHandle _pursuit;
        private int _scenario = 0;
        private bool _hasBegunAttacking = false;
        private bool _isArmed = false;
        private bool _hasPursuitBegun = false;
        private bool _CalloutFinished = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _scenario = new Random().Next(0, 100);
            _SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 100f);
            CalloutMessage = "[UC]~w~ Reports of an armed clown.";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS ASSAULT_WITH_AN_DEADLY_WEAPON CIV_ASSISTANCE IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: KillerClownWasSeen callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~KillerClown was seen", "~b~Dispatch: ~w~The ~r~KillerClown~w~ was spotted with an weapon! Respond with ~r~Code 3");

            _subject = new Ped(pedList[new Random().Next((int)pedList.Length)], _SpawnPoint, 0f);
            _subject.Inventory.GiveNewWeapon("WEAPON_UNARMED", 500, true);
            _subject.BlockPermanentEvents = true;
            _subject.IsPersistent = true;
            _subject.Tasks.Wander();

            _searcharea = _SpawnPoint.Around2D(1f, 2f);
            _Blip = new Blip(_searcharea, 80f);
            _Blip.Color = Color.Yellow;
            _Blip.EnableRoute(Color.Yellow);
            _Blip.Alpha = 0.5f;
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_Blip) _Blip.Delete();
            if (_subject) _subject.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_subject.DistanceTo(Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront)) < 25f && !_isArmed)
                {
                    _subject.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);
                    _isArmed = true;
                }
                if (_subject && _subject.DistanceTo(Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront)) < 25f && !_hasBegunAttacking)
                {
                    if (_scenario > 40)
                    {
                        _subject.KeepTasks = true;
                        _subject.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        _hasBegunAttacking = true;
                        GameFiber.Wait(2000);
                    }
                    else
                    {
                        if (!_hasPursuitBegun)
                        {
                            if (_Blip) _Blip.Delete();
                            _pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(_pursuit, _subject);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            _hasPursuitBegun = true;
                        }
                    }
                }
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (_subject && _subject.IsDead) End();
                if (_subject && Functions.IsPedArrested(_subject)) End();
            }, "KillerClown was seen [UnitedCallouts]");
            base.Process();
        }
        public override void End()
        {
            if (_subject) _subject.Dismiss();
            if (_Blip) _Blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~KillerClown was Seen", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            _CalloutFinished = true;
            base.End();
        }
    }
}
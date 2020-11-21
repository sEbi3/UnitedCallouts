using System;
using Rage;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Collections.Generic;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("DrugDeal", CalloutProbability.Medium)]
    public class DrugDeal : Callout
    {
        private string[] wepList = new string[] { "WEAPON_PISTOL", "WEAPON_SMG", "WEAPON_MACHINEPISTOL", "WEAPON_PUMPSHOTGUN" };
        private string[] pedList = new string[] { "s_m_y_dealer_01", "u_m_o_tramp_01" };
        public LHandle _pursuit;
        public Blip _Blip;
        public Blip _Blip2;
        public Vector3 _SpawnPoint;
        private Vector3 _Location1;
        private Vector3 _Location2;
        private Vector3 _Location3;
        private Vector3 _Location4;
        public Ped _Dealer;
        public Ped _Victim;
        private int _scenario = 0;
        private bool _hasPursuitBegun = false;
        private bool _isArmed = false;
        private bool _hasBegunAttacking = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _Location1 = new Vector3(13.29765f, -1033.113f, 29.21461f);
            _Location2 = new Vector3(75.73988f, -855.1366f, 30.75766f);
            _Location3 = new Vector3(791.2688f, -1112.81f, 22.73129f);
            _Location4 = new Vector3(229.2202f, -1773.199f, 28.73569f);
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
            _scenario = new Random().Next(0, 100);
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 15f);
            CalloutMessage = "Drug deal in progress";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudio("ATTENTION_ALL_UNITS A_DRUG_DEAL_IN_PROGRESS IN_OR_ON_POSITION");
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: DrugDeal callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Drug Deal", "~b~Dispatch: ~w~Try to arrest the Dealer and the buyer!. Respond with ~y~Code 2");
           
            _Dealer = new Ped(pedList[new Random().Next((int)pedList.Length)], _SpawnPoint, 0f);
            _Dealer.BlockPermanentEvents = true;
            _Dealer.IsPersistent = true;

            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "Loading ~g~Informations~w~ of the ~y~LSPD Database~w~...");
            GameFiber.Wait(1000);
            Functions.DisplayPedId(_Dealer, true);

            _Victim = new Ped(_Dealer.GetOffsetPosition(new Vector3(0, 1.8f, 0)));
            _Victim.BlockPermanentEvents = true;
            _Victim.IsPersistent = true;

            _Blip = _Dealer.AttachBlip();
            _Blip2 = _Victim.AttachBlip();
            _Blip.EnableRoute(Color.Yellow);
            return base.OnCalloutAccepted();
        }
        public override void OnCalloutNotAccepted()
        {
            if (_Dealer.Exists()) _Dealer.Delete();
            if (_Victim.Exists()) _Victim.Delete();
            if (_Blip.Exists()) _Blip.Delete();
            if (_Blip2.Exists()) _Blip2.Delete();
            base.OnCalloutNotAccepted();
        }
        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_Dealer.DistanceTo(Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront)) < 75f && !_isArmed)
                {
                    _Dealer.Face(_Victim);
                    _Victim.Face(_Dealer);
                    _Dealer.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);
                    _isArmed = true;
                }
                if (_Dealer.DistanceTo(Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront)) < 60f && !_hasBegunAttacking)
                {
                    if (_scenario > 40)
                    {
                        new RelationshipGroup("AG");
                        new RelationshipGroup("VI");
                        _Dealer.RelationshipGroup = "AG";
                        _Victim.RelationshipGroup = "AG";
                        Game.LocalPlayer.Character.RelationshipGroup = "VI";
                        Game.SetRelationshipBetweenRelationshipGroups("AG", "VI", Relationship.Hate);
                        _Dealer.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        Game.DisplayNotification("Arrest the ~o~buyer~w~, who is surrendering!");
                        _Victim.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                        _hasBegunAttacking = true;
                        GameFiber.Wait(2000);
                    }
                    else
                    {
                        if (!_hasPursuitBegun)
                        {
                            _pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(_pursuit, _Dealer);
                            Functions.AddPedToPursuit(_pursuit, _Victim);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            _hasPursuitBegun = true;
                        }
                    }
                }
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (_Dealer.IsDead && _Victim.IsDead) End();
                if (Functions.IsPedArrested(_Dealer) && Functions.IsPedArrested(_Victim)) End();
            }, "DrugDeal [UnitedCallouts]");
            base.Process();
        }
        public override void End()
        {
            if (_Blip.Exists()) _Blip.Delete();
            if (_Blip2.Exists()) _Blip2.Delete();
            if (_Victim.Exists()) _Victim.Dismiss();
            if (_Dealer.Exists()) _Dealer.Dismiss();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Drug Deal", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
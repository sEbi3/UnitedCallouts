using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using System.Collections.Generic;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Suspicious ATM Activity", CalloutProbability.Medium)]
    public class ATMActivity : Callout
    {
        private string[] wepList = new string[] { "WEAPON_PISTOL", "WEAPON_CROWBAR", "WEAPON_KNIFE" };
        public Vector3 _SpawnPoint;
        public Vector3 _searcharea;
        private Vector3 _Location1;
        private Vector3 _Location2;
        private Vector3 _Location3;
        private Vector3 _Location4;
        private Vector3 _Location5;
        private Vector3 _Location6;
        public Blip _Blip;
        public Ped _Aggressor;
        private bool _hasBegunAttacking = false;
        private bool _hasPursuitBegun = false;
        private LHandle _pursuit;
        private bool _pursuitCreated = false;
        private int _scenario = 0;
        private bool _CalloutFinished = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _Location1 = new Vector3(112.7427f, -818.8912f, 31.33836f);
            _Location2 = new Vector3(-203.61f, -861.6489f, 30.26763f);
            _Location3 = new Vector3(288.3719f, -1282.444f, 29.65594f);
            _Location4 = new Vector3(-526.4995f, -1222.698f, 18.45498f);
            _Location5 = new Vector3(-821.5978f, -1082.233f, 11.13243f);
            _Location6 = new Vector3(-618.8591f, -706.7742f, 30.05278f);
            Random random = new Random();
            List<string> list = new List<string>
            {
                "Location1",
                "Location2",
                "Location3",
                "Location4",
                "Location5",
                "Location6",
            };
            int num = random.Next(0, 6);
            if (list[num] == "Location1")
            {
                _SpawnPoint = _Location1;
                _Aggressor = new Ped(_SpawnPoint, 161.2627f);
            }
            if (list[num] == "Location2")
            {
                _SpawnPoint = _Location2;
                _Aggressor = new Ped(_SpawnPoint, 25.38977f);
            }
            if (list[num] == "Location3")
            {
                _SpawnPoint = _Location3;
                _Aggressor = new Ped(_SpawnPoint, 278.5002f);
            }
            if (list[num] == "Location4")
            {
                _SpawnPoint = _Location4;
                _Aggressor = new Ped(_SpawnPoint, 151.1967f);

            }
            if (list[num] == "Location5")
            {
                _SpawnPoint = _Location5;
                _Aggressor = new Ped(_SpawnPoint, 32.33837f);
            }
            if (list[num] == "Location6")
            {
                _SpawnPoint = _Location6;
                _Aggressor = new Ped(_SpawnPoint, 270.148f);
            }
            _scenario = new Random().Next(0, 100);
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 15f);
            CalloutMessage = "[UC]~w~ Reports of suspicious ATM activity.";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: ATMActivity callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Suspicious ATM activity", "~b~Dispatch: ~w~Someone called the police because of suspicious activitiy at an ATM. Respond with ~r~Code 3");

            _Aggressor.IsPersistent = true;
            _Aggressor.BlockPermanentEvents = true;
            _Aggressor.Armor = 200;
            _Aggressor.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);

            _searcharea = _SpawnPoint.Around2D(1f, 2f);
            _Blip = new Blip(_searcharea, 20f);
            _Blip.Color = Color.Yellow;
            _Blip.EnableRoute(Color.Yellow);
            _Blip.Alpha = 0.5f;
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_Aggressor) _Aggressor.Delete();
            if (_Blip) _Blip.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_Aggressor && _Aggressor.DistanceTo(Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront)) < 20f && !_hasBegunAttacking)
                {
                    if (_scenario > 40)
                    {
                        new RelationshipGroup("AG");
                        new RelationshipGroup("VI");
                        _Aggressor.RelationshipGroup = "AG";
                        Game.LocalPlayer.Character.RelationshipGroup = "VI";
                        Game.SetRelationshipBetweenRelationshipGroups("AG", "VI", Relationship.Hate);
                        _Aggressor.Tasks.FightAgainstClosestHatedTarget(1000f);
                        GameFiber.Wait(200);
                        _Aggressor.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        _hasBegunAttacking = true;
                        GameFiber.Wait(600);
                    }
                    else
                    {
                        if (!_hasPursuitBegun)
                        {
                            _pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(_pursuit, _Aggressor);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            _pursuitCreated = true;
                            _hasPursuitBegun = true;
                        }
                    }
                }
                if (_Aggressor && _Aggressor.IsDead) End();
                if (Functions.IsPedArrested(_Aggressor)) End();
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
            }, "ATMActivity [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_Blip) _Blip.Delete();
            if (_Aggressor) _Aggressor.Dismiss();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Suspicious ATM activity", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            _CalloutFinished = true;
            base.End();
        }
    }
}

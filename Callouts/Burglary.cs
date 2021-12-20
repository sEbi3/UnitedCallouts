using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using System.Collections.Generic;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Apartment Burglary", CalloutProbability.Medium)]
    public class Burglary : Callout
    {
        private string[] pedList = new string[] { "g_m_m_chicold_01", "mp_g_m_pros_01" };
        private string[] wepList = new string[] { "WEAPON_PISTOL", "WEAPON_SMG", "WEAPON_MACHINEPISTOL", "WEAPON_PUMPSHOTGUN" };
        public Vector3 _SpawnPoint;
        public Vector3 _searcharea;
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
        private Vector3 _Location11;
        private Vector3 _Location12;
        private Vector3 _Location13;
        public Blip _Blip;
        public Blip AblipHelp;
        public Ped _Aggressor;
        public Ped _Victim;
        private bool _notificationDisplayed = false;
        private bool _hasBegunAttacking = false;
        private bool _hasPursuitBegun = false;
        private bool _CalloutFinished = false;
        private int _scenario = 0;

        public override bool OnBeforeCalloutDisplayed()
        {
            _Location1 = new Vector3(-109.5984f, -10.19665f, 70.51959f);
            _Location2 = new Vector3(-10.93565f, -1434.329f, 31.11683f);
            _Location3 = new Vector3(-1.838376f, 523.2645f, 174.6274f);
            _Location4 = new Vector3(-801.5516f, 178.7447f, 72.83471f);
            _Location5 = new Vector3(-812.7239f, 178.7438f, 76.74079f);
            _Location6 = new Vector3(3.542758f, 526.8926f, 170.6218f);
            _Location7 = new Vector3(-1155.698f, -1519.297f, 10.63272f);
            _Location8 = new Vector3(1392.589f, 3613.899f, 38.94194f);
            _Location9 = new Vector3(2435.457f, 4966.514f, 46.8106f);
            _Location10 = new Vector3(2451.795f, 4986.356f, 46.81058f);
            _Location11 = new Vector3(2441.402f, 4970.8f, 51.56487f);
            _Location12 = new Vector3(2448.435f, 4984.749f, 51.56483f);
            _Location13 = new Vector3(2433.171f, 4965.435f, 42.3476f);
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
                "Location11",
                "Location12",
                "Location13",
            };
            int num = random.Next(0, 13);
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
            if (list[num] == "Location11")
            {
                _SpawnPoint = _Location11;
            }
            if (list[num] == "Location12")
            {
                _SpawnPoint = _Location12;
            }
            if (list[num] == "Location13")
            {
                _SpawnPoint = _Location13;
            }
            _scenario = new Random().Next(0, 100);
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 15f);
            CalloutMessage = "[UC]~w~ Reports of an apartment burglary.";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_BURGLARY_IN IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: Burglary callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Burglary into an apartment", "~b~Dispatch: ~w~Try to ~o~find the apartment~w~ and arrest the burglar. Respond with ~r~Code 3");

            _Aggressor = new Ped(pedList[new Random().Next((int)pedList.Length)], _SpawnPoint, 0f);
            _Aggressor.IsPersistent = true;
            _Aggressor.BlockPermanentEvents = true;
            _Aggressor.Armor = 200;
            _Aggressor.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);

            _Victim = new Ped(_Aggressor.GetOffsetPosition(new Vector3(0, 1.8f, 0)));
            _Victim.Tasks.PutHandsUp(-1, _Aggressor);
            _Victim.IsPersistent = true;
            _Victim.BlockPermanentEvents = true;

            NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _Aggressor, _Victim, -1, true);

            _searcharea = _SpawnPoint.Around2D(1f, 2f);
            _Blip = new Blip(_searcharea, 30f);
            _Blip.Color = Color.Yellow;
            _Blip.EnableRoute(Color.Yellow);
            _Blip.Alpha = 0.5f;
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_Aggressor) _Aggressor.Delete();
            if (_Victim) _Victim.Delete();
            if (_Blip) _Blip.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_Aggressor.DistanceTo(Game.LocalPlayer.Character) < 25f && !_notificationDisplayed)
                {
                    if (_Blip.Exists()) _Blip.Delete();
                    Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Burglary into an apartment", "~b~Dispatch: ~w~Investigate the ~y~apartment~w~. Try to arrest the ~o~burglar~w~.");
                    Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH OFFICERS_ARRIVED_ON_SCENE");
                    _notificationDisplayed = true;
                }
                if (Game.LocalPlayer.Character.DistanceTo(_Victim) < 8f)
                {
                    _Victim.PlayAmbientSpeech("GENERIC_SHOCKED_HIGH");
                }
                if (_Aggressor && _Aggressor.DistanceTo(Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront)) < 8f && !_hasBegunAttacking)
                {
                    if (_scenario > 40)
                    {
                        new RelationshipGroup("AG");
                        new RelationshipGroup("VI");
                        _Aggressor.RelationshipGroup = "AG";
                        _Victim.RelationshipGroup = "VI";
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
                            _Aggressor.Tasks.FightAgainst(Game.LocalPlayer.Character);
                            _hasBegunAttacking = true;
                            GameFiber.Wait(2000);
                        }
                    }
                }
                if (Settings.HelpMessages)
                {
                    if (_Aggressor && _Aggressor.IsDead) { Game.DisplayHelp("~y~Dispatch:~w~ Make sure no one else is in the apartment. Otherwise ~g~End~w~ the callout.", 5000); }
                    if (_Aggressor && Functions.IsPedArrested(_Aggressor)) { Game.DisplayHelp("~y~Dispatch:~w~ Make sure no one else is in the apartment. Otherwise ~g~End~w~ the callout.", 5000); }
                }
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
            }, "Burglary [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_Blip) _Blip.Delete();
            if (_Victim) _Victim.Dismiss();
            if (_Aggressor) _Aggressor.Dismiss();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Burglary into an apartment", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            _CalloutFinished = true;
            base.End();
        }      
    }
}
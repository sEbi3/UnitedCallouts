using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using UnitedCallouts.Stuff;
using System.Collections.Generic;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Apartment Burglary", CalloutProbability.Medium)]
    public class ApartmentBurglary : Callout
    {
        private string[] pedList = new string[] { "g_m_m_chicold_01", "mp_g_m_pros_01" };
        private string[] wepList = new string[] { "WEAPON_PISTOL", "WEAPON_SMG", "WEAPON_MACHINEPISTOL", "WEAPON_PUMPSHOTGUN" };
        public Vector3 _SpawnPoint;
        public Vector3 _searcharea;
        public Blip _Blip;
        public Blip AblipHelp;
        public Ped _Aggressor;
        public Ped _Victim;
        private bool _notificationDisplayed = false;
        private bool _hasBegunAttacking = false;
        private bool _hasPursuitBegun = false;
        private int _scenario = 0;

        public override bool OnBeforeCalloutDisplayed()
        {
            Random random = new Random();
            List<Vector3> list = new List<Vector3>
            {
                new Vector3(-109.5984f, -10.19665f, 70.51959f),
                new Vector3(-10.93565f, -1434.329f, 31.11683f),
                new Vector3(-1.838376f, 523.2645f, 174.6274f),
                new Vector3(-801.5516f, 178.7447f, 72.83471f),
                new Vector3(-812.7239f, 178.7438f, 76.74079f),
                new Vector3(3.542758f, 526.8926f, 170.6218f),
                new Vector3(-1155.698f, -1519.297f, 10.63272f),
                new Vector3(1392.589f, 3613.899f, 38.94194f),
                new Vector3(2435.457f, 4966.514f, 46.8106f),
                new Vector3(2451.795f, 4986.356f, 46.81058f),
                new Vector3(2441.402f, 4970.8f, 51.56487f),
                new Vector3(2448.435f, 4984.749f, 51.56483f),
                new Vector3(2433.171f, 4965.435f, 42.3476f),

        };
            _SpawnPoint = LocationChooser.chooseNearestLocation(list);
            _scenario = new Random().Next(0, 100);
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 15f);
            CalloutMessage = "[UC]~w~ Reports of an Apartment Burglary.";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_BURGLARY_IN IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: Apartment Burglary callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Apartment Burglary", "~b~Dispatch: ~w~Try to ~o~find the apartment~w~ and arrest the burglar. Respond with ~r~Code 3");

            _Aggressor = new Ped(pedList[new Random().Next((int)pedList.Length)], _SpawnPoint, 0f);
            _Aggressor.IsPersistent = true;
            _Aggressor.BlockPermanentEvents = true;
            _Aggressor.Armor = 200;
            _Aggressor.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);

            _Victim = new Ped(_Aggressor.GetOffsetPosition(new Vector3(0, 1.8f, 0)));
            _Victim.Tasks.PutHandsUp(-1, _Aggressor);
            _Victim.IsPersistent = true;
            _Victim.BlockPermanentEvents = true;

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
                    NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _Aggressor, _Victim, -1, true);
                    _notificationDisplayed = true;
                }
                if (Game.LocalPlayer.Character.DistanceTo(_Victim) < 20f)
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
                } else {  Settings.HelpMessages = false; }
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
            }, "Apartment Burglary [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_Blip) _Blip.Delete();
            if (_Victim) _Victim.Dismiss();
            if (_Aggressor) _Aggressor.Dismiss();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Apartment Burglary", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }      
    }
}
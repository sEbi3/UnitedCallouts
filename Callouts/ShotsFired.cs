using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using System.Collections.Generic;
using UnitedCallouts.Stuff;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Reports of Shots Fired", CalloutProbability.Medium)]
    public class ShotsFired : Callout
    {
        private string[] wepList = new string[] { "WEAPON_PISTOL", "WEAPON_ASSAULTRIFLE", "WEAPON_SAWNOFFSHOTGUN", "WEAPON_PISTOL50" };
        private Ped _subject;
        private Ped _V1;
        private Ped _V2;
        private Ped _V3;
        private Vector3 _SpawnPoint;
        private Vector3 _searcharea;
        private Blip _Blip;
        private int _scenario = 0;
        private bool _hasBegunAttacking = false;
        private bool _isArmed = false;
        private bool _hasPursuitBegun = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            Random random = new Random();
            List<Vector3> list = new List<Vector3>
            {
                new Vector3(-1622.711f, 214.8514f, 60.22071f),
                new Vector3(295.0424f, -578.2471f, 43.18422f),
                new Vector3(-1573.039f, -1169.825f, 2.402837f),
                new Vector3(-1323.908f, 50.76834f, 53.53567f),
                new Vector3(1155.258f, -741.4567f, 57.30391f),
                new Vector3(291.6201f, 179.956f, 104.297f),
        };
            _SpawnPoint = LocationChooser.chooseNearestLocation(list);
            _scenario = new Random().Next(0, 100);
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 100f);
            CalloutMessage = "[UC]~w~ Reports of Shots Fired.";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_SHOTS_FIRED_01 IN_OR_ON_POSITION", _SpawnPoint);
            // Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS ASSAULT_WITH_AN_DEADLY_WEAPON CIV_ASSISTANCE IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: Reports of Shots Fired callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Reports of Shots Fired", "~b~Dispatch: ~w~Someone called the police because of shots fired. Respond with ~r~Code 3");

            _subject = new Ped(_SpawnPoint);
            _subject.Inventory.GiveNewWeapon("WEAPON_UNARMED", 500, true);
            _subject.BlockPermanentEvents = true;
            _subject.IsPersistent = true;
            _subject.Tasks.Wander();

            _V1 = new Ped(_SpawnPoint);
            _V2 = new Ped(_SpawnPoint);
            _V3 = new Ped(_SpawnPoint);
            _V1.IsPersistent = true;
            _V2.IsPersistent = true;
            _V3.IsPersistent = true;
            _V1.Tasks.Wander();
            _V2.Tasks.Wander();
            _V3.Tasks.Wander();

            _searcharea = _SpawnPoint.Around2D(1f, 2f);
            _Blip = new Blip(_searcharea, 80f);
            _Blip.Color = Color.Red;
            _Blip.EnableRoute(Color.Red);
            _Blip.Alpha = 0.5f;

            if (Settings.ActivateAIBackup)
            {
                Functions.RequestBackup(_SpawnPoint, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.SwatTeam);
                Functions.RequestBackup(_SpawnPoint, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
            } else { return false; }
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_Blip) _Blip.Delete();
            if (_subject) _subject.Delete();
            if (_V1) _V1.Delete();
            if (_V2) _V2.Delete();
            if (_V3) _V3.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_subject.DistanceTo(Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront)) < 40f)
                {
                    if (_Blip) _Blip.Delete();
                }
                if (_subject.DistanceTo(Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront)) < 70f && !_isArmed)
                {
                    _subject.Inventory.GiveNewWeapon(new WeaponAsset(wepList[new Random().Next((int)wepList.Length)]), 500, true);
                    _isArmed = true;
                }
                if (_subject && _subject.DistanceTo(Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront)) < 40f && !_hasBegunAttacking)
                {
                    if (_scenario > 40)
                    {
                        new RelationshipGroup("AG");
                        new RelationshipGroup("VI");
                        _subject.RelationshipGroup = "AG";
                        _V1.RelationshipGroup = "VI";
                        _V2.RelationshipGroup = "VI";
                        _V3.RelationshipGroup = "VI";
                        _subject.KeepTasks = true;
                        Game.SetRelationshipBetweenRelationshipGroups("AG", "VI", Relationship.Hate);
                        _subject.Tasks.FightAgainstClosestHatedTarget(1000f);
                        GameFiber.Wait(2000);
                        _subject.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        _hasBegunAttacking = true;
                        GameFiber.Wait(600);
                    }
                    else
                    {
                        if (!_hasPursuitBegun)
                        {
                            _subject.Face(Game.LocalPlayer.Character);
                            _subject.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                            Game.DisplayNotification("~b~Dispatch:~w~ The suspect is surrendering. Try to ~o~arrest him~w~.");
                            _hasPursuitBegun = true;
                        }
                    }
                }
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (_subject && _subject.IsDead) End();
                if (_subject && Functions.IsPedArrested(_subject)) End();
            }, "Reports of Shots Fired [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_subject) _subject.Dismiss();
            if (_V1) _V1.Dismiss();
            if (_V2) _V2.Dismiss();
            if (_V3) _V3.Dismiss();
            if (_Blip) _Blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Reports of Shots Fired", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
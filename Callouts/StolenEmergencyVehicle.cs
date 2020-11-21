using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System;
using Rage;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("StolenEmergencyVehicle", CalloutProbability.Medium)]
    class StolenEmergencyVehicle : Callout
    {
        private string[] copVehicles = new string[] { "POLICE", "POLICE2", "POLICE3", "POLICE4", "FBI", "FBI2", "POLICEB", "SHERIFF", "SHERIFF2" };
        private Vehicle _PoliceCar;
        private Ped _subject;
        private Vector3 _SpawnPoint;
        private Blip _Blip;
        private LHandle _pursuit;
        private bool _pursuitCreated = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 15f);
            CalloutMessage = "~b~Dispatch:~s~ Stolen Emergency Vehicle";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("CRIME_OFFICER_IN_NEED_OF_ASSISTANCE_01 FOR CRIME_STOLEN_POLICE_VEHICLE UNITS_RESPOND_CODE_3", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: StolenEmergencyVehicle callout accepted.");
            _PoliceCar = new Vehicle(copVehicles[new Random().Next((int)copVehicles.Length)], _SpawnPoint);
            _PoliceCar.IsSirenOn = true;

            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "Loading ~g~Informations~w~ of the ~y~LSPD Database~w~...");
            Functions.DisplayVehicleRecord(_PoliceCar, true);
            _subject = new Ped(_SpawnPoint);
            _subject.WarpIntoVehicle(_PoliceCar, -1);
            _subject.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
            _subject.BlockPermanentEvents = true;

            _Blip = _subject.AttachBlip();

            _pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(_pursuit, _subject);
            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
            _pursuitCreated = true;

            Functions.RequestBackup(_SpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
            Functions.RequestBackup(_SpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
            Functions.RequestBackup(_SpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.AirUnit);
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_subject.Exists()) _subject.Delete();
            if (_PoliceCar.Exists()) _PoliceCar.Delete();
            if (_Blip.Exists()) _Blip.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (_subject.IsDead) End();
                if (Functions.IsPedArrested(_subject)) End();
                base.Process();
            }, "Stolen Emergency Vehicle [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_Blip.Exists()) _Blip.Delete();
            if (_PoliceCar.Exists()) _PoliceCar.Dismiss();
            if (_subject.Exists()) _subject.Dismiss();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Stolen Emergency Vehicle", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
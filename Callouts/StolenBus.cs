using Rage;
using System;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;


namespace UnitedCallouts.Callouts
{
    [CalloutInfo("StolenBus", CalloutProbability.Medium)]
    public class StolenBus : Callout
    {
        private string[] civVehicles = new string[] { "bus", "coach", "airbus" };
        private Vehicle _Bus;
        private Ped _Suspect;
        private Ped _V1;
        private Ped _V2;
        private Ped _V3;
        private Vector3 _SpawnPoint;
        private Blip _Blip;
        private LHandle _Pursuit;
        private bool _PursuitCreated = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 60f);
            CalloutMessage = "~b~Dispatch:~s~ Stolen Bus";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_GRAND_THEFT_AUTO IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: StolenBus callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Stolen Bus", "~b~Dispatch: ~w~Try to arrest the driver to rescue the hostages. Respond with ~r~Code 3");

            _Bus = new Vehicle(this.civVehicles[new Random().Next((int)civVehicles.Length)], _SpawnPoint);
            _Bus.IsPersistent = true;

            _Suspect = _Bus.CreateRandomDriver();
            _Suspect.IsPersistent = true;
            _Suspect.BlockPermanentEvents = true;
            _Suspect.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.Emergency);

            _Blip = _Suspect.AttachBlip();
            _Blip.IsFriendly = false;

            _V1 = new Ped(_SpawnPoint);
            _V2 = new Ped(_SpawnPoint);
            _V3 = new Ped(_SpawnPoint);
            _V1.WarpIntoVehicle(_Bus, 4);
            _V2.WarpIntoVehicle(_Bus, 2);
            _V3.WarpIntoVehicle(_Bus, 3);
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            if (!_PursuitCreated && Game.LocalPlayer.Character.DistanceTo(_Suspect.Position) < 60f)
            {
                _Pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(_Pursuit, _Suspect);
                Functions.SetPursuitIsActiveForPlayer(_Pursuit, true);
                _PursuitCreated = true;
            }
            if (_PursuitCreated && !Functions.IsPursuitStillRunning(_Pursuit))
            {
                End();
            }
            if (Game.LocalPlayer.Character.IsDead) End();
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (_Suspect.IsDead) End();
            if (Functions.IsPedArrested(_Suspect)) End();
            base.Process();
        }
        public override void End()
        {
            if (_Suspect.Exists()) { _Suspect.Dismiss(); }
            if (_Bus.Exists()) { _Bus.Dismiss(); }
            if (_Blip.Exists()) { _Blip.Delete(); }
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Stolen Bus", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
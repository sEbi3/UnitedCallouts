using Rage;
using System;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("StolenTruck", CalloutProbability.Medium)]
    public class StolenTruck : Callout
    {
        private string[] TruckList = new string[] { "Biff", "Mixer", "Hauler", "Mule", "Flatbed", "Packer", "Pounder" };
        private Ped _Suspect;
        private Vehicle _Truck;
        private Vector3 _SpawnPoint;
        private Blip _Blip;
        private LHandle _Pursuit;
        private bool _PursuitCreated = false;
        private bool _notificationDisplayed = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 50f);
            CalloutMessage = "~b~Dispatch:~s~ Stolen Truck";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_GRAND_THEFT_AUTO IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: StolenTruck callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Stolen Truck", "~b~Dispatch: ~w~A Truck was stolen. Respond with ~r~Code 3");

            _Truck = new Vehicle(this.TruckList[new Random().Next((int)TruckList.Length)], _SpawnPoint);
            _Truck.IsPersistent = true;

            _Suspect = _Truck.CreateRandomDriver();
            _Suspect.BlockPermanentEvents = true;
            _Suspect.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.Emergency);

            _Blip = _Suspect.AttachBlip();
            _Blip.IsFriendly = false;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (_Truck.DistanceTo(Game.LocalPlayer.Character) < 20f && !_notificationDisplayed)
            {
                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "Loading ~g~Informations~w~ of the ~y~LSPD Database~w~...");
                GameFiber.Wait(1000);
                Functions.DisplayVehicleRecord(_Truck, true); 
                _notificationDisplayed = true;
            }
            if (!_PursuitCreated && Game.LocalPlayer.Character.DistanceTo(_Suspect.Position) < 30f)
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
            if (_Truck.Exists()) { _Truck.Dismiss(); }
            if (_Blip.Exists()) { _Blip.Delete(); }
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Stolen Truck", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
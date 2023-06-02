using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage.Native;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Money Truck Theft", CalloutProbability.Medium)]
    public class MoneyTruckTheft : Callout
    {
        private Vehicle _stockade;
        private Ped _Aggressor1;
        private Ped _Aggressor2;
        private Ped _Aggressor3;
        private Ped _Aggressor4;
        private Vector3 _SpawnPoint;
        private Vector3 _vehicleSpawnPoint;
        private Blip _Blip1;
        private Blip _Blip2;
        private Blip _Blip3;
        private Blip _Blip4;
        private LHandle _Pursuit;
        private bool _PursuitCreated = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            _vehicleSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            CalloutMessage = "[UC]~w~ Reports of a Money Truck Theft.";
            CalloutPosition = _vehicleSpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_01 OFFICERS_REPORT_01 BANK_CAR UNITS_RESPOND_CODE_99_01", _vehicleSpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: Money Truck Theft callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Money Truck Theft", "~b~Dispatch: ~w~Get the MoneyTruck back. Respond with ~r~Code3");

            _stockade = new Vehicle("STOCKADE", _vehicleSpawnPoint);
            _Aggressor1 = new Ped("g_m_m_chemwork_01", _SpawnPoint, 0f);
            _Aggressor2 = new Ped("g_m_m_chemwork_01", _SpawnPoint, 0f);
            _Aggressor3 = new Ped("g_m_m_chemwork_01", _SpawnPoint, 0f);
            _Aggressor4 = new Ped("g_m_m_chemwork_01", _SpawnPoint, 0f);
            _Aggressor1.WarpIntoVehicle(_stockade, -1);
            _Aggressor2.WarpIntoVehicle(_stockade, -2);
            _Aggressor3.WarpIntoVehicle(_stockade, 1);
            _Aggressor4.WarpIntoVehicle(_stockade, 2);
            _Aggressor1.Inventory.GiveNewWeapon("weapon_gusenberg", 5000, true);
            _Aggressor2.Inventory.GiveNewWeapon("weapon_assaultrifle", 5000, true);
            _Aggressor3.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);
            _Aggressor4.Inventory.GiveNewWeapon("weapon_specialcarbine", 5000, true);
            _Aggressor1.Armor = 100;
            _Aggressor2.Armor = 100;
            _Aggressor3.Armor = 100;
            _Aggressor4.Armor = 100;

            _Pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(_Pursuit, _Aggressor1);
            Functions.AddPedToPursuit(_Pursuit, _Aggressor2);
            Functions.AddPedToPursuit(_Pursuit, _Aggressor3);
            Functions.AddPedToPursuit(_Pursuit, _Aggressor4);
            Functions.SetPursuitIsActiveForPlayer(_Pursuit, true);
            _PursuitCreated = true;

            _Blip1 = _Aggressor1.AttachBlip();
            _Blip2 = _Aggressor2.AttachBlip();
            _Blip3 = _Aggressor3.AttachBlip();
            _Blip4 = _Aggressor4.AttachBlip();

            NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _Aggressor1, Game.LocalPlayer.Character, 0, 1);
            NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _Aggressor2, Game.LocalPlayer.Character, 0, 1);
            NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _Aggressor3, Game.LocalPlayer.Character, 0, 1);
            NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _Aggressor4, Game.LocalPlayer.Character, 0, 1);

            if (Settings.ActivateAIBackup)
            {
                Functions.RequestBackup(_vehicleSpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                Functions.RequestBackup(_vehicleSpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.AirUnit);
            } else { Settings.ActivateAIBackup = false; }
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_Aggressor1) _Aggressor1.Delete();
            if (_Aggressor2) _Aggressor2.Delete();
            if (_Aggressor3) _Aggressor3.Delete();
            if (_Aggressor4) _Aggressor4.Delete();
            if (_stockade) _stockade.Delete();
            if (_Blip1) _Blip1.Delete();
            if (_Blip2) _Blip2.Delete();
            if (_Blip3) _Blip3.Delete();
            if (_Blip4) _Blip4.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (Game.LocalPlayer.Character.DistanceTo(_stockade) < 15f)
                {
                    _Aggressor1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    _Aggressor2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    _Aggressor3.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    _Aggressor4.Tasks.FightAgainst(Game.LocalPlayer.Character);
                }
                if (_Aggressor1.IsDead || Functions.IsPedArrested(_Aggressor1))
                {
                    if (_Blip1) _Blip1.Delete();
                }
                if (_Aggressor2.IsDead || Functions.IsPedArrested(_Aggressor2))
                {
                    if (_Blip2)  _Blip2.Delete();
                }
                if (_Aggressor3.IsDead || Functions.IsPedArrested(_Aggressor3))
                {
                    if (_Blip3)  _Blip3.Delete();
                }
                if (_Aggressor4.IsDead || Functions.IsPedArrested(_Aggressor4))
                {
                    if (_Blip4)  _Blip4.Delete();
                }
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (_Aggressor1 && _Aggressor1.IsDead && _Aggressor2 && _Aggressor2.IsDead && _Aggressor3 && _Aggressor3.IsDead && _Aggressor4&& _Aggressor4.IsDead) End();
                if (_Aggressor1 && Functions.IsPedArrested(_Aggressor1) && _Aggressor2 && Functions.IsPedArrested(_Aggressor2) && _Aggressor3 && Functions.IsPedArrested(_Aggressor3) && _Aggressor4 && Functions.IsPedArrested(_Aggressor4)) End();
            }, "Money Truck Theft [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_Blip1) _Blip1.Delete();
            if (_Blip2) _Blip2.Delete();
            if (_Blip3) _Blip3.Delete();
            if (_Blip4) _Blip4.Delete();
            if (_Aggressor1) _Aggressor1.Dismiss();
            if (_Aggressor2) _Aggressor2.Dismiss();
            if (_Aggressor3) _Aggressor3.Dismiss();
            if (_Aggressor4) _Aggressor4.Dismiss();
            if (_stockade) _stockade.Dismiss();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Money Truck Theft", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
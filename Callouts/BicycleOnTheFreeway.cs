using System;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using UnitedCallouts.Stuff;
using System.Collections.Generic;

namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Bicycle on the Freeway", CalloutProbability.Medium)]
    public class BicycleOnTheFreeway : Callout
    {
        private string[] pedList = new string[] { "A_F_Y_Hippie_01", "A_M_Y_Skater_01", "A_M_M_FatLatin_01", "A_M_M_EastSA_01", "A_M_Y_Latino_01", "G_M_Y_FamDNF_01", "G_M_Y_FamCA_01", "G_M_Y_BallaSout_01", "G_M_Y_BallaOrig_01", "G_M_Y_BallaEast_01", "G_M_Y_StrPunk_02", "S_M_Y_Dealer_01", "A_M_M_RurMeth_01", "A_M_Y_MethHead_01", "A_M_M_Skidrow_01", "S_M_Y_Dealer_01", "a_m_y_mexthug_01", "G_M_Y_MexGoon_03", "G_M_Y_MexGoon_02", "G_M_Y_MexGoon_01", "G_M_Y_SalvaGoon_01", "G_M_Y_SalvaGoon_02", "G_M_Y_SalvaGoon_03", "G_M_Y_Korean_01", "G_M_Y_Korean_02", "G_M_Y_StrPunk_01" };
        private string[] Bicycles = new string[] { "bmx", "Cruiser", "Fixter", "Scorcher", "tribike3", "tribike2", "tribike" };
        private Ped _subject;
        private Vehicle _Bike;
        private Vector3 _SpawnPoint;
        private Blip _Blip;
        private LHandle _pursuit;
        private bool _IsStolen = false;
        private bool _startedPursuit = false;
        private bool _alreadySubtitleIntrod = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            Random random = new Random();
            List<Vector3> list = new List<Vector3>
            {
               new Vector3(1720.068f, 1535.201f, 84.72424f),
               new Vector3(2563.921f, 5393.056f, 44.55834f),
               new Vector3(-1826.79f, 4697.899f, 56.58701f),
               new Vector3(-1344.75f, -757.6135f, 11.10569f),
               new Vector3(1163.919f, 449.0514f, 82.59987f),

        };
            _SpawnPoint = Vector3Extension.chooseNearestLocation(list);
            ShowCalloutAreaBlipBeforeAccepting(_SpawnPoint, 100f);
            switch (new Random().Next(1, 2))
            {
                case 1:
                    _IsStolen = true;
                    break;
                case 2:
                    break;
            }
            CalloutMessage = "[UC]~w~ Reports of a Bicycle on the Freeway";
            CalloutPosition = _SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS SUSPICIOUS_PERSON IN_OR_ON_POSITION", _SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("UnitedCallouts Log: Bicycle On the Freeway callout accepted.");
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Bicycle on the Freeway", "~b~Dispatch:~w~ Someone called the police because there is someone with a ~g~bicycle~w~ on the ~o~freeway~w~. Respond with ~y~Code 2");

            _subject = new Ped(pedList[new Random().Next((int)pedList.Length)], _SpawnPoint, 0f);
            _Bike = new Vehicle(Bicycles[new Random().Next((int)Bicycles.Length)], _SpawnPoint, 0f);
            _subject.WarpIntoVehicle(_Bike, -1);

            _Blip = _Bike.AttachBlip();
            _Blip.Color = Color.LightBlue;
            _Blip.EnableRoute(Color.Yellow);

            _subject.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.FollowTraffic);
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_subject) _subject.Delete();
            if (_Bike) _Bike.Delete();
            if (_Blip) _Blip.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_subject.DistanceTo(Game.LocalPlayer.Character) < 20f)
                {
                    if (_IsStolen == true && _startedPursuit == false)
                    {
                        if (_Blip) _Blip.Delete();
                        _pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(_pursuit, _subject);
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        _startedPursuit = true;
                        _Bike.IsStolen = true;
                        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch Information", "The ~g~bicycle~w~ from the suspect is a ~o~" + _Bike.Model.Name + "~w~. The ~g~bicycle~w~ was ~r~stolen~w~.");
                        GameFiber.Wait(2000);
                    }
                    if (_subject.DistanceTo(Game.LocalPlayer.Character) < 25f && Game.LocalPlayer.Character.IsOnFoot && _alreadySubtitleIntrod == false && _pursuit == null)
                    {
                        Game.DisplayNotification("Perform a normal traffic stop with the ~o~suspect~w~.");
                        Game.DisplayNotification("~b~Dispatch:~w~ Checking the serial number of the bike...");
                        GameFiber.Wait(2000);
                        Game.DisplayNotification("~b~Dispatch~w~ We checked the serial number of the bike.<br>Model: ~o~" + _Bike.Model.Name + "<br>~w~Serial number: ~o~" + _Bike.LicensePlate);
                        _alreadySubtitleIntrod = true;
                        return;
                    }
                }
                if (_subject && Functions.IsPedArrested(_subject) && _IsStolen && _subject.DistanceTo(Game.LocalPlayer.Character) < 15f)
                {
                    Game.DisplaySubtitle("~y~Suspect: ~w~Please let me go! I bring the bike back.", 4000);
                }
                if (Game.LocalPlayer.Character.IsDead) End();
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (_subject && _subject.IsDead) End();
                if (_subject && Functions.IsPedArrested(_subject)) End();
            }, "Bicycle on the Freeway [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_subject) _subject.Dismiss();
            if (_Bike) _Bike.Dismiss();
            if (_Blip) _Blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Bicycle on the Freeway", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
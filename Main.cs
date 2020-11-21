using Rage;
using LSPD_First_Response.Mod.API;
using UnitedCallouts.Callouts;
using UnitedCallouts.VersionChecker;
using System.Reflection;

namespace UnitedCallouts
{
    public class Main : Plugin
    {
        public override void Finally()
        {
            Game.DisplayHelp("UnitedCallouts loaded without problems.");
        }
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
            Settings.LoadSettings();
            Game.LogTrivial("UnitedCallouts plugin has loaded!");
        }
        static void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            if (onDuty)
                GameFiber.StartNew(delegate
                {
                    RegisterCallouts();
                    Game.Console.Print();
                    Game.Console.Print("=============================================== UnitedCallouts by sEbi3 ================================================");
                    Game.Console.Print();
                    Game.Console.Print("[LOG]: Callouts and settings loaded succesfully.");
                    Game.Console.Print("[LOG]: The config file loaded succesfully.");
                    Game.Console.Print("[VERSION]: Detected Version:  " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    Game.Console.Print("[LOG]: You are playing on the newest version.");
                    Game.Console.Print();
                    Game.Console.Print("=============================================== UnitedCallouts by sEbi3 ================================================");
                    Game.Console.Print();

                    Game.DisplayNotification(
                        "web_lossantospolicedept", // You can find all logos/images in OpenIV
                        "web_lossantospolicedept", // You can find all logos/images in OpenIV
                        "UnitedCallouts", // Title
                        "~y~v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + 
                        " ~o~by sEbi3", "~b~successfully loaded!"); // Subtitle
                    PluginCheck.isUpdateAvailable();
                });
        }
        private static void RegisterCallouts() //Register all your callouts here
        {
            Game.Console.Print();
            Game.Console.Print("=============================================== UnitedCallouts Information ================================================");
            Game.Console.Print();
            if (Settings.Burglary) { Functions.RegisterCallout(typeof(Burglary)); Game.LogTrivial("Burglary loaded"); }
            if (Settings.DrugDeal) { Functions.RegisterCallout(typeof(DrugDeal)); Game.LogTrivial("DrugDeal loaded"); }
            if (Settings.StolenTruck) { Functions.RegisterCallout(typeof(StolenTruck)); Game.LogTrivial("StolenTruck loaded"); }
            if (Settings.StolenBus) { Functions.RegisterCallout(typeof(StolenBus)); Game.LogTrivial("StolenBus loaded"); }
            if (Settings.KillerClownWasSeen) { Functions.RegisterCallout(typeof(KillerClownWasSeen)); Game.LogTrivial("KillerClownWasSeen loaded"); }
            if (Settings.MoneyTruckIsRobbed) { Functions.RegisterCallout(typeof(MoneyTruckIsRobbed)); Game.LogTrivial("MoneyTruckIsRobbed loaded"); }
            if (Settings.Fighting) { Functions.RegisterCallout(typeof(Fighting)); Game.LogTrivial("Fighting loaded"); }
            if (Settings.DrugDealer) { Functions.RegisterCallout(typeof(DrugDealer)); Game.LogTrivial("DrugDealer loaded"); }
            if (Settings.StolenEmergencyVehicle2) { Functions.RegisterCallout(typeof(StolenEmergencyVehicle2)); Game.LogTrivial("StolenEmergencyVehicle loaded"); }
            if (Settings.PersonWithKnife) { Functions.RegisterCallout(typeof(PersonWithKnife)); Game.LogTrivial("PersonWithKnife loaded"); }
            if (Settings.StolenEmergencyVehicle) { Functions.RegisterCallout(typeof(StolenEmergencyVehicle)); Game.LogTrivial("StolenEmergencyVehicle2 loaded"); }
            if (Settings.RobberyHL) { Functions.RegisterCallout(typeof(RobberyHL)); Game.LogTrivial("RobberyHL loaded"); }
            if (Settings.ArrestWarrant) { Functions.RegisterCallout(typeof(ArrestWarrant)); Game.LogTrivial("ArrestWarrant loaded"); }
            if (Settings.troublemaker) { Functions.RegisterCallout(typeof(troublemaker)); Game.LogTrivial("troublemaker loaded"); }
            if (Settings.CarTrade) { Functions.RegisterCallout(typeof(CarTrade)); Game.LogTrivial("CarTrade loaded"); }
            if (Settings.ArmoredPerson) { Functions.RegisterCallout(typeof(ArmoredPerson)); Game.LogTrivial("ArmoredPerson loaded"); }
            if (Settings.ShotsFired) { Functions.RegisterCallout(typeof(ShotsFired)); Game.LogTrivial("ShotsFired loaded"); }
            if (Settings.Bicycle) { Functions.RegisterCallout(typeof(Bicycle)); Game.LogTrivial("Bicycle loaded"); }
            if (Settings.WelfareCheck) { Functions.RegisterCallout(typeof(WelfareCheck)); Game.LogTrivial("WelfareCheck loaded"); }
            if (Settings.K9Backup) { Functions.RegisterCallout(typeof(K9Backup)); Game.LogTrivial("K9Backup loaded"); }
            if (Settings.store) { Functions.RegisterCallout(typeof(store)); Game.LogTrivial("store loaded"); }
            if (Settings.TrafficBackup) { Functions.RegisterCallout(typeof(TrafficBackup)); Game.LogTrivial("TrafficBackup loaded"); }
            if (Settings.Hostages) { Functions.RegisterCallout(typeof(Hostages)); Game.LogTrivial("Hostages loaded"); }
            Game.Console.Print();
            Game.Console.Print("=============================================== UnitedCallouts Information ================================================");
            Game.Console.Print();
        }
    }
}
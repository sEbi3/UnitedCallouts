using Rage;
using LSPD_First_Response.Mod.API;
using UnitedCallouts.Callouts;
using UnitedCallouts.VersionChecker;
using System.Reflection;

namespace UnitedCallouts
{
    public class Main : Plugin
    {
        public override void Finally() { }

        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
            Settings.LoadSettings();
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
                    Game.Console.Print("[LOG]: Callouts and settings were loaded successfully.");
                    Game.Console.Print("[LOG]: The config file was loaded succesfully.");
                    Game.Console.Print("[VERSION]: Detected Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    Game.Console.Print("[LOG]: Checking for a new UnitedCallouts version.");
                    Game.Console.Print();
                    Game.Console.Print("=============================================== UnitedCallouts by sEbi3 ================================================");
                    Game.Console.Print();

                    // You can find all textures/images in OpenIV
                    Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "UnitedCallouts", "~y~v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " ~o~by sEbi3", "~b~successfully loaded!");
                    PluginCheck.isUpdateAvailable();

                    GameFiber.Wait(300);
                    Game.DisplayHelp("You can change all ~y~keys~w~ in the ~g~UnitedCallouts.ini~w~. Press ~b~" + Settings.EndCall + "~w~ to end a callout.", 5000);
                });
        }
        private static void RegisterCallouts() //Register all your callouts here
        {
            Game.Console.Print();
            Game.Console.Print("================================================== UnitedCallouts ===================================================");
            Game.Console.Print();
            if (Settings.Burglary) { Functions.RegisterCallout(typeof(Burglary)); }
            if (Settings.DrugDeal) { Functions.RegisterCallout(typeof(DrugDeal)); }
            if (Settings.StolenTruck) { Functions.RegisterCallout(typeof(StolenTruck)); }
            if (Settings.StolenBus) { Functions.RegisterCallout(typeof(StolenBus)); }
            if (Settings.KillerClownWasSeen) { Functions.RegisterCallout(typeof(KillerClownWasSeen)); }
            if (Settings.GangAttack) { Functions.RegisterCallout(typeof(GangAttack)); }
            if (Settings.MoneyTruckIsRobbed) { Functions.RegisterCallout(typeof(MoneyTruckIsRobbed)); }
            if (Settings.Fighting) { Functions.RegisterCallout(typeof(Fighting)); }
            if (Settings.StolenEmergencyVehicle2) { Functions.RegisterCallout(typeof(StolenEmergencyVehicle2)); }
            if (Settings.PersonWithKnife) { Functions.RegisterCallout(typeof(PersonWithKnife)); }
            if (Settings.StolenEmergencyVehicle) { Functions.RegisterCallout(typeof(StolenEmergencyVehicle)); }
            if (Settings.RobberyHL) { Functions.RegisterCallout(typeof(RobberyHL)); }
            if (Settings.ArrestWarrant) { Functions.RegisterCallout(typeof(ArrestWarrant)); }
            if (Settings.troublemaker) { Functions.RegisterCallout(typeof(troublemaker)); }
            if (Settings.CarTrade) { Functions.RegisterCallout(typeof(CarTrade)); }
            if (Settings.ArmoredPerson) { Functions.RegisterCallout(typeof(ArmoredPerson)); }
            if (Settings.ShotsFired) { Functions.RegisterCallout(typeof(ShotsFired)); }
            if (Settings.Bicycle) { Functions.RegisterCallout(typeof(Bicycle)); }
            if (Settings.WelfareCheck) { Functions.RegisterCallout(typeof(WelfareCheck)); }
            if (Settings.K9Backup) { Functions.RegisterCallout(typeof(K9Backup)); }
            if (Settings.store) { Functions.RegisterCallout(typeof(store)); }
            if (Settings.TrafficBackup) { Functions.RegisterCallout(typeof(TrafficBackup)); }
            if (Settings.Hostages) { Functions.RegisterCallout(typeof(Hostages)); }
            if (Settings.JewelleryRobbery) { Functions.RegisterCallout(typeof(JewelleryRobbery)); }
            if (Settings.ATMActivity) { Functions.RegisterCallout(typeof(ATMActivity)); }
            Game.Console.Print("[LOG]: All callouts of the UnitedCallouts.ini were loaded succesfully.");
            Game.Console.Print();
            Game.Console.Print("================================================== UnitedCallouts ===================================================");
            Game.Console.Print();
        }
    }
}
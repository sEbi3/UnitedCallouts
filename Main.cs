using UnitedCallouts.Callouts;
using UnitedCallouts.VersionChecker;
using System.Reflection;

namespace UnitedCallouts;

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
                Game.Console.Print("[VERSION]: Installed Version: " + Assembly.GetExecutingAssembly().GetName().Version);
                Game.Console.Print("[LOG]: Checking the configuration file...");
                if (Settings.Config.Exists()) Game.Console.Print("[LOG]: UnitedCallouts configuration file is installed.");
                else Game.Console.Print("[WARNING]: UnitedCallouts configuration file is missing or was installed incorrectly.");
                Game.Console.Print("[LOG]: Checking for a new UnitedCallouts version...");
                //Game.Console.Print();
                //Game.Console.Print("[WARNING]: This is an unstable build of UnitedCallouts containing bug fixes, important changes and more. You may notice bugs while playing.");
                //Game.Console.Print("[WARNING]: The version control system may let you know about an older version available for UnitedCallouts as this build is not public yet.");
                Game.Console.Print();
                Game.Console.Print("=============================================== UnitedCallouts by sEbi3 ================================================");
                Game.Console.Print();

                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "UnitedCallouts", "~y~v" + Assembly.GetExecutingAssembly().GetName().Version + " ~o~by sEbi3", "~b~successfully loaded!");
                //Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "UnitedCallouts", "~y~Unstable Build", "This is an ~r~unstable build~w~ of UnitedCallouts containing bug fixes, important changes and more. You may notice bugs while playing with this build.");

                PluginCheck.IsUpdateAvailable();
                GameFiber.Wait(300);
                if (Settings.HelpMessages)
                {
                    Game.DisplayHelp("~g~UnitedCallouts ~w~loaded successfully. Configure your keybinds in the ~y~unitedcallouts.ini~w~. Current keybind to end a callout is: ~y~" + Settings.EndCall + "~w~.", 8000);
                } else { Settings.HelpMessages = false; }
            });
    }
    private static void RegisterCallouts()
    {
        Game.Console.Print();
        Game.Console.Print("=============================================== UnitedCallouts by sEbi3 ================================================");
        Game.Console.Print();
        if (Settings.ResidentialBurglary) { Functions.RegisterCallout(typeof(ResidentialBurglary)); }
        if (Settings.DrugTransaction) { Functions.RegisterCallout(typeof(DrugTransaction)); }
        if (Settings.StolenCommercialVehicle) { Functions.RegisterCallout(typeof(StolenCommercialVehicle)); }
        if (Settings.StolenBusIncident) { Functions.RegisterCallout(typeof(StolenBusIncident)); }
        if (Settings.ArmedClown) { Functions.RegisterCallout(typeof(ArmedClown)); }
        if (Settings.GangShootout) { Functions.RegisterCallout(typeof(GangShootout)); }
        if (Settings.MoneyTruckTheft) { Functions.RegisterCallout(typeof(MoneyTruckTheft)); }
        if (Settings.PublicPeaceDisturbance) { Functions.RegisterCallout(typeof(PublicPeaceDisturbance)); }
        if (Settings.PersonWithKnife) { Functions.RegisterCallout(typeof(PersonWithKnife)); }
        if (Settings.StolenEmergencyVehicle) { Functions.RegisterCallout(typeof(StolenEmergencyVehicle)); }
        if (Settings.RobberyAtHumaneLabs) { Functions.RegisterCallout(typeof(RobberyAtHumaneLabs)); }
        if (Settings.WarrantForArrest) { Functions.RegisterCallout(typeof(WarrantForArrest)); }
        if (Settings.DisturbanceAtMetroStation) { Functions.RegisterCallout(typeof(DisturbanceAtMetroStation)); }
        if (Settings.IllegalPoliceCarTrade) { Functions.RegisterCallout(typeof(IllegalPoliceCarTrade)); }
        if (Settings.ArmedTerroristAttack) { Functions.RegisterCallout(typeof(ArmedTerroristAttack)); }
        if (Settings.ShotsFired) { Functions.RegisterCallout(typeof(ShotsFired)); }
        if (Settings.CyclistOnTheHighway) { Functions.RegisterCallout(typeof(CyclistOnTheHighway)); }
        if (Settings.WelfareCheckRequest) { Functions.RegisterCallout(typeof(WelfareCheckRequest)); }
        if (Settings.K9BackupRequired) { Functions.RegisterCallout(typeof(K9BackupRequired)); }
        if (Settings.StoreRobberyInProgress) { Functions.RegisterCallout(typeof(StoreRobberyInProgress)); }
        if (Settings.TrafficStopBackupRequired) { Functions.RegisterCallout(typeof(TrafficStopBackupRequired)); }
        if (Settings.HostageSituationReported) { Functions.RegisterCallout(typeof(HostageSituationReported)); }
        if (Settings.JewelryRobbery) { Functions.RegisterCallout(typeof(JewelryRobbery)); }
        if (Settings.SuspiciousAtmActivity) { Functions.RegisterCallout(typeof(SuspiciousAtmActivity)); }
        if (Settings.MurderInvestigation) { Functions.RegisterCallout(typeof(MurderInvestigation)); }
        Game.Console.Print("[LOG]: All callouts from the configuration file were loaded successfully.");
        Game.Console.Print();
        Game.Console.Print("=============================================== UnitedCallouts by sEbi3 ================================================");
        Game.Console.Print();
    }
}
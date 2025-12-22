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
                Game.Console.Print("[LOG]: The config file was loaded successfully.");
                Game.Console.Print("[VERSION]: Detected Version: " + Assembly.GetExecutingAssembly().GetName().Version);
                Game.Console.Print("[LOG]: Checking for a new UnitedCallouts version...");
                Game.Console.Print();
                Game.Console.Print("=============================================== UnitedCallouts by sEbi3 ================================================");
                Game.Console.Print();

                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "UnitedCallouts", "~y~v" + Assembly.GetExecutingAssembly().GetName().Version + " ~o~by sEbi3", "~b~successfully loaded!");
                // Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "UnitedCallouts", "~y~Unstable Build", "This is an ~r~unstable build~w~ of UnitedCallouts for testing. You may notice bugs while playing the unstable build.");
                  
                PluginCheck.IsUpdateAvailable();
                GameFiber.Wait(300);
                if (Settings.HelpMessages)
                {
                    Game.DisplayHelp("You can change all ~y~keys~w~ in the ~g~UnitedCallouts.ini~w~. Press ~b~" + Settings.EndCall + "~w~ to end a callout.", 5000);
                } else { Settings.HelpMessages = false; }
            });
    }
    private static void RegisterCallouts()
    {
        Game.Console.Print();
        Game.Console.Print("================================================== UnitedCallouts ===================================================");
        Game.Console.Print();
        if (Settings.ApartmentBurglary) { Functions.RegisterCallout(typeof(ApartmentBurglary)); }
        if (Settings.DrugDeal) { Functions.RegisterCallout(typeof(DrugDeal)); }
        if (Settings.StolenTruckPursuit) { Functions.RegisterCallout(typeof(StolenTruckPursuit)); }
        if (Settings.StolenBusIncident) { Functions.RegisterCallout(typeof(StolenBusIncident)); }
        if (Settings.ArmedClown) { Functions.RegisterCallout(typeof(ArmedClown)); }
        if (Settings.GangShootout) { Functions.RegisterCallout(typeof(GangShootout)); }
        if (Settings.MoneyTruckTheft) { Functions.RegisterCallout(typeof(MoneyTruckTheft)); }
        if (Settings.PublicPeaceDisturbance) { Functions.RegisterCallout(typeof(PublicPeaceDisturbance)); }
        if (Settings.StolenEmergencyVehicle2) { Functions.RegisterCallout(typeof(StolenEmergencyVehicle2)); }
        if (Settings.PersonWithKnife) { Functions.RegisterCallout(typeof(PersonWithKnife)); }
        if (Settings.StolenEmergencyVehicle) { Functions.RegisterCallout(typeof(StolenEmergencyVehicle)); }
        if (Settings.RobberyHl) { Functions.RegisterCallout(typeof(RobberyHl)); }
        if (Settings.WarrantForArrest) { Functions.RegisterCallout(typeof(WarrantForArrest)); }
        if (Settings.Troublemaker) { Functions.RegisterCallout(typeof(Troublemaker)); }
        if (Settings.IllegalPoliceCarTrade) { Functions.RegisterCallout(typeof(IllegalPoliceCarTrade)); }
        if (Settings.ArmedTerroristAttack) { Functions.RegisterCallout(typeof(ArmedTerroristAttack)); }
        if (Settings.ShotsFired) { Functions.RegisterCallout(typeof(ShotsFired)); }
        if (Settings.BicycleOnTheFreeway) { Functions.RegisterCallout(typeof(BicycleOnTheFreeway)); }
        if (Settings.WelfareCheckRequest) { Functions.RegisterCallout(typeof(WelfareCheckRequest)); }
        if (Settings.K9BackupRequired) { Functions.RegisterCallout(typeof(K9BackupRequired)); }
        if (Settings.StoreRobberyInProgress) { Functions.RegisterCallout(typeof(StoreRobberyInProgress)); }
        if (Settings.TrafficStopBackupRequired) { Functions.RegisterCallout(typeof(TrafficStopBackupRequired)); }
        if (Settings.HostageSituationReported) { Functions.RegisterCallout(typeof(HostageSituationReported)); }
        if (Settings.JewelleryRobbery) { Functions.RegisterCallout(typeof(JewelleryRobbery)); }
        if (Settings.SuspiciousAtmActivity) { Functions.RegisterCallout(typeof(SuspiciousAtmActivity)); }
        if (Settings.MurderInvestigation) { Functions.RegisterCallout(typeof(MurderInvestigation)); }
        Game.Console.Print("[LOG]: All callouts of the UnitedCallouts.ini were loaded successfully.");
        Game.Console.Print();
        Game.Console.Print("================================================== UnitedCallouts ===================================================");
        Game.Console.Print();
    }
}

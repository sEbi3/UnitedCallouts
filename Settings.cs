using System.Windows.Forms;

namespace UnitedCallouts;

internal static class Settings
{
    public static readonly InitializationFile Config = new InitializationFile(@"Plugins/LSPDFR/UnitedCallouts/UnitedCallouts.ini");

    internal static bool ResidentialBurglary = true;
    internal static bool RobberyAtHumaneLabs = true;
    internal static bool StolenEmergencyVehicle = true;
    internal static bool GangShootout = true;
    internal static bool DrugTransaction = true;
    internal static bool ArmedClown = true;
    internal static bool PublicPeaceDisturbance = true;
    internal static bool PersonWithKnife = true;
    internal static bool StolenBusIncident = true;
    internal static bool StolenCommercialVehicle = true;
    internal static bool MoneyTruckTheft = true;
    internal static bool WarrantForArrest = true;
    internal static bool DisturbanceAtMetroStation = true;
    internal static bool IllegalPoliceCarTrade = true;
    internal static bool ArmedTerroristAttack = true;
    internal static bool ShotsFired = true;
    internal static bool CyclistOnTheHighway = true;
    internal static bool WelfareCheckRequest = true;
    internal static bool K9BackupRequired = true;
    internal static bool StoreRobberyInProgress = true;
    internal static bool TrafficStopBackupRequired = true;
    internal static bool HostageSituationReported = true;
    internal static bool JewelryRobbery = true;
    internal static bool SuspiciousAtmActivity = true;
    internal static bool MurderInvestigation = true;
    internal static bool ActivateAiBackup = true;
    internal static bool HelpMessages = true;
    internal static bool DetailedLogging = true;
    internal static Keys EndCall = Keys.End;
    internal static Keys Dialog = Keys.Y;

    internal static void LoadSettings()
    {
        Game.LogTrivial("[UnitedCallouts LOG]: Loading configuration file from UnitedCallouts.");
        ResidentialBurglary = Config.ReadBoolean("Callouts", "ResidentialBurglary", true);
        WarrantForArrest = Config.ReadBoolean("Callouts", "WarrantForArrest", true);
        RobberyAtHumaneLabs = Config.ReadBoolean("Callouts", "RobberyAtHumaneLabs", true);
        StolenEmergencyVehicle = Config.ReadBoolean("Callouts", "StolenEmergencyVehicle", true);
        GangShootout = Config.ReadBoolean("Callouts", "GangShootout", true);
        DrugTransaction = Config.ReadBoolean("Callouts", "DrugTransaction", true);
        ArmedClown = Config.ReadBoolean("Callouts", "ArmedClown", true);
        PublicPeaceDisturbance = Config.ReadBoolean("Callouts", "PublicPeaceDisturbance", true);
        PersonWithKnife = Config.ReadBoolean("Callouts", "PersonWithKnife", true);
        StolenBusIncident = Config.ReadBoolean("Callouts", "StolenBusIncident", true);
        StolenCommercialVehicle = Config.ReadBoolean("Callouts", "StolenCommercialVehicle", true);
        MoneyTruckTheft = Config.ReadBoolean("Callouts", "MoneyTruckTheft", true);
        DisturbanceAtMetroStation = Config.ReadBoolean("Callouts", "DisturbanceAtMetroStation", true);
        IllegalPoliceCarTrade = Config.ReadBoolean("Callouts", "IllegalPoliceCarTrade", true);
        ArmedTerroristAttack = Config.ReadBoolean("Callouts", "ArmedTerroristAttack", true);
        ShotsFired = Config.ReadBoolean("Callouts", "ShotsFired", true);
        CyclistOnTheHighway = Config.ReadBoolean("Callouts", "CyclistOnTheHighway", true);
        WelfareCheckRequest = Config.ReadBoolean("Callouts", "WelfareCheckRequest", true);
        K9BackupRequired = Config.ReadBoolean("Callouts", "K9BackupRequired", true);
        StoreRobberyInProgress = Config.ReadBoolean("Callouts", "StoreRobberyInProgress", true);
        TrafficStopBackupRequired = Config.ReadBoolean("Callouts", "TrafficStopBackupRequired", true);
        HostageSituationReported = Config.ReadBoolean("Callouts", "HostageSituationReported", true);
        JewelryRobbery = Config.ReadBoolean("Callouts", "JewelryRobbery", true);
        SuspiciousAtmActivity = Config.ReadBoolean("Callouts", "SuspiciousATMActivity", true);
        MurderInvestigation = Config.ReadBoolean("Callouts", "MurderInvestigation", true);
        ActivateAiBackup = Config.ReadBoolean("Settings", "ActivateAIBackup", true);
        HelpMessages = Config.ReadBoolean("Settings", "HelpMessages", true);
        DetailedLogging = Config.ReadBoolean("Settings", "DetailedLogging", false);
        EndCall = Config.ReadEnum("Keys", "EndCall", Keys.End);
        Dialog = Config.ReadEnum("Keys", "Dialog", Keys.Y);
    }
    public static readonly string PluginVersion = "1.5.8.2";
}

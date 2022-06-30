using System.Windows.Forms;
using Rage;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace UnitedCallouts
{
    internal static class Settings
    {
        internal static bool ApartmentBurglary = true;
        internal static bool RobberyHL = true;
        internal static bool StolenEmergencyVehicle = true;
        internal static bool StolenEmergencyVehicle2 = true;
        internal static bool GangShootout = true;
        internal static bool DrugDeal = true;
        internal static bool ArmedClown = true;
        internal static bool PublicPeaceDisturbance = true;
        internal static bool PersonWithKnife = true;
        internal static bool StolenBusIncident = true;
        internal static bool StolenTruckPursuit = true;
        internal static bool MoneyTruckTheft = true;
        internal static bool WarrantForArrest = true;
        internal static bool Troublemaker = true;
        internal static bool IllegalPoliceCarTrade = true;
        internal static bool ArmedTerroristAttack = true;
        internal static bool ShotsFired = true;
        internal static bool BicycleOnTheFreeway = true;
        internal static bool WelfareCheckRequest = true;
        internal static bool K9BackupRequired = true;
        internal static bool StoreRobberyInProgress = true;
        internal static bool TrafficStopBackupRequired = true;
        internal static bool HostageSituationReported = true;
        internal static bool JewelleryRobbery = true;
        internal static bool SuspiciousATMActivity = true;
        internal static bool MurderInvestigation = true;
        internal static bool ActivateAIBackup = true;
        internal static bool HelpMessages = true;
        internal static Keys EndCall = Keys.End;
        internal static Keys Dialog = Keys.Y;

        internal static void LoadSettings()
        {
            Game.LogTrivial("[LOG]: Loading config file from UnitedCallouts.");
            var path = "Plugins/LSPDFR/UnitedCallouts/UnitedCallouts.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            ApartmentBurglary = ini.ReadBoolean("Callouts", "ApartmentBurglary", true);
            WarrantForArrest = ini.ReadBoolean("Callouts", "WarrantForArrest", true);
            RobberyHL = ini.ReadBoolean("Callouts", "RobberyHL", true);
            StolenEmergencyVehicle = ini.ReadBoolean("Callouts", "StolenEmergencyVehicle", true);
            StolenEmergencyVehicle2 = ini.ReadBoolean("Callouts", "StolenEmergencyVehicle2", true);
            GangShootout = ini.ReadBoolean("Callouts", "GangShootout", true);
            DrugDeal = ini.ReadBoolean("Callouts", "DrugDeal", true);
            ArmedClown = ini.ReadBoolean("Callouts", "ArmedClown", true);
            PublicPeaceDisturbance = ini.ReadBoolean("Callouts", "PublicPeaceDisturbance", true);
            PersonWithKnife = ini.ReadBoolean("Callouts", "PersonWithKnife", true);
            StolenBusIncident = ini.ReadBoolean("Callouts", "StolenBusIncident", true);
            StolenTruckPursuit = ini.ReadBoolean("Callouts", "StolenTruckPursuit", true);
            MoneyTruckTheft = ini.ReadBoolean("Callouts", "MoneyTruckTheft", true);
            Troublemaker = ini.ReadBoolean("Callouts", "Troublemaker", true);
            IllegalPoliceCarTrade = ini.ReadBoolean("Callouts", "IllegalPoliceCarTrade", true);
            ArmedTerroristAttack = ini.ReadBoolean("Callouts", "ArmedTerroristAttack", true);
            ShotsFired = ini.ReadBoolean("Callouts", "ShotsFired", true);
            BicycleOnTheFreeway = ini.ReadBoolean("Callouts", "BicycleOnTheFreeway", true);
            WelfareCheckRequest = ini.ReadBoolean("Callouts", "WelfareCheckRequest", true);
            K9BackupRequired = ini.ReadBoolean("Callouts", "K9BackupRequired", true);
            StoreRobberyInProgress = ini.ReadBoolean("Callouts", "StoreRobberyInProgress", true);
            TrafficStopBackupRequired = ini.ReadBoolean("Callouts", "TrafficStopBackupRequired", true);
            HostageSituationReported = ini.ReadBoolean("Callouts", "HostageSituationReported", true);
            JewelleryRobbery = ini.ReadBoolean("Callouts", "JewelleryRobbery", true);
            SuspiciousATMActivity = ini.ReadBoolean("Callouts", "SuspiciousATMActivity", true);
            MurderInvestigation = ini.ReadBoolean("Callouts", "MurderInvestigation", true);
            ActivateAIBackup = ini.ReadBoolean("Settings", "ActivateAIBackup", true);
            ActivateAIBackup = ini.ReadBoolean("Settings", "HelpMessages", true);
            EndCall = ini.ReadEnum("Keys", "EndCall", Keys.End);
            Dialog = ini.ReadEnum("Keys", "Dialog", Keys.Y);
        }
        public static readonly string PluginVersion = "1.5.7.9";
    }
}
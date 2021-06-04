using System.Windows.Forms;
using Rage;

namespace UnitedCallouts
{
    internal static class Settings
    {
        internal static bool Burglary = true;
        internal static bool RobberyHL = true;
        internal static bool StolenEmergencyVehicle = true;
        internal static bool StolenEmergencyVehicle2 = true;
        internal static bool GangAttack = true;
        internal static bool DrugDeal = true;
        internal static bool KillerClownWasSeen = true;
        internal static bool Fighting = true;
        internal static bool PersonWithKnife = true;
        internal static bool StolenBus = true;
        internal static bool StolenTruck = true;
        internal static bool MoneyTruckIsRobbed = true;
        internal static bool ArrestWarrant = true;
        internal static bool troublemaker = true;
        internal static bool CarTrade = true;
        internal static bool ArmoredPerson = true;
        internal static bool ShotsFired = true;
        internal static bool Bicycle = true;
        internal static bool WelfareCheck = true;
        internal static bool K9Backup = true;
        internal static bool store = true;
        internal static bool TrafficBackup = true;
        internal static bool Hostages = true;
        internal static bool JewelleryRobbery = true;
        internal static bool ATMActivity = true;
        internal static Keys EndCall = Keys.End;
        internal static Keys Dialog = Keys.Y;

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading Config file from UnitedCallouts by sEbi3");
            var path = "Plugins/LSPDFR/UnitedCallouts.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            Burglary = ini.ReadBoolean("Callouts", "Burglary", true);
            ArrestWarrant = ini.ReadBoolean("Callouts", "ArrestWarrant", true);
            RobberyHL = ini.ReadBoolean("Callouts", "RobberyHL", true);
            StolenEmergencyVehicle = ini.ReadBoolean("Callouts", "StolenEmergencyVehicle", true);
            StolenEmergencyVehicle2 = ini.ReadBoolean("Callouts", "StolenEmergencyVehicle2", true);
            GangAttack = ini.ReadBoolean("Callouts", "GangAttack", true);
            DrugDeal = ini.ReadBoolean("Callouts", "DrugDeal", true);
            KillerClownWasSeen = ini.ReadBoolean("Callouts", "KillerClownWasSeen", true);
            Fighting = ini.ReadBoolean("Callouts", "Fighting", true);
            PersonWithKnife = ini.ReadBoolean("Callouts", "PersonWithKnife", true);
            StolenBus = ini.ReadBoolean("Callouts", "StolenBus", true);
            StolenTruck = ini.ReadBoolean("Callouts", "StolenTruck", true);
            MoneyTruckIsRobbed = ini.ReadBoolean("Callouts", "MoneyTruckIsRobbed", true);
            troublemaker = ini.ReadBoolean("Callouts", "troublemaker", true);
            CarTrade = ini.ReadBoolean("Callouts", "CarTrade", true);
            ArmoredPerson = ini.ReadBoolean("Callouts", "ArmoredPerson", true);
            ShotsFired = ini.ReadBoolean("Callouts", "ShotsFired", true);
            Bicycle = ini.ReadBoolean("Callouts", "Bicycle", true);
            WelfareCheck = ini.ReadBoolean("Callouts", "WelfareCheck", true);
            K9Backup = ini.ReadBoolean("Callouts", "K9Backup", true);
            store = ini.ReadBoolean("Callouts", "store", true);
            TrafficBackup = ini.ReadBoolean("Callouts", "TrafficBackup", true);
            Hostages = ini.ReadBoolean("Callouts", "Hostages", true);
            JewelleryRobbery = ini.ReadBoolean("Callouts", "JewelleryRobbery", true);
            ATMActivity = ini.ReadBoolean("Callouts", "ATMActivity", true);
            EndCall = ini.ReadEnum("Keys", "EndCall", Keys.End);
            Dialog = ini.ReadEnum("Keys", "Dialog", Keys.Y);
        }
        public static readonly string CalloutVersion = "1.5.7.6";
    }
}
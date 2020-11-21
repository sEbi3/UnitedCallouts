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
        internal static bool DrugDealer = true;
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
        internal static Keys EndCall = Keys.End;
        internal static Keys Dialog = Keys.Y;

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading Config file from UnitedCallouts by sEbi3");
            var path = "Plugins/LSPDFR/UnitedCallouts.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            Burglary = ini.ReadBoolean("Settings", "Burglary", true);
            ArrestWarrant = ini.ReadBoolean("Settings", "ArrestWarrant", true);
            RobberyHL = ini.ReadBoolean("Settings", "RobberyHL", true);
            StolenEmergencyVehicle = ini.ReadBoolean("Settings", "StolenEmergencyVehicle", true);
            StolenEmergencyVehicle2 = ini.ReadBoolean("Settings", "StolenEmergencyVehicle2", true);
            DrugDealer = ini.ReadBoolean("Settings", "DrugDealer", true);
            DrugDeal = ini.ReadBoolean("Settings", "DrugDeal", true);
            KillerClownWasSeen = ini.ReadBoolean("Settings", "KillerClownWasSeen", true);
            Fighting = ini.ReadBoolean("Settings", "Fighting", true);
            PersonWithKnife = ini.ReadBoolean("Settings", "PersonWithKnife", true);
            StolenBus = ini.ReadBoolean("Settings", "StolenBus", true);
            StolenTruck = ini.ReadBoolean("Settings", "StolenTruck", true);
            MoneyTruckIsRobbed = ini.ReadBoolean("Settings", "MoneyTruckIsRobbed", true);
            troublemaker = ini.ReadBoolean("Settings", "troublemaker", true);
            CarTrade = ini.ReadBoolean("Settings", "CarTrade", true);
            ArmoredPerson = ini.ReadBoolean("Settings", "ArmoredPerson", true);
            ShotsFired = ini.ReadBoolean("Settings", "ShotsFired", true);
            Bicycle = ini.ReadBoolean("Settings", "Bicycle", true);
            WelfareCheck = ini.ReadBoolean("Settings", "WelfareCheck", true);
            K9Backup = ini.ReadBoolean("Settings", "K9Backup", true);
            store = ini.ReadBoolean("Settings", "store", true);
            TrafficBackup = ini.ReadBoolean("Settings", "TrafficBackup", true);
            Hostages = ini.ReadBoolean("Settings", "Hostages", true);
            EndCall = ini.ReadEnum("Keys", "EndCall", Keys.End);
            Dialog = ini.ReadEnum("Keys", "Dialog", Keys.Y);
        }
        public static readonly string CalloutVersion = "1.5.7.2";
    }
}
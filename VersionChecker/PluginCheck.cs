using System;
using Rage;
using System.Net;

namespace UnitedCallouts.VersionChecker
{
    public class PluginCheck
    {
        public static bool isUpdateAvailable()
        {
            string curVersion = Settings.CalloutVersion;

            Uri latestVersionUri = new Uri("https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=20730&textOnly=1"); //Use instead of "20730" your file number on lcpdfr.com
            WebClient webClient = new WebClient();
            string receivedData = string.Empty;

            try
            {
                receivedData = webClient.DownloadString("https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=20730&textOnly=1").Trim(); //Use instead of "20730" your file number on lcpdfr.com
            }
            catch (WebException)
            {
                Game.DisplayNotification("commonmenu", "mp_alerttriangle", "~w~UnitedCallouts Warning", "~y~Failed to check for a update", "Please check if you are ~o~online~w~, or try to reload the plugin.");

                Game.Console.Print();
                Game.Console.Print("================================================ UnitedCallouts WARNING =====================================================");
                Game.Console.Print();
                Game.Console.Print("[LOG]: Failed to check for a update.");
                Game.Console.Print("[LOG]: Please check if you are online, or try to reload the plugin.");
                Game.Console.Print();
                Game.Console.Print("================================================ UnitedCallouts WARNING =====================================================");
                Game.Console.Print();
            }
            if (receivedData != Settings.CalloutVersion)
            {
                Game.DisplayNotification("commonmenu", "mp_alerttriangle", "~w~UnitedCallouts Warning", "~y~A new Update is available!", "Current Version: ~r~" + curVersion + "~w~<br>New Version: ~o~" + receivedData);

                Game.Console.Print();
                Game.Console.Print("================================================ UnitedCallouts WARNING =====================================================");
                Game.Console.Print();
                Game.Console.Print("[LOG]: A new version of UnitedCallouts is available! Update the Version, or play on your own risk.");
                Game.Console.Print("[LOG]: Current Version:  " + curVersion);
                Game.Console.Print("[LOG]: New Version:  " + receivedData);
                Game.Console.Print();
                Game.Console.Print("================================================ UnitedCallouts WARNING =====================================================");
                Game.Console.Print();
                return true;
            }
            else
            {
                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "", "Detected newest version of UC.<br>Installed Version: ~g~" + curVersion + "");
                return false;
            }
        }
    }
}

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
            Uri latestVersionUri = new Uri("https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=20730&textOnly=1");
            WebClient webClient = new WebClient();
            string receivedData = string.Empty;
            try
            {
                receivedData = webClient.DownloadString("https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=20730&textOnly=1").Trim();
            }
            catch (WebException)
            {
                Game.DisplayNotification("commonmenu", "mp_alerttriangle", "~w~UnitedCallouts Warning", "~y~Failed to check for an update", "Please check if you are ~o~online~w~, or try to reload the plugin.");
                Game.Console.Print();
                Game.Console.Print("================================================== UnitedCallouts ===================================================");
                Game.Console.Print();
                Game.Console.Print("[WARNING]: Failed to check for an update.");
                Game.Console.Print("[LOG]: Please check if you are online, or try to reload the plugin.");
                Game.Console.Print();
                Game.Console.Print("================================================== UnitedCallouts ===================================================");
                Game.Console.Print();
            }
            if (receivedData != Settings.CalloutVersion)
            {
                Game.DisplayNotification("commonmenu", "mp_alerttriangle", "~w~UnitedCallouts Warning", "~y~A new Update is available!", "Current Version: ~r~" + curVersion + "~w~<br>New Version: ~o~" + receivedData);
                Game.Console.Print();
                Game.Console.Print("================================================== UnitedCallouts ===================================================");
                Game.Console.Print();
                Game.Console.Print("[WARNING]: A new version of UnitedCallouts is available! Update to the latest build, or play on your own risk.");
                Game.Console.Print("[LOG]: Current Version:  " + curVersion);
                Game.Console.Print("[LOG]: New Version:  " + receivedData);
                Game.Console.Print();
                Game.Console.Print("================================================== UnitedCallouts ===================================================");
                Game.Console.Print();
                return true;
            }
            else
            {
                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "", "Detected latest version of UC.<br>Installed Version: ~g~" + curVersion + "");
                return false;
            }
        }
    }
}

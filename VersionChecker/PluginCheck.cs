using System.Net;

namespace UnitedCallouts.VersionChecker;

public class PluginCheck
{
    public static bool IsUpdateAvailable()
    {
            string curVersion = Settings.PluginVersion;
            Uri latestVersionUri = new Uri("https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=20730&textOnly=1");
            WebClient webClient = new WebClient();
            string receivedData = string.Empty;
            try
            {
                receivedData = webClient.DownloadString(latestVersionUri).Trim();
            }
            catch (WebException)
            {
                Game.DisplayNotification("commonmenu", "mp_alerttriangle", "~w~UnitedCallouts Warning", "~y~Update check failed", "Unable to check for updates. Please ensure you are ~y~connected~w~ to the internet or try ~y~reloading~w~ the plugin.");
                Game.Console.Print();
                Game.Console.Print("================================================== UnitedCallouts ===================================================");
                Game.Console.Print();
                Game.Console.Print("[WARNING]: Unable to check for updates.");
                Game.Console.Print("[LOG]: Please ensure you are connected to the internet or reload the plugin.");
                Game.Console.Print();
                Game.Console.Print("================================================== UnitedCallouts ===================================================");
                Game.Console.Print();
                return false;
            }
            if (receivedData != Settings.PluginVersion)
            {
                Game.DisplayNotification("commonmenu", "mp_alerttriangle", "~w~UnitedCallouts Warning", "~y~Update available", "Current Version: ~r~" + curVersion + "~w~<br>Latest Version: ~o~" + receivedData + "<br>~w~Please update to the latest build.");
                Game.Console.Print();
                Game.Console.Print("================================================== UnitedCallouts ===================================================");
                Game.Console.Print();
                Game.Console.Print("[WARNING]: A new version of UnitedCallouts is available. Updating to the latest build is strongly recommended.");
                Game.Console.Print("[LOG]: Current Version: " + curVersion);
                Game.Console.Print("[LOG]: Latest Version: " + receivedData);
                Game.Console.Print();
                Game.Console.Print("================================================== UnitedCallouts ===================================================");
                Game.Console.Print();
                return true;
            }
            else
            {
                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "", "You are running the ~g~latest~w~ version of ~y~UnitedCallouts~w~!");
                return false;
            }
        }
}
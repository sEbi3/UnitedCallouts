using LSPD_First_Response.Engine.Scripting.Entities;

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Welfare Check Request", CalloutProbability.Medium)]
public class WelfareCheckRequest : Callout
{
    private static Ped _subject;

    private static readonly string[] Suspects =
    {
        "ig_andreas", "g_m_m_armlieut_01", "a_m_m_bevhills_01", "a_m_y_business_02", "s_m_m_gaffer_01",
        "a_f_y_golfer_01", "a_f_y_bevhills_01", "a_f_y_bevhills_04", "a_f_y_fitness_02"
    };


    private Vector3 _spawnPoint;
    private Vector3 _searcharea;
    private Blip _blip;
    private int _storyLine = 1;
    private int _callOutMessage;
    private bool _scene1;
    private bool _scene2;
    private bool _scene3;
    private bool _wasClose;
    private bool _alreadySubtitleIntrod;
    private bool _notificationDisplayed;
    private bool _getAmbulance;
    private bool _arrivedAtScene;
    private Persona _subjectPersona;

    public override bool OnBeforeCalloutDisplayed()
    {
        List<Vector3> list = new List<Vector3>
        {
            new(917.1311f, -651.3591f, 57.86318f),
            new(-1905.715f, 365.4793f, 93.58082f),
            new(1661.571f, 4767.511f, 42.00745f),
            new(1878.274f, 3922.46f, 33.06999f),

        };
        _spawnPoint = LocationChooser.ChooseNearestLocation(list);
        _subject = new Ped(Suspects[Rndm.Next(Suspects.Length)], _spawnPoint, 0f);
        _subjectPersona = Functions.GetPersonaForPed(_subject);
        switch (Rndm.Next(1, 4))
        {
            case 1:
                if (_subject != null && _subject.Exists())
                {
                    _subject.Kill();
                }
                _scene1 = true;
                break;
            case 2:
                _scene3 = true;
                break;
            case 3:
                if (_subject != null && _subject.Exists())
                {
                    _subject.Dismiss();
                }
                _subject = null;
                _scene2 = true;
                break;
        }

        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 100f);
        switch (Rndm.Next(1, 4))
        {
            case 1:
                CalloutMessage = "Welfare Check Request";
                CalloutAdvisory = "Request received for a welfare check.";
                _callOutMessage = 1;
                break;
            case 2:
                CalloutMessage = "Welfare Check Request";
                CalloutAdvisory = "Concerned party requests a welfare check on an individual at the given address.";
                _callOutMessage = 2;
                break;
            case 3:
                CalloutMessage = "Welfare Check Request";
                CalloutAdvisory = "Welfare check requested for a resident due to unusual or concerning behavior.";
                _callOutMessage = 3;
                break;
        }

        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("UNITS WE_HAVE CRIME_CIVILIAN_NEEDING_ASSISTANCE_02", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Welfare Check Request callout accepted.");
        }
        else { Settings.DetailedLogging = false; }
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Welfare Check Request",
            "~b~Dispatch: ~w~Respond to a welfare check. Caller expresses concern for the wellbeing of the resident. Advise on status upon arrival. Respond with ~y~Code 2~w~.");
        GameFiber.Wait(500);
        Functions.DisplayPedId(_subject, true);

        if (_subject != null && _subject.Exists())
        {
            Functions.DisplayPedId(_subject, true);
        }
        else
        {
            if (Settings.DetailedLogging)
            {
                Game.LogTrivial("[UnitedCallouts LOG:] Welfare Check Request Callout: Subject ped is invalid or was dismissed, skipping DisplayPedId.");
            }
            else { Settings.DetailedLogging = false; }
        }

        _searcharea = _spawnPoint.Around2D(1f, 2f);
        _blip = new(_searcharea, 40f)
        {
            Color = Color.Yellow,
            Alpha = 0.5f
        };
        _blip.EnableRoute(Color.Yellow);
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_subject != null && _subject.Exists())
            _subject.Delete();
        if (_blip != null && _blip.Exists())
            _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (!_arrivedAtScene && _spawnPoint.DistanceTo(MainPlayer) < 25f)
        {
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH OFFICERS_ARRIVED_ON_SCENE");
            _arrivedAtScene = true;
        }

        if (_arrivedAtScene)
        {
            if (_scene1 && !_notificationDisplayed && !_getAmbulance && _subject != null && _subject.Exists() && _subject.DistanceTo(MainPlayer) < 6f && MainPlayer.IsOnFoot)
            {
                _notificationDisplayed = true;
                GameFiber.StartNew(() =>
                {
                    Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
                        "~y~Welfare Check Request",
                        "Individual located on the ground at the scene. An ~y~ambulance ~w~is en route to your current location. End of call when scene is secured and situation resolved.");
                    GameFiber.Wait(1000); 
                    if (Settings.HelpMessages)
                    {
                        Game.DisplayHelp("Press the ~y~" + Settings.EndCall + " ~w~key to end the welfare check callout.", 5000);
                    } else { Settings.HelpMessages = false; }

                    Functions.RequestBackup(MainPlayer.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.Ambulance);
                    _getAmbulance = true;
                });
            }

            if (_scene2 && _spawnPoint.DistanceTo(MainPlayer) < 30f && MainPlayer.IsOnFoot && !_notificationDisplayed)
            {
                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
                    "~y~Welfare Check Request", "Conduct a search of the surrounding area for the individual. Advise on contact or sightings.");
                GameFiber.Wait(1000);
                if (Settings.HelpMessages)
                {
                    Game.DisplayHelp("Press the ~y~" + Settings.EndCall + " ~w~key to end the welfare check callout if you finished.", 5000);
                }
                else { Settings.HelpMessages = false; }
                _notificationDisplayed = true;
            }

            if (_scene3 && _subject != null && _subject.Exists() && _subject.DistanceTo(MainPlayer) < 25f && MainPlayer.IsOnFoot && _alreadySubtitleIntrod == false)
            {
                Game.DisplaySubtitle("Press ~y~" + Settings.Dialog + " ~w~to speak with the suspect.", 5000);
                _alreadySubtitleIntrod = true;
                _wasClose = true;
            }

            if (_scene3 && !_scene1 && !_scene2 && _subject != null && _subject.Exists() && _subject.DistanceTo(MainPlayer) < 2f && Game.IsKeyDown(Settings.Dialog))
            {
                _subject.Face(MainPlayer);
                switch (_storyLine)
                {
                    case 1:
                        Game.DisplaySubtitle(
                            "~y~Suspect: ~w~Hello Officer, how can I help you? Is everything alright? (1/5)", 5000);
                        _storyLine++;
                        break;
                    case 2:
                        Game.DisplaySubtitle(
                            "~b~You: ~w~Hi. I'm just checking in on this address as we've had a welfare check request come through. Apparently you weren't answering your phone and someone is concerned. Is everything okay here? (2/5)",
                            5000);
                        _storyLine++;
                        break;
                    case 3:
                        Game.DisplaySubtitle("~y~Suspect: ~w~Oh dear! I didn't want to worry anyone. (3/5)", 5000);
                        _storyLine++;
                        break;
                    case 4:
                        switch (_callOutMessage)
                        {
                            case 1:
                                Game.DisplaySubtitle(
                                    "~y~Suspect: ~w~I lost my phone on the bus today, I was actually just about to head to a payphone to ring the bus depot. (4/5)",
                                    5000);
                                break;
                            case 2:
                                Game.DisplaySubtitle(
                                    "~y~Suspect: ~w~My phone battery died because I forgot to charge it earlier! I did see a missed call but didn't think anything of it. (4/5)",
                                    5000);
                                break;
                            case 3:
                                Game.DisplaySubtitle(
                                    "~y~Suspect: ~w~Let me check... Oops, I had my phone on silent! I'll call them back now. Sorry to cause such trouble! (4/5)",
                                    5000);
                                break;
                        }

                        _storyLine++;
                        break;
                    case 5:
                        switch (_callOutMessage)
                        {
                            case 1:
                                Game.DisplaySubtitle(
                                    "~b~You: ~w~Ouch. I'll let dispatch know everything is okay. Good luck finding your phone! (5/5)",
                                    5000);
                                break;
                            case 2:
                                Game.DisplaySubtitle(
                                    "~b~You: ~w~Alright, well as long as everything here is okay, I can leave. You should return that phone call though, the caller was really worried. (5/5)",
                                    5000);
                                break;
                            case 3:
                                Game.DisplaySubtitle(
                                    "~b~You: ~w~No problem, I'm just glad you're okay. I'll let dispatch know everything is fine here. (5/5)",
                                    5000);
                                break;
                        }

                        _storyLine++;
                        break;
                    case 6:
                        switch (_callOutMessage)
                        {
                            case 1:
                                End();
                                break;
                            case 2:
                                End();
                                break;
                            case 3:
                                End();
                                break;
                        }

                        _storyLine++;
                        break;
                }
            }
        }
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (MainPlayer.IsDead) End();
        base.Process();
    }

    public override void End()
    {
        if (_subject != null && _subject.Exists()) _subject.Dismiss();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Welfare Check Request", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Welfare Check Request callout ended.");
        }
        else { Settings.DetailedLogging = false; }
        base.End();
    }
}
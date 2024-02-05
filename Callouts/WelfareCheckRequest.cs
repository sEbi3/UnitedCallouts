namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Welfare Check Request", CalloutProbability.Medium)]
public class WelfareCheckRequest : Callout
{
    private Ped _subject;
    private string[] _suspects = new string[] { "ig_andreas", "g_m_m_armlieut_01", "a_m_m_bevhills_01", "a_m_y_business_02", "s_m_m_gaffer_01",
        "a_f_y_golfer_01", "a_f_y_bevhills_01", "a_f_y_bevhills_04", "a_f_y_fitness_02"};
    private Vector3 _spawnPoint;
    private Vector3 _searcharea;
    private Blip _blip;
    private int _storyLine = 1;
    private int _callOutMessage = 0;
    private bool _scene1 = false;
    private bool _scene2 = false;
    private bool _scene3 = false;
    private bool _wasClose = false;
    private bool _alreadySubtitleIntrod = false;
    private bool _notificationDisplayed = false;
    private bool _getAmbulance = false;

    public override bool OnBeforeCalloutDisplayed()
    {
           

        Random random = Rndm;
        List<Vector3> list = new List<Vector3>
        {
            new(917.1311f, -651.3591f, 57.86318f),
            new(-1905.715f, 365.4793f, 93.58082f),
            new(1661.571f, 4767.511f, 42.00745f),
            new(1878.274f, 3922.46f, 33.06999f),

        };
        _spawnPoint = LocationChooser.ChooseNearestLocation(list);
        _subject = new Ped(_suspects[Rndm.Next(_suspects.Length)], _spawnPoint, 0f);
        LSPD_First_Response.Mod.API.Functions.GetPersonaForPed(_subject);
        switch (Rndm.Next(1, 3))
        {
            case 1:
                _subject.Kill();
                _scene1 = true;
                break;
            case 2:
                _scene3 = true;
                break;
            case 3:
                _subject.Dismiss();
                _scene2 = true;
                break;
        }
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 100f);
        switch (Rndm.Next(1, 3))
        {
            case 1:
                CalloutMessage = "[UC]~w~ Welfare Check Request";
                _callOutMessage = 1;
                break;
            case 2:
                CalloutMessage = "[UC]~w~ Welfare Check Request";
                _callOutMessage = 2;
                break;
            case 3:
                CalloutMessage = "[UC]~w~ Welfare Check Request";
                _callOutMessage = 3;
                break;
        }
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("UNITS WE_HAVE CRIME_CIVILIAN_NEEDING_ASSISTANCE_02", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: WelfareCheck callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Welfare Check Request", "~b~Dispatch:~w~ Someone called the police for a welfare check. Search the ~y~yellow area~w~ for the person. Respond with ~y~Code 2");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "", "Loading ~g~Information~w~ of the ~y~LSPD Database~w~...");
        Functions.DisplayPedId(_subject, true);

        _searcharea = _spawnPoint.Around2D(1f, 2f);
        _blip = new Blip(_searcharea, 40f);
        _blip.EnableRoute(Color.Yellow);
        _blip.Color = Color.Yellow;
        _blip.Alpha = 0.5f;
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_subject) _subject.Delete();
        if (_blip) _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        GameFiber.StartNew(delegate
        {
            if (_spawnPoint.DistanceTo(MainPlayer) < 25f)
            {
                if (_scene1 == true && _subject && _subject.DistanceTo(MainPlayer) < 10f && MainPlayer.IsOnFoot && !_notificationDisplayed && !_getAmbulance)
                {
                    Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "We are going to call an ~y~ambulance~w~ to your current location, officer. Press the ~y~END~w~ key to end the welfare check callout.");
                    _notificationDisplayed = true;
                    GameFiber.Wait(1000);
                    if (Settings.HelpMessages)
                    {
                        Game.DisplayHelp("Press the ~y~" + Settings.EndCall + "~w~ key to end the wellfare check callout.");
                    }
                    Functions.RequestBackup(MainPlayer.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.Ambulance);
                    _getAmbulance = true;
                }
                if (_scene2 == true && _spawnPoint.DistanceTo(MainPlayer) < 8f && MainPlayer.IsOnFoot && !_notificationDisplayed)
                {
                    Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Dispatch", "Investigate the area. If you don't find anyone here, then ~g~End~w~ the callout.");
                    _notificationDisplayed = true;
                }
                if (_scene3 == true && _subject && _subject.DistanceTo(MainPlayer) < 25f && MainPlayer.IsOnFoot && _alreadySubtitleIntrod == false)
                {
                    Game.DisplaySubtitle("Press ~y~Y ~w~to speak with the suspect.", 5000);
                    Game.DisplayHelp("Press the ~y~END~w~ key to end the ~o~welfare check~w~ callout.", 5000);
                    Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH OFFICERS_ARRIVED_ON_SCENE");
                    _alreadySubtitleIntrod = true;
                    _wasClose = true;
                }
                if (_scene3 == true && _scene1 == false && _scene2 == false && _subject.DistanceTo(MainPlayer) < 2f && Game.IsKeyDown(Settings.Dialog))
                {
                    _subject.Face(MainPlayer);
                    switch (_storyLine)
                    {
                        case 1:
                            Game.DisplaySubtitle("~y~Suspect: ~w~Hello Officer, how can I help you? Is everything alright? (1/5)", 5000);
                            _storyLine++;
                            break;
                        case 2:
                            Game.DisplaySubtitle("~b~You: ~w~Hi. I'm just checking in on this address as we've had a welfare check request come through. Apparently you weren't answering your phone and someone is concerned. Is everything okay here? (2/5)", 5000);
                            _storyLine++;
                            break;
                        case 3:
                            Game.DisplaySubtitle("~y~Suspect: ~w~Oh dear! I didn't want to worry anyone. (3/5)", 5000);
                            _storyLine++;
                            break;
                        case 4:
                            if (_callOutMessage == 1)
                                Game.DisplaySubtitle("~y~Suspect: ~w~I lost my phone on the bus today, I was actually just about to head to a payphone to ring the bus depot. (4/5)", 5000);
                            if (_callOutMessage == 2)
                                Game.DisplaySubtitle("~y~Suspect: ~w~My phone battery died because I forgot to charge it earlier! I did see a missed call but didn't think anything of it. (4/5)", 5000);
                            if (_callOutMessage == 3)
                                Game.DisplaySubtitle("~y~Suspect: ~w~Let me check... Oops, I had my phone on silent! I'll call them back now. Sorry to cause such trouble! (4/5)", 5000);
                            _storyLine++;
                            break;
                        case 5:
                            if (_callOutMessage == 1)
                                Game.DisplaySubtitle("~b~You: ~w~Ouch. I'll let dispatch know everything is okay. Good luck finding your phone! (5/5)", 5000);
                            if (_callOutMessage == 2)
                                Game.DisplaySubtitle("~b~You: ~w~Alright, well as long as everything here is okay, I can leave. You should return that phone call though, the caller was really worried. (5/5)", 5000);
                            if (_callOutMessage == 3)
                                Game.DisplaySubtitle("~b~You: ~w~No problem, I'm just glad you're okay. I'll let dispatch know everything is fine here. (5/5)", 5000);
                            _storyLine++;
                            break;
                        case 6:
                            if (_callOutMessage == 1)
                                End();
                            if (_callOutMessage == 2)
                                End();
                            if (_callOutMessage == 3)
                                End();
                            _storyLine++;
                            break;
                        default:
                            break;
                    }
                }
            }
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (MainPlayer.IsDead) End();
        }, "Welfare Check Request [UnitedCallouts]");
        base.Process();
    }

    public override void End()
    {
        if (_subject) _subject.Dismiss();
        if (_blip) _blip.Delete();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Welfare Check Request", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
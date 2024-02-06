namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Troublemaker at Metro Station", CalloutProbability.Medium)]
public class Troublemaker : Callout
{
    private static Ped _subject;

    private static readonly string[] PedList =
    {
        "s_m_y_dealer_01", "u_m_m_jesus_01", "u_m_y_militarybum", "u_m_y_proldriver_01", "a_m_o_soucent_03",
        "u_m_o_tramp_01",
        "a_m_m_tramp_01", "a_m_o_tramp_01", "a_m_m_trampbeac_01"
    };

    private static Vector3 _spawnPoint;
    private static Vector3 _searcharea;
    private static Blip _blip;
    private static bool _attack;
    private static bool _startedPursuit = false;
    private static bool _wasClose;
    private static bool _alreadySubtitleIntrod;
    private static int _storyLine = 1;
    private static int _callOutMessage;

    public override bool OnBeforeCalloutDisplayed()
    {
        List<Vector3> list = new List<Vector3>
        {
            new(-292.7569f, -305.3849f, 10.06316f),
            new(-282.8204f, -326.8933f, 18.28812f),
            new(262.1429f, -1205.378f, 29.28906f),
            new(298.8077f, -1206.379f, 38.89511f),
            new(-854.7909f, -107.829f, 28.18498f),
            new(-824.4403f, -129.8238f, 28.17533f),
            new(-1359.522f, -472.9277f, 23.27035f),
            new(-1346.406f, -474.0514f, 15.04538f),

        };
        _spawnPoint = LocationChooser.ChooseNearestLocation(list);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
        switch (Rndm.Next(1, 4))
        {
            case 1:
                _attack = true;
                break;
            case 2:
                _attack = false;
                break;
            case 3:
                _attack = false;
                break;
        }

        switch (Rndm.Next(1, 4))
        {
            case 1:
                CalloutMessage = "[UC]~w~ Reports of a Troublemaker at Metro station";
                _callOutMessage = 1;
                break;
            case 2:
                CalloutMessage = "[UC]~w~ Reports of a Troublemaker at Metro station";
                _callOutMessage = 2;
                break;
            case 3:
                CalloutMessage = "[UC]~w~ Reports of a Troublemaker at Metro station";
                _callOutMessage = 3;
                break;
        }

        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS CRIME_SUSPECT_RESISTING_ARREST_01 IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Troublemaker callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Troublemaker at Metro station",
            "~b~Dispatch:~w~ Search the ~y~yellow area in the metro station~w~ and try to contact the troublemaker. Respond with ~y~Code 2");

        _subject = new(PedList[Rndm.Next(PedList.Length)], _spawnPoint, 0f)
        {
            Position = _spawnPoint,
            IsPersistent = true,
            BlockPermanentEvents = true
        };
        _subject.Tasks.PlayAnimation("amb@world_human_bum_standing@drunk@base", "base", 5, AnimationFlags.None);

        _searcharea = _spawnPoint.Around2D(1f, 2f);
        _blip = new(_searcharea, 80f)
        {
            Color = Color.Yellow,
            Alpha = 0.5f
        };
        _blip.EnableRoute(Color.Yellow);
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
        if (_subject.DistanceTo(MainPlayer) < 20f)
        {
            if (_attack && !_startedPursuit)
            {
                _subject.Tasks.FightAgainst(MainPlayer);
            }

            if (!_attack && _subject.DistanceTo(MainPlayer) < 6f && MainPlayer.IsOnFoot &&
                !_alreadySubtitleIntrod)
            {
                Game.DisplaySubtitle("Press ~y~Y ~w~to speak with the troublemaker.", 5000);
                _subject.Face(MainPlayer);
                _alreadySubtitleIntrod = true;
                _wasClose = true;
                if (_blip) _blip.Delete();
            }

            if (MainPlayer.IsOnFoot && !_alreadySubtitleIntrod && _subject.DistanceTo(MainPlayer) < 5f)
            {
                _subject.Face(MainPlayer);
            }

            if (Game.IsKeyDown(Settings.Dialog) && !_attack && _subject.DistanceTo(MainPlayer) < 2f)
            {
                _subject.Face(MainPlayer);
                switch (_storyLine)
                {
                    case 1:
                        switch (_callOutMessage)
                        {
                            case 1:
                                Game.DisplaySubtitle("~r~Troublemaker: ~w~Hello officer, is there a problem? (1/5)",
                                    5000);
                                break;
                            case 2:
                                Game.DisplaySubtitle("~r~Troublemaker: ~w~What do you want? (1/5)", 5000);
                                break;
                            case 3:
                                Game.DisplaySubtitle("~r~Troublemaker: ~w~I'm not drunk! (1/5)", 5000);
                                break;
                        }

                        _storyLine++;
                        break;
                    case 2:
                        switch (_callOutMessage)
                        {
                            case 1:
                                Game.DisplaySubtitle(
                                    "~b~You: ~w~We've had some reports of a male matching your description causing a disturbance here. (2/5)",
                                    5000);
                                break;
                            case 2:
                                Game.DisplaySubtitle(
                                    "~b~You: ~w~Alright, let's stay calm. What are you doing here? (2/5)", 5000);
                                break;
                            case 3:
                                Game.DisplaySubtitle(
                                    "~b~You: ~w~Sure. We've had reports of an intoxicated individual harassing members of the public. That wouldn't be you, would it? (2/5)",
                                    5000);
                                break;
                        }

                        _storyLine++;
                        break;
                    case 3:
                        switch (_callOutMessage)
                        {
                            case 1:
                                Game.DisplaySubtitle(
                                    "~r~Troublemaker: ~w~Okay, I may have had an argument down by the tracks. I just lost my temper, I'm sorry! (3/5)",
                                    5000);
                                break;
                            case 2:
                                Game.DisplaySubtitle(
                                    "~r~Troublemaker: ~w~What do you care? I'm not doing anything wrong. Leave me alone. (3/5)",
                                    5000);
                                break;
                            case 3:
                                Game.DisplaySubtitle(
                                    "~r~Troublemaker: ~w~Harassing? Please, I was trying to get the hobo down there to leave the area. I was doing your job! (3/5)",
                                    5000);
                                break;
                        }

                        _storyLine++;
                        break;
                    case 4:
                        switch (_callOutMessage)
                        {
                            case 1:
                                Game.DisplaySubtitle(
                                    "~b~You: ~w~Alright, well you need to leave the area so that I know there won't be any further issues. (4/5)",
                                    5000);
                                break;
                            case 2:
                                Game.DisplaySubtitle(
                                    "~b~You: ~w~We've had reports of someone using vulgar and offensive language. (4/5)",
                                    5000);
                                break;
                            case 3:
                                Game.DisplaySubtitle(
                                    "~b~You: ~w~Alright, well let us deal with that. Please leave the area, you're making people uncomfortable and being a nuisance. (4/5)",
                                    5000);
                                break;
                        }

                        _storyLine++;
                        break;
                    case 5:
                        switch (_callOutMessage)
                        {
                            case 1:
                                _subject.Tasks.PutHandsUp(-1, MainPlayer);
                                Game.DisplaySubtitle(
                                    "~r~Troublemaker: ~w~Of course, I'm sorry. I'll wait for my friend elsewhere. (5/5)",
                                    5000);
                                break;
                            case 2:
                                Game.DisplaySubtitle(
                                    "~r~Troublemaker: ~w~What the fuck has happened to this country? I have free speech you bitch! You can't silence me! (5/5)",
                                    5000);
                                _subject.Inventory.GiveNewWeapon("WEAPON_KNIFE", 500, true);
                                NativeFunction.Natives.TASK_COMBAT_PED(_subject, MainPlayer, 0, 16);
                                break;
                            case 3:
                                Game.DisplaySubtitle("~r~Troublemaker: ~w~Oh am I? Well how about now?! (5/5)", 5000);
                                _subject.Inventory.GiveNewWeapon("WEAPON_MOLOTOV", 500, true);
                                NativeFunction.Natives.TASK_COMBAT_PED(_subject, MainPlayer, 0, 16);
                                break;
                        }

                        _storyLine++;
                        break;
                }
            }
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (_subject && _subject.IsDead) End();
        if (_subject && Functions.IsPedArrested(_subject)) End();
        base.Process();
    }

    public override void End()
    {
        if (_subject) _subject.Dismiss();
        if (_blip) _blip.Delete();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Troublemaker at Metro station", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
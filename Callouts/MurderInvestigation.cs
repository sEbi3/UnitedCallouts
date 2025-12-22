namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Murder Investigation", CalloutProbability.Medium)]
internal class MurderInvestigation : Callout
{
    private static readonly string[] WepList =
        { "WEAPON_PISTOL", "WEAPON_COMBATPISTOL", "WEAPON_KNIFE", "WEAPON_MUSKET", "WEAPON_MACHETE" };

    private static readonly string[] CopCars = { "SHERIFF", "SHERIFF2" };

    // FIXED: Removed static from all instance fields
    private Ped _deadPerson;
    private Ped _deadPerson2;
    private Ped _murder;
    private Ped _cop1;
    private Ped _cop2;
    private Ped _coroner1;
    private Ped _coroner2;
    private Vehicle _coronerV;
    private Vehicle _copV;
    private Vector3 _searcharea;
    private Vector3 _deadPersonSpawn;
    private Vector3 _cop1Spawn;
    private Vector3 _cop2Spawn;
    private Vector3 _coroner1Spawn;
    private Vector3 _coroner2Spawn;
    private Vector3 _coronerVSpawn;
    private Vector3 _copVSpawn;
    private Vector3 _murderLocation;
    private Blip _murderLocationBlip;
    private Blip _spawnLocationBlip;
    private LHandle _pursuit;
    private int _storyLine = 1;
    private int _callOutMessage;
    private int _scenario;
    private bool _scene1;
    private bool _scene2;
    private bool _wasClose = false;
    private bool _noticed;
    private bool _notificationDisplayed = false;
    private bool _hasPursuitBegun = false;

    public override bool OnBeforeCalloutDisplayed()
    {
        _deadPersonSpawn = new(1162.029f, 2371.788f, 57.66312f);
        _cop1Spawn = new(1174.725f, 2369.399f, 57.59957f);
        _cop2Spawn = new(1167.733f, 2382.189f, 57.61982f);
        _coroner1Spawn = new(1161.486f, 2369.885f, 57.76299f);
        _coroner2Spawn = new(1165.3f, 2374.227f, 57.63049f);
        _copVSpawn = new(1174.684f, 2375.117f, 57.6276f);
        _coronerVSpawn = new(1165.686f, 2360.025f, 57.62796f);

        List<Vector3> list = new()
        {
            new(-10.93565f, -1434.329f, 31.11683f),
            new(-1.838376f, 523.2645f, 174.6274f),
            new(-801.5516f, 178.7447f, 72.83471f),
            new(-801.5516f, 178.7447f, 72.83471f),
            new(-812.7239f, 178.7438f, 76.74079f),
            new(3.542758f, 526.8926f, 170.6218f),
            new(-1155.698f, -1519.297f, 10.63272f),
            new(1392.589f, 3613.899f, 38.94194f),
            new(2435.457f, 4966.514f, 46.8106f),

        };
        _murderLocation = LocationChooser.ChooseNearestLocation(list);
        _scenario = Rndm.Next(0, 101);
        _copV = new(CopCars[Rndm.Next(CopCars.Length)], _copVSpawn, 76.214f)
        {
            IsEngineOn = true,
            IsInteriorLightOn = true,
            IsSirenOn = true,
            IsSirenSilent = true
        };

        _coronerV = new("Speedo", _coronerVSpawn, 22.32638f)
        {
            IsEngineOn = true,
            IsInteriorLightOn = true,
            IndicatorLightsStatus = VehicleIndicatorLightsStatus.Both
        };

        _deadPerson = new(_deadPersonSpawn)
        {
            IsPersistent = true,
            BlockPermanentEvents = true
        };
        _deadPerson.Tasks.PlayAnimation("random@arrests@busted", "idle_a", 8.0F, AnimationFlags.Loop);
        _deadPerson.Kill();

        _murder = new(_murderLocation)
        {
            IsPersistent = true,
            BlockPermanentEvents = true,
            Health = 200,
            Armor = 300
        };

        _deadPerson2 = new(_murder.GetOffsetPosition(new(0, 1.8f, 0)))
        {
            IsPersistent = true,
            BlockPermanentEvents = true
        };

        _cop1 = new("s_m_y_sheriff_01", _cop1Spawn, 0f)
        {
            IsInvincible = true,
            IsPersistent = true,
            BlockPermanentEvents = true,
        };

        _cop2 = new("s_m_y_sheriff_01", _cop2Spawn, 0f)
        {
            IsInvincible = true,
            IsPersistent = true,
            BlockPermanentEvents = true
        };

        _coroner1 = new("S_M_M_Doctor_01", _coroner1Spawn, 0f)
        {
            IsPersistent = true,
            IsInvincible = true,
            BlockPermanentEvents = true,
            KeepTasks = false
        };

        _coroner2 = new("S_M_M_Doctor_01", _coroner2Spawn, 0f)
        {
            IsPersistent = true,
            BlockPermanentEvents = true,
            KeepTasks = false,
            IsInvincible = true,
        };

        Functions.SetPedAsCop(_cop1);
        Functions.SetPedAsCop(_cop2);
        _coroner1.Face(_deadPerson);
        _coroner2.Face(_deadPerson);

        _murder.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
        NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_murder, _deadPerson2, -1, true);

        Rage.Object camera = new("prop_ing_camera_01", _coroner1.GetOffsetPosition(Vector3.RelativeTop * 30));
        Rage.Object camera2 = new("prop_ing_camera_01", _coroner2.GetOffsetPosition(Vector3.RelativeTop * 30));
        _coroner1.Tasks.PlayAnimation("amb@world_human_paparazzi@male@idle_a", "idle_a", 8.0F, AnimationFlags.Loop);
        _coroner2.Tasks.PlayAnimation("amb@medic@standing@kneel@base", "base", 8.0F, AnimationFlags.Loop);
        _cop2.Tasks.PlayAnimation("amb@world_human_cop_idles@male@idle_a", "idle_a", 1.5f, AnimationFlags.Loop);

        switch (Rndm.Next(1, 3))
        {
            case 1:
                _scene1 = true;
                break;
            case 2:
                _scene2 = true;
                break;
        }

        ShowCalloutAreaBlipBeforeAccepting(_deadPersonSpawn, 100f);
        AddMinimumDistanceCheck(10f, _deadPerson.Position);
        switch (Rndm.Next(1, 3))
        {
            case 1:
                CalloutMessage = "[UC]~w~ We have found a dead body and need the ~y~FIB~w~ here.";
                _callOutMessage = 1;
                break;
            case 2:
                CalloutMessage = "[UC]~w~ We have found a dead body and need the ~y~FIB~w~ here.";
                _callOutMessage = 2;
                break;
        }

        CalloutPosition = _deadPersonSpawn;
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        _spawnLocationBlip = new(_cop1);
        _spawnLocationBlip.Color = Color.LightGreen;
        _spawnLocationBlip.Sprite = BlipSprite.PointOfInterest;
        _spawnLocationBlip.EnableRoute(Color.LightBlue);

        Functions.PlayScannerAudioUsingPosition("ATTENTION_GENERIC_01 UNITS WE_HAVE A_01 CRIME_DEAD_BODY_01 CODE3",
            _deadPersonSpawn);
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Murder Investigation",
            "~b~Dispatch: ~w~The police department needs a ~b~detective~w~ on scene to find and arrest the murder. Respond with ~r~Code 3");
        GameFiber.Wait(2000);
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        // FIXED: Added exists checks before deletion
        if (_cop1 != null && _cop1.Exists()) _cop1.Delete();
        if (_cop2 != null && _cop2.Exists()) _cop2.Delete();
        if (_coroner1 != null && _coroner1.Exists()) _coroner1.Delete();
        if (_murder != null && _murder.Exists()) _murder.Delete();
        if (_deadPerson != null && _deadPerson.Exists()) _deadPerson.Delete();
        if (_deadPerson2 != null && _deadPerson2.Exists()) _deadPerson2.Delete();
        if (_coroner2 != null && _coroner2.Exists()) _coroner2.Delete();
        if (_copV != null && _copV.Exists()) _copV.Delete();
        if (_coronerV != null && _coronerV.Exists()) _coronerV.Delete();
        if (_spawnLocationBlip != null && _spawnLocationBlip.Exists()) _spawnLocationBlip.Delete();
        if (_murderLocationBlip != null && _murderLocationBlip.Exists()) _murderLocationBlip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        // FIXED: Added null and exists checks before distance calculation
        if (!_noticed && MainPlayer.IsOnFoot && _cop1 != null && _cop1.Exists() &&
            _cop1.DistanceTo(MainPlayer) < 25f)
        {
            Game.DisplaySubtitle("Press ~y~Y~w~ to speak with the officer.", 5000);
            Functions.PlayScannerAudio("ATTENTION_GENERIC_01 OFFICERS_ARRIVED_ON_SCENE");
            _cop1.Face(MainPlayer);
            if (_spawnLocationBlip != null && _spawnLocationBlip.Exists()) _spawnLocationBlip.Delete();
            _noticed = true;
        }

        // FIXED: Added null and exists checks
        if (!_noticed && _murder != null && _murder.Exists() && _murder.DistanceTo(MainPlayer) < 25f)
        {
            if (_murderLocationBlip != null && _murderLocationBlip.Exists()) _murderLocationBlip.Delete();
            _noticed = true;

            if (_scene1 && !_scene2)
            {
                if (_deadPerson2 != null && _deadPerson2.Exists()) _deadPerson2.Kill();
                if (_murder.Exists()) _murder.Tasks.FightAgainst(MainPlayer);
            }

            if (_scene2 && !_scene1)
            {
                GameFiber.StartNew(() =>
                {
                    var agRelationshipGroup = new RelationshipGroup("AG");
                    var viRelationshipGroup = new RelationshipGroup("VI");
                    _murder.RelationshipGroup = agRelationshipGroup;
                    if (_deadPerson2 != null && _deadPerson2.Exists())
                        _deadPerson2.RelationshipGroup = viRelationshipGroup;
                    agRelationshipGroup.SetRelationshipWith(viRelationshipGroup, Relationship.Hate);
                    if (_murder.Exists()) _murder.Tasks.FightAgainstClosestHatedTarget(1000f);
                    GameFiber.Wait(300);
                    if (_murder.Exists()) _murder.Tasks.FightAgainst(MainPlayer);
                }, "Murder Investigation [UnitedCallouts]");
            }
        }

        // FIXED: Added null and exists checks
        if (_cop1 != null && _cop1.Exists() && _cop1.DistanceTo(MainPlayer) < 2f &&
            Game.IsKeyDown(Settings.Dialog))
        {
            _cop1.Face(MainPlayer);
            switch (_storyLine)
            {
                case 1:
                    Game.DisplaySubtitle(
                        "~y~Officer: ~w~Hello Detective, we called you because we have a dead body here. (1/5)", 5000);
                    _storyLine++;
                    break;
                case 2:
                    Game.DisplaySubtitle("~b~You: ~w~Do we have anything about the murder? (2/5)", 5000);
                    _storyLine++;
                    break;
                case 3:
                    Game.DisplaySubtitle("~y~Officer: ~w~Yes, we have found something of interest. (3/5)", 5000);
                    _storyLine++;
                    break;
                case 4:
                    switch (_callOutMessage)
                    {
                        case 1:
                            Game.DisplaySubtitle(
                                "~y~Officer: ~w~We checked the cameras around here and there was a man without a mask, so our police department checked the identity. (4/5)",
                                5000);
                            break;
                        case 2:
                            Game.DisplaySubtitle(
                                "~y~Officer: ~w~As the coroner searched the killed person, they found an ID next to the person. (4/5)",
                                5000);
                            break;
                    }

                    _storyLine++;
                    break;
                case 5:
                    if (_murder != null && _murder.Exists())
                    {
                        var persona = Functions.GetPersonaForPed(_murder);
                        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept",
                            "~w~UnitedCallouts", "~y~Police Department",
                            $"The police department found personal details of the murderer:" +
                            $"<br>~w~Name: ~b~{persona.FullName} " +
                            $"<br>~w~Gender: ~g~{persona.Gender}" +
                            $"<br>~w~DOB: ~y~{persona.Birthday.Date}");
                    }
                    _storyLine++;
                    break;
                case 6:
                    switch (_callOutMessage)
                    {
                        case 1:
                            Game.DisplaySubtitle(
                                "~b~You: ~w~Alright, I'll check the house of the murder. Thank you for your time, officer! (5/5)",
                                5000);
                            break;
                        case 2:
                            Game.DisplaySubtitle(
                                "~b~You: ~w~Okay, thank you for letting me know! I'll find the murderer! (5/5)", 5000);
                            break;
                    }

                    _storyLine++;
                    Game.DisplayHelp("The ~y~Police Department~w~ is setting up the location on your GPS...", 5000);
                    GameFiber.Wait(3000);
                    Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
                        "~y~Police Department",
                        "~b~Detective~w~, we ~o~marked the apartment~w~ for you on the map. Search the ~y~yellow circle area~w~ on your map and try to ~y~find~w~ and ~b~arrest~w~ the ~g~murder~w~.");
                    _searcharea = _murderLocation.Around2D(1f, 2f);
                    _murderLocationBlip = new(_searcharea, 40f);
                    _murderLocationBlip.EnableRoute(Color.Yellow);
                    _murderLocationBlip.Color = Color.Yellow;
                    _murderLocationBlip.Alpha = 0.5f;
                    break;
            }
        }

        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (MainPlayer.IsDead) End();

        // FIXED: Added null checks
        if (_murder != null && _murder.IsDead) End();
        if (_murder != null && Functions.IsPedArrested(_murder)) End();

        base.Process();
    }

    public override void End()
    {
        // FIXED: Added exists checks before cleanup
        if (_cop1 != null && _cop1.Exists()) _cop1.Dismiss();
        if (_cop2 != null && _cop2.Exists()) _cop2.Dismiss();
        if (_coroner1 != null && _coroner1.Exists()) _coroner1.Dismiss();
        if (_murder != null && _murder.Exists()) _murder.Dismiss();
        if (_deadPerson != null && _deadPerson.Exists()) _deadPerson.Dismiss();
        if (_deadPerson2 != null && _deadPerson2.Exists()) _deadPerson2.Dismiss();
        if (_coroner2 != null && _coroner2.Exists()) _coroner2.Dismiss();
        if (_copV != null && _copV.Exists()) _copV.Dismiss();
        if (_coronerV != null && _coronerV.Exists()) _coronerV.Dismiss();
        if (_spawnLocationBlip != null && _spawnLocationBlip.Exists()) _spawnLocationBlip.Delete();
        if (_murderLocationBlip != null && _murderLocationBlip.Exists()) _murderLocationBlip.Delete();

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Murder Investigation", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}

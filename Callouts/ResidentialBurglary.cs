namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Residential Burglary", CalloutProbability.Medium)]
public class ResidentialBurglary : Callout
{
    private static readonly string[] PedList = { "g_m_m_chicold_01", "mp_g_m_pros_01" };
    private static readonly string[] WepList = { "WEAPON_PISTOL", "WEAPON_SMG", "WEAPON_MACHINEPISTOL", "WEAPON_PUMPSHOTGUN" };

    private Vector3 _spawnPoint;
    private Vector3 _searchArea;
    private Blip _blip;
    private Ped _aggressor;
    private Ped _victim;
    private bool _notificationDisplayed;
    private bool _notificationDisplayed2;
    private bool _notificationDisplayed3;
    private bool _hasBegunAttacking;
    private int _scenario;


    public override bool OnBeforeCalloutDisplayed()
    {
        List<Vector3> list = new()
        {
            new(-109.5984f, -10.19665f, 70.51959f),
            new(-10.93565f, -1434.329f, 31.11683f),
            new(-1.838376f, 523.2645f, 174.6274f),
            new(-801.5516f, 178.7447f, 72.83471f),
            new(-812.7239f, 178.7438f, 76.74079f),
            new(3.542758f, 526.8926f, 170.6218f),
            new(-1155.698f, -1519.297f, 10.63272f),
            new(1392.589f, 3613.899f, 38.94194f),
            new(2435.457f, 4966.514f, 46.8106f),
            new(2451.795f, 4986.356f, 46.81058f),
            new(2441.402f, 4970.8f, 51.56487f),
            new(2448.435f, 4984.749f, 51.56483f),
            new(2433.171f, 4965.435f, 42.3476f),
        };
        _spawnPoint = LocationChooser.ChooseNearestLocation(list);
        _scenario = Rndm.Next(0, 101);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 15f);
        CalloutMessage = "Reports of a Residential Burglary";
        CalloutAdvisory = "Respond to a reported residential burglary. No suspect information at this time.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_BURGLARY_IN IN_OR_ON_POSITION UNITS_RESPOND_CODE_3", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Residential Burglary callout accepted.");
        } else { Settings.DetailedLogging = false; }
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Residential Burglary",
            "~b~Dispatch: ~w~Respond to a reported residential burglary at an apartment building. Suspect unknown. Proceed with caution. Respond with ~r~Code 3");

        _aggressor = new(PedList[Rndm.Next(PedList.Length)], _spawnPoint, 0f)
        {
            IsPersistent = true,
            BlockPermanentEvents = true,
            Armor = 200
        };
        _aggressor.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);

        _victim = new Ped(_aggressor.GetOffsetPosition(new Vector3(0, 1.8f, 0)))
        {
            IsPersistent = true,
            BlockPermanentEvents = true
        };
        _victim.Tasks.PutHandsUp(-1, _aggressor);

        _searchArea = _spawnPoint.Around2D(1f, 2f);
        _blip = new(_searchArea, 30f)
        {
            Color = Color.Yellow,
            Alpha = 0.5f
        };
        _blip.EnableRoute(Color.Yellow);

        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_aggressor != null && _aggressor.Exists()) _aggressor.Delete();
        if (_victim != null && _victim.Exists()) _victim.Delete();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (_aggressor != null && _aggressor.Exists() && _aggressor.DistanceTo(MainPlayer) < 25f && !_notificationDisplayed)
        {
            if (_blip != null && _blip.Exists()) _blip.Delete();
            if (Settings.DetailedLogging)
            {
                Game.LogTrivial("[UnitedCallouts LOG:] Residential Burglary Callout: Player arrived on scene.");
            } else { Settings.DetailedLogging = false; }

            if (Settings.HelpMessages)
            {
                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
                                         "~y~Residential Burglary",
                                         "~b~Dispatch: ~w~Be advised, no further information at this time. Secure the scene and make entry when ready.");
            } else { Settings.HelpMessages = false; }

            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH OFFICERS_ARRIVED_ON_SCENE");
            NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_aggressor, _victim, -1, true);
            _notificationDisplayed = true;
        }

        if (_victim != null && _victim.Exists() && MainPlayer.DistanceTo(_victim) < 10f)
        {
            _victim.PlayAmbientSpeech("GENERIC_SHOCKED_HIGH");
        }

        if (_aggressor && _aggressor.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 8f && !_hasBegunAttacking)
        {
            _hasBegunAttacking = true;
            GameFiber.StartNew(() =>
            {
                switch (_scenario)
                {
                    case > 40:
                        RelationshipGroup agRelationshipGroup = new("AG");
                        RelationshipGroup viRelationshipGroup = new("VI");

                        _aggressor.RelationshipGroup = agRelationshipGroup;
                        if (_victim != null && _victim.Exists()) _victim.RelationshipGroup = viRelationshipGroup;
                        agRelationshipGroup.SetRelationshipWith(viRelationshipGroup, Relationship.Hate);
                        _aggressor.Tasks.FightAgainstClosestHatedTarget(1000f);
                        GameFiber.Wait(200);
                        agRelationshipGroup.SetRelationshipWith(MainPlayer.RelationshipGroup, Relationship.Hate);
                        agRelationshipGroup.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
                        _aggressor.Tasks.FightAgainstClosestHatedTarget(150f, -1);
                        GameFiber.Wait(600);
                        break;
                    default:
                        _aggressor.Tasks.FightAgainst(MainPlayer);
                        GameFiber.Wait(2000);
                        break;
                }
            }, "Residential Burglary [UnitedCallouts]");
        }

        if (_aggressor != null && _aggressor.IsDead && !_notificationDisplayed2)
        {
            if (Settings.HelpMessages)
            {
                Game.DisplayHelp("~b~Dispatch: ~w~Copy that, suspect detained. Secure the scene and confirm if medical is needed. ~y~End the callout ~w~if you finished.", 5000);
            }
            else { Settings.HelpMessages = false; }
            _notificationDisplayed2 = true;
        }

        if (_aggressor != null && Functions.IsPedArrested(_aggressor) && !_notificationDisplayed3)
        {
            if (Settings.HelpMessages)
            {
                Game.DisplayHelp("~b~Dispatch: ~w~Copy that, suspect in custody. Advise on injuries and request EMS if required. ~y~End the callout ~w~if you finished.", 5000);
            }
            else { Settings.HelpMessages = false; }
            _notificationDisplayed3 = true;
        }
        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        base.Process();
    }

    public override void End()
    {
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_victim != null && _victim.Exists()) _victim.Dismiss();
        if (_aggressor != null && _aggressor.Exists()) _aggressor.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Residential Burglary", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Residental Burglary callout ended.");
        }
        else { Settings.DetailedLogging = false; }
        base.End();
    }
}
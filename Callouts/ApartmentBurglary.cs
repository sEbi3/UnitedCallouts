namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Apartment Burglary", CalloutProbability.Medium)]
public class ApartmentBurglary : Callout
{
    private static readonly string[] PedList = { "g_m_m_chicold_01", "mp_g_m_pros_01" };

    private static readonly string[] WepList =
        { "WEAPON_PISTOL", "WEAPON_SMG", "WEAPON_MACHINEPISTOL", "WEAPON_PUMPSHOTGUN" };

    private static Vector3 _spawnPoint;
    private static Vector3 _searchArea;
    private static Blip _blip;
    private static Ped _aggressor;
    private static Ped _victim;
    private static bool _notificationDisplayed;
    private static bool _hasBegunAttacking;
    private static int _scenario;

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
        CalloutMessage = "[UC]~w~ Reports of an Apartment Burglary.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_BURGLARY_IN IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Apartment Burglary callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Apartment Burglary",
            "~b~Dispatch: ~w~Try to ~o~find the apartment~w~ and arrest the burglar. Respond with ~r~Code 3");

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
        if (_aggressor) _aggressor.Delete();
        if (_victim) _victim.Delete();
        if (_blip) _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (_aggressor.DistanceTo(MainPlayer) < 25f && !_notificationDisplayed)
        {
            if (_blip.Exists()) _blip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
                "~y~Burglary into an apartment",
                "~b~Dispatch: ~w~Investigate the ~y~apartment~w~. Try to arrest the ~o~burglar~w~.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH OFFICERS_ARRIVED_ON_SCENE");
            NativeFunction.Natives.TASK_AIM_GUN_AT_ENTITY(_aggressor, _victim, -1, true);
            _notificationDisplayed = true;
        }

        if (MainPlayer.DistanceTo(_victim) < 20f)
        {
            _victim.PlayAmbientSpeech("GENERIC_SHOCKED_HIGH");
        }

        if (_aggressor &&
            _aggressor.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 8f &&
            !_hasBegunAttacking)
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
                        _victim.RelationshipGroup = viRelationshipGroup;

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
            }, "Apartment Burglary [UnitedCallouts]");
        }

        if (Settings.HelpMessages)
        {
            if (_aggressor && _aggressor.IsDead)
            {
                Game.DisplayHelp(
                    "~y~Dispatch:~w~ Make sure no one else is in the apartment. Otherwise ~g~End~w~ the callout.",
                    5000);
            }

            if (_aggressor && Functions.IsPedArrested(_aggressor))
            {
                Game.DisplayHelp(
                    "~y~Dispatch:~w~ Make sure no one else is in the apartment. Otherwise ~g~End~w~ the callout.",
                    5000);
            }
        }
        else
        {
            Settings.HelpMessages = false;
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        base.Process();
    }

    public override void End()
    {
        if (_blip) _blip.Delete();
        if (_victim) _victim.Dismiss();
        if (_aggressor) _aggressor.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Apartment Burglary", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
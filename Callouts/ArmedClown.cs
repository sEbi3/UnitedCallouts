namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of an Armed Clown", CalloutProbability.Medium)]
public class ArmedClown : Callout
{
    private static readonly string[] PedList = { "s_m_y_clown_01" };
    private static readonly string[] WepList = { "WEAPON_PISTOL", "WEAPON_BAT", "WEAPON_KNIFE", "WEAPON_BOTTLE", "WEAPON_MUSKET", "WEAPON_MACHETE" };

    private Ped _subject;
    private Vector3 _spawnPoint;
    private Vector3 _searchArea;
    private Blip _blip;
    private LHandle _pursuit;
    private int _scenario;
    private bool _hasBegunAttacking;
    private bool _isArmed;
    private bool _hasPursuitBegun;

    public override bool OnBeforeCalloutDisplayed()
    {
        _scenario = Rndm.Next(0, 101);
        _spawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 100f);
        CalloutMessage = "Reports of an Armed Clown";
        CalloutAdvisory = "Suspect described as a male in a clown costume, possibly armed. No further information at this time.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS ASSAULT_WITH_AN_DEADLY_WEAPON CIV_ASSISTANCE IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Reports of an Armed Clown callout accepted.");
        }
        else { Settings.DetailedLogging = false; }
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Reports of an Armed Clown",
            "~b~Dispatch: ~w~Respond to reports of an armed male wearing a clown costume. Weapon reported. Use extreme caution. Respond with ~r~Code 3");

        _subject = new(PedList[Rndm.Next(PedList.Length)], _spawnPoint, 0f)
        {
            BlockPermanentEvents = true,
            IsPersistent = true
        };
        _subject.Tasks.Wander();

        _searchArea = _spawnPoint.Around2D(1f, 2f);
        _blip = new Blip(_searchArea, 80f)
        {
            Color = Color.Yellow,
            Alpha = 0.5f
        };
        _blip.EnableRoute(Color.Yellow);
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_subject != null && _subject.Exists()) _subject.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (_subject != null && _subject.Exists() && _subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 25f && !_isArmed)
        {
            if (Settings.DetailedLogging)
            {
                Game.LogTrivial("[UnitedCallouts LOG:] Reports of an Armed Clown Callout: Player arrived on scene.");
            }
            else { Settings.DetailedLogging = false; }
            _subject.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
            _isArmed = true;
        }

        if (_subject != null && _subject.Exists() && _subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 25f && !_hasBegunAttacking)
        {
            GameFiber.StartNew(() =>
            {
                _hasBegunAttacking = true;
                switch (_scenario)
                {
                    case > 40:
                        _subject.KeepTasks = true;
                        _subject.Tasks.FightAgainst(MainPlayer);
                        _hasBegunAttacking = true;
                        GameFiber.Wait(2000);
                        break;
                    default:
                        if (!_hasPursuitBegun)
                        {
                            if (Settings.DetailedLogging)
                            {
                                Game.LogTrivial("[UnitedCallouts LOG:] Reports of an Armed Clown Callout: Pursuit has started.");
                            }
                            else { Settings.DetailedLogging = false; }
                            if (_blip != null && _blip.Exists()) _blip.Delete();
                            _pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(_pursuit, _subject);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            _hasPursuitBegun = true;
                        }
                        break;
                }
            }, "Reports of an Armed Clown [UnitedCallouts]");
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (_subject != null && _subject.IsDead) End();
        if (_subject != null && Functions.IsPedArrested(_subject)) End();
        base.Process();
    }

    public override void End()
    {
        if (_subject != null && _subject.Exists()) _subject.Dismiss();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Reports of an Armed Clown", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        if (Functions.IsPursuitStillRunning(_pursuit)) { Functions.ForceEndPursuit(_pursuit); }
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Armed Clown callout ended.");
        }
        else { Settings.DetailedLogging = false; }
        base.End();
    }
}
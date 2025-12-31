namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of an Armed Clown", CalloutProbability.Medium)]
public class ArmedClown : Callout
{
    private static readonly string[] PedList =
        { "s_m_y_clown_01" };

    private static readonly string[] WepList =
        { "WEAPON_PISTOL", "WEAPON_BAT", "WEAPON_KNIFE", "WEAPON_BOTTLE", "WEAPON_MUSKET", "WEAPON_MACHETE" };

    // FIXED: Removed static from all instance fields
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
        CalloutMessage = "[UC]~w~ Reports of an Armed Clown.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS ASSAULT_WITH_AN_DEADLY_WEAPON CIV_ASSISTANCE IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Reports of an Armed Clown callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Reports of an Armed Clown",
            "~b~Dispatch: ~w~The ~r~Armed Clown~w~ was spotted with a firearm! Respond with ~r~Code 3");

        _subject = new(PedList[Rndm.Next(PedList.Length)], _spawnPoint, 0f)
        {
            BlockPermanentEvents = true,
            IsPersistent = true
        };
        _subject.Inventory.GiveNewWeapon("WEAPON_UNARMED", -1, true);
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
        // FIXED: Added exists checks before deletion
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_subject != null && _subject.Exists()) _subject.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        // FIXED: Added null and exists checks before distance calculation
        if (_subject != null && _subject.Exists() &&
            _subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 25f && !_isArmed)
        {
            _subject.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
            _isArmed = true;
        }

        // FIXED: Added null and exists checks
        if (_subject != null && _subject.Exists() &&
            _subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 25f &&
            !_hasBegunAttacking)
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

        // FIXED: Added null checks
        if (_subject != null && _subject.IsDead) End();
        if (_subject != null && Functions.IsPedArrested(_subject)) End();

        base.Process();
    }

    public override void End()
    {
        // FIXED: Added exists checks before cleanup
        if (_subject != null && _subject.Exists()) _subject.Dismiss();
        if (_blip != null && _blip.Exists()) _blip.Delete();

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Reports of an Armed Clown", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of an Armed Clown", CalloutProbability.Medium)]
public class ArmedClown : Callout
{
    private static readonly string[] PedList = 
        { "s_m_y_clown_01" };

    private static readonly string[] WepList =
        { "WEAPON_PISTOL", "WEAPON_BAT", "WEAPON_KNIFE", "WEAPON_BOTTLE", "WEAPON_MUSKET", "WEAPON_MACHETE" };

    private static Ped _subject;
    private static Vector3 _spawnPoint;
    private static Vector3 _searchArea;
    private static Blip _blip;
    private static LHandle _pursuit;
    private static int _scenario;
    private static bool _hasBegunAttacking;
    private static bool _isArmed;
    private static bool _hasPursuitBegun;

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
        _subject.Inventory.EquippedWeapon = "WEAPON_UNARMED";
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
        if (_blip) _blip.Delete();
        if (_subject) _subject.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (_subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 25f && !_isArmed)
        {
            _subject.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
            _isArmed = true;
        }

        if (_subject && _subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 25f &&
            !_hasBegunAttacking)
        {
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
                        if (_blip) _blip.Delete();
                        _pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(_pursuit, _subject);
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        _hasPursuitBegun = true;
                    }
                    break;
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
            "~y~Reports of an Armed Clown", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
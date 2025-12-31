namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Drug Deal In Progress", CalloutProbability.Medium)]
public class DrugDeal : Callout
{
    private static readonly string[] WepList =
        { "WEAPON_PISTOL", "WEAPON_SMG", "WEAPON_MACHINEPISTOL", "WEAPON_PUMPSHOTGUN" };

    private static readonly string[] PedList = { "s_m_y_dealer_01", "u_m_o_tramp_01" };

    // FIXED: Removed static from all instance fields
    private LHandle _pursuit;
    private Blip _blip;
    private Blip _blip2;
    private Vector3 _spawnPoint;
    private Ped _dealer;
    private Ped _victim;
    private int _scenario;
    private bool _hasPursuitBegun;
    private bool _isArmed;
    private bool _hasBegunAttacking;

    public override bool OnBeforeCalloutDisplayed()
    {
        List<Vector3> list = new List<Vector3>
        {
            new(13.29765f, -1033.113f, 29.21461f),
            new(75.73988f, -855.1366f, 30.75766f),
            new(791.2688f, -1112.81f, 22.73129f),
            new(229.2202f, -1773.199f, 28.73569f),

        };

        _spawnPoint = LocationChooser.ChooseNearestLocation(list);
        _scenario = Rndm.Next(0, 100);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 15f);
        CalloutMessage = "[UC]~w~ Reports of a Drug Deal in Progress.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudio("ATTENTION_ALL_UNITS A_DRUG_DEAL_IN_PROGRESS IN_OR_ON_POSITION");
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: DrugDeal callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Drug Deal In Progress",
            "~b~Dispatch: ~w~Try to arrest the Dealer and the Buyer. Respond with ~y~Code 2");

        _dealer = new(PedList[Rndm.Next(PedList.Length)], _spawnPoint, 0f)
        {
            BlockPermanentEvents = true,
            IsPersistent = true
        };

        _victim = new Ped(_dealer.GetOffsetPosition(new Vector3(0, 1.8f, 0)))
        {
            BlockPermanentEvents = true,
            IsPersistent = true
        };

        _blip = _dealer.AttachBlip();
        _blip2 = _victim.AttachBlip();
        _blip.EnableRoute(Color.Yellow);
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        // FIXED: Added exists checks before deletion
        if (_dealer != null && _dealer.Exists()) _dealer.Delete();
        if (_victim != null && _victim.Exists()) _victim.Delete();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_blip2 != null && _blip2.Exists()) _blip2.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        // FIXED: Added null and exists checks before distance calculation
        if (_dealer != null && _dealer.Exists() &&
            _dealer.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 75f && !_isArmed)
        {
            _dealer.Face(_victim);
            if (_victim != null && _victim.Exists()) _victim.Face(_dealer);
            _dealer.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
            _isArmed = true;
        }

        // FIXED: Added null and exists checks
        if (!_hasBegunAttacking && _dealer != null && _dealer.Exists() &&
            _dealer.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 60f)
        {
            _hasBegunAttacking = true;
            GameFiber.StartNew(() =>
            {
                switch (_scenario)
                {
                    case > 40:
                        var agRelationshipGroup = new RelationshipGroup("AG");
                        _dealer.RelationshipGroup = agRelationshipGroup;
                        if (_victim != null && _victim.Exists()) _victim.RelationshipGroup = agRelationshipGroup;
                        agRelationshipGroup.SetRelationshipWith(MainPlayer.RelationshipGroup, Relationship.Hate);
                        _dealer.Tasks.FightAgainst(MainPlayer);
                        Game.DisplayNotification("Arrest the ~o~buyer~w~ who is surrendering!");
                        if (_victim != null && _victim.Exists()) _victim.Tasks.PutHandsUp(-1, MainPlayer);

                        GameFiber.Wait(2000);
                        break;
                    default:
                        if (!_hasPursuitBegun)
                        {
                            _pursuit = Functions.CreatePursuit();
                            if (_dealer != null && _dealer.Exists()) Functions.AddPedToPursuit(_pursuit, _dealer);
                            if (_victim != null && _victim.Exists()) Functions.AddPedToPursuit(_pursuit, _victim);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            _hasPursuitBegun = true;
                        }

                        break;
                }
            }, "Drug Deal In Progress [UnitedCallouts]");
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();

        // FIXED: Added null checks and simplified logic
        if (_dealer != null && _dealer.IsDead && _blip != null && _blip.Exists())
        {
            _blip.Delete();
        }
        if (_dealer != null && Functions.IsPedArrested(_dealer) && _blip != null && _blip.Exists())
        {
            _blip.Delete();
        }
        if (_victim != null && _victim.IsDead && _blip2 != null && _blip2.Exists())
        {
            _blip2.Delete();
        }
        if (_victim != null && Functions.IsPedArrested(_victim) && _blip2 != null && _blip2.Exists())
        {
            _blip2.Delete();
        }

        if (_victim != null && _victim.IsDead && _dealer != null && _dealer.IsDead) End();
        if (_victim != null && Functions.IsPedArrested(_victim) && _dealer != null && Functions.IsPedArrested(_dealer)) End();
        if (_victim != null && Functions.IsPedArrested(_victim) && _dealer != null && _dealer.IsDead) End();
        if (_dealer != null && Functions.IsPedArrested(_dealer) && _victim != null && _victim.IsDead) End();

        base.Process();
    }

    public override void End()
    {
        // FIXED: Added exists checks before cleanup
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_blip2 != null && _blip2.Exists()) _blip2.Delete();
        if (_victim != null && _victim.Exists()) _victim.Dismiss();
        if (_dealer != null && _dealer.Exists()) _dealer.Dismiss();

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Drug Deal in Progress", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}

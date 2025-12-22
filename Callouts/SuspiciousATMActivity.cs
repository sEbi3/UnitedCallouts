using System.Linq;

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Suspicious ATM Activity", CalloutProbability.Medium)]
public class SuspiciousAtmActivity : Callout
{
    private static readonly string[] WepList = { "WEAPON_PISTOL", "WEAPON_CROWBAR", "WEAPON_KNIFE" };

    // FIXED: Removed static from all instance fields
    private Vector3 _spawnPoint;
    private Vector3 _searcharea;
    private Blip _blip;
    private Ped _aggressor;
    private bool _hasBegunAttacking;
    private bool _hasPursuitBegun;
    private LHandle _pursuit;
    private bool _pursuitCreated;
    private int _scenario;

    public override bool OnBeforeCalloutDisplayed()
    {
        Tuple<Vector3, float>[] spawningLocationList =
        {
            Tuple.Create(new Vector3(112.7427f, -818.8912f, 31.33836f), 161.2627f),
            Tuple.Create(new Vector3(-203.61f, -861.6489f, 30.26763f), 25.38977f),
            Tuple.Create(new Vector3(288.3719f, -1282.444f, 29.65594f), 278.5002f),
            Tuple.Create(new Vector3(-526.4995f, -1222.698f, 18.45498f), 151.1967f),
            Tuple.Create(new Vector3(-821.5978f, -1082.233f, 11.13243f), 32.33837f),
            Tuple.Create(new Vector3(-618.8591f, -706.7742f, 30.05278f), 270.148f),
        };
        List<Vector3> list = spawningLocationList.Select(t => t.Item1).ToList();
        int num = LocationChooser.NearestLocationIndex(list);
        _spawnPoint = spawningLocationList[num].Item1;
        _aggressor = new Ped(_spawnPoint, spawningLocationList[num].Item2);
        _scenario = Rndm.Next(0, 100);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 15f);
        CalloutMessage = "[UC]~w~ Reports of Suspicious ATM Activity.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_SUSPICIOUS_ACTIVITY_01 IN_OR_ON_POSITION",
            _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: ATMActivity callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Suspicious ATM Activity",
            "~b~Dispatch: ~w~Someone called the police because of suspicious activitiy at an ATM. Respond with ~r~Code 3");

        _aggressor.IsPersistent = true;
        _aggressor.BlockPermanentEvents = true;
        _aggressor.Armor = 200;
        _aggressor.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);

        _searcharea = _spawnPoint.Around2D(1f, 2f);
        _blip = new Blip(_searcharea, 20f)
        {
            Color = Color.Yellow,
            Alpha = 0.5f
        };
        _blip.EnableRoute(Color.Yellow);
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        // FIXED: Added exists checks
        if (_aggressor != null && _aggressor.Exists()) _aggressor.Delete();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        // FIXED: Added null and exists checks
        if (_aggressor != null && _aggressor.Exists() && !_hasBegunAttacking &&
            _aggressor.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 20f)
        {
            switch (_scenario)
            {
                case > 40:
                    _hasBegunAttacking = true;
                    GameFiber.StartNew(() =>
                    {
                        var agRelationshipGroup = new RelationshipGroup("AG");
                        var viRelationshipGroup = new RelationshipGroup("VI");
                        _aggressor.RelationshipGroup = agRelationshipGroup;
                        MainPlayer.RelationshipGroup = viRelationshipGroup;
                        agRelationshipGroup.SetRelationshipWith(viRelationshipGroup, Relationship.Hate);
                        agRelationshipGroup.SetRelationshipWith(MainPlayer.RelationshipGroup, Relationship.Hate);
                        _aggressor.Tasks.FightAgainstClosestHatedTarget(1000f);
                        GameFiber.Wait(800);
                    });
                    break;
                default:
                    {
                        if (!_hasPursuitBegun)
                        {
                            _pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(_pursuit, _aggressor);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            _pursuitCreated = true;
                            _hasPursuitBegun = true;
                        }
                        break;
                    }
            }
        }

        // FIXED: Added null checks
        if (_aggressor != null && _aggressor.IsDead) End();
        if (_aggressor != null && Functions.IsPedArrested(_aggressor)) End();
        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        base.Process();
    }

    public override void End()
    {
        // FIXED: Added exists checks
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_aggressor != null && _aggressor.Exists()) _aggressor.Dismiss();

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Suspicious ATM Activity", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Suspicious ATM Activity", CalloutProbability.Medium)]
public class SuspiciousAtmActivity : Callout
{
    private string[] _wepList = new string[] { "WEAPON_PISTOL", "WEAPON_CROWBAR", "WEAPON_KNIFE" };
    public Vector3 SpawnPoint;
    public Vector3 Searcharea;
    public Blip Blip;
    public Ped Aggressor;
    private bool _hasBegunAttacking = false;
    private bool _hasPursuitBegun = false;
    private LHandle _pursuit;
    private bool _pursuitCreated = false;
    private int _scenario = 0;

    public override bool OnBeforeCalloutDisplayed()
    {
        Random random = Rndm;
        List<Vector3> list = new List<Vector3>();
        Tuple<Vector3, float>[] spawningLocationList =
        {
            Tuple.Create(new Vector3(112.7427f, -818.8912f, 31.33836f),161.2627f),
            Tuple.Create(new Vector3(-203.61f, -861.6489f, 30.26763f),25.38977f),
            Tuple.Create(new Vector3(288.3719f, -1282.444f, 29.65594f),278.5002f),
            Tuple.Create(new Vector3(-526.4995f, -1222.698f, 18.45498f),151.1967f),
            Tuple.Create(new Vector3(-821.5978f, -1082.233f, 11.13243f),32.33837f),
            Tuple.Create(new Vector3(-618.8591f, -706.7742f, 30.05278f),270.148f),

        };
        for (int i = 0; i < spawningLocationList.Length; i++)
        {
            list.Add(spawningLocationList[i].Item1);
        }
        int num = LocationChooser.NearestLocationIndex(list);
        SpawnPoint = spawningLocationList[num].Item1;
        Aggressor = new Ped(SpawnPoint, spawningLocationList[num].Item2);
        _scenario = Rndm.Next(0, 100);
        ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 15f);
        CalloutMessage = "[UC]~w~ Reports of Suspicious ATM Activity.";
        CalloutPosition = SpawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_SUSPICIOUS_ACTIVITY_01 IN_OR_ON_POSITION", SpawnPoint);
        // Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS IN_OR_ON_POSITION", _SpawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: ATMActivity callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Suspicious ATM Activity", "~b~Dispatch: ~w~Someone called the police because of suspicious activitiy at an ATM. Respond with ~r~Code 3");

        Aggressor.IsPersistent = true;
        Aggressor.BlockPermanentEvents = true;
        Aggressor.Armor = 200;
        Aggressor.Inventory.GiveNewWeapon(new WeaponAsset(_wepList[Rndm.Next((int)_wepList.Length)]), 500, true);

        Searcharea = SpawnPoint.Around2D(1f, 2f);
        Blip = new Blip(Searcharea, 20f);
        Blip.Color = Color.Yellow;
        Blip.EnableRoute(Color.Yellow);
        Blip.Alpha = 0.5f;
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (Aggressor) Aggressor.Delete();
        if (Blip) Blip.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        GameFiber.StartNew(delegate
        {
            if (Aggressor && Aggressor.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 20f && !_hasBegunAttacking)
            {
                if (_scenario > 40)
                {
                    new RelationshipGroup("AG");
                    new RelationshipGroup("VI");
                    Aggressor.RelationshipGroup = "AG";
                    MainPlayer.RelationshipGroup = "VI";
                    Game.SetRelationshipBetweenRelationshipGroups("AG", "VI", Relationship.Hate);
                    Aggressor.Tasks.FightAgainstClosestHatedTarget(1000f);
                    GameFiber.Wait(200);
                    Aggressor.Tasks.FightAgainst(MainPlayer);
                    _hasBegunAttacking = true;
                    GameFiber.Wait(600);
                }
                else
                {
                    if (!_hasPursuitBegun)
                    {
                        _pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(_pursuit, Aggressor);
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        _pursuitCreated = true;
                        _hasPursuitBegun = true;
                    }
                }
            }
            if (Aggressor && Aggressor.IsDead) End();
            if (Functions.IsPedArrested(Aggressor)) End();
            if (MainPlayer.IsDead) End();
            if (Game.IsKeyDown(Settings.EndCall)) End();
        }, "Suspicious ATM Activity [UnitedCallouts]");
        base.Process();
    }

    public override void End()
    {
        if (Blip) Blip.Delete();
        if (Aggressor) Aggressor.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Suspicious ATM Activity", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
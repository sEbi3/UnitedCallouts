namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of Shots Fired", CalloutProbability.Medium)]
public class ShotsFired : Callout
{
    private string[] _wepList = new string[] { "WEAPON_PISTOL", "WEAPON_ASSAULTRIFLE", "WEAPON_SAWNOFFSHOTGUN", "WEAPON_PISTOL50" };
    private Ped _subject;
    private Ped _v1;
    private Ped _v2;
    private Ped _v3;
    private Vector3 _spawnPoint;
    private Vector3 _searcharea;
    private Blip _blip;
    private int _scenario = 0;
    private bool _hasBegunAttacking = false;
    private bool _isArmed = false;
    private bool _hasPursuitBegun = false;

    public override bool OnBeforeCalloutDisplayed()
    {
        Random random = Rndm;
        List<Vector3> list = new List<Vector3>
        {
            new Vector3(-1622.711f, 214.8514f, 60.22071f),
            new Vector3(295.0424f, -578.2471f, 43.18422f),
            new Vector3(-1573.039f, -1169.825f, 2.402837f),
            new Vector3(-1323.908f, 50.76834f, 53.53567f),
            new Vector3(1155.258f, -741.4567f, 57.30391f),
            new Vector3(291.6201f, 179.956f, 104.297f),
        };
        _spawnPoint = LocationChooser.ChooseNearestLocation(list);
        _scenario = Rndm.Next(0, 100);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 100f);
        CalloutMessage = "[UC]~w~ Reports of Shots Fired.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_SHOTS_FIRED_01 IN_OR_ON_POSITION", _spawnPoint);
        // Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS ASSAULT_WITH_AN_DEADLY_WEAPON CIV_ASSISTANCE IN_OR_ON_POSITION", _SpawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Reports of Shots Fired callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Reports of Shots Fired", "~b~Dispatch: ~w~Someone called the police because of shots fired. Respond with ~r~Code 3");

        _subject = new Ped(_spawnPoint);
        _subject.Inventory.GiveNewWeapon("WEAPON_UNARMED", 500, true);
        _subject.BlockPermanentEvents = true;
        _subject.IsPersistent = true;
        _subject.Tasks.Wander();

        _v1 = new Ped(_spawnPoint);
        _v2 = new Ped(_spawnPoint);
        _v3 = new Ped(_spawnPoint);
        _v1.IsPersistent = true;
        _v2.IsPersistent = true;
        _v3.IsPersistent = true;
        _v1.Tasks.Wander();
        _v2.Tasks.Wander();
        _v3.Tasks.Wander();

        _searcharea = _spawnPoint.Around2D(1f, 2f);
        _blip = new Blip(_searcharea, 80f);
        _blip.Color = Color.Red;
        _blip.EnableRoute(Color.Red);
        _blip.Alpha = 0.5f;

        if (Settings.ActivateAiBackup)
        {
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.SwatTeam);
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
        } else { return false; }
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_blip) _blip.Delete();
        if (_subject) _subject.Delete();
        if (_v1) _v1.Delete();
        if (_v2) _v2.Delete();
        if (_v3) _v3.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        GameFiber.StartNew(delegate
        {
            if (_subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 40f)
            {
                if (_blip) _blip.Delete();
            }
            if (_subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 70f && !_isArmed)
            {
                _subject.Inventory.GiveNewWeapon(new WeaponAsset(_wepList[Rndm.Next((int)_wepList.Length)]), 500, true);
                _isArmed = true;
            }
            if (_subject && _subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 40f && !_hasBegunAttacking)
            {
                if (_scenario > 40)
                {
                    new RelationshipGroup("AG");
                    new RelationshipGroup("VI");
                    _subject.RelationshipGroup = "AG";
                    _v1.RelationshipGroup = "VI";
                    _v2.RelationshipGroup = "VI";
                    _v3.RelationshipGroup = "VI";
                    _subject.KeepTasks = true;
                    Game.SetRelationshipBetweenRelationshipGroups("AG", "VI", Relationship.Hate);
                    _subject.Tasks.FightAgainstClosestHatedTarget(1000f);
                    GameFiber.Wait(2000);
                    _subject.Tasks.FightAgainst(MainPlayer);
                    _hasBegunAttacking = true;
                    GameFiber.Wait(600);
                }
                else
                {
                    if (!_hasPursuitBegun)
                    {
                        _subject.Face(MainPlayer);
                        _subject.Tasks.PutHandsUp(-1, MainPlayer);
                        Game.DisplayNotification("~b~Dispatch:~w~ The suspect is surrendering. Try to ~o~arrest him~w~.");
                        _hasPursuitBegun = true;
                    }
                }
            }
            if (MainPlayer.IsDead) End();
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (_subject && _subject.IsDead) End();
            if (_subject && Functions.IsPedArrested(_subject)) End();
        }, "Reports of Shots Fired [UnitedCallouts]");
        base.Process();
    }

    public override void End()
    {
        if (_subject) _subject.Dismiss();
        if (_v1) _v1.Dismiss();
        if (_v2) _v2.Dismiss();
        if (_v3) _v3.Dismiss();
        if (_blip) _blip.Delete();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Reports of Shots Fired", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
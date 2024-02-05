namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of a Gang Shootout", CalloutProbability.Medium)]
public class GangShootout : Callout
{
    // BALLAS Peds
    private static Ped _grovePed1;
    private static Ped _grovePed2;

    private static Ped _grovePed3;

    // GROVE Peds
    private static Ped _ballasPed1;
    private static Ped _ballasPed2;
    private static Ped _ballasPed3;
    private static Vector3 _spawnPoint;
    private static Blip _blip;
    private static Blip _blip2;
    private static Blip _blip3;
    private static Blip _blip4;
    private static Blip _blip5;
    private static Blip _blip6;
    private static bool _hasBegunAttacking;

    public override bool OnBeforeCalloutDisplayed()
    {
        List<Vector3> list = new List<Vector3>
        {
            new(105.1732f, -1937.076f, 20.41693f),
            new(-183.6035f, -1669.903f, 33.10927f),
            new(327.7954f, -2034.417f, 20.5504f),
            new(-743.142f, -923.304f, 18.68627f),
            new(1111.735f, -1610.99f, 4.408495f),

        };
        _spawnPoint = LocationChooser.ChooseNearestLocation(list);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 70f);
        CalloutMessage = "[UC]~w~ Reports of a Gang Shootout.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_01 CITIZENS_REPORT_01 GANG_RELATED_VIOLENCE UNITS_RESPOND_CODE_99_01", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Gang Shootout callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Gang Shootout", "~b~Dispatch: ~w~The gang members are marked on the map. Respond with ~r~Code 3");

        _grovePed1 = new Ped("g_m_y_ballasout_01", _spawnPoint.ExtensionAround(20f), 0f);
        _grovePed2 = new Ped("g_m_y_ballasout_01", _spawnPoint.ExtensionAround(30f), 0f);
        _grovePed3 = new Ped("g_m_y_ballasout_01", _spawnPoint.ExtensionAround(22f), 0f);
        _ballasPed1 = new Ped("g_m_y_famca_01", _spawnPoint.ExtensionAround(24f), 0f);
        _ballasPed2 = new Ped("g_m_y_famca_01", _spawnPoint.ExtensionAround(26f), 0f);
        _ballasPed3 = new Ped("g_m_y_famca_01", _spawnPoint.ExtensionAround(28f), 0f);

        _grovePed1.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
        _grovePed2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
        _grovePed3.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
        _ballasPed1.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
        _ballasPed2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
        _ballasPed3.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);

        _blip = _grovePed1.AttachBlip();
        _blip2 = _grovePed2.AttachBlip();
        _blip3 = _grovePed3.AttachBlip();
        _blip4 = _ballasPed1.AttachBlip();
        _blip5 = _ballasPed2.AttachBlip();
        _blip6 = _ballasPed3.AttachBlip();
        _blip.EnableRoute(Color.Yellow);

        if (Settings.ActivateAiBackup)
        {
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Code3,
                LSPD_First_Response.EBackupUnitType.SwatTeam);
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Code3,
                LSPD_First_Response.EBackupUnitType.LocalUnit);
        }
        else
        {
            Settings.ActivateAiBackup = false;
        }

        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_blip) _blip.Delete();
        if (_blip2) _blip2.Delete();
        if (_blip3) _blip3.Delete();
        if (_blip4) _blip4.Delete();
        if (_blip5) _blip5.Delete();
        if (_blip6) _blip6.Delete();
        if (_grovePed1) _grovePed1.Delete();
        if (_grovePed2) _grovePed2.Delete();
        if (_grovePed3) _grovePed3.Delete();
        if (_ballasPed1) _ballasPed1.Delete();
        if (_ballasPed2) _ballasPed2.Delete();
        if (_ballasPed3) _ballasPed3.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (MainPlayer.DistanceTo(_spawnPoint) < 100f && !_hasBegunAttacking)
        {
            var ballasRelationshipGroup = new RelationshipGroup("BALLAS");
            var groveRelationshipGroup = new RelationshipGroup("GROVE");
            _grovePed1.RelationshipGroup = ballasRelationshipGroup;
            _grovePed2.RelationshipGroup = ballasRelationshipGroup;
            _grovePed3.RelationshipGroup = ballasRelationshipGroup;
            _ballasPed1.RelationshipGroup = groveRelationshipGroup;
            _ballasPed2.RelationshipGroup = groveRelationshipGroup;
            _ballasPed3.RelationshipGroup = groveRelationshipGroup;

            ballasRelationshipGroup.SetRelationshipWith(groveRelationshipGroup, Relationship.Hate);
            groveRelationshipGroup.SetRelationshipWith(ballasRelationshipGroup, Relationship.Hate);
            ballasRelationshipGroup.SetRelationshipWith(MainPlayer.RelationshipGroup, Relationship.Hate);
            groveRelationshipGroup.SetRelationshipWith(MainPlayer.RelationshipGroup, Relationship.Hate);
            ballasRelationshipGroup.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            groveRelationshipGroup.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            
            _grovePed1.Tasks.FightAgainstClosestHatedTarget(1000f);
            _grovePed2.Tasks.FightAgainstClosestHatedTarget(1000f);
            _grovePed3.Tasks.FightAgainstClosestHatedTarget(1000f);
            _ballasPed1.Tasks.FightAgainstClosestHatedTarget(1000f);
            _ballasPed2.Tasks.FightAgainstClosestHatedTarget(1000f);
            _ballasPed3.Tasks.FightAgainstClosestHatedTarget(1000f);
            _hasBegunAttacking = true;
            GameFiber.Sleep(5000);
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (_grovePed1 && _grovePed1.IsDead && _grovePed2 && _grovePed2.IsDead && _grovePed3 && _grovePed3.IsDead &&
            _ballasPed1 && _ballasPed1.IsDead && _ballasPed2 && _ballasPed2.IsDead && _ballasPed3 &&
            _ballasPed3.IsDead) End();
        if (_grovePed1 && Functions.IsPedArrested(_grovePed1) && _grovePed2 && Functions.IsPedArrested(_grovePed2) &&
            _grovePed3 && Functions.IsPedArrested(_grovePed3) && _ballasPed1 && Functions.IsPedArrested(_ballasPed1) &&
            _ballasPed2 && Functions.IsPedArrested(_ballasPed2) && _ballasPed3 &&
            Functions.IsPedArrested(_ballasPed3)) End();
        base.Process();
    }

    public override void End()
    {
        if (_blip) _blip.Delete();
        if (_blip2) _blip2.Delete();
        if (_blip3) _blip3.Delete();
        if (_blip4) _blip4.Delete();
        if (_blip5) _blip5.Delete();
        if (_blip6) _blip6.Delete();
        if (_grovePed1) _grovePed1.Dismiss();
        if (_grovePed2) _grovePed2.Dismiss();
        if (_grovePed3) _grovePed3.Dismiss();
        if (_ballasPed1) _ballasPed1.Dismiss();
        if (_ballasPed2) _ballasPed2.Dismiss();
        if (_ballasPed3) _ballasPed3.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Gang Shootout", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        // Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
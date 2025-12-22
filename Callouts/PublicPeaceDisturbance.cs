namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Public Peace Disturbance", CalloutProbability.Medium)]
class PublicPeaceDisturbance : Callout
{
    // FIXED: Removed static from all instance fields
    private Ped _ag1;
    private Ped _ag2;
    private Vector3 _spawnPoint;
    private Blip _blip;
    private Blip _blip2;
    private bool _hasBegunAttacking;

    public override bool OnBeforeCalloutDisplayed()
    {
        List<Vector3> list = new List<Vector3>
        {
            new(-227.4819f, 285.4045f, 91.66596f),
            new(-358.5807f, 265.6691f, 84.38922f),
            new(-434.0634f, 257.6701f, 82.99319f),
            new(1986.708f, 3050.588f, 47.2151f),
            new(99.72031f, 215.0221f, 107.9258f),
            new(-292.416f, -303.66f, 10.06316f),
            new(-199.3456f, -786.0698f, 30.45403f),
            new(-708.6214f, -913.0787f, 19.21559f),
            new(-828.1153f, -1074.215f, 11.32811f),
        };
        _spawnPoint = LocationChooser.ChooseNearestLocation(list);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 70f);
        CalloutMessage = "[UC]~w~ Reports of a Public Peace Disturbance in Progress.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_CIVILIAN_NEEDING_ASSISTANCE_IN",
            _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Public Peace Disturbance callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Public Peace Disturbance",
            "~b~Dispatch: ~w~Stop the two People who are fighting! Respond with ~r~Code 3");

        _ag1 = new Ped(_spawnPoint)
        {
            Armor = 300,
            Health = 300
        };
        _ag2 = new Ped(_spawnPoint)
        {
            Armor = 300,
            Health = 300
        };

        _blip = _ag1.AttachBlip();
        _blip2 = _ag2.AttachBlip();
        _blip.EnableRoute(Color.Yellow);

        if (Settings.ActivateAiBackup)
        {
            Functions.RequestBackup(_spawnPoint, LSPD_First_Response.EBackupResponseType.Code3,
                LSPD_First_Response.EBackupUnitType.LocalUnit);
        }

        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        // FIXED: Added exists checks before deletion
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_blip2 != null && _blip2.Exists()) _blip2.Delete();
        if (_ag1 != null && _ag1.Exists()) _ag1.Delete();
        if (_ag2 != null && _ag2.Exists()) _ag2.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (!_hasBegunAttacking && MainPlayer.DistanceTo(_spawnPoint) < 80f)
        {
            var ballasRelationshipGroup = new RelationshipGroup("BALLAS");
            var groveRelationshipGroup = new RelationshipGroup("GROVE");

            // FIXED: Added null and exists checks
            if (_ag1 != null && _ag1.Exists()) _ag1.RelationshipGroup = ballasRelationshipGroup;
            if (_ag2 != null && _ag2.Exists()) _ag2.RelationshipGroup = groveRelationshipGroup;

            ballasRelationshipGroup.SetRelationshipWith(groveRelationshipGroup, Relationship.Hate);
            groveRelationshipGroup.SetRelationshipWith(ballasRelationshipGroup, Relationship.Hate);

            if (_ag1 != null && _ag1.Exists()) _ag1.Tasks.FightAgainstClosestHatedTarget(1000f);
            if (_ag2 != null && _ag2.Exists()) _ag2.Tasks.FightAgainstClosestHatedTarget(1000f);
            _hasBegunAttacking = true;
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();

        // FIXED: Added null checks and simplified logic
        if (_ag1 != null && _ag1.IsDead && _blip != null && _blip.Exists()) _blip.Delete();
        if (_ag2 != null && _ag2.IsDead && _blip2 != null && _blip2.Exists()) _blip2.Delete();
        if (_ag1 != null && _ag1.IsDead && _ag2 != null && _ag2.IsDead) End();
        if (_ag1 != null && Functions.IsPedArrested(_ag1) && _ag2 != null && Functions.IsPedArrested(_ag2)) End();

        base.Process();
    }

    public override void End()
    {
        // FIXED: Added exists checks before cleanup
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_blip2 != null && _blip2.Exists()) _blip2.Delete();
        if (_ag1 != null && _ag1.Exists()) _ag1.Dismiss();
        if (_ag2 != null && _ag2.Exists()) _ag2.Dismiss();

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Public Peace Disturbance", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}

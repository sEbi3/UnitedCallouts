using System.Linq;

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of a Gang Shootout", CalloutProbability.Medium)]
public class GangShootout : Callout
{
    // GROVE Peds
    private static Ped GrovePed1 =>_grovePeds[0];
    private static Ped GrovePed2 =>_grovePeds[1];
    private static Ped GrovePed3 =>_grovePeds[2];

    // BALLAS Peds
    private static Ped BallasPed1 => _ballasPeds[0];
    private static Ped BallasPed2 => _ballasPeds[1];
    private static Ped BallasPed3 => _ballasPeds[2];
    
    private static Vector3 _spawnPoint;
    
    // Blips
    private static Blip _blip;
    private static Blip _blip2;
    private static Blip _blip3;
    private static Blip _blip4;
    private static Blip _blip5;
    private static Blip _blip6;
    private static bool _hasBegunAttacking;

    // Arrays
    private static Ped[] _grovePeds = new Ped[3];
    private static Ped[] _ballasPeds = new Ped[3];
    
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

        for (int i = 0; i < _grovePeds.Length; i++)
        {
            _grovePeds[i] = new("g_m_y_ballasout_01", _spawnPoint.ExtensionAround(30f), 0f);
        }
        for (int i = 0; i < _ballasPeds.Length; i++)
        {
            _ballasPeds[i] = new("g_m_y_famca_01", _spawnPoint.ExtensionAround(30f), 0f);
        }

        GrovePed1.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
        GrovePed2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
        GrovePed3.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
        BallasPed1.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
        BallasPed2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
        BallasPed3.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);

        _blip = GrovePed1.AttachBlip();
        _blip2 = GrovePed2.AttachBlip();
        _blip3 = GrovePed3.AttachBlip();
        _blip4 = BallasPed1.AttachBlip();
        _blip5 = BallasPed2.AttachBlip();
        _blip6 = BallasPed3.AttachBlip();
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
        foreach (var ped in _grovePeds)
        {
            if (ped) ped.Delete();
        }
        foreach (var ped in _ballasPeds)
        {
            if (ped) ped.Delete();
        }
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (!_hasBegunAttacking && MainPlayer.DistanceTo(_spawnPoint) < 100f)
        {
            _hasBegunAttacking = true;

            GameFiber.StartNew(() =>
            {
                var ballasRelationshipGroup = new RelationshipGroup("BALLAS");
                var groveRelationshipGroup = new RelationshipGroup("GROVE");
            
                ballasRelationshipGroup.SetRelationshipWith(groveRelationshipGroup, Relationship.Hate);
                groveRelationshipGroup.SetRelationshipWith(ballasRelationshipGroup, Relationship.Hate);
                ballasRelationshipGroup.SetRelationshipWith(MainPlayer.RelationshipGroup, Relationship.Hate);
                groveRelationshipGroup.SetRelationshipWith(MainPlayer.RelationshipGroup, Relationship.Hate);
                ballasRelationshipGroup.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
                groveRelationshipGroup.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
                foreach (var ped in _grovePeds)
                {
                    ped.RelationshipGroup = groveRelationshipGroup;
                    ped.Tasks.FightAgainstClosestHatedTarget(1000f);
                }
                foreach (var ped in _ballasPeds)
                {
                    ped.RelationshipGroup = ballasRelationshipGroup;
                    ped.Tasks.FightAgainstClosestHatedTarget(1000f);
                }
                GameFiber.Sleep(5000);
            }, "Gang Shootout [UnitedCallouts]");
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (GrovePed1 && GrovePed1.IsDead && GrovePed2 && GrovePed2.IsDead && GrovePed3 && GrovePed3.IsDead &&
            BallasPed1 && BallasPed1.IsDead && BallasPed2 && BallasPed2.IsDead && BallasPed3 &&
            BallasPed3.IsDead) End();
        if (GrovePed1 && Functions.IsPedArrested(GrovePed1) && GrovePed2 && Functions.IsPedArrested(GrovePed2) &&
            GrovePed3 && Functions.IsPedArrested(GrovePed3) && BallasPed1 && Functions.IsPedArrested(BallasPed1) &&
            BallasPed2 && Functions.IsPedArrested(BallasPed2) && BallasPed3 &&
            Functions.IsPedArrested(BallasPed3)) End();
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
        if (GrovePed1) GrovePed1.Dismiss();
        if (GrovePed2) GrovePed2.Dismiss();
        if (GrovePed3) GrovePed3.Dismiss();
        if (BallasPed1) BallasPed1.Dismiss();
        if (BallasPed2) BallasPed2.Dismiss();
        if (BallasPed3) BallasPed3.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Gang Shootout", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        // Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH WE_ARE_CODE FOUR NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}

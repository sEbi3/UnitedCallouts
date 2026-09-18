using Rage;

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of a Gang Shootout", CalloutProbability.Medium)]
public class GangShootout : Callout
{
    private static readonly string[] WepList =
    {
        "WEAPON_SMG", "WEAPON_PUMPSHOTGUN", "weapon_microsmg", "weapon_machinepistol", "weapon_compactrifle",
        "weapon_combatpistol", "weapon_pistol"
    };

    private Ped GrovePed1 => _grovePeds[0];
    private Ped GrovePed2 => _grovePeds[1];
    private Ped GrovePed3 => _grovePeds[2];
    private Ped BallasPed1 => _ballasPeds[0];
    private Ped BallasPed2 => _ballasPeds[1];
    private Ped BallasPed3 => _ballasPeds[2];
    private Vector3 _spawnPoint;
    private Blip _blip;
    private Blip _blip2;
    private Blip _blip3;
    private Blip _blip4;
    private Blip _blip5;
    private Blip _blip6;
    private bool _hasBegunAttacking;
    private Ped[] _grovePeds = new Ped[3];
    private Ped[] _ballasPeds = new Ped[3];

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
        CalloutMessage = "Reports of a Gang Shootout";
        CalloutAdvisory = "Reports received of an active gang-related shooting in progress.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_01 CITIZENS_REPORT_01 GANG_RELATED_VIOLENCE UNITS_RESPOND_CODE_99_01", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Reports of a Gang Shootout callout accepted.");
        }
        else { Settings.DetailedLogging = false; }
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Reports of a Gang Shootout", "~b~Dispatch: ~w~Respond to an active gang-related shooting. Multiple shooters reported. Use extreme caution and await for backup. Respond with ~r~Code 3~w~.");

        for (int i = 0; i < _grovePeds.Length; i++)
        {
            _grovePeds[i] = new("g_m_y_ballasout_01", _spawnPoint.ExtensionAround(30f), 0f);
        }
        for (int i = 0; i < _ballasPeds.Length; i++)
        {
            _ballasPeds[i] = new("g_m_y_famca_01", _spawnPoint.ExtensionAround(30f), 0f);
        }

        GrovePed1.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
        GrovePed2.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
        GrovePed3.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
        BallasPed1.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
        BallasPed2.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
        BallasPed3.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);

        GrovePed1.Armor = 200;
        GrovePed2.Armor = 200;
        GrovePed3.Armor = 200;
        BallasPed1.Armor = 200;
        BallasPed2.Armor = 200;
        BallasPed3.Armor = 200;

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
        } else { Settings.ActivateAiBackup = false; }

        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_blip2 != null && _blip2.Exists()) _blip2.Delete();
        if (_blip3 != null && _blip3.Exists()) _blip3.Delete();
        if (_blip4 != null && _blip4.Exists()) _blip4.Delete();
        if (_blip5 != null && _blip5.Exists()) _blip5.Delete();
        if (_blip6 != null && _blip6.Exists()) _blip6.Delete();
        foreach (var ped in _grovePeds)
        {
            if (ped != null && ped.Exists()) ped.Delete();
        }
        foreach (var ped in _ballasPeds)
        {
            if (ped != null && ped.Exists()) ped.Delete();
        }
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (!_hasBegunAttacking && MainPlayer.DistanceTo(_spawnPoint) < 200f)
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
                    if (ped != null && ped.Exists())
                    {
                        ped.RelationshipGroup = groveRelationshipGroup;
                        ped.Tasks.FightAgainstClosestHatedTarget(1000f);
                    }
                }
                foreach (var ped in _ballasPeds)
                {
                    if (ped != null && ped.Exists())
                    {
                        ped.RelationshipGroup = ballasRelationshipGroup;
                        ped.Tasks.FightAgainstClosestHatedTarget(1000f);
                    }
                }

                if (Settings.DetailedLogging)
                {
                    Game.LogTrivial("[UnitedCallouts LOG:] Gang Shootout: Shootout scenario has started.");
                }
                else { Settings.DetailedLogging = false; }
                GameFiber.Sleep(5000);
            }, "Reports of a Gang Shootout [UnitedCallouts]");
        }

        if (GrovePed1 != null && GrovePed1.IsDead || Functions.IsPedArrested(GrovePed1))
        {
            if (_blip != null && _blip.Exists()) _blip.Delete();
        }

        if (GrovePed2 != null && GrovePed2.IsDead || Functions.IsPedArrested(GrovePed2))
        {
            if (_blip2 != null && _blip2.Exists()) _blip2.Delete();
        }

        if (GrovePed3 != null && GrovePed3.IsDead || Functions.IsPedArrested(GrovePed3))
        {
            if (_blip3 != null && _blip3.Exists()) _blip3.Delete();
        }

        if (BallasPed1 != null && BallasPed1.IsDead || Functions.IsPedArrested(BallasPed1))
        {
            if (_blip4 != null && _blip4.Exists()) _blip4.Delete();
        }

        if (BallasPed2 != null && BallasPed2.IsDead || Functions.IsPedArrested(BallasPed2))
        {
            if (_blip5 != null && _blip5.Exists()) _blip5.Delete();
        }

        if (BallasPed3 != null && BallasPed3.IsDead || Functions.IsPedArrested(BallasPed3))
        {
            if (_blip6 != null && _blip6.Exists()) _blip6.Delete();
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (GrovePed1 != null && GrovePed1.Exists() && Functions.IsPedArrested(GrovePed1) &&
            GrovePed2 != null && GrovePed2.Exists() && Functions.IsPedArrested(GrovePed2) &&
            GrovePed3 != null && GrovePed3.Exists() && Functions.IsPedArrested(GrovePed3) &&
            BallasPed1 != null && BallasPed1.Exists() && Functions.IsPedArrested(BallasPed1) &&
            BallasPed2 != null && BallasPed2.Exists() && Functions.IsPedArrested(BallasPed2) &&
            BallasPed3 != null && BallasPed3.Exists() && Functions.IsPedArrested(BallasPed3)) End();

        if (GrovePed1 != null && GrovePed1.Exists() && GrovePed1.IsDead &&
            GrovePed2 != null && GrovePed2.Exists() && GrovePed2.IsDead &&
            GrovePed3 != null && GrovePed3.Exists() && GrovePed3.IsDead &&
            BallasPed1 != null && BallasPed1.Exists() && BallasPed1.IsDead &&
            BallasPed2 != null && BallasPed2.Exists() && BallasPed2.IsDead &&
            BallasPed3 != null && BallasPed3.Exists() && BallasPed3.IsDead) End();
        base.Process();
    }

    public override void End()
    {
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_blip2 != null && _blip2.Exists()) _blip2.Delete();
        if (_blip3 != null && _blip3.Exists()) _blip3.Delete();
        if (_blip4 != null && _blip4.Exists()) _blip4.Delete();
        if (_blip5 != null && _blip5.Exists()) _blip5.Delete();
        if (_blip6 != null && _blip6.Exists()) _blip6.Delete();

        if (GrovePed1 != null && GrovePed1.Exists()) GrovePed1.Dismiss();
        if (GrovePed2 != null && GrovePed2.Exists()) GrovePed2.Dismiss();
        if (GrovePed3 != null && GrovePed3.Exists()) GrovePed3.Dismiss();
        if (BallasPed1 != null && BallasPed1.Exists()) BallasPed1.Dismiss();
        if (BallasPed2 != null && BallasPed2.Exists()) BallasPed2.Dismiss();
        if (BallasPed3 != null && BallasPed3.Exists()) BallasPed3.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Reports of a Gang Shootout", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Reports of a Gang Shootout callout ended.");
        }
        else { Settings.DetailedLogging = false; }
        base.End();
    }
}
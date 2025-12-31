namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Heavily-Armed Terrorist Attack", CalloutProbability.Medium)]
public class ArmedTerroristAttack : Callout
{
    private static readonly string[] WepList =
        { "WEAPON_MINIGUN", "WEAPON_MG", "WEAPON_COMBATMG", "weapon_combatmg_mk2", "weapon_gusenberg" };

    // FIXED: Removed static from all instance fields
    private Ped _subject;
    private Ped _v1;
    private Ped _v2;
    private Ped _v3;
    private Vector3 _spawnPoint;
    private Vector3 _searcharea;
    private Blip _blip;
    private int _scenario;
    private bool _hasBegunAttacking;
    private bool _isArmed;
    private bool _hasPursuitBegun = false;

    public override bool OnBeforeCalloutDisplayed()
    {
        _scenario = Rndm.Next(0, 100);
        _spawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 100f);
        CalloutMessage = "[UC]~w~ Reports of a Heavily-Armed Terrorist Attack";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS ASSAULT_WITH_AN_DEADLY_WEAPON CIV_ASSISTANCE IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Heavily-Armed Terrorist Attack callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Terrorist Attack",
            "~b~Dispatch: ~w~The ~r~armored person~w~ was spotted with a firearm! Search the ~y~area~w~ for the armored person. Respond with ~r~Code 3");

        _subject = new("u_m_y_juggernaut_01", _spawnPoint, 0f)
        {
            BlockPermanentEvents = true,
            IsPersistent = true,
            CanRagdoll = false,
            Armor = 1200,
            Health = 1200,
            MaxHealth = 1200,
            CanAttackFriendlies = true
        };
        _subject.Inventory.GiveNewWeapon("WEAPON_UNARMED", -1, true);
        _subject.Tasks.Wander();
        NativeFunction.Natives.SET_PED_SUFFERS_CRITICAL_HITS(_subject, false);
        NativeFunction.Natives.SetPedPathCanUseClimbovers(_subject, true);
        Functions.SetPedCantBeArrestedByPlayer(_subject, true);

        _v1 = new Ped(_spawnPoint);
        _v2 = new Ped(_spawnPoint);
        _v3 = new Ped(_spawnPoint);
        _v1.Health = 150;
        _v2.Health = 150;
        _v3.Health = 150;
        _v1.Tasks.Wander();
        _v2.Tasks.Wander();
        _v3.Tasks.Wander();

        _searcharea = _spawnPoint.Around2D(1f, 2f);
        _blip = new Blip(_searcharea, 90f)
        {
            Color = Color.Yellow,
            Alpha = 0.5f
        };
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
        // FIXED: Added exists checks before deletion
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_subject != null && _subject.Exists()) _subject.Delete();
        if (_v1 != null && _v1.Exists()) _v1.Dismiss();
        if (_v2 != null && _v2.Exists()) _v2.Dismiss();
        if (_v3 != null && _v3.Exists()) _v3.Dismiss();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        // FIXED: Added null and exists checks before distance calculation
        if (_subject != null && _subject.Exists() && !_isArmed &&
            _subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 35f)
        {
            if (_blip != null && _blip.Exists()) _blip.Delete();
            _subject.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
            _isArmed = true;
        }

        // FIXED: Added null and exists checks before distance calculation
        if (_subject != null && _subject.Exists() && !_hasBegunAttacking &&
            _subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 50f)
        {
            RelationshipGroup agRelationshipGroup = new("AG");
            RelationshipGroup viRelationshipGroup = new("VI");

            _subject.RelationshipGroup = agRelationshipGroup;
            if (_v1 != null && _v1.Exists()) _v1.RelationshipGroup = viRelationshipGroup;
            if (_v2 != null && _v2.Exists()) _v2.RelationshipGroup = viRelationshipGroup;
            if (_v3 != null && _v3.Exists()) _v3.RelationshipGroup = viRelationshipGroup;

            agRelationshipGroup.SetRelationshipWith(MainPlayer.RelationshipGroup, Relationship.Hate);
            agRelationshipGroup.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);

            _subject.KeepTasks = true;

            _hasBegunAttacking = true;
            GameFiber.StartNew(() =>
            {
                switch (_scenario)
                {
                    case > 40:
                        agRelationshipGroup.SetRelationshipWith(viRelationshipGroup, Relationship.Hate);
                        if (_subject != null && _subject.Exists()) _subject.Tasks.FightAgainstClosestHatedTarget(1000f);
                        GameFiber.Wait(2000);
                        if (_subject != null && _subject.Exists()) _subject.Tasks.FightAgainstClosestHatedTarget(1000f, -1);
                        GameFiber.Wait(600);
                        break;
                    default:
                        if (!_hasPursuitBegun)
                        {
                            if (_subject != null && _subject.Exists()) _subject.Tasks.FightAgainstClosestHatedTarget(1000f, -1);
                            GameFiber.Wait(2000);
                        }
                        break;
                }
            }, "Armored Person [UnitedCallouts]");
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
        if (_v1 != null && _v1.Exists()) _v1.Dismiss();
        if (_v2 != null && _v2.Exists()) _v2.Dismiss();
        if (_v3 != null && _v3.Exists()) _v3.Dismiss();

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Terrorist Attack", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}

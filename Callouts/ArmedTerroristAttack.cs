namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Heavily-Armed Terrorist Attack", CalloutProbability.Medium)]
public class ArmedTerroristAttack : Callout
{
    private static readonly string[] WepList =
        { "WEAPON_MINIGUN", "WEAPON_MG", "WEAPON_COMBATMG", "weapon_combatmg_mk2", "weapon_gusenberg" };

    private static Ped _subject;
    private static Ped _v1;
    private static Ped _v2;
    private static Ped _v3;
    private static Vector3 _spawnPoint;
    private static Vector3 _searcharea;
    private static Blip _blip;
    private static int _scenario;
    private static bool _hasBegunAttacking;
    private static bool _isArmed;
    private static bool _hasPursuitBegun = false;

    public override bool OnBeforeCalloutDisplayed()
    {
        _scenario = Rndm.Next(0, 100);
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

        _spawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
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
        if (_blip) _blip.Delete();
        if (_subject) _subject.Delete();
        if (_v1) _v1.Dismiss();
        if (_v2) _v2.Dismiss();
        if (_v3) _v3.Dismiss();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (_subject && !_isArmed && _subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 35f)
        {
            if (_blip) _blip.Delete();
            _subject.Inventory.GiveNewWeapon(new WeaponAsset(WepList[Rndm.Next(WepList.Length)]), 500, true);
            _isArmed = true;
        }

        if (_subject  &&
            !_hasBegunAttacking && _subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 50f)
        {
            RelationshipGroup agRelationshipGroup = new("AG");
            RelationshipGroup viRelationshipGroup = new("VI");
                    
            _subject.RelationshipGroup = agRelationshipGroup;
            _v1.RelationshipGroup = viRelationshipGroup;
            _v2.RelationshipGroup = viRelationshipGroup;
            _v3.RelationshipGroup = viRelationshipGroup;
            agRelationshipGroup.SetRelationshipWith(MainPlayer.RelationshipGroup, Relationship.Hate);
            agRelationshipGroup.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            
            _subject.KeepTasks = true;
            
            switch (_scenario)
            {
                case > 40:
                    agRelationshipGroup.SetRelationshipWith(viRelationshipGroup, Relationship.Hate);
                    _subject.Tasks.FightAgainstClosestHatedTarget(1000f);
                    GameFiber.Wait(2000);
                    _subject.Tasks.FightAgainstClosestHatedTarget(1000f, -1);
                    _hasBegunAttacking = true;
                    GameFiber.Wait(600);
                    break;
                default:
                    if (!_hasPursuitBegun)
                    {
                        _subject.Tasks.FightAgainstClosestHatedTarget(1000f, -1);
                        _hasBegunAttacking = true;
                        GameFiber.Wait(2000);
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
        if (_v1) _v1.Dismiss();
        if (_v2) _v2.Dismiss();
        if (_v3) _v3.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Terrorist Attack", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
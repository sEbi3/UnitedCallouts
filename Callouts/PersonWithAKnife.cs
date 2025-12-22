namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of a Person With a Knife", CalloutProbability.Medium)]
public class PersonWithKnife : Callout
{
    private static readonly string[] PedList =
    {
        "A_F_M_SouCent_01", "A_F_M_SouCent_02", "A_M_Y_Skater_01", "A_M_M_FatLatin_01", "A_M_M_EastSA_01",
        "A_M_Y_Latino_01", "G_M_Y_FamDNF_01",
        "G_M_Y_FamCA_01", "G_M_Y_BallaSout_01", "G_M_Y_BallaOrig_01", "G_M_Y_BallaEast_01", "G_M_Y_StrPunk_02",
        "S_M_Y_Dealer_01", "A_M_M_RurMeth_01",
        "A_M_M_Skidrow_01", "A_M_Y_MexThug_01", "G_M_Y_MexGoon_03", "G_M_Y_MexGoon_02", "G_M_Y_MexGoon_01",
        "G_M_Y_SalvaGoon_01", "G_M_Y_SalvaGoon_02",
        "G_M_Y_SalvaGoon_03", "G_M_Y_Korean_01", "G_M_Y_Korean_02", "G_M_Y_StrPunk_01"
    };

    // FIXED: Removed static from all instance fields
    private Ped _subject;
    private Vector3 _spawnPoint;
    private Vector3 _searcharea;
    private Blip _blip;
    private LHandle _pursuit;
    private int _scenario;
    private bool _hasBegunAttacking;
    private bool _isArmed;
    private bool _hasPursuitBegun;
    private bool _hasSpoke;
    private bool _pursuitCreated = false;

    public override bool OnBeforeCalloutDisplayed()
    {
        _scenario = Rndm.Next(0, 101);
        _spawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 100f);
        CalloutMessage = "[UC]~w~ Reports of a Person With a Knife.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS ASSAULT_WITH_AN_DEADLY_WEAPON CIV_ASSISTANCE IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Person With a Knife", "~b~Dispatch: ~w~Try to arrest the suspect. Respond with ~r~Code 3");

        _subject = new Ped(PedList[Rndm.Next(PedList.Length)], _spawnPoint, 0f);
        _subject.BlockPermanentEvents = true;
        _subject.IsPersistent = true;
        _subject.Tasks.Wander();

        _searcharea = _spawnPoint.Around2D(1f, 2f);
        _blip = new Blip(_searcharea, 80f);
        _blip.Color = Color.Yellow;
        _blip.EnableRoute(Color.Yellow);
        _blip.Alpha = 0.5f;
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        // FIXED: Added exists checks before deletion
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_subject != null && _subject.Exists()) _subject.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        // FIXED: Added null and exists checks
        if (_subject != null && _subject.Exists())
        {
            if (!_subject.Inventory.Weapons.Contains(WeaponHash.Knife) &&
                _subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 18f)
            {
                _subject.Inventory.GiveNewWeapon("WEAPON_KNIFE", 500, true);
                _isArmed = true;
            }
            else if (!_isArmed && _subject.Inventory.Weapons.Contains(WeaponHash.Knife) &&
                     _subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 18f)
            {
                _subject.Inventory.EquippedWeapon = WeaponHash.Knife;
                _isArmed = true;
            }
        }

        // FIXED: Added null and exists checks
        if (!_hasBegunAttacking && _subject != null && _subject.Exists() &&
            _subject.DistanceTo(MainPlayer.GetOffsetPosition(Vector3.RelativeFront)) < 18f)
        {
            _hasBegunAttacking = true;
            GameFiber.StartNew(() =>
            {
                switch (_scenario)
                {
                    case > 40:
                        _subject.KeepTasks = true;
                        _subject.Tasks.FightAgainst(MainPlayer);
                        switch (Rndm.Next(1, 4))
                        {
                            case 1:
                                Game.DisplaySubtitle("~r~Suspect: ~w~I do not want to live anymore!", 4000);
                                _hasSpoke = true;
                                break;
                            case 2:
                                Game.DisplaySubtitle(
                                    "~r~Suspect: ~w~Go away! - I'm not going back to the psychiatric hospital!", 4000);
                                _hasSpoke = true;
                                break;
                            case 3:
                                Game.DisplaySubtitle("~r~Suspect: ~w~I'm not going back to the psychiatric hospital!",
                                    4000);
                                _hasSpoke = true;
                                break;
                        }

                        GameFiber.Wait(2000);
                        break;
                    default:
                        if (!_hasPursuitBegun)
                        {
                            _pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(_pursuit, _subject);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            _hasPursuitBegun = true;
                        }

                        break;
                }
            });
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

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Person With a Knife", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}

namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of an Illegal Police Car Trade", CalloutProbability.Medium)]
public class IllegalPoliceCarTrade : Callout
{
    private static readonly string[] CarList =
        { "POLICE", "POLICE2", "POLICE3", "SHERIFF", "POLICE4", "SHERIFF2", "FBI", "FBI2", "POLICEB" };

    private static readonly string[] SellerList =
        { "ig_andreas", "ig_bankman", "ig_barry", "a_m_m_business_01", "a_m_y_business_02" };

    private static Ped _buyer;
    private static Ped _seller;
    private static Vector3 _spawnPoint;
    private static Vector3 _buyerSpawn;
    private static Vector3 _carSpawn = new(-30.4387f, -1089.152f, 26.42208f);
    private static Vehicle _car;
    private static Blip _blip;
    private static LHandle _pursuit;
    private static bool _attack;
    private static int _storyLine = 1;
    private static bool _startedPursuit;
    private static bool _wasClose;
    private static bool _alreadySubtitleIntrod;
    private static bool _hasTalkedBack = false;
    private static int _callOutMessage;

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = new(-34.79253f, -1096.583f, 26.42235f);
        _buyerSpawn = new(-33.81899f, -1089.764f, 26.42229f);

        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
        _attack = Rndm.Next(1, 4) == 1;
        switch (Rndm.Next(1, 4))
        {
            case 1:
                CalloutMessage = "[UC]~w~ Reports of an Illegal Police Car Trade.";
                _callOutMessage = 1;
                break;
            case 2:
                CalloutMessage = "[UC]~w~ Reports of an Illegal Police Car Trade.";
                _callOutMessage = 2;
                break;
            case 3:
                CalloutMessage = "[UC]~w~ Reports of an Illegal Police Car Trade.";
                _callOutMessage = 3;
                break;
        }

        CalloutPosition = _spawnPoint;
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("UnitedCallouts Log: Illegal Police Car Trade callout accepted.");
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Illegal Police Car Trade",
            "~b~Dispatch:~w~ Try to arrest the buyer and seller from the illegal trade. Respond with ~y~Code 2");

        _seller = new Ped(SellerList[Rndm.Next(SellerList.Length)], _spawnPoint, 0f);
        _seller.Position = _spawnPoint;
        _seller.IsPersistent = true;
        _seller.BlockPermanentEvents = true;

        _buyer = new Ped(_buyerSpawn);
        _buyer.Position = _buyerSpawn;
        _buyer.IsPersistent = true;
        _buyer.BlockPermanentEvents = true;
        _buyer.RelationshipGroup = RelationshipGroup.AggressiveInvestigate;
        _seller.RelationshipGroup = RelationshipGroup.AggressiveInvestigate;

        _car = new Vehicle(CarList[Rndm.Next(CarList.Length)], _carSpawn);
        _car.IsStolen = true;

        _blip = _car.AttachBlip();
        _blip.Color = Color.LightBlue;
        _blip.IsFriendly = false;
        _blip.EnableRoute(Color.Yellow);
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS CRIME_SUSPECT_RESISTING_ARREST_01 IN_OR_ON_POSITION CODE2", _spawnPoint);
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        base.OnCalloutNotAccepted();
        if (_buyer) _buyer.Delete();
        if (_blip) _blip.Delete();
        if (_seller) _seller.Delete();
        if (_car) _car.Delete();
    }

    public override void Process()
    {
        if (_seller.DistanceTo(MainPlayer) < 20f)
        {
            if (_attack && !_startedPursuit)
            {
                _pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(_pursuit, _seller);
                Functions.AddPedToPursuit(_pursuit, _buyer);
                Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                _startedPursuit = true;
            }

            if (_seller.DistanceTo(MainPlayer) < 15f && MainPlayer.IsOnFoot && _alreadySubtitleIntrod == false &&
                _pursuit == null)
            {
                Game.DisplaySubtitle("Press ~y~Y ~w~to speak with the Seller", 5000);
                _buyer.Face(_car);
                _seller.Face(MainPlayer);
                Functions.PlayScannerAudio("ATTENTION_GENERIC_01 OFFICERS_ARRIVED_ON_SCENE");
                _alreadySubtitleIntrod = true;
                _wasClose = true;
            }

            if (_attack == false && _seller.DistanceTo(MainPlayer) < 2f && Game.IsKeyDown(Settings.Dialog))
            {
                _seller.Face(MainPlayer);
                switch (_storyLine)
                {
                    case 1:
                        Game.DisplaySubtitle(
                            "~y~Seller: ~w~Oh, hello officer, I didn't hear you. How can I help? (1/5)", 5000);
                        _storyLine++;
                        break;
                    case 2:
                        Game.DisplaySubtitle("~b~You: ~w~Are you the owner? (2/5)", 5000);
                        _storyLine++;
                        break;
                    case 3:
                        Game.DisplaySubtitle("~y~Suspect: ~w~Ah...yes I am the owner! Is anything wrong? (3/5)", 5000);
                        _storyLine++;
                        break;
                    case 4:
                        switch (_callOutMessage)
                        {
                            case 1:
                                Game.DisplaySubtitle(
                                    "~b~You: ~w~Is there a reason you have a police vehicle in your garage? (4/5)",
                                    5000);
                                break;
                            case 2:
                                Game.DisplaySubtitle(
                                    "~b~You: ~w~That's a police vehicle... what's going on here? (4/5)",
                                    5000);
                                break;
                            case 3:
                                Game.DisplaySubtitle(
                                    "~b~You: ~w~I couldn't help but notice that police vehicle. Care to explain? (4/5)",
                                    5000);
                                break;
                        }

                        _storyLine++;
                        break;
                    case 5:
                        if (_callOutMessage == 1)
                            Game.DisplaySubtitle(
                                "~y~Suspect: ~w~Uh... Yes! It's here because... Ah, forget it. Do what you need to do. (5/5)",
                                5000);
                        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept",
                            "~w~UnitedCallouts", "~y~Dispatch Information",
                            "The plate of the ~b~" + _car.Model.Name + "~w~ is ~o~" + _car.LicensePlate +
                            "~w~. The car was ~r~stolen~w~ from the police station in ~b~Mission Row~w~.");
                        Game.DisplayHelp("~y~Arrest the owner and the buyer.", 5000);
                        if (_callOutMessage == 2)
                        {
                            Game.DisplaySubtitle("~y~Suspect: ~w~You weren't meant to see this! (5/5)", 5000);
                            _buyer.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
                            NativeFunction.CallByName<uint>("TASK_COMBAT_PED", _buyer, MainPlayer, 0, 16);
                        }

                        if (_callOutMessage == 3)
                        {
                            Game.DisplaySubtitle(
                                "~y~Suspect: ~w~I could, but there's no point in talking to a corpse! (5/5)", 5000);
                            _seller.Inventory.GiveNewWeapon("WEAPON_KNIFE", 500, true);
                            NativeFunction.Natives.TASK_COMBAT_PED(_seller, MainPlayer, 0, 16);
                            NativeFunction.Natives.TASK_COMBAT_PED(_buyer, MainPlayer, 0, 16);
                        }

                        _storyLine++;
                        break;
                }
            }
        }

        if (MainPlayer.IsDead) End();
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (_seller && _seller.IsDead && _buyer.Exists() && _buyer.IsDead) End();
        if (_seller && Functions.IsPedArrested(_seller) && _buyer.Exists() && Functions.IsPedArrested(_buyer)) End();
        base.Process();
    }

    public override void End()
    {
        if (_seller) _seller.Dismiss();
        if (_blip) _blip.Delete();
        if (_buyer) _buyer.Dismiss();
        if (_car) _car.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Illegal Police Car Trade", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        base.End();
    }
}
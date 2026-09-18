namespace UnitedCallouts.Callouts;

[CalloutInfo("[UC] Reports of an Illegal Police Car Trade", CalloutProbability.Medium)]
public class IllegalPoliceCarTrade : Callout
{
    private static readonly string[] CarList =
        { "POLICE", "POLICE2", "POLICE3", "SHERIFF", "POLICE4", "SHERIFF2", "FBI", "FBI2", "POLICEB" };

    private static readonly string[] SellerList =
        { "ig_andreas", "ig_bankman", "ig_barry", "a_m_m_business_01", "a_m_y_business_02" };

    private Ped _buyer;
    private Ped _seller;
    private Vector3 _spawnPoint;
    private Vector3 _buyerSpawn;
    private Vector3 _carSpawn = new(-30.4387f, -1089.152f, 26.42208f);
    private Vehicle _car;
    private Blip _blip;
    private Blip _SuspectBlip;
    private LHandle _pursuit;
    private bool _attack;
    private int _storyLine = 1;
    private bool _startedPursuit;
    private bool _alreadySubtitleIntrod;
    private bool _alreadySubtitleIntrod2;
    private bool _hasTalkedBack = false;
    private int _callOutMessage;

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = new(-34.79253f, -1096.583f, 26.42235f);
        _buyerSpawn = new(-33.81899f, -1089.764f, 26.42229f);

        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
        _attack = Rndm.Next(1, 4) == 1;
        switch (Rndm.Next(1, 4))
        {
            case 1:
                CalloutMessage = "Reports of an Illegal Police Car Trade.";
                CalloutAdvisory = "Reports received of a suspected unauthorized sale of a police vehicle.";
                _callOutMessage = 1;
                break;
            case 2:
                CalloutMessage = "Reports of an Illegal Police Car Trade.";
                CalloutAdvisory = "A police vehicle is allegedly being sold illegally at a private property.";
                _callOutMessage = 2;
                break;
            case 3:
                CalloutMessage = "Reports of an Illegal Police Car Trade.";
                CalloutAdvisory = "Information received regarding the unlawful transfer of a police car.";
                _callOutMessage = 3;
                break;
        }

        CalloutPosition = _spawnPoint;
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Illegal Police Car Trade callout accepted.");
        }
        else { Settings.DetailedLogging = false; }

        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Illegal Police Car Trade",
            "~b~Dispatch: ~w~Respond to a reported illegal sale of a police vehicle. Verify vehicle status and identify parties involved. Exercise caution and report back on scene.");

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
        _blip.Sprite = BlipSprite.GangVehicle;
        _blip.Color = Color.LightBlue;
        _blip.EnableRoute(Color.Yellow);
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS CRIME_SUSPECT_RESISTING_ARREST_01 IN_OR_ON_POSITION CODE2", _spawnPoint);
        return base.OnCalloutAccepted();
    }

    public override void OnCalloutNotAccepted()
    {
        if (_buyer != null && _buyer.Exists()) _buyer.Delete();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_seller != null && _seller.Exists()) _seller.Delete();
        if (_car != null && _car.Exists()) _car.Delete();
        base.OnCalloutNotAccepted();
    }

    public override void Process()
    {
        if (_seller != null && _seller.Exists() && _seller.DistanceTo(MainPlayer) < 30f)
        {
            if (_attack && !_startedPursuit)
            {
                _pursuit = Functions.CreatePursuit();
                if (_seller.Exists()) Functions.AddPedToPursuit(_pursuit, _seller);
                if (_buyer != null && _buyer.Exists()) Functions.AddPedToPursuit(_pursuit, _buyer);
                Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                _startedPursuit = true;
            }

            if (!_alreadySubtitleIntrod2 && _seller.DistanceTo(MainPlayer) < 15f)
            {
                Functions.PlayScannerAudio("ATTENTION_GENERIC_01 OFFICERS_ARRIVED_ON_SCENE");
                _alreadySubtitleIntrod2 = true;
            }

            if (!_alreadySubtitleIntrod && _seller.DistanceTo(MainPlayer) < 20f && MainPlayer.IsOnFoot && _pursuit == null)
            {
                Game.DisplaySubtitle("Press ~y~" + Settings.Dialog + " ~w~to speak with the suspect that might be the seller.", 5000);
                if (_buyer != null && _buyer.Exists() && _car != null && _car.Exists()) _buyer.Face(_car);
                _SuspectBlip = _seller.AttachBlip();
                _SuspectBlip.Sprite = BlipSprite.PointOfInterest;
                _alreadySubtitleIntrod = true;
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
                        Game.DisplaySubtitle("~b~You: ~w~Are you the owner of this car dealership? (2/5)", 5000);
                        _storyLine++;
                        break;
                    case 3:
                        Game.DisplaySubtitle("~y~Suspect: ~w~Uhm.. yes I am the owner! Is anything wrong? (3/5)", 5000);
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
                        if (_callOutMessage == 1 && _car != null && _car.Exists())
                        {
                            Game.DisplaySubtitle("~y~Suspect: ~w~Uhm... Yes! It's here because... Ah, forget it! Do what you need to do. (5/5)", 5000);
                            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept",
                                "~w~UnitedCallouts", "~y~Dispatch Information",
                                "The plate of the ~o~police vehicle~w~ is: ~y~" + _car.LicensePlate +
                                "~w~. The car was ~o~stolen~w~ from the police station in Mission Row.");

                            if (Settings.HelpMessages)
                            {
                                Game.DisplayHelp("Both parties involved in the illegal sale of the police vehicle are to be taken into custody. Proceed with arrests and advise when secured.", 5000);
                            }
                            else { Settings.HelpMessages = false; }
                        }

                        if (_callOutMessage == 2 && _buyer != null && _buyer.Exists())
                        {
                            Game.DisplaySubtitle("~y~Suspect: ~w~Fuck you! (5/5)", 5000);
                            _buyer.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
                            NativeFunction.Natives.TASK_COMBAT_PED(_buyer, MainPlayer, 0, 16);
                        }

                        if (_callOutMessage == 3 && _seller != null && _seller.Exists() && _buyer != null && _buyer.Exists())
                        {
                            Game.DisplaySubtitle(
                                "~y~Suspect: ~w~You know what? There's no point in talking to a dead body! (5/5)", 5000);
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
        if (_seller != null && _seller.IsDead && _buyer != null && _buyer.Exists() && _buyer.IsDead) End();
        if (_seller != null && Functions.IsPedArrested(_seller) && _buyer != null && _buyer.Exists() && Functions.IsPedArrested(_buyer)) End();
        base.Process();
    }

    public override void End()
    {
        if (_seller != null && _seller.Exists()) _seller.Dismiss();
        if (_blip != null && _blip.Exists()) _blip.Delete();
        if (_SuspectBlip != null && _SuspectBlip.Exists()) _SuspectBlip.Delete();
        if (_buyer != null && _buyer.Exists()) _buyer.Dismiss();
        if (_car != null && _car.Exists()) _car.Dismiss();
        Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts",
            "~y~Illegal Police Car Trade", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
        Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
        if (Functions.IsPursuitStillRunning(_pursuit)) { Functions.ForceEndPursuit(_pursuit); }
        if (Settings.DetailedLogging)
        {
            Game.LogTrivial("[UnitedCallouts LOG:] Illegal Police Car Trade callout ended.");
        }
        else { Settings.DetailedLogging = false; }
        base.End();
    }
}
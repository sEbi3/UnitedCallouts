using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Drawing;
using System.Collections.Generic;
using Rage.Native;
using UnitedCallouts.Stuff;
namespace UnitedCallouts.Callouts
{
    [CalloutInfo("[UC] Murder Investigation", CalloutProbability.Medium)]
    internal class MurderInvestigation : Callout
    {
        private string[] _WepList = new string[] { "WEAPON_PISTOL", "WEAPON_COMBATPISTOL", "WEAPON_KNIFE", "WEAPON_MUSKET", "WEAPON_MACHETE" };
        private string[] _CopCars = new string[] { "SHERIFF", "SHERIFF2" };
        private Ped _DeadPerson;
        private Ped _DeadPerson2;
        private Ped _Murder;
        private Ped _Cop1;
        private Ped _Cop2;
        private Ped _Coroner1;
        private Ped _Coroner2;
        private Vehicle _CoronerV;
        private Vehicle _CopV;
        private Vector3 _searcharea;
        private Vector3 _DeadPersonSpawn;
        private Vector3 _Cop1Spawn;
        private Vector3 _Cop2Spawn;
        private Vector3 _Coroner1Spawn;
        private Vector3 _Coroner2Spawn;
        private Vector3 _CoronerVSpawn;
        private Vector3 _CopVSpawn;
        private Vector3 _MurderLocation;
        private Blip MurderLocationBlip;
        private Blip SpawnLocationBlip;
        private LHandle pursuit;
        private int storyLine = 1;
        private int _callOutMessage = 0;
        private int scenario = 0;
        private bool _Scene1 = false;
        private bool _Scene2 = false;
        private bool wasClose = false;
        private bool Noticed = false;
        private bool notificationDisplayed = false;
        private bool hasPursuitBegun = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            _DeadPersonSpawn = new Vector3(1162.029f, 2371.788f, 57.66312f);
            _Cop1Spawn = new Vector3(1174.725f, 2369.399f, 57.59957f);
            _Cop2Spawn = new Vector3(1167.733f, 2382.189f, 57.61982f);
            _Coroner1Spawn = new Vector3(1161.486f, 2369.885f, 57.76299f);
            _Coroner2Spawn = new Vector3(1165.3f, 2374.227f, 57.63049f);
            _CopVSpawn = new Vector3(1174.684f, 2375.117f, 57.6276f);
            _CoronerVSpawn = new Vector3(1165.686f, 2360.025f, 57.62796f);

            Random random = new Random();
            List<Vector3> list = new List<Vector3>
            {
                new Vector3(-10.93565f, -1434.329f, 31.11683f),
                new Vector3(-1.838376f, 523.2645f, 174.6274f),
                new Vector3(-801.5516f, 178.7447f, 72.83471f),
                new Vector3(-801.5516f, 178.7447f, 72.83471f),
                new Vector3(-812.7239f, 178.7438f, 76.74079f),
                new Vector3(3.542758f, 526.8926f, 170.6218f),
                new Vector3(-1155.698f, -1519.297f, 10.63272f),
                new Vector3(1392.589f, 3613.899f, 38.94194f),
                new Vector3(2435.457f, 4966.514f, 46.8106f),

        };
            _MurderLocation = LocationChooser.chooseNearestLocation(list);
            scenario = new Random().Next(0, 100);
            _CopV = new Vehicle(_CopCars[new Random().Next((int)_CopCars.Length)], _CopVSpawn, 76.214f);
            _CopV.IsEngineOn = true;
            _CopV.IsInteriorLightOn = true;
            _CopV.IsSirenOn = true;
            _CopV.IsSirenSilent = true;
            _CoronerV = new Vehicle("Speedo", _CoronerVSpawn, 22.32638f);
            _CoronerV.IsEngineOn = true;
            _CoronerV.IsInteriorLightOn = true;
            _CoronerV.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Both;

            _DeadPerson = new Ped(_DeadPersonSpawn);
            _Murder = new Ped(_MurderLocation);
            _DeadPerson2 = new Ped(_Murder.GetOffsetPosition(new Vector3(0, 1.8f, 0)));
            _DeadPerson2.IsPersistent = true;
            _DeadPerson2.BlockPermanentEvents = true;
            _Cop1 = new Ped("s_m_y_sheriff_01", _Cop1Spawn, 0f);
            _Cop2 = new Ped("s_m_y_sheriff_01", _Cop2Spawn, 0f);
            _Coroner1 = new Ped("S_M_M_Doctor_01", _Coroner1Spawn, 0f);
            _Coroner2 = new Ped("S_M_M_Doctor_01", _Coroner2Spawn, 0f);
            _DeadPerson.Kill();
            _DeadPerson.IsPersistent = true;
            _DeadPerson.BlockPermanentEvents = true;

            _Murder.IsPersistent = true;
            _Murder.Inventory.GiveNewWeapon(new WeaponAsset(_WepList[new Random().Next((int)_WepList.Length)]), 500, true);
            _Murder.BlockPermanentEvents = true;
            _Murder.Health = 200;
            _Murder.Armor = 300;
            NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", _Murder, _DeadPerson2, -1, true);
            _DeadPerson.Tasks.PlayAnimation("random@arrests@busted", "idle_a", 8.0F, AnimationFlags.Loop);


            Functions.IsPedACop(_Cop1);
            Functions.IsPedACop(_Cop2);
            _Cop1.IsInvincible = true;
            _Cop1.IsPersistent = true;
            _Cop1.BlockPermanentEvents = true;
            _Cop1.IsInvincible = true;
            _Cop1.IsPersistent = true;
            _Cop1.BlockPermanentEvents = true;

            _Coroner1.IsPersistent = true;
            _Coroner1.IsInvincible = true;
            _Coroner1.Face(_DeadPerson);
            _Coroner1.BlockPermanentEvents = true;
            _Coroner2.IsInvincible = true;
            _Coroner2.Face(_DeadPerson);
            _Coroner2.IsPersistent = true;
            _Coroner2.BlockPermanentEvents = true;
            _Coroner1.KeepTasks = false;
            _Coroner2.KeepTasks = false;

            Rage.Object camera = new Rage.Object("prop_ing_camera_01", _Coroner1.GetOffsetPosition(Vector3.RelativeTop * 30));
            Rage.Object camera2 = new Rage.Object("prop_ing_camera_01", _Coroner2.GetOffsetPosition(Vector3.RelativeTop * 30));
            _Coroner1.Tasks.PlayAnimation("amb@world_human_paparazzi@male@idle_a", "idle_a", 8.0F, AnimationFlags.Loop);
            _Coroner2.Tasks.PlayAnimation("amb@medic@standing@kneel@base", "base", 8.0F, AnimationFlags.Loop);
            _Cop2.Tasks.PlayAnimation("amb@world_human_cop_idles@male@idle_a", "idle_a", 1.5f, AnimationFlags.Loop);

            switch (new Random().Next(1, 2))
            {
                case 1:
                    _Scene1 = true;
                    break;
                case 2:
                    _Scene2 = true;
                    break;
            }

            ShowCalloutAreaBlipBeforeAccepting(_DeadPersonSpawn, 100f);
            AddMinimumDistanceCheck(10f, _DeadPerson.Position);
            switch (new Random().Next(1, 2))
            {
                case 1:
                    CalloutMessage = "[UC]~w~ We have found a dead body and need the ~y~FIB~w~ here.";
                    _callOutMessage = 1;
                    break;
                case 2:
                    CalloutMessage = "[UC]~w~ We have found a dead body and need the ~y~FIB~w~ here.";
                    _callOutMessage = 2;
                    break;
            }
            CalloutPosition = _DeadPersonSpawn;
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            SpawnLocationBlip = new Blip(_Cop1);
            SpawnLocationBlip.Color = Color.LightGreen;
            SpawnLocationBlip.Sprite = BlipSprite.PointOfInterest;
            SpawnLocationBlip.EnableRoute(Color.LightBlue);

            Functions.PlayScannerAudioUsingPosition("ATTENTION_GENERIC_01 UNITS WE_HAVE A_01 CRIME_DEAD_BODY_01 CODE3", _DeadPersonSpawn);
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Murder Investigation", "~b~Dispatch: ~w~The police department needs a ~b~detective~w~ on scene to find and arrest the murder. Respond with ~r~Code 3");
            GameFiber.Wait(2000);
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (_Cop1) _Cop1.Delete();
            if (_Cop2) _Cop2.Delete();
            if (_Coroner1) _Coroner1.Delete();
            if (_Murder) _Murder.Delete();
            if (_DeadPerson) _DeadPerson.Delete();
            if (_DeadPerson2) _DeadPerson2.Delete();
            if (_Coroner2) _Coroner2.Delete();
            if (_CopV) _CopV.Delete();
            if (_CoronerV) _CoronerV.Delete();
            if (SpawnLocationBlip) SpawnLocationBlip.Delete();
            if (MurderLocationBlip) MurderLocationBlip.Delete();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            GameFiber.StartNew(delegate
            {
                if (_Cop1.DistanceTo(Game.LocalPlayer.Character) < 25f && Game.LocalPlayer.Character.IsOnFoot && !Noticed)
                {
                    Game.DisplaySubtitle("Press ~y~Y~w~ to speak with the officer.", 5000);
                    Functions.PlayScannerAudio("ATTENTION_GENERIC_01 OFFICERS_ARRIVED_ON_SCENE");
                    _Cop1.Face(Game.LocalPlayer.Character);
                    if (SpawnLocationBlip) SpawnLocationBlip.Delete();
                    Noticed = true;
                }
                if (_Murder.DistanceTo(Game.LocalPlayer.Character) < 25f & !Noticed)
                {
                    if (MurderLocationBlip) MurderLocationBlip.Delete();
                    Noticed = true;

                    if (_Scene1 == true && _Scene2 == false)
                    {
                        _DeadPerson2.Kill();
                        _Murder.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                    if (_Scene2 == true && _Scene1 == false)
                    {
                        new RelationshipGroup("AG");
                        new RelationshipGroup("VI");
                        _Murder.RelationshipGroup = "AG";
                        _DeadPerson2.RelationshipGroup = "VI";
                        Game.SetRelationshipBetweenRelationshipGroups("AG", "VI", Relationship.Hate);
                        _Murder.Tasks.FightAgainstClosestHatedTarget(1000f);
                        GameFiber.Wait(300);
                        _Murder.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                }
                if (_Cop1.DistanceTo(Game.LocalPlayer.Character) < 2f && Game.IsKeyDown(Settings.Dialog))
                {
                    _Cop1.Face(Game.LocalPlayer.Character);
                    switch (storyLine)
                    {
                        case 1:
                            Game.DisplaySubtitle("~y~Officer: ~w~Hello Detective, we called you because we have a dead body here. (1/5)", 5000);
                            storyLine++;
                            break;
                        case 2:
                            Game.DisplaySubtitle("~b~You: ~w~Do we have anything about the murder? (2/5)", 5000);
                            storyLine++;
                            break;
                        case 3:
                            Game.DisplaySubtitle("~y~Officer: ~w~Yes, we have found something of interest. (3/5)", 5000);
                            storyLine++;
                            break;
                        case 4:
                            if (_callOutMessage == 1)
                                Game.DisplaySubtitle("~y~Officer: ~w~We checked the cameras around here and there was a man without a mask, so our police department checked the identity. (4/5)", 5000);
                            if (_callOutMessage == 2)
                                Game.DisplaySubtitle("~y~Officer: ~w~As the coroner searched the killed person, they found an ID next to the person. (4/5)", 5000);
                            storyLine++;                            
                            break;
                        case 5:
                            if (_callOutMessage == 1)
                                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Police Department",
                                "The police department found personal details of the murder:<br>~w~Name: ~b~" 
                                + LSPD_First_Response.Engine.Scripting.Entities.Persona.FromExistingPed(_Murder).FullName + "<br>~w~Gender: ~g~" 
                                + LSPD_First_Response.Engine.Scripting.Entities.Persona.FromExistingPed(_Murder).Gender + "<br>~w~DOB: ~y~" 
                                + LSPD_First_Response.Engine.Scripting.Entities.Persona.FromExistingPed(_Murder).Birthday.Date + ""); 
                            if (_callOutMessage == 2)
                                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Police Department",
                                "The police department found personal details of the murder:<br>~w~Name: ~b~"
                                + LSPD_First_Response.Engine.Scripting.Entities.Persona.FromExistingPed(_Murder).FullName + "<br>~w~Gender: ~g~"
                                + LSPD_First_Response.Engine.Scripting.Entities.Persona.FromExistingPed(_Murder).Gender + "<br>~w~DOB: ~y~"
                                + LSPD_First_Response.Engine.Scripting.Entities.Persona.FromExistingPed(_Murder).Birthday.Date + ""); 
                            storyLine++;
                            break;
                        case 6:
                            if (_callOutMessage == 1)
                                Game.DisplaySubtitle("~b~You: ~w~Alright, I'll check the house of the murder. Thank you for your time, officer! (5/5)", 5000);
                            if (_callOutMessage == 2)
                                Game.DisplaySubtitle("~b~You: ~w~Okay, thank you for letting me know! I'll find the murder! (5/5)", 5000);
                            storyLine++;
                            Game.DisplayHelp("The ~y~Police Department~w~ is setting up the location on your GPS...", 5000);
                            GameFiber.Wait(3000);
                            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Police Department",
                                                     "~b~Detective~w~, we ~o~marked the apartment~w~ for you on the map. Search the ~y~yellow circle area~w~ on your map and try to ~y~find~w~ and ~b~arrest~w~ the ~g~murder~w~.");
                            _searcharea = _MurderLocation.Around2D(1f, 2f);
                            MurderLocationBlip = new Blip(_searcharea, 40f);
                            MurderLocationBlip.EnableRoute(Color.Yellow);
                            MurderLocationBlip.Color = Color.Yellow;
                            MurderLocationBlip.Alpha = 0.5f;
                            break;
                        default:
                            break;
                    }
                }
                if (Game.IsKeyDown(Settings.EndCall)) End();
                if (Game.LocalPlayer.Character.IsDead) End();
                if (_Murder && _Murder.IsDead) End();
                if (_Murder && Functions.IsPedArrested(_Murder)) End();
            }, "Murder Investigation [UnitedCallouts]");
            base.Process();
        }

        public override void End()
        {
            if (_Cop1) _Cop1.Dismiss();
            if (_Cop2) _Cop2.Dismiss();
            if (_Coroner1) _Coroner1.Dismiss();
            if (_Murder) _Murder.Dismiss();
            if (_DeadPerson) _DeadPerson.Dismiss();
            if (_DeadPerson2) _DeadPerson2.Dismiss();
            if (_Coroner2) _Coroner2.Dismiss();
            if (_CopV) _CopV.Dismiss();
            if (_CoronerV) _CoronerV.Dismiss();
            if (SpawnLocationBlip) SpawnLocationBlip.Delete();
            if (MurderLocationBlip) MurderLocationBlip.Delete();
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "~w~UnitedCallouts", "~y~Murder Investigation", "~b~You: ~w~Dispatch we're code 4. Show me ~g~10-8.");
            Functions.PlayScannerAudio("ATTENTION_THIS_IS_DISPATCH_HIGH ALL_UNITS_CODE4 NO_FURTHER_UNITS_REQUIRED");
            base.End();
        }
    }
}
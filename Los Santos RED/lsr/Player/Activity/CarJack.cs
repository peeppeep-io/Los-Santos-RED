﻿using LosSantosRED.lsr.Interface;
using LSR.Vehicles;
using Rage;
using Rage.Native;
using System;

//lots more refactoring please
public class CarJack
{
    private string Dictionary;
    private Ped Driver;
    private Vector3 DriverSeatCoordinates;
    private uint GameTimeLastTriedCarJacking;
    private string PerpAnimation;
    private ICarStealable Player;
    private int PlayerScene;
    private int SeatTryingToEnter;
    private Vehicle TargetVehicle;
    private PedExt Victim;
    private string VictimAnimation;
    private int VictimScene;
    private bool WantToCancel;
    private WeaponInformation Weapon;
    private bool WasEngineOn;
    private VehicleExt VehicleExt;
    public CarJack(ICarStealable player, VehicleExt vehicle, Ped DriverPed, PedExt DriverExt, int EntrySeat, WeaponInformation weapon)
    {
        Player = player;
        VehicleExt = vehicle;
        TargetVehicle = VehicleExt.Vehicle;
        Driver = DriverPed;
        SeatTryingToEnter = EntrySeat;
        Weapon = weapon;
    }
    private bool CanArmedCarJack
    {
        get
        {
            if (SeatTryingToEnter != -1)
                return false;

            if (TargetVehicle.HasBone("door_dside_f") && TargetVehicle.HasBone("door_pside_f"))
            {
                if (Game.LocalPlayer.Character.DistanceTo2D(TargetVehicle.GetBonePosition("door_dside_f")) > Game.LocalPlayer.Character.DistanceTo2D(TargetVehicle.GetBonePosition("door_pside_f")))
                {
                    return false;//Closer to passenger side, animations dont work
                }
            }
            return true;
        }
    }
    public void StartCarJack()
    {
        WasEngineOn = VehicleExt.Vehicle.IsEngineOn;
        if (WasEngineOn)
        {
            VehicleExt.Engine.Toggle(true);
        }
        if (CanArmedCarJack && Game.GameTime - GameTimeLastTriedCarJacking > 500 && Weapon != null && Weapon.Category != WeaponCategory.Melee)
        {
            ArmedCarJack();
        }
        else
        {
            UnarmedCarJack();
        }
    }
    private void ArmedCarJack()
    {
        if (Victim != null)
            Victim.CanBeTasked = false;
        try
        {
            GameFiber CarJackPedWithWeapon = GameFiber.StartNew(delegate
            {
                if (!SetupCarJack())
                {
                    if (Victim != null)
                        Victim.CanBeTasked = true;
                    return;
                }
                if (!CarJackAnimation())
                {
                    if (Victim != null)
                        Victim.CanBeTasked = true;
                    return;
                }

                FinishCarJack();
                if (Victim != null)
                    Victim.CanBeTasked = true;

                //CameraManager.RestoreGameplayerCamera();
            }, "CarJackPedWithWeapon");
        }
        catch (Exception e)
        {
            Player.IsCarJacking = false;
            //EntryPoint.WriteToConsole("UnlockCarDoor" + e.Message + e.StackTrace);
        }
    }
    private bool CarJackAnimation()
    {
        Player.IsCarJacking = true;
        bool locOpenDoor = false;
        WantToCancel = false;
        Vector3 OriginalCarPosition = TargetVehicle.Position;
        //CameraManager.TransitionToAltCam(TargetVehicle, GetCameraPosition(), 1500);
        while (NativeFunction.CallByName<float>("GET_SYNCHRONIZED_SCENE_PHASE", PlayerScene) < 0.75f)
        {
            float ScenePhase = NativeFunction.CallByName<float>("GET_SYNCHRONIZED_SCENE_PHASE", PlayerScene);
            GameFiber.Yield();
            if (ScenePhase <= 0.4f && Player.IsMoveControlPressed)
            {
                WantToCancel = true;
                break;
            }
            if (Game.LocalPlayer.Character.IsDead)
            {
                WantToCancel = true;
                break;
            }
            if (!NativeFunction.CallByName<bool>("IS_SYNCHRONIZED_SCENE_RUNNING", VictimScene))
            {
                WantToCancel = true;
                break;
            }

            if (!locOpenDoor && ScenePhase > 0.05f && TargetVehicle.Doors[0].IsValid() && !TargetVehicle.Doors[0].IsFullyOpen)
            {
                locOpenDoor = true;
                TargetVehicle.Doors[0].Open(false, false);
            }
            if (TargetVehicle.DistanceTo2D(OriginalCarPosition) >= 0.1f)
            {
                WantToCancel = true;
                break;
            }
            if (Player.IsVisiblyArmed && Game.IsControlPressed(2, GameControl.Attack))//Game.LocalPlayer.Character.IsConsideredArmed()
            {
                Vector3 TargetCoordinate = Driver.GetBonePosition(PedBoneId.Head);
                Player.ShootAt(TargetCoordinate);

                if (ScenePhase <= 0.35f)
                {
                    Driver.WarpIntoVehicle(TargetVehicle, -1);
                    Game.LocalPlayer.Character.Tasks.Clear();
                    NativeFunction.CallByName<bool>("SET_PLAYER_FORCED_AIM", Game.LocalPlayer.Character, true);
                    break;
                }
            }
            if (Player.IsVisiblyArmed && Game.IsControlJustPressed(2, GameControl.Aim))//Game.LocalPlayer.Character.IsConsideredArmed()
            {
                if (NativeFunction.CallByName<float>("GET_SYNCHRONIZED_SCENE_PHASE", PlayerScene) <= 0.4f)
                {
                    Driver.WarpIntoVehicle(TargetVehicle, -1);
                    Game.LocalPlayer.Character.Tasks.Clear();
                    NativeFunction.CallByName<bool>("SET_PLAYER_FORCED_AIM", Game.LocalPlayer.Character, true);
                    break;
                }
            }
            if (ScenePhase >= 0.5f)
            {
                //CameraManager.RestoreGameplayerCamera();
            }
            if (Player.IsBusted || Player.IsDead)
            {
                WantToCancel = true;
                break;
            }
        }

        //CameraManager.RestoreGameplayerCamera();

        if (Player.IsDead)
        {
            Player.IsCarJacking = false;
            if (Victim != null)
            {
                Victim.CanBeTasked = true;
            }
            return false;
        }
        return true;
    }
    private bool FinishCarJack()
    {
        float FinalScenePhase = NativeFunction.CallByName<float>("GET_SYNCHRONIZED_SCENE_PHASE", PlayerScene);
        if (FinalScenePhase <= 0.4f)
        {
            if (WantToCancel || Driver.IsDead)
            {
                Driver.BlockPermanentEvents = false;
                Driver.WarpIntoVehicle(TargetVehicle, -1);
                Game.LocalPlayer.Character.Tasks.Clear();
            }
        }
        else
        {
            if (WantToCancel && FinalScenePhase <= 0.6f)
            {
                Driver.BlockPermanentEvents = false;
                Driver.WarpIntoVehicle(TargetVehicle, -1);
                Game.LocalPlayer.Character.Tasks.Clear();
            }
            else
            {
                Game.LocalPlayer.Character.WarpIntoVehicle(TargetVehicle, -1);

                ////This needs to be moved out of here!!!!, might need to add it back
                //VehicleExt MyCar = World.GetVehicle(TargetVehicle);
                //if (MyCar != null && MyCar.Vehicle.Exists())
                //{
                //    MyCar.Vehicle.IsEngineOn = true;
                //   // MyCar.ToggleEngine(true);
                //}
                if (WasEngineOn)
                {
                    VehicleExt.Engine.Toggle(true);
                }
                if (TargetVehicle.Doors[0].IsValid())
                {
                    NativeFunction.CallByName<bool>("SET_VEHICLE_DOOR_CONTROL", TargetVehicle, 0, 4, 0f);
                }
            }
        }

        if (Victim != null)
        {
            Victim.CanBeTasked = true;
        }

        if (WantToCancel)
        {
            Player.IsCarJacking = false;
            return false;
        }

        if (TargetVehicle.Doors[0].IsValid())
            NativeFunction.CallByName<bool>("SET_VEHICLE_DOOR_CONTROL", TargetVehicle, 0, 4, 0f);

        if (Driver.IsInAnyVehicle(false))
        {
            //EntryPoint.WriteToConsole("CarjackAnimation Driver In Vehicle");
        }
        else
        {
            //EntryPoint.WriteToConsole("CarjackAnimation Driver Out of Vehicle");
            if (Driver.IsAlive)
            {
                Driver.Tasks.ClearImmediately();
                Driver.Tasks.Flee(Game.LocalPlayer.Character, 500f, 0);
                Driver.IsRagdoll = false;
                Driver.BlockPermanentEvents = false;
            }
        }
        GameFiber.Sleep(5000);
        Player.IsCarJacking = false;
        return true;
    }
    private Vector3 GetCameraPosition()
    {
        Vector3 CameraPosition;
        float Distance = 6f;//General.MyRand.NextFloat(5f, 8f);
        float XVariance = 3f;// General.MyRand.NextFloat(0.5f, 3f);
        float YVariance = 3f;// 3f;// General.MyRand.NextFloat(0.5f, 3f);
        float ZVariance = 2f;// 1.8f;//General.MyRand.NextFloat(1.8f, 3f);

        if (TargetVehicle != null && TargetVehicle.Exists())
        {
            bool IsDriverSide = true;//for now..
            if (IsDriverSide)
            {
                Distance *= -1f;
                XVariance *= -1f;
            }
            CameraPosition = TargetVehicle.GetOffsetPositionRight(Distance);
        }
        else
        {
            CameraPosition = Game.LocalPlayer.Character.GetOffsetPositionRight(Distance);
        }

        CameraPosition += new Vector3(XVariance, YVariance, ZVariance);
        return CameraPosition;
    }
    private bool GetCarjackingAnimations()
    {
        if (Weapon == null || (!Weapon.IsTwoHanded && !Weapon.IsOneHanded))
            return false;

        int intVehicleClass = NativeFunction.CallByName<int>("GET_VEHICLE_CLASS", TargetVehicle);
        VehicleClass VehicleClass = (VehicleClass)intVehicleClass;
        if (VehicleClass == VehicleClass.Boat || VehicleClass == VehicleClass.Cycle || VehicleClass == VehicleClass.Industrial || VehicleClass == VehicleClass.Motorcycle || VehicleClass == VehicleClass.Plane || VehicleClass == VehicleClass.Service)
        {
            return false;//maybe add utility?
        }

        if (!TargetVehicle.Doors[0].IsValid())
        {
            return false;
        }

        float? GroundZ = Rage.World.GetGroundZ(DriverSeatCoordinates, true, false);
        if (GroundZ == null)
            GroundZ = 0f;
        float DriverDistanceToGround = DriverSeatCoordinates.Z - (float)GroundZ;
        //EntryPoint.WriteToConsole(string.Format("GetCarjackingAnimations VehicleClass {0},DriverSeatCoordinates: {1},GroundZ: {2}, PedHeight: {3}", VehicleClass, DriverSeatCoordinates, GroundZ, DriverDistanceToGround));
        if (VehicleClass == VehicleClass.Van && DriverDistanceToGround > 1.5f)
        {
            if (Weapon.IsTwoHanded)
            {
                Dictionary = "veh@jacking@2h";
                PerpAnimation = "van_perp_ds_a";
                VictimAnimation = "van_victim_ds_a";
            }
            else if (Weapon.IsOneHanded)
            {
                Dictionary = "veh@jacking@1h";
                PerpAnimation = "van_perp_ds_a";
                VictimAnimation = "van_victim_ds_a";
            }
        }
        else if (VehicleClass == VehicleClass.Helicopter)
        {
            if (Weapon.IsTwoHanded)
            {
                Dictionary = "veh@jacking@2h";
                PerpAnimation = "heli_perp_ds_a";
                VictimAnimation = "heli_victim_ds_a";
            }
            else if (Weapon.IsOneHanded)
            {
                Dictionary = "veh@jacking@1h";
                PerpAnimation = "heli_perp_ds_a";
                VictimAnimation = "heli_victim_ds_a";
            }
        }
        else if (VehicleClass == VehicleClass.Commercial)
        {
            if (Weapon.IsTwoHanded)
            {
                Dictionary = "veh@jacking@2h";
                PerpAnimation = "truck_perp_ds_a";
                VictimAnimation = "truck_victim_ds_a";
            }
            else if (Weapon.IsOneHanded)
            {
                Dictionary = "veh@jacking@1h";
                PerpAnimation = "truck_perp_ds_a";
                VictimAnimation = "truck_victim_ds_a";
            }
        }
        else if (DriverDistanceToGround > 2f)//1.75f
        {
            if (Weapon.IsTwoHanded)
            {
                Dictionary = "veh@jacking@2h";
                PerpAnimation = "truck_perp_ds_a";
                VictimAnimation = "truck_victim_ds_a";
            }
            else if (Weapon.IsOneHanded)
            {
                Dictionary = "veh@jacking@1h";
                PerpAnimation = "truck_perp_ds_a";
                VictimAnimation = "truck_victim_ds_a";
            }
        }
        else if (DriverDistanceToGround < 0.5f)
        {
            if (Weapon.IsTwoHanded)
            {
                Dictionary = "veh@jacking@2h";
                PerpAnimation = "low_perp_ds_a";
                VictimAnimation = "low_victim_ds_a";
            }
            else if (Weapon.IsOneHanded)
            {
                Dictionary = "veh@jacking@1h";
                PerpAnimation = "low_perp_ds_a";
                VictimAnimation = "low_victim_ds_a";
            }
        }
        else
        {
            if (Weapon.IsTwoHanded)
            {
                Dictionary = "veh@jacking@2h";
                PerpAnimation = "std_perp_ds_a";
                VictimAnimation = "std_victim_ds_a";
            }
            else if (Weapon.IsOneHanded)
            {
                Dictionary = "veh@jacking@1h";
                PerpAnimation = "std_perp_ds";
                VictimAnimation = "std_victim_ds";
            }
        }
        return true;
    }
    private Vector3 GetEntryPosition() => NativeFunction.CallByHash<Vector3>(0xC0572928C0ABFDA3, TargetVehicle, 0);
    private bool SetupCarJack()
    {
        Player.SetPlayerToLastWeapon();
        NativeFunction.CallByName<uint>("TASK_VEHICLE_TEMP_ACTION", Driver, TargetVehicle, 27, -1);
        Driver.BlockPermanentEvents = true;

        Vector3 GameEntryPosition = GetEntryPosition();
        float DesiredHeading = TargetVehicle.Heading - 90f;
        int BoneIndexSpine = NativeFunction.CallByName<int>("GET_PED_BONE_INDEX", Driver, 57597);//11816
        DriverSeatCoordinates = NativeFunction.CallByName<Vector3>("GET_PED_BONE_COORDS", Driver, BoneIndexSpine, 0f, 0f, 0f);

        GameTimeLastTriedCarJacking = Game.GameTime;

        if (!GetCarjackingAnimations())//couldnt find animations
        {
            Game.LocalPlayer.Character.Tasks.ClearImmediately();
            GameFiber.Sleep(200);
            Game.LocalPlayer.Character.Tasks.EnterVehicle(TargetVehicle, SeatTryingToEnter);
            return false;
        }

        AnimationDictionary.RequestAnimationDictionay(Dictionary);
        Player.SetPlayerToLastWeapon();

        if (!Driver.IsInAnyVehicle(false))
            Driver.WarpIntoVehicle(TargetVehicle, -1);

        float DriverHeading = Driver.Heading;
        PlayerScene = NativeFunction.CallByName<int>("CREATE_SYNCHRONIZED_SCENE", GameEntryPosition.X, GameEntryPosition.Y, Game.LocalPlayer.Character.Position.Z, 0.0f, 0.0f, DesiredHeading, 2);//270f //old
        NativeFunction.CallByName<bool>("SET_SYNCHRONIZED_SCENE_LOOPED", PlayerScene, false);
        NativeFunction.CallByName<bool>("TASK_SYNCHRONIZED_SCENE", Game.LocalPlayer.Character, PlayerScene, Dictionary, PerpAnimation, 1000.0f, -4.0f, 64, 0, 0x447a0000, 0);//std_perp_ds_a
        NativeFunction.CallByName<bool>("SET_SYNCHRONIZED_SCENE_PHASE", PlayerScene, 0.0f);

        VictimScene = NativeFunction.CallByName<int>("CREATE_SYNCHRONIZED_SCENE", DriverSeatCoordinates.X, DriverSeatCoordinates.Y, DriverSeatCoordinates.Z, 0.0f, 0.0f, DriverHeading, 2);//270f
        NativeFunction.CallByName<bool>("SET_SYNCHRONIZED_SCENE_LOOPED", VictimScene, false);
        NativeFunction.CallByName<bool>("TASK_SYNCHRONIZED_SCENE", Driver, VictimScene, Dictionary, VictimAnimation, 1000.0f, -4.0f, 64, 0, 0x447a0000, 0);
        NativeFunction.CallByName<bool>("SET_SYNCHRONIZED_SCENE_PHASE", VictimScene, 0.0f);

        return true;
    }
    private void UnarmedCarJack()
    {
        GameFiber CarJackPed = GameFiber.StartNew(delegate
        {
            if (Victim != null)
                Victim.CanBeTasked = false;

            GameFiber.Sleep(4000);
            if (Victim != null)
                Victim.CanBeTasked = true;

            GameFiber.Sleep(4000);
        }, "CarJackPed");
    }
}
﻿using LosSantosRED.lsr.Locations;
using LSR.Vehicles;
using Rage;

namespace LosSantosRED.lsr.Interface
{
    public interface IViolateable
    {
        LocationData CurrentLocation { get; set; }
        RelationshipManager RelationshipManager { get; }
        WeaponEquipment WeaponEquipment { get; }
        Investigation Investigation { get; }
        PoliceResponse PoliceResponse { get; }
        Intoxication Intoxication { get; }
        SurrenderActivity Surrendering { get; }

        ActivityManager ActivityManager { get; }

        bool AnyHumansNear { get; }
        bool AnyPoliceCanHearPlayer { get; }
        bool AnyPoliceCanRecognizePlayer { get; }
        bool AnyPoliceCanSeePlayer { get; }
        bool AnyPoliceRecentlySeenPlayer { get; }
        Ped Character { get; }
        float ClosestPoliceDistanceToPlayer { get; }
        VehicleExt CurrentSeenVehicle { get; }
        PedExt CurrentTargetedPed { get; }
        VehicleExt CurrentVehicle { get; }
        int GroupID { get; }
        bool HasBeenMoving { get; }
        bool HasBeenMovingFast { get; }
        string DebugString { get; set; }
        bool IsAliveAndFree { get; }
        bool IsBeingANuisance { get; set; }
        bool IsBreakingIntoCar { get; }
        bool IsChangingLicensePlates { get; }
      //  bool IsCommitingSuicide { get; }
        bool IsDealingDrugs { get; }
        bool IsDealingIllegalGuns { get; }
        bool IsDoingSuspiciousActivity { get; }
        bool IsDriver { get; }
     //   bool IsHoldingHostage { get; }
      //  bool IsHoldingUp { get; }
        bool IsInAirVehicle { get; }
        bool IsInAutomobile { get; }
        bool IsInVehicle { get; }
        bool IsOnMotorcycle { get; }
        bool IsSleeping { get; }
        bool IsVisiblyArmed { get; }
        bool IsWanted { get; }
        Vector3 Position { get; }
        bool RecentlyBribedPolice { get; }
        bool RecentlyFedUpCop { get; }
        bool RecentlyPaidFine { get; }
        bool RecentlyResistedArrest { get; }
        bool RecentlyShot { get; }
        bool RecentlyStartedPlaying { get; }
        bool ShouldCheckViolations { get; }
        float VehicleSpeedMPH { get; }
        int WantedLevel { get; }
        bool IsNotWanted { get; }

        //  bool IsLootingBody { get; }

        void AddCrime(Crime violating, bool v1, Vector3 currentPosition, VehicleExt currentSeenVehicle, WeaponInformation currentSeenWeapon, bool v2, bool announceCrime, bool IsForPlayer);
        void OnVehicleCrashed();
        void OnKilledCop();
        void OnKilledCivilian();
    }
}
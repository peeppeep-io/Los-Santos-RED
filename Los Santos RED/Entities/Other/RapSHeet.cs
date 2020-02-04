﻿using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class RapSheet
{
    public int TimeAimedAtPolice;
    public int MaxWantedLevel = 0;

    

    public Crime KillingPolice = new Crime() { Severity = CrimeLevel.Felony, ResultsInLethalForce = true, ResultingWantedLevel = 3,  DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportOfficerDown, 1), DebugName = "Police Fatality" };
    public Crime FiringWeaponNearPolice = new Crime() { Severity = CrimeLevel.Felony, ResultsInLethalForce = true, ResultingWantedLevel = 3, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportShotsFired, 2), DebugName = "Shots Fired at Police" };
    public Crime AimingWeaponAtPolice = new Crime() { Severity = CrimeLevel.Felony, ResultsInLethalForce = true, ResultingWantedLevel = 3, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportThreateningWithFirearm, 3), DebugName = "Aiming Weapons At Police" };
    public Crime HurtingPolice = new Crime() { Severity = CrimeLevel.Felony, ResultsInLethalForce = true, ResultingWantedLevel = 3, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportAssualtOnOfficer, 4), DebugName = "Assaulting Police" };
    public Crime TrespessingOnGovtProperty = new Crime() { Severity = CrimeLevel.Felony, ResultsInLethalForce = true, ResultingWantedLevel = 3, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportTrespassingOnGovernmentProperty, 5), DebugName = "Trespassing on Government Property" };
    public Crime GotInAirVehicleDuringChase = new Crime() { Severity = CrimeLevel.Felony, ResultsInLethalForce = true, ResultingWantedLevel = 4, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportStolenAirVehicle, 6), DebugName = "Stealing an Air Vehicle" };


    public Crime ResistingArrest = new Crime() { Severity = CrimeLevel.Felony, ResultsInLethalForce = false, ResultingWantedLevel = 2, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportIncreasedWanted, 6), DebugName = "Resisiting Arrest" };

    public Crime KillingCivilians = new Crime() { Severity = CrimeLevel.Misdemeanor, ResultsInLethalForce = false, ResultingWantedLevel = 2, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportCivilianKilled, 7), DebugName = "Civilian Fatality" };
    public Crime BrandishingWeapon = new Crime() { Severity = CrimeLevel.Misdemeanor, ResultsInLethalForce = false, ResultingWantedLevel = 2, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportCarryingWeapon, 8), DebugName = "Brandishing Weapon" };
    public Crime ChangingPlates = new Crime() { Severity = CrimeLevel.Misdemeanor, ResultsInLethalForce = false, ResultingWantedLevel = 2, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportSuspiciousActivity, 9), DebugName = "Changing Plates" };
    public Crime BreakingIntoCars = new Crime() { Severity = CrimeLevel.Misdemeanor, ResultsInLethalForce = false, ResultingWantedLevel = 2, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportGrandTheftAuto, 10) , DebugName = "Braking into Cars" };
    public Crime HurtingCivilians = new Crime() { Severity = CrimeLevel.Misdemeanor, ResultsInLethalForce = false, ResultingWantedLevel = 2, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportLowLevelCiviliansInjured, 10), DebugName = "Assaulting Civilians" };

    public Crime DrivingAgainstTraffic = new Crime() { Severity = CrimeLevel.Traffic, ResultsInLethalForce = false, ResultingWantedLevel = 1, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportRecklessDriver, 10) { ResultsInStolenCarSpotted = true,IsTrafficViolation = true }, DebugName = "Driving Against Traffic" };
    public Crime DrivingOnPavement = new Crime() { Severity = CrimeLevel.Traffic, ResultsInLethalForce = false, ResultingWantedLevel = 1, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportRecklessDriver, 10) { ResultsInStolenCarSpotted = true, IsTrafficViolation = true }, DebugName = "Driving On Pavement" };
    public Crime HitPedWithCar = new Crime() { Severity = CrimeLevel.Traffic, ResultsInLethalForce = false, ResultingWantedLevel = 2, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportPedHitAndRun, 8) { ResultsInStolenCarSpotted = true, IsTrafficViolation = true }, DebugName = "Pedestrian Hit and Run" };
    public Crime HitCarWithCar = new Crime() { Severity = CrimeLevel.Traffic, ResultsInLethalForce = false, ResultingWantedLevel = 1, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportVehicleHitAndRun, 9) { ResultsInStolenCarSpotted = true, IsTrafficViolation = true }, DebugName = "Hit and Run" };
    public Crime NonRoadworthyVehicle = new Crime() { Severity = CrimeLevel.Traffic, ResultsInLethalForce = false, ResultingWantedLevel = 1, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportSuspiciousVehicle, 10) { ResultsInStolenCarSpotted = true, IsTrafficViolation = true }, DebugName = "Driving a non-roadworthy vehicle" };
    public Crime FelonySpeeding = new Crime() { Severity = CrimeLevel.Traffic, ResultsInLethalForce = false, ResultingWantedLevel = 1, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportFelonySpeeding, 10) { ResultsInStolenCarSpotted = true, IsTrafficViolation = true }, DebugName = "Speeding" };
    public Crime RunningARedLight = new Crime() { Severity = CrimeLevel.Traffic, ResultsInLethalForce = false, ResultingWantedLevel = 1, DispatchToPlay = new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportRunningRed, 10) { ResultsInStolenCarSpotted = true, IsTrafficViolation = true }, DebugName = "Running a Red Light" };

    public List<GTALicensePlate> WantedPlates = new List<GTALicensePlate>();
    public uint GameTimeWantedStarted;
    public uint GameTimeWantedEnded;
    public bool PlayerSeenDuringWanted = false;

    public uint GameTimeLastKilledCivilian;
    public uint GameTimeLastKilledCop;
    public bool LethalForceAuthorized
    {
        get
        {
            return GetListOfCrimes().Any(x => x.ResultsInLethalForce && x.HasBeenWitnessedByPolice);
        }
    }
    public bool CommittedAnyCrimes
    {
        get
        {
            return GetListOfCrimes().Any(x => x.HasBeenWitnessedByPolice);
        }
    }
    public List<Crime> GetListOfCrimes()
    {
        List<Crime> CrimeList = new List<Crime>() { KillingPolice, FiringWeaponNearPolice, AimingWeaponAtPolice, HurtingPolice, TrespessingOnGovtProperty, GotInAirVehicleDuringChase, KillingCivilians, BrandishingWeapon, ChangingPlates, BreakingIntoCars, HurtingCivilians, DrivingAgainstTraffic, DrivingOnPavement, HitPedWithCar, HitCarWithCar, NonRoadworthyVehicle, FelonySpeeding, RunningARedLight };
        return CrimeList;
    }
    public RapSheet()
    {

    }
    public string PrintCrimes()
    {
        string CrimeString = "";
        foreach (Crime MyCrime in GetListOfCrimes().Where(x => x.HasBeenWitnessedByPolice).OrderByDescending(x => x.Severity).Take(3))
        {
            CrimeString += string.Format("~n~{0}{1} ({2})~s~", GetColorSeverity(MyCrime.Severity),MyCrime.DebugName,MyCrime.InstancesObserved);
        }
        return CrimeString;
    }
    private static string GetColorSeverity(CrimeLevel Severity)
    {
        if (Severity == CrimeLevel.Felony)
            return "~r~";
        else if (Severity == CrimeLevel.Misdemeanor)
            return "~o~";
        else if (Severity == CrimeLevel.Traffic)
            return "~y~";
        else
            return "~s~";

    }
    public string DebugPrintCrimes()
    {
        string CrimeString = "";
        foreach(Crime MyCrime in GetListOfCrimes().Where(x => x.HasBeenWitnessedByPolice))
        {
            CrimeString += Environment.NewLine + MyCrime.CrimeStats();
        }
        return CrimeString;
    }
    public void CheckCrimes()
    {
        if (LosSantosRED.IsBusted || LosSantosRED.IsDead)
            return;

        if (KillingCivilians.CanObserveCrime && (Civilians.RecentlyKilledCivilian(5000) || Civilians.NearMurderVictim(15f)) && Police.AnyPoliceCanSeePlayer)///HasBeenWitnessedByPolice
        {
            KillingCivilians.CrimeObserved();
        }

        if (HurtingCivilians.CanObserveCrime && Civilians.RecentlyHurtCivilian(5000) && Police.AnyPoliceCanSeePlayer)//HasBeenWitnessedByPolice
        {
            HurtingCivilians.CrimeObserved();
        }

        if (FiringWeaponNearPolice.CanObserveCrime && (LosSantosRED.PlayerRecentlyShot(3000) || Police.PlayerArtificiallyShooting) && (PoliceScanning.CopPeds.Any(x => x.RecentlySeenPlayer() || (x.DistanceToPlayer <= 45f && !Game.LocalPlayer.Character.IsCurrentWeaponSilenced)))) //if (!firedWeapon && Game.LocalPlayer.Character.IsShooting && (PoliceScanning.CopPeds.Any(x => x.canSeePlayer || x.CopPed.IsInRangeOf(Game.LocalPlayer.Character.Position, 100f))))
        {
            FiringWeaponNearPolice.CrimeObserved();
        }

        if (TrespessingOnGovtProperty.CanObserveCrime && LosSantosRED.PlayerIsWanted && PlayerLocation.PlayerCurrentZone == Zones.JAIL && Police.AnyPoliceCanSeePlayer)
        {
            TrespessingOnGovtProperty.CrimeObserved();
        }

        CheckAimingAtPolice();
        if (!AimingWeaponAtPolice.CanObserveCrime && TimeAimedAtPolice >= 100)
        {
            AimingWeaponAtPolice.CrimeObserved();
        }

        if (BrandishingWeapon.CanObserveCrime && Police.AnyPoliceCanSeePlayer && LosSantosRED.PlayerIsConsideredArmed && Game.LocalPlayer.Character.Inventory.EquippedWeapon != null && !LosSantosRED.PlayerInVehicle)
        {
            GTAWeapon MatchedWeapon = GTAWeapons.GetWeaponFromHash((ulong)Game.LocalPlayer.Character.Inventory.EquippedWeapon.Hash);
            if (MatchedWeapon != null && MatchedWeapon.WeaponLevel >= 2)
                BrandishingWeapon.ResultingWantedLevel = MatchedWeapon.WeaponLevel;

            BrandishingWeapon.DispatchToPlay.WeaponToReport = MatchedWeapon;
            BrandishingWeapon.CrimeObserved();
        }

        if (ChangingPlates.CanObserveCrime && LicensePlateChanging.PlayerChangingPlate && Police.AnyPoliceCanSeePlayer)
        {
            ChangingPlates.CrimeObserved();
        }

        if (BreakingIntoCars.CanObserveCrime && CarStealing.PlayerBreakingIntoCar && Police.AnyPoliceCanSeePlayer)
        {
            BreakingIntoCars.CrimeObserved();
        }

        if (GotInAirVehicleDuringChase.CanObserveCrime && LosSantosRED.PlayerIsWanted && LosSantosRED.PlayerInVehicle && Game.LocalPlayer.Character.IsInAirVehicle)
        {
            GotInAirVehicleDuringChase.CrimeObserved();
        }
    }
    private void CheckAimingAtPolice()
    {
        if (!AimingWeaponAtPolice.HasBeenWitnessedByPolice && LosSantosRED.PlayerIsConsideredArmed && Game.LocalPlayer.IsFreeAiming && Police.AnyPoliceCanSeePlayer && PoliceScanning.CopPeds.Any(x => Game.LocalPlayer.IsFreeAimingAtEntity(x.Pedestrian)))
            TimeAimedAtPolice++;
        else
            TimeAimedAtPolice = 0;
    }
}
public class Crime
{
    public int ResultingWantedLevel = 0;
    public bool HasBeenWitnessedByPolice = false;
    public uint GameTimeLastWitnessed;
    public bool ResultsInLethalForce = false;
    public bool HasBeenReportedByDispatch = false;
    public string DebugName;
    public CrimeLevel Severity = CrimeLevel.Unknown;
    public int InstancesObserved = 0;
    public DispatchAudio.DispatchQueueItem DispatchToPlay;
    private uint GameTimeLastReported;
    private uint InstanceDuration = 20000;
   public bool CanObserveCrime
    {
        get
        {
            if (!HasBeenWitnessedByPolice)
                return true;
            else if (Game.GameTime - GameTimeLastWitnessed >= InstanceDuration)
                return true;
            else
                return false;
        }
    }
    public bool RecentlyCommittedCrime(uint TimeSince)
    {
        if (!HasBeenWitnessedByPolice)
            return false;
        else if (Game.GameTime - GameTimeLastWitnessed <= TimeSince)
            return true;
        else
            return false;
    }
    public bool RecentlyReportedCrime(uint TimeSince)
    {
        if (!HasBeenReportedByDispatch)
            return false;
        else if (Game.GameTime - GameTimeLastReported <= TimeSince)
            return true;
        else
            return false;
    }
    public Crime()
    {

    }
    public void CrimeObserved()
    {
        if ((!HasBeenReportedByDispatch || Severity == CrimeLevel.Felony) && !RecentlyReportedCrime(25000) && LosSantosRED.PlayerWantedLevel <= ResultingWantedLevel)//if (!HasBeenWitnessedByPolice && !HasBeenReportedByDispatch && InstantAction.PlayerWantedLevel <= ResultingWantedLevel)
        {
            GameTimeLastReported = Game.GameTime;
            DispatchToPlay.ResultsInLethalForce = ResultsInLethalForce;
            DispatchAudio.AddDispatchToQueue(DispatchToPlay);
        }
        HasBeenWitnessedByPolice = true;
        InstancesObserved++;
        GameTimeLastWitnessed = Game.GameTime;
        Police.SetWantedLevel(ResultingWantedLevel, DebugName);
        HasBeenReportedByDispatch = true;
        Debugging.WriteToLog("Crime Logged", DebugName);

        //if (SetOnDispatchEnd)
        //{
        //    GameFiber WaitDispatchEnd = GameFiber.StartNew(delegate
        //    {
        //        while (DispatchAudio.IsPlayingAudio)
        //            GameFiber.Sleep(200);

        //        LogCrime();
        //    }, "WaitDispatchEnd");
        //    Debugging.GameFibers.Add(WaitDispatchEnd);
        //}
        //else
        //{
        //    LogCrime();
        //}
        
    }
    //private void LogCrime()
    //{

    //}
    public string CrimeStats()
    {
        return string.Format("CrimeName {0}, HasBeenWitnessedByPolice: {1},GameTimeLastWitnessed {2},InstancesObserved {3},HasBeenReportedByDispatch {4}", DebugName, HasBeenWitnessedByPolice, GameTimeLastWitnessed, InstancesObserved, HasBeenReportedByDispatch);
    }
}
public enum CrimeLevel
{
    Unknown,
    Traffic,
    Misdemeanor,
    Felony
    
}


/*
 using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class RapSheet
{
    public int TimeAimedAtPolice;
    public int MaxWantedLevel = 0;
    public int CopsKilledByPlayer = 0;
    public int CiviliansKilledByPlayer = 0;
    public bool PlayerHurtPolice = false;
    public bool PlayerKilledPolice = false;
    public bool PlayerKilledCivilians = false;
    public bool PlayerAimedAtPolice = false;
    public bool PlayerFiredWeaponNearPolice = false;
    public bool PlayerWentNearPrisonDuringChase = false;
    public bool PlayerCaughtWithGun = false;
    public bool PlayerGotInAirVehicleDuringChase = false;
    public bool PlayerCaughtChangingPlates = false;
    public bool PlayerCaughtBreakingIntoCar = false;
    public bool PlayerKilledCiviliansInFrontOfPolice = false;


    public Crime HurtingPolice = new Crime() { ResultingDispatch = DispatchAudio.ReportDispatch.ReportAssualtOnOfficer, ResultsInLethalForce = true, DebugName = "HurtingPolice" };

    public bool DispatchReportedOfficerDown = false;
    public bool DispatchReportedLethalForceAuthorized = false;
    public bool DispatchReportedAssaultOnOfficer = false;
    public bool DispatchReportedShotsFired = false;
    public bool DispatchReportedTrespassingOnGovernmentProperty = false;
    public bool DispatchReportedCarryingWeapon = false;
    public bool DispatchReportedThreateningWithAFirearm = false;
    public bool DispatchReportedGrandTheftAuto = false;
    public bool DispatchReportedSuspiciousVehicle = false;
    public bool DispatchReportedCivilianShot = false;
    public bool DispatchReportedWeaponsFree = false;
    public bool DispatchReportedStolenAirVehicle = false;

    public List<GTALicensePlate> WantedPlates = new List<GTALicensePlate>();
    public uint GameTimeWantedStarted;
    public uint GameTimeWantedEnded;
    public bool PlayerSeenDuringWanted = false;
    //public bool IsExpired = false;

    private static bool PrevPlayerKilledPolice = false;
    private static int PrevCopsKilledByPlayer = 0;
    private bool PrevPlayerHurtPolice = false;

    public uint GameTimeLastKilledCivilian;
    public uint GameTimeLastKilledCop;
    public uint GameTimeLastHurtPolice;
    public uint GameTimeLastHurtCivilian;

    public bool RecentlyHurtCivilian(uint TimeSince)
    {
        if (GameTimeLastHurtCivilian == 0)
            return false;
        else if (Game.GameTime - GameTimeLastHurtCivilian <= TimeSince)
            return true;
        else
            return false;
    }
    public bool RecentlyHurtPolice(uint TimeSince)
    {
        if (GameTimeLastHurtPolice == 0)
            return false;
        else if (Game.GameTime - GameTimeLastHurtPolice <= TimeSince)
            return true;
        else
            return false;
    }
    public bool RecentlyKilledCivilian(uint TimeSince)
    {
        if (GameTimeLastKilledCivilian == 0)
            return false;
        else if (Game.GameTime - GameTimeLastKilledCivilian <= TimeSince)
            return true;
        else
            return false;
    }
    public bool RecentlyKilledPolice(uint TimeSince)
    {
        if (GameTimeLastKilledCop == 0)
            return false;
        else if (Game.GameTime - GameTimeLastKilledCop <= TimeSince)
            return true;
        else
            return false;
    }
    public RapSheet()
    {

    }
    public string PrintCrimes()
    {
        return string.Format("---MaxWantedLevel: {0},PlayerHurtPolice: {1}, PlayerKilledPolice: {2},PlayerKilledCivilians: {3},PlayerAimedAtPolice: {4},PlayerFiredWeaponNearPolice {5}" + Environment.NewLine +
                        ",PlayerWentNearPrisonDuringChase: {6},PlayerCaughtWithGun: {7},PlayerGotInAirVehicleDuringChase: {8},PlayerCaughtChangingPlates: {9},PlayerCaughtBreakingIntoCar: {10},PlayerKilledCiviliansInFrontOfPolice: {11}" + Environment.NewLine +
                        ",GameTimeWantedStarted: {12}, GameTimeWantedEnded: {13},PlayerSeenDuringWanted: {14}---",

                                                                MaxWantedLevel, PlayerHurtPolice, PlayerKilledPolice, PlayerKilledCivilians, PlayerAimedAtPolice, PlayerFiredWeaponNearPolice, PlayerWentNearPrisonDuringChase, PlayerCaughtWithGun, PlayerGotInAirVehicleDuringChase, PlayerCaughtChangingPlates, 
                                                                PlayerCaughtBreakingIntoCar, PlayerKilledCiviliansInFrontOfPolice, GameTimeWantedStarted, GameTimeWantedEnded, PlayerSeenDuringWanted);
    }
    public void CheckCrimes()
    {
        //if (Police.CurrentPoliceState == Police.PoliceState.ArrestedWait || Police.CurrentPoliceState == Police.PoliceState.DeadlyChase)
        //    return;

        if (!PlayerKilledCiviliansInFrontOfPolice && RecentlyKilledCivilian(5000) && Police.AnyPoliceCanSeePlayer)
        {
            PlayerKilledCiviliansInFrontOfPolice = true;
            PlayerKilledCivilianInFrontOfPoliceChanged();
        }

        if (!PlayerFiredWeaponNearPolice && (Game.LocalPlayer.Character.IsShooting || Police.PlayerArtificiallyShooting) && (PoliceScanning.CopPeds.Any(x => x.canSeePlayer || (x.DistanceToPlayer <= 20f && !Game.LocalPlayer.Character.IsCurrentWeaponSilenced)))) //if (!firedWeapon && Game.LocalPlayer.Character.IsShooting && (PoliceScanning.CopPeds.Any(x => x.canSeePlayer || x.CopPed.IsInRangeOf(Game.LocalPlayer.Character.Position, 100f))))
        {
            PlayerFiredWeaponNearPolice = true;
            FiredWeaponChanged();
        }

        if (!PlayerWentNearPrisonDuringChase && InstantAction.PlayerIsWanted && PlayerLocation.PlayerCurrentZone == Zones.JAIL && Police.AnyPoliceCanSeePlayer)
        {
            PlayerWentNearPrisonDuringChase = true;
            PlayerWentNearPrisonDuringChaseChanged();
        }

        if (!PlayerAimedAtPolice && InstantAction.PlayerIsConsideredArmed && Game.LocalPlayer.IsFreeAiming && Police.AnyPoliceCanSeePlayer && PoliceScanning.CopPeds.Any(x => Game.LocalPlayer.IsFreeAimingAtEntity(x.Pedestrian)))
            TimeAimedAtPolice++;
        else
            TimeAimedAtPolice = 0;

        if (!PlayerAimedAtPolice && TimeAimedAtPolice >= 100)
        {
            PlayerAimedAtPolice = true;
            AimedAtPoliceChanged();
        }

        if (!PlayerCaughtWithGun && Police.AnyPoliceCanSeePlayer && InstantAction.PlayerIsConsideredArmed && Game.LocalPlayer.Character.Inventory.EquippedWeapon != null && !InstantAction.PlayerInVehicle)
        {
            PlayerCaughtWithGun = true;
            PlayerCaughtWithGunChanged();
        }

        if (!PlayerCaughtChangingPlates && LicensePlateChanging.PlayerChangingPlate && Police.AnyPoliceCanSeePlayer)
        {
            PlayerCaughtChangingPlates = true;
            PlayerCaughtChangingPlatesChanged();
        }

        if (!PlayerCaughtBreakingIntoCar && CarStealing.PlayerBreakingIntoCar && Police.AnyPoliceCanSeePlayer)
        {
            PlayerCaughtBreakingIntoCar = true;
            PlayerCaughtBreakingIntoCarChanged();
        }

        if (!PlayerGotInAirVehicleDuringChase && InstantAction.PlayerIsWanted && InstantAction.PlayerInVehicle && Game.LocalPlayer.Character.IsInAirVehicle)
        {
            PlayerGotInAirVehicleDuringChase = true;
            PlayerGotInAirVehicleDuringChaseChanged();
        }

        if (PrevPlayerKilledPolice != PlayerKilledPolice)
            PlayerKilledPoliceChanged();

        if (PrevCopsKilledByPlayer != CopsKilledByPlayer)
            CopsKilledChanged();

    }
    private void PlayerGotInAirVehicleDuringChaseChanged()
    {
        if (PlayerGotInAirVehicleDuringChase)
        {
            Police.SetWantedLevel(4, "You tried to get away in an air vehicle during a chase");
            if (InstantAction.PlayerWantedLevel <= 4)
                DispatchAudio.AddDispatchToQueue(new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportStolenAirVehicle, 1)
                {
                    ResultsInLethalForce = true,
                    VehicleToReport = InstantAction.GetPlayersCurrentTrackedVehicle()
                });
        }
    }
    private void PlayerCaughtBreakingIntoCarChanged()
    {
        if (PlayerCaughtBreakingIntoCar)
        {
            Police.SetWantedLevel(2, "Police saw you breaking into a car");
            if (InstantAction.PlayerWantedLevel <= 2)
                DispatchAudio.AddDispatchToQueue(new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportGrandTheftAuto, 3));
        }
    }
    private void CopsKilledChanged()
    {
        PrevCopsKilledByPlayer = CopsKilledByPlayer;
    }
    private void PlayerHurtPoliceChanged()
    {
        if (PlayerHurtPolice)
        {
            Police.SetWantedLevel(3, "You hurt a police officer");
            if (InstantAction.PlayerWantedLevel <= 3 && !PlayerKilledPolice)
                DispatchAudio.AddDispatchToQueue(new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportAssualtOnOfficer, 3));
        }
        PrevPlayerHurtPolice = PlayerHurtPolice;
    }
    private void PlayerKilledPoliceChanged()
    {
        if (PlayerKilledPolice)
        {
            Police.SetWantedLevel(3, "You killed a police officer");
            if (InstantAction.PlayerWantedLevel <= 3)
                DispatchAudio.AddDispatchToQueue(new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportOfficerDown, 1) { ResultsInLethalForce = true });
        }
        PrevPlayerKilledPolice = PlayerKilledPolice;
    }
    private void FiredWeaponChanged()
    {
        if (PlayerFiredWeaponNearPolice)
        {
            Police.SetWantedLevel(3, "You fired a weapon at the police");
            if (InstantAction.PlayerWantedLevel <= 3)
                DispatchAudio.AddDispatchToQueue(new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportShotsFired, 2) { ResultsInLethalForce = true });
        }
    }
    private void AimedAtPoliceChanged()
    {
        if (PlayerAimedAtPolice)
        {
            Police.SetWantedLevel(3, "You aimed at the police too long");
            if (InstantAction.PlayerWantedLevel <= 3)
                DispatchAudio.AddDispatchToQueue(new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportThreateningWithFirearm, 2) { ResultsInLethalForce = true });
        }
    }
    private void PlayerCaughtChangingPlatesChanged()
    {
        if (PlayerCaughtChangingPlates)
        {
            Police.SetWantedLevel(2, "Police saw you changing plates");
            if (InstantAction.PlayerWantedLevel <= 2)
                DispatchAudio.AddDispatchToQueue(new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportSuspiciousActivity, 3));
        }
    }
    private void PlayerCaughtWithGunChanged()
    {
        if (PlayerCaughtWithGun)
        {
            if (Game.LocalPlayer.Character.Inventory.EquippedWeapon == null)
                return;

            GTAWeapon MatchedWeapon = GTAWeapons.GetWeaponFromHash((ulong)Game.LocalPlayer.Character.Inventory.EquippedWeapon.Hash);

            int DesiredWantedLevel = 2;
            if (MatchedWeapon != null && MatchedWeapon.WeaponLevel >= 2)
                DesiredWantedLevel = MatchedWeapon.WeaponLevel;

            Police.SetWantedLevel(DesiredWantedLevel, "Cops saw you with a gun");
            if (InstantAction.PlayerWantedLevel <= DesiredWantedLevel)
                DispatchAudio.AddDispatchToQueue(new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportCarryingWeapon, 3) { WeaponToReport = MatchedWeapon });
        }
    }
    private void PlayerWentNearPrisonDuringChaseChanged()
    {
        if (PlayerWentNearPrisonDuringChase)
        {
            Police.SetWantedLevel(3, "You went too close to the prison with a wanted level");
            if (InstantAction.PlayerWantedLevel <= 3)
                DispatchAudio.AddDispatchToQueue(new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportTrespassingOnGovernmentProperty, 3) { ResultsInLethalForce = true });
        }
    }
    private void PlayerKilledCivilianInFrontOfPoliceChanged()
    {
        if (PlayerKilledCiviliansInFrontOfPolice)
        {
            Police.SetWantedLevel(3, "You killed someone in front of the police");
            if (InstantAction.PlayerWantedLevel <= 3)
                DispatchAudio.AddDispatchToQueue(new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportCivilianKilled, 2));
        }
    }



}
public class Crime
{
    public int ResultingWantedLevel = 0;
    public bool HasBeenWitnessedByPolice = false;
    public uint GameTimeLastWitnessed;
    public bool ResultsInLethalForce = false;
    public bool HasBeenReportedByDispatch = false;
    public DispatchAudio.ReportDispatch ResultingDispatch;
    public GTAWeapon WeaponUsed;
    public string DebugName;

    public Crime()
    {

    }
}

    */

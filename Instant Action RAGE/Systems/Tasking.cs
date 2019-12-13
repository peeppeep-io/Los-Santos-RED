﻿using ExtensionsMethods;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Tasking
{
    private static readonly Random rnd;
    private static readonly List<PoliceTask> CopsToTask = new List<PoliceTask>();
    private static uint LastBust;
    private static int ForceSurrenderTime;
    private static bool SurrenderBust = false;
    private static uint GameTimeLastResetWeapons;
    public static string CurrentPoliceTickRunning { get; set; }
    public static bool IsRunning { get; set; } = true;

    static Tasking()
    {
        rnd = new Random();
    }
    public static void Initialize()
    {
        MainLoop();
    }
    private static void MainLoop()
    {
        GameFiber.StartNew(delegate
        {
            try
            {
                while (IsRunning)
                {
                    ProcessQueue();
                    GameFiber.Sleep(50);//was 100
                    PoliceStateTick();
                    GameFiber.Sleep(50);


                }
            }
            catch (Exception e)
            {
                InstantAction.Dispose();
                Debugging.WriteToLog("Error", e.Message + " : " + e.StackTrace);
            }
        });

        GameFiber.StartNew(delegate
        {
            try
            {
                while (IsRunning)
                {
                    if(Game.LocalPlayer.WantedLevel > 0)//Dont need to do this each tick if we arent wanted?
                        PoliceVehicleTick();

                    DisplayQueue(); //Temp Crap to show me the queue

                    GameFiber.Yield();
                }
            }
            catch (Exception e)
            {
                InstantAction.Dispose();
                Debugging.WriteToLog("Error", e.Message + " : " + e.StackTrace);
            }
        });
        
    }
    private static void DisplayQueue()
    {
        string Tasking = "";//string.Format("ToTask: {0}", CopsToTask.Count());
        foreach(PoliceTask MyTask in CopsToTask)
        {
            Tasking = MyTask.CopToAssign.CopPed.Handle.ToString() + ":" + MyTask.TaskToAssign.ToString();
        }
        UI.Text(Tasking, 0.8f, 0.16f, 0.35f, false, Color.White, UI.eFont.FontChaletComprimeCologne);
    }
    public static void Dispose()
    {
        IsRunning = false;
    }
    private static void ProcessQueue()
    {
        int _ToTask = CopsToTask.Count;

        if (_ToTask > 0)
        {
            LocalWriteToLog("TaskQueue", string.Format("Cops To Task: {0}", _ToTask));
            PoliceTask _policeTask = CopsToTask[0];
            _policeTask.CopToAssign.isTasked = true;

            if (_policeTask.TaskToAssign == PoliceTask.Task.Untask && CopsToTask.Any(x => x.CopToAssign == _policeTask.CopToAssign && x.TaskToAssign != PoliceTask.Task.Untask && x.GameTimeAssigned >= _policeTask.GameTimeAssigned))
            {
                _policeTask.CopToAssign.TaskIsQueued = false;
                CopsToTask.RemoveAt(0);
            }
            else
            {
                if (_policeTask.TaskToAssign == PoliceTask.Task.Arrest)
                    TaskChasing(_policeTask.CopToAssign);
                else if (_policeTask.TaskToAssign == PoliceTask.Task.Chase)
                    TaskChasing(_policeTask.CopToAssign);
                else if (_policeTask.TaskToAssign == PoliceTask.Task.Untask)
                    Untask(_policeTask.CopToAssign);
                else if (_policeTask.TaskToAssign == PoliceTask.Task.SimpleArrest)
                    TaskSimpleArrest(_policeTask.CopToAssign);
                else if (_policeTask.TaskToAssign == PoliceTask.Task.SimpleChase)
                    TaskSimpleChase(_policeTask.CopToAssign);
                else if (_policeTask.TaskToAssign == PoliceTask.Task.VehicleChase)
                    TaskVehicleChase(_policeTask.CopToAssign);
                else if (_policeTask.TaskToAssign == PoliceTask.Task.SimpleInvestigate)
                    TaskSimpleInvestigate(_policeTask.CopToAssign);
                else if (_policeTask.TaskToAssign == PoliceTask.Task.GoToWantedCenter)
                    TaskGoToWantedCenter(_policeTask.CopToAssign);
                else if (_policeTask.TaskToAssign == PoliceTask.Task.RandomSpawnIdle)
                    RandomSpawnIdle(_policeTask.CopToAssign);

                _policeTask.CopToAssign.TaskIsQueued = false;
                CopsToTask.RemoveAt(0);
                   
            }
        }
    }
    public static void AddItemToQueue(PoliceTask MyTask)
    {
        if (!CopsToTask.Any(x => x.CopToAssign == MyTask.CopToAssign && x.TaskToAssign == MyTask.TaskToAssign))
        {
            MyTask.GameTimeAssigned = Game.GameTime;
            CopsToTask.Add(MyTask);
            MyTask.CopToAssign.TaskIsQueued = true;
            LocalWriteToLog("InstantActionTick", string.Format("Queued: {0}, For: {1}", MyTask.TaskToAssign, MyTask.CopToAssign.CopPed.Handle));
        }
    }
    private static void PoliceVehicleTick()
    {
        foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => x.isInVehicle && !x.isInHelicopter))
        {
            if (Police.CurrentPoliceState == Police.PoliceState.DeadlyChase)
            {
                NativeFunction.CallByName<bool>("SET_DRIVER_ABILITY", Cop.CopPed, 100f);
                NativeFunction.CallByName<bool>("SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG", Cop.CopPed, 4, true);
                NativeFunction.CallByName<bool>("SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG", Cop.CopPed, 8, true);
                NativeFunction.CallByName<bool>("SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG", Cop.CopPed, 16, true);
                NativeFunction.CallByName<bool>("SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG", Cop.CopPed, 512, true);
                NativeFunction.CallByName<bool>("SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG", Cop.CopPed, 262144, true);
                NativeFunction.CallByName<bool>("SET_TASK_VEHICLE_CHASE_IDEAL_PURSUIT_DISTANCE", Cop.CopPed, 8f);
            }
            else
            {
                NativeFunction.CallByName<bool>("SET_DRIVER_ABILITY", Cop.CopPed, 100f);
                NativeFunction.CallByName<bool>("SET_TASK_VEHICLE_CHASE_IDEAL_PURSUIT_DISTANCE", Cop.CopPed, 8f);
                NativeFunction.CallByName<bool>("SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG", Cop.CopPed, 32, true);
            }

            if (PlayerLocation.PlayerIsOffroad && Cop.DistanceToPlayer <= 200f)
            {
                NativeFunction.CallByName<bool>("SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG", Cop.CopPed, 4194304, true);
            }
            else
            {
                NativeFunction.CallByName<bool>("SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG", Cop.CopPed, 4194304, false);
            }
        }
    }
    private static void PoliceStateTick()
    {
        PoliceScanning.CopPeds.RemoveAll(x => !x.CopPed.Exists());

        if (Police.CurrentPoliceState == Police.PoliceState.Normal && InstantAction.PlayerWantedLevel == 0)//if (CurrentPoliceState == PoliceState.Normal)
            PoliceTickNormal();
        else if (Police.PoliceInSearchMode)
            PoliceTickSearchMode();
        else if (Police.CurrentPoliceState == Police.PoliceState.UnarmedChase)
            PoliceTickUnarmedChase();
        else if (Police.CurrentPoliceState == Police.PoliceState.CautiousChase)
            PoliceTickCautiousChase();
        else if (Police.CurrentPoliceState == Police.PoliceState.DeadlyChase)
            PoliceTickDeadlyChase();
        else if (Police.CurrentPoliceState == Police.PoliceState.ArrestedWait)
            PoliceTickArrestedWait();
        else
            CurrentPoliceTickRunning = "";

        if(Police.CurrentPoliceState == Police.PoliceState.UnarmedChase || Police.CurrentPoliceState == Police.PoliceState.CautiousChase || Police.CurrentPoliceState == Police.PoliceState.ArrestedWait)
            SearchModeStopping.StopSearchMode = true;
        else
            SearchModeStopping.StopSearchMode = false;
    }
    private static void PoliceTickNormal()
    {
        CurrentPoliceTickRunning = "Normal";
        foreach (GTACop Cop in PoliceScanning.CopPeds)
        {
            if (Cop.isTasked && !Cop.TaskIsQueued && Cop.TaskType != PoliceTask.Task.RandomSpawnIdle)
            {
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.Untask));
            }
            else if (Cop.WasRandomSpawn && !Cop.isTasked && !Cop.TaskIsQueued)
            {
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.RandomSpawnIdle));
            }
        }
        if (Game.GameTime - Police.GameTimePoliceStateStart >= 8000 && Game.GameTime - GameTimeLastResetWeapons >= 10000)//Only reset them every 10 seconds if they need it after 8 seconds of being at normal. Incase you go from normal to deadly real fast.
        {
            foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => x.SetDeadly || x.SetTazer || x.SetUnarmed))
            {
                ResetCopWeapons(Cop);//just in case they get stuck
            }
            GameTimeLastResetWeapons = Game.GameTime;
        }
    }
    private static void PoliceTickUnarmedChase()
    {
        CurrentPoliceTickRunning = "Unarmed Chase";
        foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => !x.isTasked && x.CopPed.Exists()))
        {
            if (Cop.CopPed.IsOnBike || Cop.CopPed.IsInHelicopter)
                SetUnarmed(Cop);
            else
                SetCopTazer(Cop);

            if (Cop.DistanceToPlayer <= 55f)
            {
                int TotalFootChaseTasked = PoliceScanning.CopPeds.Where(x => (x.isTasked || x.TaskIsQueued) && x.TaskType == PoliceTask.Task.Chase).Count();
                int TotalVehicleChaseTasked = PoliceScanning.CopPeds.Where(x => (x.isTasked || x.TaskIsQueued) && x.TaskType == PoliceTask.Task.VehicleChase).Count();

                if (!InstantAction.IsBusted && Cop.RecentlySeenPlayer() && !Cop.TaskIsQueued && TotalFootChaseTasked <= 4 && !Cop.CopPed.IsInAnyVehicle(false) && Cop.DistanceToPlayer <= 55f && (!Game.LocalPlayer.Character.IsInAnyVehicle(false) || Game.LocalPlayer.Character.CurrentVehicle.Speed <= 5f))
                {
                    Cop.TaskIsQueued = true;
                    AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.Chase));
                }
                else if (!InstantAction.IsBusted && Cop.RecentlySeenPlayer() && !Cop.TaskIsQueued && TotalFootChaseTasked > 0 && TotalVehicleChaseTasked <= 5 && Cop.isInVehicle && !Cop.isInHelicopter && Cop.DistanceToPlayer <= 55f && !Game.LocalPlayer.Character.IsInAnyVehicle(false))
                {
                    Cop.TaskIsQueued = true;
                    AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.VehicleChase));
                }
                if ((InstantAction.HandsAreUp || Game.LocalPlayer.Character.IsStunned || Game.LocalPlayer.Character.IsRagdoll) && !InstantAction.IsBusted && Cop.DistanceToPlayer <= 4f && !Police.PlayerWasJustJacking && !Cop.isInVehicle)
                    SurrenderBust = true;
            }
        }
        if (PoliceScanning.CopPeds.Any(x => x.DistanceToPlayer <= 4f && !x.isInVehicle) && (Game.LocalPlayer.Character.IsRagdoll || Game.LocalPlayer.Character.Speed <= 1.0f) && !InstantAction.PlayerInVehicle && !InstantAction.IsBusted)
            SurrenderBust = true;

        if (SurrenderBust && !IsBustTimeOut())
            SurrenderBustEvent();

        SearchModeStopping.StopSearchMode = true;
       //StopSearchMode();
    }
    private static void PoliceTickArrestedWait()
    {
        CurrentPoliceTickRunning = "Arrested Wait";
        foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => !x.isTasked && x.CopPed.Exists())) // Exist/Dead Check
        {
            bool InVehicle = Cop.CopPed.IsInAnyVehicle(false);
            if (InVehicle)
            {
                SetUnarmed(Cop);
            }
            else
            {
                if (!Cop.TaskIsQueued && PoliceScanning.CopPeds.Where(x => x.isTasked || x.TaskIsQueued).Count() <= 3 && Cop.DistanceToPlayer <= 45f)
                {
                    Cop.TaskIsQueued = true;
                    AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.Arrest));
                }
                else if (!Cop.TaskIsQueued && (Cop.CopPed.Tasks.CurrentTaskStatus == Rage.TaskStatus.NoTask || Cop.CopPed.Tasks.CurrentTaskStatus == Rage.TaskStatus.Preparing || Cop.CopPed.Tasks.CurrentTaskStatus == Rage.TaskStatus.Interrupted) && (Cop.RecentlySeenPlayer() || Cop.DistanceToPlayer <= 65f))
                {
                    Cop.TaskIsQueued = true;
                    Cop.GameTimeLastTask = Game.GameTime;
                    AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.SimpleArrest));
                }
                else if (!Cop.TaskIsQueued && Game.GameTime - Cop.GameTimeLastTask > 3500 && Cop.RecentlySeenPlayer() && Cop.CopPed.Tasks.CurrentTaskStatus == Rage.TaskStatus.InProgress && Cop.DistanceToPlayer > 45f)
                {
                    Cop.TaskIsQueued = true;
                    Cop.GameTimeLastTask = Game.GameTime;
                    AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.SimpleArrest)); //retask the arrest
                }
            }
        }
        Police.SetWantedLevel(InstantAction.MaxWantedLastLife,"Changing it back to what it was max during your last life");

        if (PoliceScanning.CopPeds.Any(x => x.DistanceToPlayer <= 4f && !x.isInVehicle) && (Game.LocalPlayer.Character.IsRagdoll || Game.LocalPlayer.Character.Speed <= 1.0f) && !InstantAction.PlayerInVehicle && !InstantAction.IsBusted)
            SurrenderBust = true;

        if (SurrenderBust && !IsBustTimeOut())
            SurrenderBustEvent();

        SearchModeStopping.StopSearchMode = true;
        //StopSearchMode();
    }
    private static void PoliceTickCautiousChase()
    {
        CurrentPoliceTickRunning = "Cautious Chase";
        foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => !x.isTasked && !x.isInVehicle && !x.isInHelicopter && x.CopPed.Exists()))
        {
            SetCopDeadly(Cop);
            if (!Cop.TaskIsQueued && PoliceScanning.CopPeds.Where(x => x.isTasked || x.TaskIsQueued).Count() <= 4 && Cop.DistanceToPlayer <= 45f)
            {
                Cop.TaskIsQueued = true;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.Arrest));
            }
            else if (!Cop.TaskIsQueued && Cop.CopPed.Tasks.CurrentTaskStatus == Rage.TaskStatus.NoTask && (Cop.RecentlySeenPlayer() || Cop.DistanceToPlayer <= 65f))
            {
                Cop.TaskIsQueued = true;
                Cop.GameTimeLastTask = Game.GameTime;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.SimpleArrest));
            }
            else if (!Cop.TaskIsQueued && Game.GameTime - Cop.GameTimeLastTask > 3500 && Cop.RecentlySeenPlayer() && Cop.CopPed.Tasks.CurrentTaskStatus == Rage.TaskStatus.InProgress && Cop.DistanceToPlayer > 35f)
            {
                Cop.TaskIsQueued = true;
                Cop.GameTimeLastTask = Game.GameTime;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.SimpleArrest));
            }

        }
        foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => x.isTasked && x.TaskType != PoliceTask.Task.NoTask))//foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => x.isTasked && x.SimpleTaskName != "")) NoTask
        {
            if (!Cop.TaskIsQueued && Game.GameTime - Cop.GameTimeLastTask > 20000 && Cop.RecentlySeenPlayer() && Cop.CopPed.Tasks.CurrentTaskStatus == Rage.TaskStatus.InProgress && Cop.DistanceToPlayer > 25f)
            {
                Cop.TaskIsQueued = true;
                Cop.GameTimeLastTask = Game.GameTime;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.SimpleArrest));
            }
            else if (!Cop.TaskIsQueued && Game.GameTime - Cop.GameTimeLastTask > 20000 && !Cop.RecentlySeenPlayer() && Cop.DistanceToPlayer > 35f)
            {
                Cop.TaskIsQueued = true;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.Untask));
            }

        }
        foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => !x.isTasked && (x.isInHelicopter || x.isOnBike)))
        {
            SetUnarmed(Cop);
        }

        if (PoliceScanning.CopPeds.Any(x => x.DistanceToPlayer <= 8f && !x.isInVehicle) && Game.LocalPlayer.Character.Speed <= 4.0f && !Game.LocalPlayer.Character.IsInAnyVehicle(false) && !InstantAction.IsBusted && !Police.PlayerWasJustJacking)
            ForceSurrenderTime++;
        else
            ForceSurrenderTime = 0;

        if (ForceSurrenderTime >= 500)
            SurrenderBust = true;

        if (SurrenderBust && !IsBustTimeOut())
            SurrenderBustEvent();

        SearchModeStopping.StopSearchMode = true;
        //StopSearchMode();
    }
    private static void PoliceTickDeadlyChase()
    {
        CurrentPoliceTickRunning = "Deadly Chase";
        foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => !x.isInVehicle))
        {
            SetCopDeadly(Cop);
            if (!InstantAction.HandsAreUp && !InstantAction.BeingArrested && !Cop.TaskIsQueued && Cop.isTasked)
            {
                Cop.TaskIsQueued = true;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.Untask));
            }
        }
        foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => !x.isTasked && x.isInHelicopter))
        {
            if (!InstantAction.HandsAreUp && Game.LocalPlayer.WantedLevel >= 4)
                SetCopDeadly(Cop);
            else
                SetUnarmed(Cop);
        }
        if (Settings.IssuePoliceHeavyWeapons)
        {
            foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => x.isInVehicle && x.IssuedHeavyWeapon == null))
            {
                Police.IssueCopHeavyWeapon(Cop);
                break;
            }
        }

        if (Police.CopsKilledByPlayer >= Settings.PoliceKilledSurrenderLimit && InstantAction.PlayerWantedLevel < 4)
        {
            Police.SetWantedLevel(4,"You killed too many cops");
            DispatchAudio.AddDispatchToQueue(new DispatchAudio.DispatchQueueItem(DispatchAudio.ReportDispatch.ReportWeaponsFree, 2, false));
        }

        if (SurrenderBust && !IsBustTimeOut())
            SurrenderBustEvent();
    }
    private static void PoliceTickSearchMode()
    {
        CurrentPoliceTickRunning = "Search Mode";
        foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => x.DistanceToLastSeen <= 350f || x.DistanceToPlayer <= 250f))//.Where(x => !x.isTasked))
        {
            if (Cop.isInVehicle)
            {
                SetUnarmed(Cop);
            }
            if (!Cop.AtWantedCenterDuringSearchMode && !Cop.TaskIsQueued && Cop.TaskType != PoliceTask.Task.GoToWantedCenter && Cop.DistanceToLastSeen >= 35f && Cop.CopPed.IsDriver())//((InVehicle && Cop.CopPed.CurrentVehicle.Driver == Cop.CopPed) || !InVehicle))
            {
                Cop.TaskIsQueued = true;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.GoToWantedCenter));
            }
            else if (!Cop.TaskIsQueued && Cop.TaskType != PoliceTask.Task.SimpleInvestigate && Cop.DistanceToLastSeen < 35f)
            {
                Cop.AtWantedCenterDuringSearchMode = true;
                Cop.TaskIsQueued = true;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.SimpleInvestigate));
            }
        }


    }
    private static void SurrenderBustEvent()
    {
        if (Game.LocalPlayer.WantedLevel == 0)
        {
            SurrenderBust = false;
        }
        else
        {
            InstantAction.BeingArrested = true;
            Police.CurrentPoliceState = Police.PoliceState.ArrestedWait;
            NativeFunction.CallByName<bool>("SET_CURRENT_PED_WEAPON", Game.LocalPlayer.Character, (uint)2725352035, true);
            InstantAction.HandsAreUp = false;
            SurrenderBust = false;
            LastBust = Game.GameTime;
            LocalWriteToLog("SurrenderBust", "SurrenderBust Executed");
        }
    }

    private static void SetUnarmed(GTACop Cop)
    {
        if (!Cop.CopPed.Exists() || (Cop.SetUnarmed && !Cop.NeedsWeaponCheck))
            return;
        if (Settings.OverridePoliceAccuracy)
            Cop.CopPed.Accuracy = Settings.PoliceGeneralAccuracy;
        NativeFunction.CallByName<bool>("SET_PED_SHOOT_RATE", Cop.CopPed, 0);
        if (!(Cop.CopPed.Inventory.EquippedWeapon == null))
        {
            NativeFunction.CallByName<bool>("SET_CURRENT_PED_WEAPON", Cop.CopPed, (uint)2725352035, true); //Unequip weapon so you don't get shot
            NativeFunction.CallByName<bool>("SET_PED_CAN_SWITCH_WEAPON", Cop.CopPed, false);
        }
        Cop.SetTazer = false;
        Cop.SetUnarmed = true;
        Cop.SetDeadly = false;
        Cop.GameTimeLastWeaponCheck = Game.GameTime;
    }
    private static void ResetCopWeapons(GTACop Cop)
    {
        if (!Cop.CopPed.Exists() || (!Cop.SetDeadly && !Cop.SetTazer && !Cop.SetUnarmed && !Cop.NeedsWeaponCheck))
            return;
        if (Settings.OverridePoliceAccuracy)
            Cop.CopPed.Accuracy = Settings.PoliceGeneralAccuracy;
        NativeFunction.CallByName<bool>("SET_PED_SHOOT_RATE", Cop.CopPed, 30);
        if (!Cop.CopPed.Inventory.Weapons.Contains(Cop.IssuedPistol.Name))
            Cop.CopPed.Inventory.GiveNewWeapon(Cop.IssuedPistol.Name, -1, false);
        NativeFunction.CallByName<bool>("SET_PED_CAN_SWITCH_WEAPON", Cop.CopPed, true);
        Cop.SetTazer = false;
        Cop.SetUnarmed = false;
        Cop.SetDeadly = false;
        Cop.GameTimeLastWeaponCheck = Game.GameTime;
    }
    private static void SetCopDeadly(GTACop Cop)
    {
        if (!Cop.CopPed.Exists() || (Cop.SetDeadly && !Cop.NeedsWeaponCheck))
            return;
        if (Settings.OverridePoliceAccuracy)
            Cop.CopPed.Accuracy = Settings.PoliceGeneralAccuracy;
        NativeFunction.CallByName<bool>("SET_PED_SHOOT_RATE", Cop.CopPed, 30);
        if (!Cop.CopPed.Inventory.Weapons.Contains(Cop.IssuedPistol.Name))
            Cop.CopPed.Inventory.GiveNewWeapon(Cop.IssuedPistol.Name, -1, true);

        if ((Cop.CopPed.Inventory.EquippedWeapon == null || Cop.CopPed.Inventory.EquippedWeapon.Hash == WeaponHash.StunGun) && Game.LocalPlayer.WantedLevel >= 0)
            Cop.CopPed.Inventory.GiveNewWeapon(Cop.IssuedPistol.Name, -1, true);

        if (Settings.AllowPoliceWeaponVariations)
            InstantAction.ApplyWeaponVariation(Cop.CopPed, (uint)Cop.IssuedPistol.Hash, Cop.PistolVariation);
        NativeFunction.CallByName<bool>("SET_PED_CAN_SWITCH_WEAPON", Cop.CopPed, true);
        Cop.SetTazer = false;
        Cop.SetUnarmed = false;
        Cop.SetDeadly = true;
        Cop.GameTimeLastWeaponCheck = Game.GameTime;
    }
    private static void SetCopTazer(GTACop Cop)
    {
        if (!Cop.CopPed.Exists() || (Cop.SetTazer && !Cop.NeedsWeaponCheck))
            return;

        if (Settings.OverridePoliceAccuracy)
            Cop.CopPed.Accuracy = Settings.PoliceTazerAccuracy;
        NativeFunction.CallByName<bool>("SET_PED_SHOOT_RATE", Cop.CopPed, 100);
        if (!Cop.CopPed.Inventory.Weapons.Contains(WeaponHash.StunGun))
        {
            Cop.CopPed.Inventory.GiveNewWeapon(WeaponHash.StunGun, 100, true);
        }
        else if (Cop.CopPed.Inventory.EquippedWeapon != WeaponHash.StunGun)
        {
            Cop.CopPed.Inventory.EquippedWeapon = WeaponHash.StunGun;
        }
        NativeFunction.CallByName<bool>("SET_PED_CAN_SWITCH_WEAPON", Cop.CopPed, false);
        Cop.SetTazer = true;
        Cop.SetUnarmed = false;
        Cop.SetDeadly = false;
        Cop.GameTimeLastWeaponCheck = Game.GameTime;
    }


    private static bool IsBustTimeOut()
    {
        if (Game.GameTime - LastBust >= 10000)
            return false;
        else
            return true;
    }
    private static void TaskChasing(GTACop Cop)
    {
        if (Cop.CopPed.IsInRangeOf(Game.LocalPlayer.Character.Position, 100f) && Cop.TaskFiber != null && Cop.TaskFiber.Name == "Chase" && !Cop.RecentlySeenPlayer())
        {
            return;
        }
        if (!Cop.CopPed.IsInRangeOf(Game.LocalPlayer.Character.Position, 100f) && Cop.TaskFiber != null)
        {
            Cop.CopPed.Tasks.Clear();
            Cop.CopPed.BlockPermanentEvents = false;
            Cop.TaskFiber.Abort();
            Cop.TaskFiber = null;
            LocalWriteToLog("Task Chasing", string.Format("Initial Return: {0}", Cop.CopPed.Handle));
            return;
        }
        
        Cop.TaskFiber =
        GameFiber.StartNew(delegate
        {
            if (!Cop.CopPed.Exists())
                return;
            LocalWriteToLog("Task Chasing", string.Format("Started Chase: {0}", Cop.CopPed.Handle));
            uint TaskTime = 0;// = Game.GameTime;
            string LocalTaskName = "GoTo";
            double cool = rnd.NextDouble() * (1.17 - 1.075) + 1.075;//(1.175 - 1.1) + 1.1;
            float MoveRate = (float)cool;
            //Cop.SimpleTaskName = "Chase";
            Cop.TaskType = PoliceTask.Task.Chase;
            NativeFunction.CallByName<bool>("SET_PED_PATH_CAN_USE_CLIMBOVERS", Cop.CopPed, true);
            NativeFunction.CallByName<bool>("SET_PED_PATH_CAN_USE_LADDERS", Cop.CopPed, true);
            NativeFunction.CallByName<bool>("SET_PED_PATH_CAN_DROP_FROM_HEIGHT", Cop.CopPed, true);
            Cop.CopPed.BlockPermanentEvents = true;

            //Main Loop
            while (Cop.CopPed.Exists() && !Cop.CopPed.IsDead)
            {
                Cop.CopPed.BlockPermanentEvents = true;

                NativeFunction.CallByName<uint>("SET_PED_MOVE_RATE_OVERRIDE", Cop.CopPed, MoveRate);
                if (TaskTime == 0 || Game.GameTime - TaskTime >= 250)//250
                {
                    ArmCopAppropriately(Cop);
                    if (Cop.DistanceToPlayer > 100f || !Cop.RecentlySeenPlayer())
                        break;

                    if (Cop.CopPed.IsGettingIntoVehicle)
                    {
                        if (Game.LocalPlayer.Character.IsInAnyVehicle(false) && Game.LocalPlayer.Character.CurrentVehicle.Handle == Cop.CopPed.VehicleTryingToEnter.Handle)
                        {
                            Cop.CopPed.Tasks.Clear();
                            NativeFunction.CallByName<bool>("TASK_GOTO_ENTITY_AIMING", Cop.CopPed, Game.LocalPlayer.Character, 4f, 20f);
                            Cop.CopPed.KeepTasks = true;
                            TaskTime = Game.GameTime;
                            LocalTaskName = "Arrest";
                            LocalWriteToLog("TaskChasing", string.Format("Cop SubTasked with Car Arrest From Carjacking!!!!, {0}", Cop.CopPed.Handle));
                        }
                    }

                    if (InstantAction.PlayerInVehicle && Game.LocalPlayer.Character.CurrentVehicle.Speed <= 2.5f)
                    {
                        if (Cop.isPursuitPrimary && Cop.DistanceToPlayer <= 25f && LocalTaskName != "CarJack")
                        {
                            Cop.CopPed.CanRagdoll = false;
                            //NativeFunction.CallByName<bool>("TASK_ENTER_VEHICLE", Cop.CopPed, Game.LocalPlayer.Character.CurrentVehicle, -1, -1, 2f, 9);

                            NativeFunction.CallByName<bool>("TASK_OPEN_VEHICLE_DOOR", Cop.CopPed, Game.LocalPlayer.Character.CurrentVehicle, -1, -1, 10f);
                            Cop.CopPed.KeepTasks = true;
                            TaskTime = Game.GameTime;
                            LocalTaskName = "CarJack";
                            LocalWriteToLog("TaskChasing", "Primary Cop SubTasked with CarJack 2");
                        }
                        else if (!Cop.isPursuitPrimary && Cop.DistanceToPlayer <= 25f && LocalTaskName != "Arrest")
                        {
                            NativeFunction.CallByName<bool>("TASK_GOTO_ENTITY_AIMING", Cop.CopPed, Game.LocalPlayer.Character, 4f, 20f);
                            Cop.CopPed.KeepTasks = true;
                            TaskTime = Game.GameTime;
                            LocalTaskName = "Arrest";
                            LocalWriteToLog("TaskChasing", string.Format("Cop SubTasked with Car Arrest, {0}", Cop.CopPed.Handle));
                        }
                    }
                    else
                    {
                        if (LocalTaskName != "Arrest" && (Police.CurrentPoliceState == Police.PoliceState.ArrestedWait || (Police.CurrentPoliceState == Police.PoliceState.CautiousChase && Cop.DistanceToPlayer <= 15f)))
                        {
                            unsafe
                            {
                                int lol = 0;
                                NativeFunction.CallByName<bool>("OPEN_SEQUENCE_TASK", &lol);
                                NativeFunction.CallByName<bool>("TASK_GO_TO_ENTITY", 0, Game.LocalPlayer.Character, -1, 20f, 500f, 1073741824, 1); //Original and works ok
                                NativeFunction.CallByName<bool>("TASK_GOTO_ENTITY_AIMING", 0, Game.LocalPlayer.Character, 4f, 20f);
                                NativeFunction.CallByName<bool>("TASK_AIM_GUN_AT_ENTITY", 0, Game.LocalPlayer.Character, 10000, false);
                                NativeFunction.CallByName<bool>("SET_SEQUENCE_TO_REPEAT", lol, true);
                                NativeFunction.CallByName<bool>("CLOSE_SEQUENCE_TASK", lol);
                                NativeFunction.CallByName<bool>("TASK_PERFORM_SEQUENCE", Cop.CopPed, lol);
                                NativeFunction.CallByName<bool>("CLEAR_SEQUENCE_TASK", &lol);
                            }
                            TaskTime = Game.GameTime;
                            Cop.CopPed.KeepTasks = true;
                            LocalTaskName = "Arrest";
                        }
                        else if (LocalTaskName != "GotoShooting" && Police.CurrentPoliceState == Police.PoliceState.UnarmedChase && Cop.DistanceToPlayer <= 7f)
                        {
                            Cop.CopPed.CanRagdoll = true;
                            NativeFunction.CallByName<bool>("TASK_GO_TO_ENTITY_WHILE_AIMING_AT_ENTITY", Cop.CopPed, Game.LocalPlayer.Character, Game.LocalPlayer.Character, 200f, true, 4.0f, 200f, false, false, (uint)FiringPattern.DelayFireByOneSecond);
                            Cop.CopPed.KeepTasks = true;
                            TaskTime = Game.GameTime;
                            LocalTaskName = "GotoShooting";
                        }
                        else if (LocalTaskName != "Goto" && (Police.CurrentPoliceState == Police.PoliceState.UnarmedChase || Police.CurrentPoliceState == Police.PoliceState.CautiousChase) && Cop.DistanceToPlayer >= 15) //was 15f
                        {
                            Cop.CopPed.CanRagdoll = true;
                            NativeFunction.CallByName<bool>("TASK_GO_TO_ENTITY", Cop.CopPed, Game.LocalPlayer.Character, -1, 5.0f, 500f, 1073741824, 1); //Original and works ok
                            Cop.CopPed.KeepTasks = true;
                            TaskTime = Game.GameTime;
                            LocalTaskName = "Goto";
                        }

                    }

                    if ((InstantAction.HandsAreUp || Game.LocalPlayer.Character.IsStunned || Game.LocalPlayer.Character.IsRagdoll) && !InstantAction.IsBusted && Cop.DistanceToPlayer <= 4f && !Police.PlayerWasJustJacking && !Cop.isInVehicle)
                        SurrenderBust = true;

                    if (Game.LocalPlayer.Character.IsInAnyVehicle(false) && Game.LocalPlayer.Character.CurrentVehicle.Speed <= 4f && !InstantAction.IsBusted && Cop.DistanceToPlayer <= 4f && !Police.PlayerWasJustJacking && !Cop.isInVehicle)
                        SurrenderBust = true;

                    if (InstantAction.PlayerInVehicle && (Cop.DistanceToPlayer >= 45f || Game.LocalPlayer.Character.CurrentVehicle.Speed >= 10f))
                    {
                        GameFiber.Sleep(rnd.Next(500, 2000));//GameFiber.Sleep(rnd.Next(900, 1500));//reaction time?
                        break;
                    }
                    Cop.CopPed.KeepTasks = true;
                    TaskTime = Game.GameTime;
                }

                GameFiber.Yield();
                if (Police.CurrentPoliceState == Police.PoliceState.Normal || Police.CurrentPoliceState == Police.PoliceState.DeadlyChase || InstantAction.IsDead)
                {
                    GameFiber.Sleep(rnd.Next(500, 2000));//GameFiber.Sleep(rnd.Next(900, 1500));//reaction time?
                    break;
                }
            }
            if (Cop.CopPed.Exists() && !Cop.CopPed.IsDead)
            {
                Cop.CopPed.BlockPermanentEvents = false;
                Cop.CopPed.Tasks.Clear();
                if (Cop.CopPed.LastVehicle.Exists() && !Cop.CopPed.LastVehicle.IsPoliceVehicle)
                    Cop.CopPed.ClearLastVehicle();
            }
            LocalWriteToLog("Task Chasing", string.Format("Loop End: {0}", Cop.CopPed.Handle));
            Cop.TaskFiber = null;
            Cop.isTasked = false;
            Cop.TaskType = PoliceTask.Task.NoTask;
            //Cop.SimpleTaskName = "";
            if (Cop.CopPed.Exists() && !Cop.CopPed.IsDead)
                Cop.CopPed.CanRagdoll = true;

        }, "Chase");
        Debugging.GameFibers.Add(Cop.TaskFiber);
    }
    private static void TaskSimpleChase(GTACop Cop)
    {
        Cop.TaskType = PoliceTask.Task.SimpleChase;
        Cop.CopPed.BlockPermanentEvents = true;
        //Cop.SimpleTaskName = "SimpleChase";
        Cop.CopPed.Tasks.GoToWhileAiming(Game.LocalPlayer.Character, 10f, 40f);
        Cop.CopPed.KeepTasks = true;
        LocalWriteToLog("TaskSimpleChase", "How many times i this getting called?");
    }
    private static void TaskSimpleArrest(GTACop Cop)
    {
        Cop.TaskType = PoliceTask.Task.SimpleArrest;
        Cop.CopPed.BlockPermanentEvents = true;
        //Cop.SimpleTaskName = "SimpleArrest";
        unsafe
        {
            int lol = 0;
            NativeFunction.CallByName<bool>("OPEN_SEQUENCE_TASK", &lol);
            NativeFunction.CallByName<bool>("TASK_GO_TO_ENTITY", 0, Game.LocalPlayer.Character, -1, 20f, 500f, 1073741824, 1); //Original and works ok
            NativeFunction.CallByName<bool>("TASK_GOTO_ENTITY_AIMING", 0, Game.LocalPlayer.Character, 4f, 20f);
            NativeFunction.CallByName<bool>("TASK_AIM_GUN_AT_ENTITY", 0, Game.LocalPlayer.Character, -1, false);
            NativeFunction.CallByName<bool>("SET_SEQUENCE_TO_REPEAT", lol, true);
            NativeFunction.CallByName<bool>("CLOSE_SEQUENCE_TASK", lol);
            NativeFunction.CallByName<bool>("TASK_PERFORM_SEQUENCE", Cop.CopPed, lol);
            NativeFunction.CallByName<bool>("CLEAR_SEQUENCE_TASK", &lol);
        }
        Cop.CopPed.KeepTasks = true;
        LocalWriteToLog("TaskSimpleArrest", string.Format("Started SimpleArrest: {0}", Cop.CopPed.Handle));
    }
    private static void TaskVehicleChase(GTACop Cop)
    {
        if (!PoliceScanning.CopPeds.Any(x => x.TaskType == PoliceTask.Task.Chase))
        {
            LocalWriteToLog("Task Vehicle Chasing", string.Format("Didn't Start Vehicle Chase: {0}", Cop.CopPed.Handle));
            return; //Only task this is we already have officers on foot
        }

        Cop.TaskFiber =
        GameFiber.StartNew(delegate
        {
            LocalWriteToLog("Task Vehicle Chasing", string.Format("Started Vehicle Chase: {0}", Cop.CopPed.Handle));
            uint TaskTime = Game.GameTime;
            Cop.CopPed.BlockPermanentEvents = true;
            Cop.TaskType = PoliceTask.Task.VehicleChase;
            //Cop.SimpleTaskName = "VehicleChase";

            NativeFunction.CallByName<bool>("SET_DRIVER_ABILITY", Cop.CopPed, 100f);
            NativeFunction.CallByName<bool>("SET_PED_COMBAT_ATTRIBUTES", Cop.CopPed, 3, false);
            Cop.CopPed.KeepTasks = true;

            while (Cop.CopPed.Exists() && !Cop.CopPed.IsDead)
            {

                if (Game.GameTime - TaskTime >= 250)
                {
                    if (!Cop.CopPed.IsInAnyVehicle(false))
                    {
                        LocalWriteToLog("Task Vehicle Chase", string.Format("I got out of my car like a dummy: {0}", Cop.CopPed.Handle));
                        break;
                    }
                    if (InstantAction.PlayerInVehicle)
                    {
                        LocalWriteToLog("Task Vehicle Chase", string.Format("Player got in a vehicle, letting ai takeover: {0}", Cop.CopPed.Handle));
                        break;
                    }
                    if (!Cop.RecentlySeenPlayer())
                    {
                        LocalWriteToLog("Task Vehicle Chase", string.Format("Lost the player, let AI takeover: {0}", Cop.CopPed.Handle));
                        break;
                    }
                    Vector3 PlayerPos = Game.LocalPlayer.Character.Position;
                    Vector3 DrivingCoords = World.GetNextPositionOnStreet(PlayerPos);
                    NativeFunction.CallByName<bool>("SET_DRIVE_TASK_DRIVING_STYLE", Cop.CopPed, 6);
                    NativeFunction.CallByName<bool>("TASK_VEHICLE_GOTO_NAVMESH", Cop.CopPed, Cop.CopPed.CurrentVehicle, DrivingCoords.X, DrivingCoords.Y, DrivingCoords.Z, 25f, 110, 10f);
                    Cop.CopPed.KeepTasks = true;
                    TaskTime = Game.GameTime;
                }
                GameFiber.Yield();
                if (Police.CurrentPoliceState == Police.PoliceState.Normal || Police.CurrentPoliceState == Police.PoliceState.DeadlyChase || Police.CurrentPoliceState == Police.PoliceState.ArrestedWait || InstantAction.IsBusted || InstantAction.IsDead)
                {
                    GameFiber.Sleep(rnd.Next(500, 2000));//GameFiber.Sleep(rnd.Next(900, 1500));//reaction time?
                    break;
                }
            }
            if (Cop.CopPed.Exists() && !Cop.CopPed.IsDead)
            {
                NativeFunction.CallByName<bool>("SET_PED_COMBAT_ATTRIBUTES", Cop.CopPed, 3, true);
                Cop.CopPed.BlockPermanentEvents = false;
                Cop.CopPed.Tasks.Clear();
            }
            LocalWriteToLog("Task Vehicle Chase", string.Format("Loop End: {0}", Cop.CopPed.Handle));
            Cop.TaskFiber = null;
            Cop.isTasked = false;
            //Cop.SimpleTaskName = "";
            Cop.TaskType = PoliceTask.Task.NoTask;
        }, "VehicleChase");
        Debugging.GameFibers.Add(Cop.TaskFiber);
    }
    private static void TaskSimpleInvestigate(GTACop Cop)
    {
        if (!Cop.CopPed.Exists())
            return;
        Cop.TaskType = PoliceTask.Task.SimpleInvestigate;
        Cop.CopPed.BlockPermanentEvents = false;
        //Cop.SimpleTaskName = "SimpleInvestigate";

        Vector3 TargetLocation = Police.PlacePlayerLastSeen.Around2D(65f);//(Police.PlayerLastSeenForwardVector * 55f).Around2D(75f);
        Blip MyBlip = new Blip(TargetLocation, 15f)
        {
            Color = Color.Purple,
            Alpha = 0.5f
        };
        Police.TempBlips.Add(MyBlip);

        if (Cop.isInVehicle && Police.PlayerLastSeenInVehicle)
        {
            Cop.CopPed.Tasks.CruiseWithVehicle(30f, VehicleDrivingFlags.Emergency);
            LocalWriteToLog("TaskSimpleInvestigate", string.Format("Started SimpleInvestigate(CruiseWithVehicle): {0}", Cop.CopPed.Handle));
        }
        if (Cop.isInVehicle && !Police.PlayerLastSeenInVehicle && Police.AnyPoliceSeenPlayerThisWanted)
        {
            Vehicle CopCar = Cop.CopPed.CurrentVehicle;
            unsafe
            {
                int lol = 0;
                NativeFunction.CallByName<bool>("OPEN_SEQUENCE_TASK", &lol);
                NativeFunction.CallByName<bool>("TASK_VEHICLE_TEMP_ACTION", 0, CopCar, 27, 8000);     
                NativeFunction.CallByName<bool>("TASK_LEAVE_VEHICLE", 0, CopCar, 256);



                NativeFunction.CallByName<bool>("TASK_GO_STRAIGHT_TO_COORD", 0, TargetLocation.X, TargetLocation.Y, TargetLocation.Z, 500f, -1, Police.PlayerLastSeenHeading, 1f);
                //NativeFunction.CallByName<bool>("TASK_WANDER_STANDARD", 0, CopCar, 30f, 0);
                NativeFunction.CallByName<bool>("SET_SEQUENCE_TO_REPEAT", lol, false);
                NativeFunction.CallByName<bool>("CLOSE_SEQUENCE_TASK", lol);
                NativeFunction.CallByName<bool>("TASK_PERFORM_SEQUENCE", Cop.CopPed, lol);
                NativeFunction.CallByName<bool>("CLEAR_SEQUENCE_TASK", &lol);
            }
            LocalWriteToLog("TaskSimpleInvestigate", string.Format("Started SimpleInvestigate(ExitVehicleWander): {0}", Cop.CopPed.Handle));
        }
        else if(!Cop.isInVehicle)
        {
            LocalWriteToLog("TaskSimpleInvestigate", string.Format("Started SimpleInvestigate(Wander): {0}", Cop.CopPed.Handle));
            Cop.CopPed.Tasks.GoStraightToPosition(TargetLocation, 500f, Police.PlayerLastSeenHeading, 1f, -1);
            //Cop.CopPed.Tasks.Wander();
        }
    }
    private static void TaskGoToWantedCenter(GTACop Cop)
    {
        if (!Cop.CopPed.Exists())
            return;
        Cop.TaskType = PoliceTask.Task.GoToWantedCenter;
        Cop.CopPed.BlockPermanentEvents = false;
        //Cop.SimpleTaskName = "GoToWantedCenter";
        Vector3 WantedCenter = NativeFunction.CallByName<Vector3>("GET_PLAYER_WANTED_CENTRE_POSITION", Game.LocalPlayer);
        if (Cop.isInVehicle)
            NativeFunction.CallByName<bool>("TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE", Cop.CopPed, Cop.CopPed.CurrentVehicle, WantedCenter.X, WantedCenter.Y, WantedCenter.Z, 70f, 4 | 16 | 32 | 262144, 35f);
        else
            NativeFunction.CallByName<bool>("TASK_GO_STRAIGHT_TO_COORD", Cop.CopPed, WantedCenter.X, WantedCenter.Y, WantedCenter.Z, 500f, -1, 0f, 2f);

        //Cop.CopPed.KeepTasks = true;
        LocalWriteToLog("TaskGoToWantedCenter", string.Format("Started GoToWantedCenter: {0}", Cop.CopPed.Handle));
    }
    public static void TaskK9(GTACop Cop)
    {
        Cop.TaskFiber =
        GameFiber.StartNew(delegate
        {
            //LocalWriteToLog("Task K9 Chasing", string.Format("Started Chase: {0}", Cop.CopPed.Handle));
            uint TaskTime = Game.GameTime;
            string LocalTaskName = "GoTo";

            Cop.CopPed.BlockPermanentEvents = true;
            while (Cop.CopPed.Exists() && !Cop.CopPed.IsDead && Cop.CopPed.IsInAnyVehicle(false) && !Cop.CopPed.CurrentVehicle.IsSeatFree(-1))
                GameFiber.Sleep(2000);

            LocalWriteToLog("Task K9 Chasing", string.Format("Near Player Chase: {0}", Cop.CopPed.Handle));

            while (Cop.CopPed.Exists() && !Cop.CopPed.IsDead)
            {
                NativeFunction.CallByName<uint>("SET_PED_MOVE_RATE_OVERRIDE", Cop.CopPed, 1.5f);
                Cop.CopPed.KeepTasks = true;
                Cop.CopPed.BlockPermanentEvents = true;

                if (Game.GameTime - TaskTime >= 500)
                {

                    float _locrangeTo = Cop.CopPed.RangeTo(Game.LocalPlayer.Character.Position);
                    if (LocalTaskName != "Exit" && Cop.CopPed.IsInAnyVehicle(false) && Cop.CopPed.CurrentVehicle.Speed <= 5 && !Cop.CopPed.CurrentVehicle.HasDriver && _locrangeTo <= 75f)
                    {
                        NativeFunction.CallByName<bool>("TASK_LEAVE_VEHICLE", Cop.CopPed, Cop.CopPed.CurrentVehicle, 16);
                        Cop.CopPed.FaceEntity(Game.LocalPlayer.Character);
                        //Cop.CopPed.Heading = Game.LocalPlayer.Character.Heading;
                        TaskTime = Game.GameTime;
                        LocalTaskName = "Exit";
                        LocalWriteToLog("TaskK9Chasing", "Cop SubTasked with Exit");
                    }
                    else if (Police.CurrentPoliceState == Police.PoliceState.ArrestedWait && LocalTaskName != "Arrest")
                    {
                        NativeFunction.CallByName<bool>("TASK_GO_TO_ENTITY", Cop.CopPed, Game.LocalPlayer.Character, -1, 5.0f, 500f, 1073741824, 1); //Original and works ok
                        TaskTime = Game.GameTime;
                        LocalTaskName = "Arrest";
                        LocalWriteToLog("TaskK9Chasing", "Cop SubTasked with Arresting");
                    }
                    else if ((Police.CurrentPoliceState == Police.PoliceState.UnarmedChase || Police.CurrentPoliceState == Police.PoliceState.CautiousChase || Police.CurrentPoliceState == Police.PoliceState.DeadlyChase) && LocalTaskName != "GotoFighting" && _locrangeTo <= 10f) //was 10f
                    {
                        NativeFunction.CallByName<bool>("TASK_COMBAT_PED", Cop.CopPed, Game.LocalPlayer.Character, 0, 16);
                        Cop.CopPed.KeepTasks = true;
                        TaskTime = Game.GameTime;
                        LocalTaskName = "GotoFighting";
                        //GameFiber.Sleep(25000);
                        LocalWriteToLog("TaskK9Chasing", "Cop SubTasked with Fighting");
                    }
                    else if ((Police.CurrentPoliceState == Police.PoliceState.UnarmedChase || Police.CurrentPoliceState == Police.PoliceState.CautiousChase || Police.CurrentPoliceState == Police.PoliceState.DeadlyChase) && LocalTaskName != "Goto" && _locrangeTo >= 15f) //was 15f
                    {
                        NativeFunction.CallByName<bool>("TASK_GO_TO_ENTITY", Cop.CopPed, Game.LocalPlayer.Character, -1, 5.0f, 500f, 1073741824, 1); //Original and works ok
                        Cop.CopPed.KeepTasks = true;
                        TaskTime = Game.GameTime;
                        LocalTaskName = "Goto";
                        LocalWriteToLog("TaskK9Chasing", "Cop SubTasked with GoTo");
                    }

                    if (Police.CurrentPoliceState == Police.PoliceState.Normal || Police.CurrentPoliceState == Police.PoliceState.DeadlyChase)
                    {
                        GameFiber.Sleep(rnd.Next(500, 2000));//GameFiber.Sleep(rnd.Next(900, 1500));//reaction time?
                        break;
                    }
                }
                GameFiber.Yield();
            }
            LocalWriteToLog("Task K9 Chasing", string.Format("Loop End: {0}", Cop.CopPed.Handle));
            Cop.TaskFiber = null;

            if (Cop.CopPed.Exists() && !Cop.CopPed.IsDead)
            {
                Cop.CopPed.IsPersistent = false;
                Cop.CopPed.BlockPermanentEvents = false;
                if (!Cop.CopPed.IsInAnyVehicle(false))
                    Cop.CopPed.Tasks.ReactAndFlee(Game.LocalPlayer.Character);
            }
        }, "K9");
        Debugging.GameFibers.Add(Cop.TaskFiber);
    }
    public static void RetaskAllRandomSpawns()
    {
        foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => x.WasRandomSpawn))
        {
            if (!Cop.TaskIsQueued)
            {
                Cop.TaskIsQueued = true;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.RandomSpawnIdle));
            }
        }
        LocalWriteToLog("RetaskAllRandomSpawns", "Done");
    }
    private static void RandomSpawnIdle(GTACop Cop)
    {
        if (Cop.CopPed.Exists())
        {
            if (!Cop.CopPed.IsInAnyVehicle(false))
            {
                Vehicle LastVehicle = Cop.CopPed.LastVehicle;
                if (LastVehicle.Exists() && LastVehicle.IsDriveable && Cop.WasRandomSpawnDriver)
                {
                    unsafe
                    {
                        int lol = 0;
                        NativeFunction.CallByName<bool>("OPEN_SEQUENCE_TASK", &lol);
                        NativeFunction.CallByName<bool>("TASK_ENTER_VEHICLE", 0, LastVehicle, -1, -1, 2f, 9);
                        NativeFunction.CallByName<bool>("TASK_VEHICLE_DRIVE_WANDER", 0, LastVehicle, 18f, 183);
                        NativeFunction.CallByName<bool>("SET_SEQUENCE_TO_REPEAT", lol, false);
                        NativeFunction.CallByName<bool>("CLOSE_SEQUENCE_TASK", lol);
                        NativeFunction.CallByName<bool>("TASK_PERFORM_SEQUENCE", Cop.CopPed, lol);
                        NativeFunction.CallByName<bool>("CLEAR_SEQUENCE_TASK", &lol);
                    }
                    LocalWriteToLog("RetaskAllRandomSpawns", "Told him to get in and drive");
                }
                else
                {
                    Cop.CopPed.Tasks.Wander();
                    LocalWriteToLog("RetaskAllRandomSpawns", "Told him to wander");
                }
            }
            else
            {
                Cop.CopPed.Tasks.CruiseWithVehicle(Cop.CopPed.CurrentVehicle, 15f, VehicleDrivingFlags.Normal);
                Cop.CopPed.CurrentVehicle.IsSirenOn = false;
                //NativeFunction.CallByName<bool>("TASK_VEHICLE_DRIVE_WANDER", Cop.CopPed, Cop.CopPed.CurrentVehicle, 18f, 183);
                //Cop.CopPed.Tasks.Wander();
                LocalWriteToLog("RetaskAllRandomSpawns", "Told him to drive");
            }
        }

    }
    public static void UntaskAll(bool OnlyTasked)
    {
        foreach (GTACop Cop in PoliceScanning.CopPeds)
        {

            if (OnlyTasked && Cop.isTasked && !Cop.TaskIsQueued)
            {
                Cop.TaskIsQueued = true;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.Untask));
            }
            else
            {
                Cop.TaskIsQueued = true;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.Untask));
            }
        }
        foreach (GTACop Cop in PoliceScanning.K9Peds)
        {
            if (Cop.isTasked && !Cop.TaskIsQueued)
            {
                Cop.TaskIsQueued = true;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.Untask));
            }
        }
        LocalWriteToLog("UntaskAll", "");
    }
    public static void UntaskAllRandomSpawns(bool OnlyTasked)
    {
        foreach (GTACop Cop in PoliceScanning.CopPeds.Where(x => x.WasRandomSpawn))
        {
            if (OnlyTasked && Cop.isTasked && !Cop.TaskIsQueued)
            {
                Cop.TaskIsQueued = true;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.Untask));
            }
            else
            {
                Cop.TaskIsQueued = true;
                AddItemToQueue(new PoliceTask(Cop, PoliceTask.Task.Untask));
            }
        }

        LocalWriteToLog("UntaskAll Random", "");
    }
    private static void Untask(GTACop Cop)
    {
        if (Cop.CopPed.Exists())
        {
            if (Cop.TaskFiber != null)
            {
                Cop.TaskFiber.Abort();
                Cop.TaskFiber = null;
            }
            int seatIndex = 0;
            Vehicle CurrentVehicle = null;
            bool WasInVehicle = false;
            if (Cop.WasRandomSpawn && Cop.CopPed.IsInAnyVehicle(false))
            {
                WasInVehicle = true;
                CurrentVehicle = Cop.CopPed.CurrentVehicle;
                seatIndex = Cop.CopPed.SeatIndex;
            }
            Cop.CopPed.Tasks.Clear();

            Cop.CopPed.BlockPermanentEvents = false;

            if (!Cop.WasRandomSpawn)
                Cop.CopPed.IsPersistent = false;

            if (Cop.WasRandomSpawn && WasInVehicle && !Cop.CopPed.IsInAnyVehicle(false) && CurrentVehicle != null)
            {
                Cop.CopPed.WarpIntoVehicle(CurrentVehicle, seatIndex);

            }

            if (WasInVehicle)
                LocalWriteToLog("Untask", string.Format("Untasked: {0} in vehicle", Cop.CopPed.Handle));
            else
                LocalWriteToLog("Untask", string.Format("Untasked: {0}", Cop.CopPed.Handle));
        }

        Cop.TaskType = PoliceTask.Task.NoTask;
        //Cop.SimpleTaskName = "";
        Cop.isTasked = false;
    }
    private static void ArmCopAppropriately(GTACop Cop)
    {
        if (Police.CurrentPoliceState == Police.PoliceState.UnarmedChase)
        {
            SetCopTazer(Cop);
        }
        else if (Police.CurrentPoliceState == Police.PoliceState.CautiousChase)
        {
            SetCopDeadly(Cop);
        }
        else if (Police.CurrentPoliceState == Police.PoliceState.ArrestedWait && Police.LastPoliceState == Police.PoliceState.UnarmedChase)
        {
            SetCopTazer(Cop);
        }
        else if (Police.CurrentPoliceState == Police.PoliceState.ArrestedWait && Police.LastPoliceState != Police.PoliceState.UnarmedChase)
        {
            SetCopDeadly(Cop);
        }
    }
    private static void LocalWriteToLog(string ProcedureString, string TextToLog)
    {
        if (Settings.PoliceTaskingLogging)
            Debugging.WriteToLog(ProcedureString, TextToLog);
    }
}


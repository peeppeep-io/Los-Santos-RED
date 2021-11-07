﻿using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Idle : ComplexTask
{
    private bool NeedsUpdates;
    private Task CurrentTask = Task.Nothing;
    private uint GameTimeClearedIdle;
    private PedExt OtherTargetPed;

    private enum Task
    {
        GetInCar,
        Wander,
        Nothing,
        OtherTarget,
    }
    private Task CurrentTaskDynamic
    {
        get
        {
            //if(OtherTargets != null && OtherTargets.Any())
            //{
            //    return Task.OtherTarget;
            //}
            //else if(NativeFunction.Natives.GET_PED_ALERTNESS<int>(Ped.Pedestrian) > 0)
            //{
            //    return Task.Nothing;
            //}
          //  else 
            if (Ped.DistanceToPlayer <= 75f && !Ped.Pedestrian.IsInAnyVehicle(false) && Ped.Pedestrian.LastVehicle.Exists() && Ped.Pedestrian.LastVehicle.IsDriveable && Ped.Pedestrian.LastVehicle.FreeSeatsCount > 0)//(Ped.DistanceToPlayer <= 75f && Ped.Pedestrian.Tasks.CurrentTaskStatus != Rage.TaskStatus.InProgress && !Ped.Pedestrian.IsInAnyVehicle(false) && Ped.Pedestrian.LastVehicle.Exists() && Ped.Pedestrian.LastVehicle.IsDriveable && Ped.Pedestrian.LastVehicle.FreeSeatsCount > 0)
            {
                return Task.GetInCar;
            }
            else if (CurrentTask == Task.GetInCar && !Ped.Pedestrian.IsInAnyVehicle(false))
            {
                return Task.GetInCar;
            }
            else
            {
                return Task.Wander;
            }
        }
    }
    public Idle(IComplexTaskable cop, ITargetable player) : base(player, cop, 1500)
    {
        Name = "Idle";
        SubTaskName = "";
    }
    public override void Start()
    {
        if (Ped.Pedestrian.Exists())
        {
            EntryPoint.WriteToConsole($"TASKER: Idle Start: {Ped.Pedestrian.Handle}", 5);
            ClearTasks(true);
            Update();
        }
    }
    private void ClearTasks(bool resetAlertness)//temp public
    {
        if (Ped.Pedestrian.Exists())
        {
            int seatIndex = 0;
            Vehicle CurrentVehicle = null;
            bool WasInVehicle = false;
            if (Ped.Pedestrian.IsInAnyVehicle(false))
            {
                WasInVehicle = true;
                CurrentVehicle = Ped.Pedestrian.CurrentVehicle;
                seatIndex = Ped.Pedestrian.SeatIndex;
            }
            NativeFunction.Natives.CLEAR_PED_TASKS(Ped.Pedestrian);
            Ped.Pedestrian.BlockPermanentEvents = false;
            Ped.Pedestrian.KeepTasks = false;
            NativeFunction.Natives.CLEAR_PED_TASKS(Ped.Pedestrian);
            if (resetAlertness)
            {
                NativeFunction.Natives.SET_PED_ALERTNESS(Ped.Pedestrian, 0);
            }
            // Ped.Pedestrian.RelationshipGroup.SetRelationshipWith(RelationshipGroup.Player, Relationship.Neutral);
            if (WasInVehicle && !Ped.Pedestrian.IsInAnyVehicle(false) && CurrentVehicle != null)
            {
                Ped.Pedestrian.WarpIntoVehicle(CurrentVehicle, seatIndex);
            }
            //EntryPoint.WriteToConsole(string.Format("     ClearedTasks: {0}", Ped.Pedestrian.Handle));
        }
    }
    public override void Update()
    {
        if (Ped.Pedestrian.Exists() && ShouldUpdate)
        {
            if (CurrentTask != CurrentTaskDynamic)
            {
                CurrentTask = CurrentTaskDynamic;
                //EntryPoint.WriteToConsole($"      Idle SubTask Changed: {Ped.Pedestrian.Handle} to {CurrentTask} {CurrentDynamic}");
                ExecuteCurrentSubTask(true);
            }
            else if (NeedsUpdates)
            {
                ExecuteCurrentSubTask(false);
            }
            Ped.Pedestrian.RelationshipGroup.SetRelationshipWith(RelationshipGroup.Player, Relationship.Neutral);
            SetSiren();
        }
    }
    private void ExecuteCurrentSubTask(bool IsFirstRun)
    {
        if (CurrentTask == Task.Wander)
        {
            SubTaskName = "Wander";
            Wander(IsFirstRun);
        }
        else if (CurrentTask == Task.GetInCar)
        {
            SubTaskName = "GetInCar";
            GetInCar(IsFirstRun);
        }
        else if (CurrentTask == Task.Nothing)
        {
            SubTaskName = "Nothing";
            Nothing(IsFirstRun);
        }
        //else if (CurrentTask == Task.OtherTarget)
        //{
        //    SubTaskName = "OtherTarget";
        //    OtherTarget(IsFirstRun);
        //}
        GameTimeLastRan = Game.GameTime;
    }
    private void Wander(bool IsFirstRun)
    {
        
        if (IsFirstRun)
        {
            EntryPoint.WriteToConsole($"COP EVENT: Wander Idle Start: {Ped.Pedestrian.Handle}", 3);
            NeedsUpdates = true;
            ClearTasks(true);
            WanderTask();
        }
        else if (Ped.Pedestrian.Tasks.CurrentTaskStatus == Rage.TaskStatus.NoTask)
        {
            WanderTask();
        }

    }
    private void WanderTask()
    {
        Ped.Pedestrian.BlockPermanentEvents = true;
        Ped.Pedestrian.KeepTasks = true;
        if (Ped.Pedestrian.Exists())
        {
            if (Ped.Pedestrian.IsInAnyVehicle(false))
            {
                if (Ped.IsDriver && Ped.Pedestrian.CurrentVehicle.Exists())
                {
                    unsafe
                    {
                        int lol = 0;
                        NativeFunction.CallByName<bool>("OPEN_SEQUENCE_TASK", &lol);
                        NativeFunction.CallByName<bool>("TASK_PAUSE", 0, RandomItems.MyRand.Next(4000, 8000));
                        NativeFunction.CallByName<bool>("TASK_VEHICLE_DRIVE_WANDER", 0, Ped.Pedestrian.CurrentVehicle, 10f, (int)VehicleDrivingFlags.Normal, 10f);
                        NativeFunction.CallByName<bool>("SET_SEQUENCE_TO_REPEAT", lol, false);
                        NativeFunction.CallByName<bool>("CLOSE_SEQUENCE_TASK", lol);
                        NativeFunction.CallByName<bool>("TASK_PERFORM_SEQUENCE", Ped.Pedestrian, lol);
                        NativeFunction.CallByName<bool>("CLEAR_SEQUENCE_TASK", &lol);
                    }
                }
            }
            else
            {
                //Ped.Pedestrian.Tasks.Wander();
                NativeFunction.Natives.TASK_WANDER_STANDARD(Ped.Pedestrian, 0, 0);
            }
        }
    }
    private void GetInCar(bool IsFirstRun)
    {
        
        if (IsFirstRun)
        {
            EntryPoint.WriteToConsole($"COP EVENT: Get in Car Idle Start: {Ped.Pedestrian.Handle}", 3);
            NeedsUpdates = true;
            GetInCarTask();
        }
        else if (Ped.Pedestrian.Tasks.CurrentTaskStatus == Rage.TaskStatus.NoTask)
        {
            GetInCarTask();
        }
    }
    private void GetInCarTask()
    {
        Ped.Pedestrian.BlockPermanentEvents = true;
        Ped.Pedestrian.KeepTasks = true;
        if (Ped.Pedestrian.LastVehicle.Exists())
        {
            int SeatIndex = Ped.LastSeatIndex;
            if (!Ped.Pedestrian.LastVehicle.IsSeatFree(Ped.LastSeatIndex))
            {
                int? PossileSeat = Ped.Pedestrian.LastVehicle.GetFreeSeatIndex(-1, 0);
                if(PossileSeat != null)
                {
                    SeatIndex = PossileSeat ?? default(int);
                }
            }
            unsafe
            {
                int lol = 0;
                NativeFunction.CallByName<bool>("OPEN_SEQUENCE_TASK", &lol);
                NativeFunction.CallByName<bool>("TASK_ENTER_VEHICLE", 0, Ped.Pedestrian.LastVehicle, -1, SeatIndex, 1f, 9);
                NativeFunction.CallByName<bool>("TASK_PAUSE", 0, RandomItems.MyRand.Next(4000, 8000));
                NativeFunction.CallByName<bool>("SET_SEQUENCE_TO_REPEAT", lol, false);
                NativeFunction.CallByName<bool>("CLOSE_SEQUENCE_TASK", lol);
                NativeFunction.CallByName<bool>("TASK_PERFORM_SEQUENCE", Ped.Pedestrian, lol);
                NativeFunction.CallByName<bool>("CLEAR_SEQUENCE_TASK", &lol);
            }

        }
    }
    private void Nothing(bool IsFirstRun)
    {
        EntryPoint.WriteToConsole($"COP EVENT: Nothing Idle Start: {Ped.Pedestrian.Handle}", 3);
        if (IsFirstRun)
        {
            ClearTasks(false);
            GameTimeClearedIdle = Game.GameTime;
        }
        else if (Game.GameTime - GameTimeClearedIdle >= 10000)
        {
            NativeFunction.Natives.SET_PED_ALERTNESS(Ped.Pedestrian, 0);
        }
    }

    //private void OtherTarget(bool IsFirstRun)
    //{
    //    PedExt ClosestPed = OtherTargets.Where(x=> x.Pedestrian.Exists()).OrderByDescending(x => x.WantedLevel).OrderBy(x => x.Pedestrian.DistanceTo2D(Ped.Pedestrian)).FirstOrDefault();
    //    if (IsFirstRun)
    //    {
    //        OtherTargetPed = ClosestPed;
    //        EntryPoint.WriteToConsole($"COP EVENT: OtherTarget Idle Start: {Ped.Pedestrian.Handle}", 3);
    //        if(ClosestPed != null && ClosestPed.Pedestrian.Exists())
    //        {
    //            EntryPoint.WriteToConsole($"COP EVENT {Ped.Pedestrian.Handle}:                      OtherTarget Start Target Handle: {ClosestPed.Pedestrian.Handle}", 3);
    //        }
    //        NeedsUpdates = true;
    //        RunInterval = 2000;
    //        ClearTasks(true);
    //        OtherTargetTask();
    //    }
    //    if(OtherTargetPed != null && !OtherTargetPed.Pedestrian.Exists())
    //    {
    //        OtherTargetPed = null;
    //    }
    //    if(ClosestPed == null)
    //    {
    //        OtherTargetPed = null;
    //    }
    //    if(ClosestPed != null && OtherTargetPed == null)
    //    {
    //        OtherTargetPed = ClosestPed;
    //        EntryPoint.WriteToConsole($"COP EVENT {Ped.Pedestrian.Handle}:                                      OtherTarget Idle Ped Target Changed: {OtherTargetPed.Pedestrian.Handle}", 3);
    //        OtherTargetTask();
    //    }
    //    if(ClosestPed != null && OtherTargetPed != null && ClosestPed.Pedestrian.Exists() && OtherTargetPed.Pedestrian.Exists() && ClosestPed.Pedestrian.Handle != OtherTargetPed.Pedestrian.Handle)
    //    {
    //        OtherTargetPed = ClosestPed;
    //        EntryPoint.WriteToConsole($"COP EVENT {Ped.Pedestrian.Handle}:                                  OtherTarget Idle Ped Target Changed: {OtherTargetPed.Pedestrian.Handle}", 3);
    //        OtherTargetTask();
    //    }
    //    if (Ped.Pedestrian.Exists())
    //    {
    //        string stuff = $"COP EVENT {Ped.Pedestrian.Handle}: OtherTarget Idle ";
    //        if(OtherTargetPed != null)
    //        {
    //            stuff += $"OtherTargetPed {OtherTargetPed != null} {OtherTargetPed.Pedestrian.Exists()}";
    //        }
    //        if (OtherTargets != null)
    //        {
    //            stuff += $"     OtherTargets: {OtherTargets.Count()}";
    //        }
    //        EntryPoint.WriteToConsole(stuff, 3);
    //    }
    //}
    //private void OtherTargetTask()
    //{
    //    Ped.Pedestrian.BlockPermanentEvents = true;
    //    Ped.Pedestrian.KeepTasks = true;
    //    if (Ped.Pedestrian.Exists())
    //    {
    //        if(OtherTargetPed != null && OtherTargetPed.Pedestrian.Exists())
    //        {
    //            Ped.Pedestrian.Tasks.FightAgainst(OtherTargetPed.Pedestrian, -1);
    //        }
    //    }
    //}


    private void SetSiren()
    {
        if (Ped.Pedestrian.CurrentVehicle.Exists() && Ped.Pedestrian.CurrentVehicle.HasSiren && Ped.Pedestrian.CurrentVehicle.IsSirenOn)
        {
            Ped.Pedestrian.CurrentVehicle.IsSirenOn = false;
            Ped.Pedestrian.CurrentVehicle.IsSirenSilent = false;
        }
    }
    public override void Stop()
    {

    }
}


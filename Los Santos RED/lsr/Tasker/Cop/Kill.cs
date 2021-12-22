﻿using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Kill : ComplexTask
{
    private bool TargettingCar = false;
    private uint GametimeLastRetasked;

    public Kill(IComplexTaskable cop, ITargetable player) : base(player, cop, 1000)
    {
        Name = "Kill";
        SubTaskName = "";
    }
    public override void Start()
    {
        if (Ped.Pedestrian.Exists())
        {
            ClearTasks();
            //NativeFunction.Natives.SET_PED_SHOOT_RATE(Ped.Pedestrian, 100);//30
            NativeFunction.Natives.SET_PED_ALERTNESS(Ped.Pedestrian, 3);//very altert
            NativeFunction.Natives.SET_PED_COMBAT_ABILITY(Ped.Pedestrian, 2);//professional
            NativeFunction.Natives.SET_PED_COMBAT_RANGE(Ped.Pedestrian, 2);//far
            NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(Ped.Pedestrian, 2);//offensinve
            NativeFunction.Natives.SET_DRIVER_ABILITY(Ped.Pedestrian, 100f);


            Ped.Pedestrian.BlockPermanentEvents = true;
            Ped.Pedestrian.KeepTasks = true;

            NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(Ped.Pedestrian, (int)eCombatAttributes.BF_AlwaysFight, true);
            NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(Ped.Pedestrian, (int)eCombatAttributes.BF_CanFightArmedPedsWhenNotArmed, true);


            if (Ped.IsDriver)
            {
                if (Ped.IsInHelicopter)
                {
                    Vector3 pedPos = Player.Character.Position;
                    if(Player.Character.CurrentVehicle.Exists())
                    {
                        NativeFunction.Natives.TASK_HELI_MISSION(Ped.Pedestrian, Ped.Pedestrian.CurrentVehicle, Player.Character.CurrentVehicle, Player.Character, pedPos.X, pedPos.Y, pedPos.Z, 9, 50f, 150f, -1f, -1, 30, -1.0f, 0);//NativeFunction.Natives.TASK_HELI_MISSION(Ped.Pedestrian, Ped.Pedestrian.CurrentVehicle, Player.Character.CurrentVehicle, Player.Character, pedPos.X, pedPos.Y, pedPos.Z, 9, 50f, 150f, -1f, -1, 30, -1.0f, 0);
                    }
                    else
                    {
                        NativeFunction.Natives.TASK_HELI_MISSION(Ped.Pedestrian, Ped.Pedestrian.CurrentVehicle, 0, Player.Character, pedPos.X, pedPos.Y, pedPos.Z, 9, 50f, 150f, -1f, -1, 30, -1.0f, 0);//NativeFunction.Natives.TASK_HELI_MISSION(Ped.Pedestrian, Ped.Pedestrian.CurrentVehicle, 0, Player.Character, pedPos.X, pedPos.Y, pedPos.Z, 9, 50f, 150f, -1f, -1, 30, -1.0f, 0);
                    }
                }
                else if (Ped.IsInBoat)
                {
                    NativeFunction.Natives.TASK_VEHICLE_CHASE(Ped.Pedestrian, Player.Character);
                }
                else
                {
                    NativeFunction.Natives.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Ped.Pedestrian, (int)eChaseBehaviorFlag.FullContact, true);
                    int DesiredStyle = (int)eDrivingStyles.AvoidEmptyVehicles | (int)eDrivingStyles.AvoidPeds | (int)eDrivingStyles.AvoidObject | (int)eDrivingStyles.AllowWrongWay | (int)eDrivingStyles.ShortestPath;
                    NativeFunction.Natives.SET_DRIVE_TASK_DRIVING_STYLE(Ped.Pedestrian, DesiredStyle);
                    NativeFunction.Natives.TASK_COMBAT_HATED_TARGETS_AROUND_PED(Ped.Pedestrian, 300f, 0);
                }
            }
            else
            {
                NativeFunction.Natives.TASK_COMBAT_HATED_TARGETS_AROUND_PED(Ped.Pedestrian, 300f, 0);
               // NativeFunction.Natives.TASK_COMBAT_PED(Ped.Pedestrian, Player.Character, 0, 16);
            }
            NativeFunction.Natives.TASK_COMBAT_PED(Ped.Pedestrian, Player.Character, 0, 16);
        }  
    }
    public override void Update()
    {
        if (Ped.Pedestrian.Exists())
        {
            NativeFunction.Natives.SET_DRIVER_ABILITY(Ped.Pedestrian, 100f);
            if(Ped.IsInHelicopter)
            {
                if(Ped.IsDriver)
                {
                    if (Ped.DistanceToPlayer <= 100f && Player.Character.Speed < 32f)//70 mph
                    {
                        NativeFunction.Natives.SET_DRIVE_TASK_CRUISE_SPEED(Ped.Pedestrian, 10f);
                    }
                    else
                    {
                        NativeFunction.Natives.SET_DRIVE_TASK_CRUISE_SPEED(Ped.Pedestrian, 100f);
                    }
                }
            }
            if(!Ped.IsDriver && Ped.DistanceToPlayer <= 100f && Ped.Pedestrian.Tasks.CurrentTaskStatus == Rage.TaskStatus.NoTask && Game.GameTime - GametimeLastRetasked >= 1000)
            {
                //NativeFunction.Natives.SET_PED_SHOOT_RATE(Ped.Pedestrian, 100);//30
                NativeFunction.Natives.SET_PED_ALERTNESS(Ped.Pedestrian, 3);//very altert
                NativeFunction.Natives.SET_PED_COMBAT_ABILITY(Ped.Pedestrian, 2);//professional
                NativeFunction.Natives.SET_PED_COMBAT_RANGE(Ped.Pedestrian, 2);//far
                NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(Ped.Pedestrian, 2);//offensinve
                NativeFunction.Natives.SET_DRIVER_ABILITY(Ped.Pedestrian, 100f);


                Ped.Pedestrian.BlockPermanentEvents = true;
                Ped.Pedestrian.KeepTasks = true;

                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(Ped.Pedestrian, (int)eCombatAttributes.BF_AlwaysFight, true);
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(Ped.Pedestrian, (int)eCombatAttributes.BF_CanFightArmedPedsWhenNotArmed, true);

                if (1==1)//mod spawned only here?
                {
                    unsafe
                    {
                        int lol = 0;
                        NativeFunction.CallByName<bool>("OPEN_SEQUENCE_TASK", &lol);
                        NativeFunction.CallByName<bool>("TASK_COMBAT_PED", 0, Player.Character, 0, 16);
                        NativeFunction.CallByName<bool>("SET_SEQUENCE_TO_REPEAT", lol, true);
                        NativeFunction.CallByName<bool>("CLOSE_SEQUENCE_TASK", lol);
                        NativeFunction.CallByName<bool>("TASK_PERFORM_SEQUENCE", Ped.Pedestrian, lol);
                        NativeFunction.CallByName<bool>("CLEAR_SEQUENCE_TASK", &lol);
                    }
                }
                else
                {
                    unsafe
                    {
                        int lol = 0;
                        NativeFunction.CallByName<bool>("OPEN_SEQUENCE_TASK", &lol);
                        NativeFunction.CallByName<bool>("TASK_GO_TO_ENTITY_WHILE_AIMING_AT_ENTITY", 0, Player.Character, Player.Character, 200f, true, 4.0f, 200f, false, false, (uint)FiringPattern.DelayFireByOneSecond);
                        NativeFunction.CallByName<bool>("SET_SEQUENCE_TO_REPEAT", lol, true);
                        NativeFunction.CallByName<bool>("CLOSE_SEQUENCE_TASK", lol);
                        NativeFunction.CallByName<bool>("TASK_PERFORM_SEQUENCE", Ped.Pedestrian, lol);
                        NativeFunction.CallByName<bool>("CLEAR_SEQUENCE_TASK", &lol);
                    }
                }




                //NativeFunction.Natives.TASK_SHOOT_AT_ENTITY(Ped.Pedestrian, Player.Character, -1, (uint)FiringPattern.DelayFireByOneSecond);
                //NativeFunction.Natives.TASK_COMBAT_PED(Ped.Pedestrian, Player.Character, 0, 16);
                EntryPoint.WriteToConsole($"KillTask: {Ped.Pedestrian.Handle} Reset Combat", 5);
                GametimeLastRetasked = Game.GameTime;
            }
        }
    }
    public void ClearTasks()//temp public
    {
        if (Ped.Pedestrian.Exists())
        {
            //int seatIndex = 0;
            //Vehicle CurrentVehicle = null;
            //bool WasInVehicle = false;
            //if (Ped.Pedestrian.IsInAnyVehicle(false))
            //{
            //    WasInVehicle = true;
            //    CurrentVehicle = Ped.Pedestrian.CurrentVehicle;
            //    seatIndex = Ped.Pedestrian.SeatIndex;
            //}
            ////Ped.Pedestrian.Tasks.Clear();
            //NativeFunction.Natives.CLEAR_PED_TASKS(Ped.Pedestrian);
            ////Ped.Pedestrian.BlockPermanentEvents = false;
            ////Ped.Pedestrian.KeepTasks = false;
            ////Ped.Pedestrian.RelationshipGroup.SetRelationshipWith(RelationshipGroup.Player, Relationship.Neutral);
            //if (WasInVehicle && !Ped.Pedestrian.IsInAnyVehicle(false) && CurrentVehicle.Exists())
            //{
            //    Ped.Pedestrian.WarpIntoVehicle(CurrentVehicle, seatIndex);
            //}            
            //EntryPoint.WriteToConsole(string.Format("     ClearedTasks: {0}", Ped.Pedestrian.Handle));
        }
    }
    public override void Stop()
    {

    }
}


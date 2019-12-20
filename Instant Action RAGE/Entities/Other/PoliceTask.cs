﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PoliceTask
{
    public Task TaskToAssign { get; set; }
    public GTACop CopToAssign { get; set; }
    public uint GameTimeAssigned { get; set; }

    public enum Task
    {
        Chase = 0,
        Arrest = 1,
        Untask = 2,
        SimpleArrest = 3,
        SimpleChase = 4,
        VehicleChase = 5,
        NoTask = 6,
        SimpleInvestigate = 7,
        GoToWantedCenter = 8,
        RandomSpawnIdle = 9,
        HeliChase = 10,
    }

    public PoliceTask(GTACop _CopToAssign,Task _TaskToAssign)
    {
        CopToAssign = _CopToAssign;
        TaskToAssign = _TaskToAssign;
        CopToAssign.TaskType = _TaskToAssign;
    }
    public PoliceTask(GTACop _CopToAssign, Task _TaskToAssign,uint _GameTimeAssigned)
    {
        CopToAssign = _CopToAssign;
        TaskToAssign = _TaskToAssign;
        CopToAssign.TaskType = _TaskToAssign;
        GameTimeAssigned = _GameTimeAssigned;
    }
}


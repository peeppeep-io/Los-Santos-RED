﻿using ExtensionsMethods;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class Intoxicant//THIS IS THE OBJECT, THX, ALOCOHL, ETC
{


    public Intoxicant()
    {

    }

    public Intoxicant(string name, uint intoxicatingIntervalTime, uint soberingIntervalTime, float maxEffectAllowed, string overLayEffect, bool changesWalking, bool causesSwerve)
    {
        Name = name;
        IntoxicatingIntervalTime = intoxicatingIntervalTime;
        MaxEffectAllowed = maxEffectAllowed;
        SoberingIntervalTime = soberingIntervalTime;
        OverLayEffect = overLayEffect;
        ChangesWalking = changesWalking;
        CausesSwerve = causesSwerve;
    }
    public string Name { get; set; }
    public uint IntoxicatingIntervalTime { get; set; }
    public float MaxEffectAllowed { get; set; }
    public uint SoberingIntervalTime { get; set; }
    public string OverLayEffect { get; set; } = "";
    public bool ChangesWalking { get; set; } = true;
    public bool CausesSwerve { get; set; } = true;
    public bool HasOverlay => OverLayEffect != "";
    public float EffectIntoxicationLimit { get; set; } = 0.25f;
    public bool ContinuesWithoutCurrentUse { get; set; } = false;

}


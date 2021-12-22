﻿using LosSantosRED.lsr;
using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Intoxicants : IIntoxicants
{
    private readonly string ConfigFileName = "Plugins\\LosSantosRED\\Itoxicants.xml";
    private List<Intoxicant> IntoxicantList;
    public List<Intoxicant> Items => IntoxicantList;
    public void ReadConfig()
    {
        if (File.Exists(ConfigFileName))
        {
            IntoxicantList = Serialization.DeserializeParams<Intoxicant>(ConfigFileName);
        }
        else
        {
            DefaultConfig();
            Serialization.SerializeParams(IntoxicantList, ConfigFileName);
        }
    }
    private void DefaultConfig()
    {
        IntoxicantList = new List<Intoxicant>
        {
            new Intoxicant("Marijuana", 60000, 120000, 3.0f, "Barry1_Stoned",false,false) {  EffectIntoxicationLimit = 0.5f },
            new Intoxicant("Alcohol", 25000, 60000, 3.5f, "Drunk",true,true),
            new Intoxicant("Mushrooms", 25000, 60000, 10.0f, "DRUG_gas_huffin",true,true) {ContinuesWithoutCurrentUse = true },
            new Intoxicant("Nicotine", 120000, 60000, 1.0f, "HeatHaze",false,false),


            new Intoxicant("SPANK", 45000, 40000, 5.0f, "BeastIntro01",true,true) {  EffectIntoxicationLimit = 0.5f, ContinuesWithoutCurrentUse = true },
            new Intoxicant("Cocaine", 20000, 30000, 5.0f, "BikerFormFlash",false,true) {  EffectIntoxicationLimit = 0.5f, ContinuesWithoutCurrentUse = true },
            new Intoxicant("Meth", 15000, 60000, 5.0f, "BeastIntro02",true,true) { ContinuesWithoutCurrentUse = true },
            new Intoxicant("Toilet Cleaner", 10000, 60000, 5.0f, "dying",true,true) { ContinuesWithoutCurrentUse = true },


            new Intoxicant("Bull Shark Testosterone", 45000, 60000, 2.0f, "drug_wobbly",false,false) { ContinuesWithoutCurrentUse = true },
            new Intoxicant("Alco Patch", 25000, 60000, 4.0f, "Drunk",true,true) { ContinuesWithoutCurrentUse = true },
            new Intoxicant("Mollis", 15000, 60000, 3.0f, "drug_wobbly",true,true) {  EffectIntoxicationLimit = 0.5f, ContinuesWithoutCurrentUse = true },
            new Intoxicant("Chesty", 10000, 60000, 1.0f, "HeatHaze",true,true) { ContinuesWithoutCurrentUse = true },
            new Intoxicant("Equanox", 30000, 60000, 5.0f, "drug_wobbly",true,true) { ContinuesWithoutCurrentUse = true },
            new Intoxicant("Zombix", 25000, 60000, 5.0f, "BeastIntro01",true,true) {  EffectIntoxicationLimit = 0.5f, ContinuesWithoutCurrentUse = true },
        };
    }
    public Intoxicant Get(string name)
    {
        return IntoxicantList.FirstOrDefault(x => x.Name == name);
    }
}


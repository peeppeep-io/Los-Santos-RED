﻿using LosSantosRED.lsr.Interface;
using LosSantosRED.lsr.Player;
using System;
using System.Xml.Serialization;

[Serializable()]
public class FlashlightItem : ModItem
{
    public bool LightFollowsCamera { get; set; } = true;
    public bool AllowPropRotation { get; set; } = true;
    public bool UseFakeEmissive { get; set; } = true;
    public float PitchMax { get; set; } = 25f;
    public float PitchMin { get; set; } = -25f;
    public float HeadingMax { get; set; } = 30f;
    public float HeadingMin { get; set; } = -30f;

    public float EmissiveDistance { get; set; } = 100f;
    public float EmissiveBrightness { get; set; } = 1.0f;
    public float EmissiveHardness { get; set; } = 0.0f;
    public float EmissiveRadius { get; set; } = 13.0f;
    public float EmissiveFallOff { get; set; } = 1.0f;

    public float FakeEmissiveDistance { get; set; } = 0.4f;
    public float FakeEmissiveBrightness { get; set; } = 5.0f;
    public float FakeEmissiveHardness { get; set; } = 0.0f;
    public float FakeEmissiveRadius { get; set; } = 100f;
    public float FakeEmissiveFallOff { get; set; } = 1.0f;

    public bool IsCellphone { get; set; } = false;
    public bool CanSearch { get; set; } = true;
    public FlashlightItem()
    {
    }
    public FlashlightItem(string name, string description, ItemType itemType) : base(name, description, ItemType.Tools)
    {

    }
    public FlashlightItem(string name) : base(name, ItemType.Tools)
    {

    }
    public override void UseItem(IActionable actionable, ISettingsProvideable settings)
    {
        EntryPoint.WriteToConsole("I AM IN FLASHLIGHT ACTIVITY!!!!!!!!!!");
       actionable.ActivityManager.StartUpperBodyActivity(new FlashlightActivity(actionable, settings, this));
    }
}


﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ActivitySettings : ISettingsDefaultable
{

    [Description("Will teleport to the sitting entry point instead of walking. Useful when there are objects in the way like a large table you dont want to hit.")]
    public bool TeleportWhenSitting { get; set; }
    [Description("Allow the player to start converstaions with random peds.")]
    public bool AllowPedConversations { get; set; }
    [Description("Use the simplified conversation system (Similar to RDR2).")]
    public bool UseSimpleConversation { get; set; }
    [Description("Allow the player to loot dead or unconscious peds.")]
    public bool AllowPedLooting { get; set; }
    [Description("Allow the player to drag dead or unconscious peds.")]
    public bool AllowDraggingOtherPeds { get; set; }
    [Description("Plays an animation the dragged ped.")]
    public bool PlayDraggingPedAnimation { get; set; }
    [Description("Allow the player to hold a ped hostage at gunpoint.")]
    public bool AllowTakingOtherPedsHostage { get; set; }
    [Description("Allow the player to start random scenarios around the world.")]
    public bool AllowStartingScenarios { get; set; }
    [Description("Allow the player to go into crouch mode.")]
    public bool AllowPlayerCrouching { get; set; }
    [Description("Change player movement when in crouch mode.")]
    public bool CrouchingAdjustsMovementSpeed { get; set; }
    [Description("Amount of override player movement when crouching.")]
    public float CrouchMovementSpeedOverride { get; set; }
    [Description("Set a cinematic camera when sitting")]
    public bool UseAltCameraWhenSitting { get; set; }
    [Description("Force sitting when close to a seat")]
    public bool ForceSitWhenClose { get; set; }
    [Description("Time (in ms) before force sit kicks in")]
    public uint ForceSitTimeOut { get; set; }
    [Description("Distance (in meters) before force sit kicks in")]
    public float ForceSitDistance { get; set; }
    [Description("Distance (in meters) to slide when sitting")]
    public float SittingSlideDistance { get; set; }
    public ActivitySettings()
    {
        SetDefault();

    }
    public void SetDefault()
    {
        TeleportWhenSitting = false;
        AllowPedConversations = true;
        AllowPedLooting = true;
        AllowDraggingOtherPeds = true;
        AllowTakingOtherPedsHostage = true;
        AllowStartingScenarios = false;
        AllowPlayerCrouching = true;
        PlayDraggingPedAnimation = true;
        UseSimpleConversation = true;
        CrouchingAdjustsMovementSpeed = true;
        CrouchMovementSpeedOverride = 5.0f;
        UseAltCameraWhenSitting = false;
        ForceSitWhenClose = true;
        ForceSitTimeOut = 3000;
        ForceSitDistance = 0.7f;
        SittingSlideDistance = 0.5f;//0.1f
    }
}
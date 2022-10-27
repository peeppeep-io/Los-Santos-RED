﻿using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CivilianTasker
{
    private IEntityProvideable PedProvider;
    private ITargetable Player;
    private IWeapons Weapons;
    private ISettingsProvideable Settings;
    private Tasker Tasker;
    private IPlacesOfInterest PlacesOfInterest;
    public CivilianTasker(Tasker tasker, IEntityProvideable pedProvider, ITargetable player, IWeapons weapons, ISettingsProvideable settings, IPlacesOfInterest placesOfInterest)
    {
        Tasker = tasker;
        PedProvider = pedProvider;
        Player = player;
        Weapons = weapons;
        Settings = settings;
        PlacesOfInterest = placesOfInterest;
    }

    public void Setup()
    {

    }
    public void Update()
    {
        if (Settings.SettingsManager.CivilianSettings.ManageCivilianTasking)
        {
            PedProvider.Pedestrians.ExpireSeatAssignments();
            foreach (PedExt civilian in PedProvider.Pedestrians.CivilianList.Where(x => x.Pedestrian.Exists()))
            {
                try
                {
                    if (civilian.CanBeTasked && civilian.CanBeAmbientTasked)
                    {
                        if (civilian.DistanceToPlayer >= 230f)
                        {
                            civilian.CurrentTask = null;
                            continue;
                        }
                        if (civilian.NeedsTaskAssignmentCheck)
                        {
                            if (civilian.DistanceToPlayer <= 200f)
                            {
                                UpdateCurrentTask(civilian);//has yields if it does anything
                            }
                            else if (civilian.CurrentTask != null)
                            {
                                civilian.CurrentTask = null;
                            }
                        }
                        if (civilian.CurrentTask != null && civilian.CurrentTask.ShouldUpdate)
                        {
                            civilian.UpdateTask(null);
                            GameFiber.Yield();
                        }
                    }
                    else if (civilian.IsBusted || civilian.IsWanted)
                    {
                        UpdateCurrentTask(civilian);
                        if (civilian.CurrentTask != null && civilian.CurrentTask.ShouldUpdate)
                        {
                            civilian.UpdateTask(null);
                            GameFiber.Yield();
                        }
                    }
                    else if (!civilian.IsBusted && !civilian.CanBeTasked)
                    {
                        if(civilian.CurrentTask != null)
                        {
                            civilian.CurrentTask = null;
                        }
                    }
                }
                catch (Exception e)
                {
                    EntryPoint.WriteToConsole("Error" + e.Message + " : " + e.StackTrace, 0);
                    Game.DisplayNotification("CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", "~o~Error", "Los Santos ~r~RED", "Los Santos ~r~RED ~s~ Error Setting Civilian Task");
                }
            }
            foreach (Merchant merchant in PedProvider.Pedestrians.MerchantList.Where(x => x.Pedestrian.Exists()))
            {
                try
                {
                    if (merchant.CanBeTasked && merchant.CanBeAmbientTasked)
                    {
                        if (merchant.DistanceToPlayer >= 230f)
                        {
                            merchant.CurrentTask = null;
                            continue;
                        }
                        if (merchant.NeedsTaskAssignmentCheck)
                        {
                            if (merchant.DistanceToPlayer <= 200f)
                            {
                                UpdateCurrentTask(merchant);
                            }
                            else if (merchant.CurrentTask != null)
                            {
                                merchant.CurrentTask = null;
                            }
                        }
                        if (merchant.CurrentTask != null && merchant.CurrentTask.ShouldUpdate)
                        {
                            merchant.UpdateTask(null);
                            GameFiber.Yield();
                        }
                    }
                    else if (merchant.IsBusted || merchant.IsWanted)
                    {
                        UpdateCurrentTask(merchant);
                        if (merchant.CurrentTask != null && merchant.CurrentTask.ShouldUpdate)
                        {
                            merchant.UpdateTask(null);
                            GameFiber.Yield();
                        }
                    }
                    else if (!merchant.IsBusted && !merchant.CanBeTasked)
                    {
                        if (merchant.CurrentTask != null)
                        {
                            merchant.CurrentTask = null;
                        }
                    }
                }
                catch (Exception e)
                {
                    EntryPoint.WriteToConsole("Error" + e.Message + " : " + e.StackTrace, 0);
                    Game.DisplayNotification("CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", "~o~Error", "Los Santos ~r~RED", "Los Santos ~r~RED ~s~ Error Setting Civilian Task");
                }
            }
        }
    }
    private void UpdateCurrentTask(PedExt Civilian)//this should be moved out?
    {
        if (Civilian.IsBusted)
        {
            if (Civilian.DistanceToPlayer <= 175f)//75f
            {
                if (Civilian.CurrentTask?.Name != "GetArrested")
                {
                    Civilian.CurrentTask = new GetArrested(Civilian, Player, PedProvider);
                    GameFiber.Yield();//TR Added back 7
                    Civilian.CurrentTask?.Start();
                }
            }
        }
        else if (Civilian.IsWanted)
        {
            if(Civilian.WillFightPolice)
            {
                if (Civilian.CurrentTask?.Name != "Fight")
                {
                    Civilian.CurrentTask = new Fight(Civilian, Player, GetWeaponToIssue(Civilian.IsGangMember));
                    GameFiber.Yield();//TR Added back 7
                    Civilian.CurrentTask?.Start();
                }
            }
            else
            {
                if (Civilian.CurrentTask?.Name != "Flee")
                {
                    Civilian.CurrentTask = new Flee(Civilian, Player);
                    GameFiber.Yield();//TR Added back 7
                    Civilian.CurrentTask?.Start();
                }
            }
        }
        else if (Civilian.DistanceToPlayer <= 75f)//50f
        {
            Civilian.PedReactions.Update();
            if (Civilian.PedReactions.HasSeenScaryCrime || Civilian.PedReactions.HasSeenAngryCrime)
            {
                if (Civilian.WillCallPolice || (Civilian.WillCallPoliceIntense && Civilian.PedReactions.HasSeenIntenseCrime))
                {
                    if (Civilian.CurrentTask?.Name != "ScaredCallIn")
                    {
                        Civilian.CurrentTask = new ScaredCallIn(Civilian, Player) { OtherTarget = Civilian.PedReactions.HighestPriorityCrime?.Perpetrator };
                        GameFiber.Yield();//TR Added back 7
                        Civilian.CurrentTask?.Start();

                    }
                }
                else if (Civilian.WillFight)
                {
                    if (Civilian.PedReactions.HasSeenAngryCrime && Player.IsNotWanted)
                    {
                        if (Civilian.CurrentTask?.Name != "Fight")
                        {
                            Civilian.CurrentTask = new Fight(Civilian, Player, GetWeaponToIssue(Civilian.IsGangMember)) { OtherTarget = Civilian.PedReactions.HighestPriorityCrime?.Perpetrator };
                            GameFiber.Yield();//TR Added back 7
                            Civilian.CurrentTask?.Start();
                        }
                    }
                    else
                    {
                        if (Civilian.CurrentTask?.Name != "Flee")
                        {
                            Civilian.CurrentTask = new Flee(Civilian, Player) { OtherTarget = Civilian.PedReactions.HighestPriorityCrime?.Perpetrator };
                            GameFiber.Yield();//TR Added back 7
                            Civilian.CurrentTask?.Start();
                        }
                    }
                }
                else
                {
                    if (Civilian.CurrentTask?.Name != "Flee")
                    {
                        Civilian.CurrentTask = new Flee(Civilian, Player) { OtherTarget = Civilian.PedReactions.HighestPriorityCrime?.Perpetrator };
                        GameFiber.Yield();//TR Added back 7
                        Civilian.CurrentTask?.Start();
                    }
                }
            }
            else if (Civilian.CanAttackPlayer && Civilian.WillFight)// && !Civilian.IsGangMember )
            {
                if (Civilian.CurrentTask?.Name != "Fight")
                {
                    Civilian.CurrentTask = new Fight(Civilian, Player, GetWeaponToIssue(Civilian.IsGangMember)) { OtherTarget = Civilian.PedReactions.HighestPriorityCrime?.Perpetrator };//gang memebrs already have guns
                    GameFiber.Yield();//TR Added back 7
                    Civilian.CurrentTask?.Start();
                }
            }
            else if (Civilian.PedReactions.HasSeenMundaneCrime)
            {
                if (Civilian.WillCallPolice)
                {
                    if (Civilian.CurrentTask?.Name != "CalmCallIn")
                    {
                        Civilian.CurrentTask = new CalmCallIn(Civilian, Player);//oither target not needed, they just call in all crimes
                        GameFiber.Yield();//TR Added back 7
                        Civilian.CurrentTask?.Start();
                    }
                }
            }
            else if(Civilian.WasModSpawned && Civilian.CurrentTask == null)
            {
                SetIdle(Civilian);
            }
        }
        Civilian.GameTimeLastUpdatedTask = Game.GameTime;
    }

    private void SetIdle(PedExt ped)
    {
        if (ped.CurrentTask?.Name != "GangIdle")
        {
            //EntryPoint.WriteToConsole($"TASKER: gm {ped.Pedestrian.Handle} Task Changed from {ped.CurrentTask?.Name} to Idle", 3);
            ped.CurrentTask = new GangIdle(ped, Player, PedProvider, PlacesOfInterest);
            GameFiber.Yield();//TR Added back 4
            ped.CurrentTask.Start();
        }
    }

    private WeaponInformation GetWeaponToIssue(bool IsGangMember)
    {
        WeaponInformation ToIssue;
        if (IsGangMember)
        {
            if (RandomItems.RandomPercent(70))
            {
                ToIssue = Weapons.GetRandomRegularWeapon(WeaponCategory.Pistol);
            }
            else
            {
                ToIssue = Weapons.GetRandomRegularWeapon(WeaponCategory.Melee);
            }
        }
        else if (RandomItems.RandomPercent(40))
        {
            ToIssue = Weapons.GetRandomRegularWeapon(WeaponCategory.Pistol);
        }
        else
        {
            if (RandomItems.RandomPercent(65))
            {
                ToIssue = Weapons.GetRandomRegularWeapon(WeaponCategory.Melee);
            }
            else
            {
                ToIssue = null;
            }
        }
        return ToIssue;
    }
}

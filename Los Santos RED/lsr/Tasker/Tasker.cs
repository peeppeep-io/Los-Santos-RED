﻿using LosSantosRED.lsr.Interface;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Tasker
{
    private IEntityProvideable PedProvider;
    private ITargetable Player;
    private IWeapons Weapons;
    private ISettingsProvideable Settings;
    public Tasker(IEntityProvideable pedProvider, ITargetable player, IWeapons weapons, ISettingsProvideable settings)
    {
        PedProvider = pedProvider;
        Player = player;
        Weapons = weapons;
        Settings = settings;
    }
    public void RunPoliceTasks()
    {
        List<PedExt> otherTargets = PedProvider.CivilianList.Where(x => x.Pedestrian.Exists() && x.Pedestrian.IsAlive && x.WantedLevel > Player.WantedLevel && x.AnyPoliceSeenViolationCommitted).ToList();
        foreach (Cop Cop in PedProvider.PoliceList.Where(x => x.CurrentTask != null && x.CurrentTask.ShouldUpdate).OrderBy(x => x.DistanceToPlayer))
        {
            Cop.UpdateTask(otherTargets);
            GameFiber.Yield();
        }
    }
    public void RunCiviliansTasks()
    {
        foreach (PedExt Ped in PedProvider.CivilianList.Where(x => x.CurrentTask != null && x.CurrentTask.ShouldUpdate).OrderBy(x => x.DistanceToPlayer))//.OrderBy(x => x.CurrentTask.GameTimeLastRan))
        {
            Ped.UpdateTask(null);
            GameFiber.Yield();
        }
    }
    public void UpdatePoliceTasks()
    {
        if (Settings.SettingsManager.PoliceSettings.ManageTasking)
        {
            foreach (Cop Cop in PedProvider.PoliceList.Where(x => x.Pedestrian.Exists() && x.HasBeenSpawnedFor >= 2000 && x.NeedsTaskAssignmentCheck).OrderBy(x => x.DistanceToPlayer))
            {
                UpdateCurrentTask(Cop);
                GameFiber.Yield();
            }
        }
    }
    public void UpdateCivilianTasks()
    {
        if (Settings.SettingsManager.CivilianSettings.ManageCivilianTasking)
        {
            foreach (PedExt Civilian in PedProvider.CivilianList.Where(x => x.Pedestrian.Exists() && x.DistanceToPlayer <= 75f && x.NeedsTaskAssignmentCheck).OrderBy(x => x.DistanceToPlayer))//.OrderBy(x => x.GameTimeLastUpdatedTask).Take(10))//2//10)//2
            {
                if (Civilian.DistanceToPlayer <= 75f)
                {
                    UpdateCurrentTask(Civilian);
                }
                else if (Civilian.CurrentTask != null)
                {
                    Civilian.CurrentTask = null;
                }
            }
            foreach (PedExt Civilian in PedProvider.CivilianList.Where(x => x.Pedestrian.Exists() && x.DistanceToPlayer > 100f))
            {
                Civilian.CurrentTask = null;
            }
            GameFiber.Yield();
        }
    }
    private void UpdateCurrentTask(Cop Cop)//this should be moved out?
    {
        if (Cop.DistanceToPlayer <= Player.ActiveDistance)// && !Cop.IsInHelicopter)//heli, dogs, boats come next?
        {
            if (Player.IsWanted)
            {
                if (Player.IsInSearchMode)
                {
                    if (Cop.CurrentTask?.Name != "Locate")
                    {
                        Cop.CurrentTask = new Locate(Cop, Player);
                        Cop.CurrentTask.Start();
                    }
                }
                else
                {
                    if (Cop.DistanceToPlayer <= 150f)//200f
                    {
                        if (Player.PoliceResponse.IsDeadlyChase && (Player.PoliceResponse.IsWeaponsFree || !Player.IsAttemptingToSurrender))
                        {
                            if (Cop.CurrentTask?.Name != "Kill")
                            {
                                Cop.CurrentTask = new Kill(Cop, Player);
                                Cop.CurrentTask.Start();
                            }
                        }
                        else
                        {
                            if (Cop.CurrentTask?.Name != "Chase")
                            {
                                Cop.CurrentTask = new Chase(Cop, Player);
                                Cop.CurrentTask.Start();
                            }
                        }
                    }
                    else// if (Cop.DistanceToPlayer <= Player.ActiveDistance)//1000f
                    {
                        if (Cop.CurrentTask?.Name != "Locate")
                        {
                            Cop.CurrentTask = new Locate(Cop, Player);
                            Cop.CurrentTask.Start();
                        }
                    }
                }
            }
            else if (Player.Investigation.IsActive)
            {
                if (Cop.CurrentTask?.Name != "Investigate")
                {
                    Cop.CurrentTask = new Investigate(Cop, Player);
                    Cop.CurrentTask.Start();
                }
            }
            else
            {
                if (Cop.CurrentTask?.Name != "Idle")// && Cop.WasModSpawned)
                {
                    Cop.CurrentTask = new Idle(Cop, Player);
                    Cop.CurrentTask.Start();
                }
                //else
                //{
                //    if (Cop.CurrentTask != null && Cop.Pedestrian.Exists())
                //    {
                //        Cop.Pedestrian.Tasks.Clear();
                //        Cop.CurrentTask = null;
                //    }

                //}
            }
        }
        else
        {
            if (Cop.CurrentTask?.Name != "Idle" && Cop.WasModSpawned)
            {
                Cop.CurrentTask = new Idle(Cop, Player);
                Cop.CurrentTask.Start();
            }
            else
            {
                if(Cop.CurrentTask != null && Cop.Pedestrian.Exists())
                {
                    Cop.Pedestrian.Tasks.Clear();
                    Cop.CurrentTask = null;
                }
                
            }
        }
        Cop.GameTimeLastUpdatedTask = Game.GameTime;
    }
    private void UpdateCurrentTask(PedExt Civilian)//this should be moved out?
    {
        if (Civilian.DistanceToPlayer <= 75f && Civilian.CanBeTasked && Civilian.CanBeAmbientTasked)//50f
        {
            //bool SeenAnyReportableCrime = Civilian.CrimesWitnessed.Any(x => x.CanBeReportedByCivilians);
            bool SeenScaryCrime = Civilian.CrimesWitnessed.Any(x => x.ScaresCivilians && x.CanBeReportedByCivilians);
            bool SeenAngryCrime = Civilian.CrimesWitnessed.Any(x => x.AngersCivilians && x.CanBeReportedByCivilians);

            if (SeenScaryCrime || SeenAngryCrime)
            {
                if (Civilian.WillCallPolice)
                {
                    if (Civilian.CurrentTask?.Name != "CallIn")
                    {
                        Civilian.CurrentTask = new CallIn(Civilian, Player);
                        Civilian.CurrentTask.Start();
                    }
                }
                else if (Civilian.WillFight)
                {
                    if (SeenAngryCrime)
                    {
                        if (Civilian.CurrentTask?.Name != "Fight")
                        {
                            WeaponInformation ToIssue = Civilian.IsGangMember ? Weapons.GetRandomRegularWeapon(WeaponCategory.Pistol) : Weapons.GetRandomRegularWeapon(WeaponCategory.Melee);
                            Civilian.CurrentTask = new Fight(Civilian, Player, ToIssue); ;
                            Civilian.CurrentTask.Start();
                        }
                    }
                    else
                    {
                        if (Civilian.CurrentTask?.Name != "Flee")
                        {
                            Civilian.CurrentTask = new Flee(Civilian, Player);
                            Civilian.CurrentTask.Start();
                        }
                    }
                }
                else
                {
                    if (Civilian.CurrentTask?.Name != "Flee")
                    {
                        Civilian.CurrentTask = new Flee(Civilian, Player);
                        Civilian.CurrentTask.Start();
                    }
                }
            }
            else if (Civilian.IsFedUpWithPlayer && Civilian.WillFight)
            {
                if (Civilian.CurrentTask?.Name != "Fight")
                {
                    WeaponInformation ToIssue = Civilian.IsGangMember ? Weapons.GetRandomRegularWeapon(WeaponCategory.Pistol) : Weapons.GetRandomRegularWeapon(WeaponCategory.Melee);
                    Civilian.CurrentTask = new Fight(Civilian, Player, ToIssue); ;
                    Civilian.CurrentTask.Start();
                }
            }     
        }
        Civilian.GameTimeLastUpdatedTask = Game.GameTime;




        //if (Civilian.DistanceToPlayer <= 75f && Civilian.CanBeTasked)//50f
        //{
        //    if (Civilian.CurrentTask?.Name != "Fight" && Civilian.HasSeenPlayerCommitCrime)
        //    {
        //        if (Civilian.WillCallPolice && Player.IsNotWanted && Civilian.CrimesWitnessed.Any(x => (x.ScaresCivilians || x.AngersCivilians) && x.CanBeReportedByCivilians))
        //        {
        //            if (Civilian.CurrentTask?.Name != "CallIn")
        //            {
        //                Civilian.CurrentTask = new CallIn(Civilian, Player);
        //                Civilian.CurrentTask.Start();
        //            }
        //        }
        //        else if (Civilian.CurrentTask?.Name != "Fight" && Civilian.WillFight && Player.IsNotWanted && Civilian.CrimesWitnessed.Any(x => x.AngersCivilians))
        //        {
        //            WeaponInformation ToIssue = Civilian.IsGangMember ? Weapons.GetRandomRegularWeapon(WeaponCategory.Pistol) : Weapons.GetRandomRegularWeapon(WeaponCategory.Melee);
        //            Civilian.CurrentTask = new Fight(Civilian, Player, ToIssue); ;
        //            Civilian.CurrentTask.Start();
        //        }
        //        else
        //        {
        //            if (Civilian.CurrentTask?.Name != "Flee" && Civilian.CurrentTask?.Name != "Fight" && Civilian.CrimesWitnessed.Any(x => x.ScaresCivilians))
        //            {
        //                Civilian.CurrentTask = new Flee(Civilian, Player);
        //                Civilian.CurrentTask.Start();
        //            }
        //        }
        //    }
        //    else if (Civilian.CurrentTask?.Name != "Fight" && Civilian.IsFedUpWithPlayer)
        //    {
        //        WeaponInformation ToIssue = Civilian.IsGangMember ? Weapons.GetRandomRegularWeapon(WeaponCategory.Pistol) : Weapons.GetRandomRegularWeapon(WeaponCategory.Melee);
        //        Civilian.CurrentTask = new Fight(Civilian, Player, ToIssue); ;
        //        Civilian.CurrentTask.Start();
        //    }
        //}
        //Civilian.GameTimeLastUpdatedTask = Game.GameTime;
    }
}


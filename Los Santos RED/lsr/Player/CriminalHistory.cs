﻿using LosSantosRED.lsr;
using LosSantosRED.lsr.Interface;
using LSR.Vehicles;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr
{
    public class CriminalHistory
    {
        private List<PoliceResponse> RapSheetList = new List<PoliceResponse>();
        private IPoliceRespondable Player;
        public CriminalHistory(IPoliceRespondable currentPlayer)
        {
            Player = currentPlayer;
        }
        public int LastWantedMaxLevel => LastResponse == null ? 0 : LastResponse.ObservedMaxWantedLevel;
        public float SearchRadius => LastWantedMaxLevel > 0 ? LastWantedMaxLevel * 400f : 400f;
        private bool HasHistory => RapSheetList.Any();
        private PoliceResponse LastResponse => RapSheetList.Where(x => x.PlayerSeenDuringWanted).OrderByDescending(x => x.GameTimeWantedEnded).OrderByDescending(x => x.GameTimeWantedStarted).FirstOrDefault();
        public void StoreCriminalHistory(PoliceResponse rapSheet)
        {
            RapSheetList.Add(rapSheet);
            EntryPoint.WriteToConsole($" PLAYER EVENT: Criminal History Store", 3);
        }
        public void Update()
        {
            if (Player.IsAliveAndFree)
            {
                if (HasHistory && Player.AnyPoliceCanRecognizePlayer)
                {
                    if (Player.IsWanted)
                    {
                        ApplyLastWantedStats();
                    }
                    else if (Player.PoliceResponse.NearLastWanted(SearchRadius) && Player.PoliceResponse.HasBeenNotWantedFor >= 5000)
                    {
                        ApplyLastWantedStats();
                    }
                    else if (Player.IsInVehicle && Player.CurrentVehicle != null && Player.CurrentVehicle.CopsRecognizeAsStolen)
                    {
                        ApplyWantedStatsForPlate(Player.CurrentVehicle.CarPlate.PlateNumber);
                    }
                }
               // RapSheetList.RemoveAll(x => x.HasBeenNotWantedFor >= 120000);
            }
        }
        public void Clear()
        {
            RapSheetList.Clear();
            EntryPoint.WriteToConsole($" PLAYER EVENT: Criminal History Clear", 3);
        }
        public void PrintCriminalHistory()
        {
            foreach(PoliceResponse rs in RapSheetList)
            {
                EntryPoint.WriteToConsole("-------------------------------OBS", 3);
               // EntryPoint.WriteToConsole($" RapSheet: Observed Max Wanted {rs.ObservedMaxWantedLevel}", 3);
                foreach(CrimeEvent ab in rs.CrimesObserved)
                {
                    EntryPoint.WriteToConsole($" Observed Crime: {ab.AssociatedCrime.Name}", 3);
                }
                foreach (CrimeEvent ab in rs.CrimesReported)
                {
                    EntryPoint.WriteToConsole($" Reported Crime: {ab.AssociatedCrime.Name}", 3);
                }
                EntryPoint.WriteToConsole("-------------------------------REP", 3);
            }
        }
        private void ApplyLastWantedStats()
        {
            ApplyWantedStats(LastResponse);
        }
        private void ApplyWantedStats(PoliceResponse CriminalHistory)
        {
            if (CriminalHistory != null)
            {
                RapSheetList.Remove(CriminalHistory);
                foreach (CrimeEvent crime in CriminalHistory.CrimesObserved.OrderByDescending(x => x.AssociatedCrime.Priority))
                {
                    EntryPoint.WriteToConsole($"PLAYER EVENT: APPLYING WANTED STATS: ADDING CRIME: {crime.AssociatedCrime.Name}", 3);
                    Player.AddCrime(crime.AssociatedCrime, true, Player.Position, Player.CurrentSeenVehicle, Player.CurrentSeenWeapon, true);
                }
                Player.OnAppliedWantedStats();
                //GameTimeLastAppliedWantedStats = Game.GameTime;
                EntryPoint.WriteToConsole($"PLAYER EVENT: APPLYING WANTED STATS", 3);
            }
        }
        private void ApplyWantedStatsForPlate(string PlateNumber)
        {
            ApplyWantedStats(RapSheetList.Where(x => x.PlayerSeenDuringWanted && x.WantedPlates.Any(y => y.PlateNumber == PlateNumber)).OrderByDescending(x => x.GameTimeWantedEnded).OrderByDescending(x => x.GameTimeWantedStarted).FirstOrDefault());
        }
    }
}
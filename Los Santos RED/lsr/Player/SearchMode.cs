﻿using ExtensionsMethods;
using LosSantosRED.lsr;
using LosSantosRED.lsr.Interface;
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
    public class SearchMode
    {
        private IPoliceRespondable Player;
        private bool PrevIsInSearchMode;
        private bool PrevIsInActiveMode;
        private uint GameTimeStartedSearchMode;
        private uint GameTimeStartedActiveMode;
        private ISettingsProvideable Settings;
        public bool IsActive { get; private set; } = true;
        public SearchMode(IPoliceRespondable currentPlayer, ISettingsProvideable settings)
        {
            Player = currentPlayer;
            Settings = settings;
        }
        public float SearchModePercentage => IsInSearchMode ? 1.0f - ((float)TimeInSearchMode / (float)CurrentSearchTime) : 0;
        public bool IsInStartOfSearchMode => IsInSearchMode && SearchModePercentage >= Settings.SettingsManager.PoliceSettings.SearchModeStartPercent;
        public bool IsInSearchMode { get; private set; }
        public bool IsInActiveMode { get; private set; }
        public uint TimeInSearchMode => IsInSearchMode && GameTimeStartedSearchMode != 0 ? Game.GameTime - GameTimeStartedSearchMode : 0;
        public uint TimeInActiveMode => IsInActiveMode ? Game.GameTime - GameTimeStartedActiveMode : 0;
        public uint CurrentSearchTime => (uint)Player.WantedLevel * Settings.SettingsManager.PlayerOtherSettings.SearchMode_SearchTimeMultiplier;//30000;//30 seconds each
        public uint CurrentActiveTime => (uint)Player.WantedLevel * 30000;//30 seconds each
        public string DebugString { get; set; }
        public void Update()
        {
            if (IsActive)
            {
                DetermineMode();
                ToggleModes();
                Player.IsInSearchMode = IsInSearchMode;
            }
            DebugString = IsInSearchMode ? $"TimeInSearchMode: {TimeInSearchMode}, CurrentSearchTime: {CurrentSearchTime}" + $" SearchModePercentage: {SearchModePercentage}" : $"TimeInActiveMode: {TimeInActiveMode}, CurrentActiveTime: {CurrentActiveTime}";
        }
        public void Dispose()
        {
            IsActive = false;
        }
        private void DetermineMode()
        {
            if (Player.IsWanted)// && Player.HasBeenWantedFor >= 5000)
            {
                if (Player.AnyPoliceRecentlySeenPlayer)
                {
                    IsInActiveMode = true;
                    IsInSearchMode = false;
                }
                else
                {
                    if (IsInSearchMode && TimeInSearchMode >= CurrentSearchTime)
                    {
                        IsInActiveMode = false;
                        IsInSearchMode = false;
                        Player.OnSuspectEluded();
                    }
                    else
                    {
                        IsInActiveMode = false;
                        IsInSearchMode = true;
                    }
                }
            }
            else
            {
                IsInActiveMode = false;
                IsInSearchMode = false;
            }
        }
        private void ToggleModes()
        {
            if (PrevIsInActiveMode != IsInActiveMode)
            {
                if (IsInActiveMode)
                {
                    StartActiveMode();
                }
            }

            if (PrevIsInSearchMode != IsInSearchMode)
            {
                if (IsInSearchMode)
                {
                    StartSearchMode();
                }
                else
                {
                    EndSearchMode();
                }
            }
        }
        private void StartSearchMode()
        {
            IsInActiveMode = false;
            IsInSearchMode = true;
            PrevIsInSearchMode = IsInSearchMode;
            PrevIsInActiveMode = IsInActiveMode;
            GameTimeStartedSearchMode = Game.GameTime;
            GameTimeStartedActiveMode = 0;
            Player.OnWantedSearchMode();
            EntryPoint.WriteToConsole("SearchMode Start Search Mode",5);
        }
        private void StartActiveMode()
        {
            IsInActiveMode = true;
            IsInSearchMode = false;
            PrevIsInSearchMode = IsInSearchMode;
            PrevIsInActiveMode = IsInActiveMode;
            GameTimeStartedActiveMode = Game.GameTime;
            GameTimeStartedSearchMode = 0;
            Player.OnWantedActiveMode();
            EntryPoint.WriteToConsole("SEARCH MODE: Start Active Mode",5);
        }
        private void EndSearchMode()
        {
            IsInActiveMode = false;
            IsInSearchMode = false;
            PrevIsInSearchMode = IsInSearchMode;
            PrevIsInActiveMode = IsInActiveMode;
            GameTimeStartedSearchMode = 0;
            GameTimeStartedActiveMode = 0;
            Player.SetWantedLevel(0, "Search Mode Timeout", true);
            EntryPoint.WriteToConsole("SEARCH MODE: End Search Mode", 5);
        }
    }
}
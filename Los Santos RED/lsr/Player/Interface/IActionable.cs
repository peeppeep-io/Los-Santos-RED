﻿using LSR.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Interface
{
    public interface IActionable
    {
        bool IsDead { get; }
        bool IsBusted { get; }
        bool IsInVehicle { get; }
        bool IsPerformingActivity { get; }
        string AutoTuneStation { get; set; }
        bool CanPerformActivities { get; }
        List<LicensePlate> SpareLicensePlates { get; }
        List<ConsumableInventoryItem> ConsumableItems { get; }

        void StartSmokingPot();
        void StartSmoking();
        void StartDrinkingActivity();
        void CommitSuicide();
        void DisplayPlayerNotification();
        void GiveMoney(int v);
        void RemovePlate();
        void ChangePlate(int Index);
        void StopDynamicActivity();
        void ChangePlate(LicensePlate selectedItem);
        void TakeOwnershipOfNearestCar();
        void CallPolice();
        //void StartEatingActivity(ConsumableSubstance selectedStuff);
        void RemoveFromInventory(ConsumableSubstance selectedStuff, int v);
        void StartConsumingActivity(ConsumableSubstance selectedStuff);
    }
}

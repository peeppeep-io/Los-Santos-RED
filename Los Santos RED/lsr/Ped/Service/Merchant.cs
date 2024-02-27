﻿using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


public class Merchant : PedExt, IWeaponIssuable
{
    public GameLocation AssociatedStore { get; set; }
    public Merchant(Ped _Pedestrian, ISettingsProvideable settings, string _Name, ICrimes crimes, IWeapons weapons, IEntityProvideable world) : base(_Pedestrian, settings, crimes, weapons, _Name, "Vendor", world)
    {
        WeaponInventory = new WeaponInventory(this, settings);
    }
    public override bool KnowsDrugAreas => false;
    public override bool KnowsGangAreas => false;
    public override bool CanTransact => IsNearSpawnPosition && base.CanTransact;
    public override bool IsMerchant { get; set; } = true;
    public override bool CanBeIdleTasked => false;
    public IssuableWeapon GetRandomMeleeWeapon(IWeapons weapons) => AssociatedStore?.GetRandomMeleeWeapon(weapons);
    public IssuableWeapon GetRandomWeapon(bool v, IWeapons weapons) => AssociatedStore?.GetRandomWeapon(v, weapons);
    public WeaponInventory WeaponInventory { get; private set; }
    public bool HasTaser { get; set; } = false;
    public bool IsUsingMountedWeapon { get; set; } = false;
    public override string InteractPrompt(IButtonPromptable player)
    {
        return $"Transact with {FormattedName}";
    }

    public void SetStats(DispatchablePerson dispatchablePerson, IShopMenus shopMenus, IWeapons weapons, bool addBlip, bool forceMelee, bool forceSidearm, bool forceLongGun, GameLocation store)
    {
        if (!Pedestrian.Exists())
        {
            return;
        }
        Pedestrian.Money = 0;
        IsTrustingOfPlayer = RandomItems.RandomPercent(Settings.SettingsManager.CivilianSettings.PercentageTrustingOfPlayer);
        Money = RandomItems.GetRandomNumberInt(Settings.SettingsManager.CivilianSettings.MerchantMoneyMin, Settings.SettingsManager.CivilianSettings.MerchantMoneyMax);
        WillFight = RandomItems.RandomPercent(CivilianFightPercentage());
        WillCallPolice = RandomItems.RandomPercent(CivilianCallPercentage());
        WillCallPoliceIntense = RandomItems.RandomPercent(CivilianSeriousCallPercentage());
        WillFightPolice = RandomItems.RandomPercent(CivilianFightPolicePercentage());
        WillCower = RandomItems.RandomPercent(CivilianCowerPercentage());
        CanSurrender = RandomItems.RandomPercent(Settings.SettingsManager.CivilianSettings.PossibleSurrenderPercentage);
        LocationTaskRequirements = new LocationTaskRequirements() { TaskRequirements = TaskRequirements.Guard, ForcedScenarios = new List<string>() { "WORLD_HUMAN_STAND_IMPATIENT" } };
        if (store != null)
        {
            SetupTransactionItems(store.Menu, true);
        }
        if (addBlip)
        {
            AddBlip();
        }
        if (dispatchablePerson == null)
        {
            return;
        }
        dispatchablePerson.SetPedExtPermanentStats(this, Settings.SettingsManager.CivilianSettings.OverrideHealth, false, Settings.SettingsManager.CivilianSettings.OverrideAccuracy);//has a yield
        if (!Pedestrian.Exists())
        {
            return;
        }
        if (AssociatedStore != null)
        {
            WeaponInventory.IssueWeapons(weapons, forceMelee || RandomItems.RandomPercent(AssociatedStore.VendorMeleePercent), forceSidearm || RandomItems.RandomPercent(AssociatedStore.VendorSidearmPercent), forceLongGun || RandomItems.RandomPercent(AssociatedStore.VendorLongGunPercent), dispatchablePerson);
        }

        if (Pedestrian.Exists() && Settings.SettingsManager.CivilianSettings.SightDistance > 60f)
        {
            NativeFunction.Natives.SET_PED_SEEING_RANGE(Pedestrian, Settings.SettingsManager.CivilianSettings.SightDistance);
        }
    }
}


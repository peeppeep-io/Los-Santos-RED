﻿using LosSantosRED.lsr.Data;
using LosSantosRED.lsr.Interface;
using LSR.Vehicles;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System.Collections.Generic;
using System.Drawing;

public class SaveMenu : Menu
{
    private UIMenu Saves;
    private UIMenuItem SaveGameItem;
    private UIMenuListScrollerItem<GameSave> GameSaveMenuList;
    private ISaveable PlayerSave;
    private IWeapons Weapons;
    private IGameSaves GameSaves;
    private IPedSwap PedSwap;
    private IInventoryable PlayerInvetory;
    private ISettingsProvideable Settings;
    private IEntityProvideable World;
    private IGangs Gangs;
    private ITimeControllable Time;
    public SaveMenu(MenuPool menuPool, UIMenu parentMenu, ISaveable playersave, IGameSaves gameSaves, IWeapons weapons, IPedSwap pedSwap, IInventoryable playerinventory, ISettingsProvideable settings, IEntityProvideable world, IGangs gangs, ITimeControllable time)
    {
        PlayerSave = playersave;
        GameSaves = gameSaves;
        Weapons = weapons;
        PedSwap = pedSwap;
        PlayerInvetory = playerinventory;
        Settings = settings;
        World = world;
        Gangs = gangs;
        Time = time;
        Saves = menuPool.AddSubMenu(parentMenu, "Save/Load Player");
        Saves.SetBannerType(EntryPoint.LSRedColor);
        Saves.OnItemSelect += OnActionItemSelect;
        CreateSavesMenu();
    }
    public override void Hide()
    {
        Saves.Visible = false;
    }
    public override void Show()
    {
        Update();
        Saves.Visible = true;
    }
    public override void Toggle()
    {
        if (!Saves.Visible)
        {
            Saves.Visible = true;
        }
        else
        {
            Saves.Visible = false;
        }
    }
    public void Update()
    {
        CreateSavesMenu();
    }
    private void CreateSavesMenu()
    {
        Saves.Clear();
        GameSaveMenuList = new UIMenuListScrollerItem<GameSave>("Load Player", "Load the selected player", GameSaves.GameSaveList);
        SaveGameItem = new UIMenuItem("Save Player", "Save current player");
        Saves.AddItem(GameSaveMenuList);
        Saves.AddItem(SaveGameItem);

        GameSaveMenuList.Items = GameSaves.GameSaveList;//dont ask me why this is needed.....
    }
    private void OnActionItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (selectedItem == GameSaveMenuList)
        {
            GameSaves.Load(GameSaveMenuList.SelectedItem, Weapons, PedSwap, PlayerInvetory, Settings, World, Gangs, Time);
        }
        else if (selectedItem == SaveGameItem)
        {
            GameSaves.Save(PlayerSave, Weapons, Time);
        }
        Saves.Visible = false;
        GameSaveMenuList.Items = GameSaves.GameSaveList;//dont ask me why this is needed.....
    }
}
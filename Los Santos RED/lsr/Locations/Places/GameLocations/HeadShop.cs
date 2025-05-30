﻿using LosSantosRED.lsr.Interface;
using Rage;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

public class HeadShop : GameLocation
{
    public HeadShop() : base()
    {

    }
    public override string TypeName { get; set; } = "Head Shop";
    public override int MapIcon { get; set; } = 648;
    public override string ButtonPromptText { get; set; }
    public HeadShop(Vector3 _EntrancePosition, float _EntranceHeading, string _Name, string _Description, string menuID) : base(_EntrancePosition, _EntranceHeading, _Name, _Description)
    {
        MenuID = menuID;
    }
    public override bool CanCurrentlyInteract(ILocationInteractable player)
    {
        ButtonPromptText = $"Shop At {Name}";
        return true;
    }
    public override void AddLocation(PossibleLocations possibleLocations)
    {
        possibleLocations.HeadShops.Add(this);
        base.AddLocation(possibleLocations);
    }
}


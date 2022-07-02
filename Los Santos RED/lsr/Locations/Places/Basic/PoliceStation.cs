﻿using LosSantosRED.lsr.Interface;
using Rage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

public class PoliceStation : BasicLocation
{
    public PoliceStation(Vector3 _EntrancePosition, float _EntranceHeading, string _Name, string _Description) : base(_EntrancePosition, _EntranceHeading, _Name, _Description)
    {

    }
    public PoliceStation() : base()
    {

    }
    public override string TypeName { get; set; } = "Police Station";
    public override int MapIcon { get; set; } = (int)BlipSprite.PoliceStation;
    public override Color MapIconColor { get; set; } = Color.White;
    public override float MapIconScale { get; set; } = 1.0f;
    public List<ConditionalLocation> PossibleCopPedSpawns { get; set; }
    public List<ConditionalLocation> PossibleCopVehicleSpawns { get; set; }


    public string AssignedAgencyID { get; set; }

    [XmlIgnore]
    public Agency AssignedAgency { get; set; }

    [XmlIgnore]
    public bool IsDispatchFilled { get; set; } = false;

}


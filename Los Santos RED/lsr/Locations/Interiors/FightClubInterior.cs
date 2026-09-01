using Rage;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

public class FightClubInterior : Interior
{
    protected FightClub fightClub;
    public FightClub FightClub => fightClub;
    public List<FightClubInteract> FightClubInteracts { get; set; } = new List<FightClubInteract>();

    [XmlIgnore]
    public override List<InteriorInteract> AllInteractPoints
    {
        get
        {
            List<InteriorInteract> AllInteracts = new List<InteriorInteract>();
            AllInteracts.AddRange(InteractPoints);
            AllInteracts.AddRange(FightClubInteracts);
            return AllInteracts;
        }
    }
    public FightClubInterior()
    {

    }
    public FightClubInterior(int iD, string name) : base(iD, name)
    {

    }
    public void SetFightClub(FightClub newFightClub)
    {
        fightClub = newFightClub;
        foreach (FightClubInteract test in FightClubInteracts)
        {
            test.FightClub = newFightClub;
        }
    }
    protected override void LoadDoors(bool isOpen, bool reLockForcedEntry)
    {
        if (isOpen && FightClub != null && FightClub.IsAvailableForPlayer())
        {
            foreach (InteriorDoor door in Doors)
            {
                door.UnLockDoor();
            }
        }
        else
        {
            if (reLockForcedEntry)
            {
                foreach (InteriorDoor door in Doors.Where(x => x.LockWhenClosed))
                {
                    door.LockDoor();
                }
            }
            else
            {
                foreach (InteriorDoor door in Doors.Where(x => x.LockWhenClosed && !x.HasBeenForcedOpen))
                {
                    door.LockDoor();
                }
            }
        }
    }
    public override void AddDistanceOffset(Vector3 offsetToAdd)
    {
        foreach (FightClubInteract bdi in FightClubInteracts)
        {
            bdi.AddDistanceOffset(offsetToAdd);
        }
        base.AddDistanceOffset(offsetToAdd);
    }
    public override void AddLocation(PossibleInteriors interiorList)
    {
        interiorList.FightClubInteriors.Add(this);
    }
}
﻿using ExtensionsMethods;
using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;

public class LEDispatcher
{
    private readonly IAgencies Agencies;
    private readonly IDispatchable Player;
    private readonly int LikelyHoodOfAnySpawn = 5;
    private readonly int LikelyHoodOfCountySpawn = 10;
    private readonly float MinimumDeleteDistance = 150f;//200f
    private readonly uint MinimumExistingTime = 20000;
    private readonly ISettingsProvideable Settings;
    private readonly IStreets Streets;
    private readonly IEntityProvideable World;
    private readonly IJurisdictions Jurisdictions;
    private readonly IZones Zones;
    private bool HasDispatchedThisTick;
    private uint GameTimeAttemptedDispatch;
    private uint GameTimeAttemptedDispatchRoadblock;
    private uint GameTimeAttemptedRecall;
    private uint GameTimeLastSpawnedRoadblock;
    private Roadblock Roadblock;
    private Agency LastAgencySpawned;
    private IWeapons Weapons;
   // private int World.TotalWantedLevel;
    private bool TotalIsWanted;
    private INameProvideable Names;
    public LEDispatcher(IEntityProvideable world, IDispatchable player, IAgencies agencies, ISettingsProvideable settings, IStreets streets, IZones zones, IJurisdictions jurisdictions, IWeapons weapons, INameProvideable names)
    {
        Player = player;
        World = world;
        Agencies = agencies;
        Settings = settings;
        Streets = streets;
        Zones = zones;
        Jurisdictions = jurisdictions;
        Weapons = weapons;
        Names = names;
    }
    private float ClosestPoliceSpawnToOtherPoliceAllowed => TotalIsWanted ? 200f : 500f;
    private float ClosestPoliceSpawnToSuspectAllowed => TotalIsWanted ? 150f : 250f;
    private List<Cop> DeletableCops => World.Pedestrians.PoliceList.Where(x => (x.RecentlyUpdated && x.DistanceToPlayer >= MinimumDeleteDistance && x.HasBeenSpawnedFor >= MinimumExistingTime) || x.CanRemove).ToList();//NEED TO ADD WAS MOD SPAWNED HERE, LET THE REST OF THE FUCKERS MANAGE THEIR OWN STUFF?
    private float DistanceToDelete => TotalIsWanted ? 600f : 1000f;
    private float DistanceToDeleteOnFoot => TotalIsWanted ? 125f : 1000f;
    private bool HasNeedToDispatch => World.Pedestrians.TotalSpawnedPolice < SpawnedCopLimit && World.Vehicles.SpawnedPoliceVehiclesCount < SpawnedCopVehicleLimit;
    private bool HasNeedToDispatchRoadblock => Settings.SettingsManager.PoliceSettings.RoadblockEnabled && Player.WantedLevel >= Settings.SettingsManager.PoliceSettings.RoadblockMinWantedLevel && Player.WantedLevel <= Settings.SettingsManager.PoliceSettings.RoadblockMaxWantedLevel && Roadblock == null;//roadblocks are only for player
    private bool IsTimeToDispatch => Game.GameTime - GameTimeAttemptedDispatch >= TimeBetweenSpawn;
    private bool IsTimeToDispatchRoadblock => Game.GameTime - GameTimeLastSpawnedRoadblock >= TimeBetweenRoadblocks && Player.PoliceResponse.HasBeenAtCurrentWantedLevelFor >= 30000;
    private bool IsTimeToRecall => Game.GameTime - GameTimeAttemptedRecall >= TimeBetweenRecall;
    private float MaxDistanceToSpawn
    {
        get
        {
            if (TotalIsWanted)
            {
                if (!Player.AnyPoliceRecentlySeenPlayer)
                {
                    return Settings.SettingsManager.PoliceSettings.MaxDistanceToSpawn_WantedUnseen;
                }
                else
                {
                    return Settings.SettingsManager.PoliceSettings.MaxDistanceToSpawn_WantedSeen;
                }
            }
            else if (Player.Investigation.IsActive)
            {
                return Player.Investigation.Distance;
            }
            else
            {
                return Settings.SettingsManager.PoliceSettings.MaxDistanceToSpawn_NotWanted;
            }
        }
    }
    private float MinDistanceToSpawn
    {
        get
        {
            if (TotalIsWanted)
            {
                if (!Player.AnyPoliceRecentlySeenPlayer)
                {
                    return Settings.SettingsManager.PoliceSettings.MinDistanceToSpawn_WantedUnseen - (World.TotalWantedLevel * -40);
                }
                else
                {
                    return Settings.SettingsManager.PoliceSettings.MinDistanceToSpawn_WantedSeen - (World.TotalWantedLevel * -40);
                }
            }
            else if (Player.Investigation.IsActive)
            {
                return Player.Investigation.Distance / 2;
            }
            else
            {
                return Settings.SettingsManager.PoliceSettings.MinDistanceToSpawn_NotWanted;
            }
        }
    }
    private int SpawnedCopLimit
    {
        get
        {
            if (World.TotalWantedLevel == 6)
            {
                return Settings.SettingsManager.PoliceSettings.PedSpawnLimit_Wanted6;//35;//35
            }
            if (World.TotalWantedLevel == 5)
            {
                return Settings.SettingsManager.PoliceSettings.PedSpawnLimit_Wanted5;//35;//35
            }
            else if (World.TotalWantedLevel == 4)
            {
                return Settings.SettingsManager.PoliceSettings.PedSpawnLimit_Wanted4;//25;//25
            }
            else if (World.TotalWantedLevel == 3)
            {
                return Settings.SettingsManager.PoliceSettings.PedSpawnLimit_Wanted3;//18;//18
            }
            else if (World.TotalWantedLevel == 2)
            {
                return Settings.SettingsManager.PoliceSettings.PedSpawnLimit_Wanted2;//10;// 12;//10
            }
            else if (World.TotalWantedLevel == 1)
            {
                return Settings.SettingsManager.PoliceSettings.PedSpawnLimit_Wanted1;//7;// 10;//7
            }
            else if (Player.Investigation.IsActive)
            {
                return Settings.SettingsManager.PoliceSettings.PedSpawnLimit_Investigation;//6;// 9;//6
            }
            if (World.TotalWantedLevel == 0)
            {
                return Settings.SettingsManager.PoliceSettings.PedSpawnLimit_Default;//5;// 8;//5
            }
            else
            {
                return Settings.SettingsManager.PoliceSettings.PedSpawnLimit_Default;//5;//15
            }
        }
    }
    private int SpawnedHeliLimit
    {
        get
        {
            if (World.TotalWantedLevel == 6)
            {
                return 2;
            }
            if (World.TotalWantedLevel == 5)
            {
                return 2;
            }
            else if (World.TotalWantedLevel == 4)
            {
                return 1;
            }
            else if (World.TotalWantedLevel == 3)
            {
                return 1;
            }
            else if (World.TotalWantedLevel == 2)
            {
                return 0;
            }
            else if (World.TotalWantedLevel == 1)
            {
                return 0;
            }
            else if (Player.Investigation.IsActive)
            {
                return 0;
            }
            if (World.TotalWantedLevel == 0)
            {
                return 0;
            }
            else
            {
                return 0;
            }
        }
    }
    private int SpawnedBoatLimit
    {
        get
        {
            if (World.TotalWantedLevel == 6)
            {
                return 1;
            }
            if (World.TotalWantedLevel == 5)
            {
                return 1;
            }
            else if (World.TotalWantedLevel == 4)
            {
                return 1;
            }
            else if (World.TotalWantedLevel == 3)
            {
                return 1;
            }
            else if (World.TotalWantedLevel == 2)
            {
                return 0;
            }
            else if (World.TotalWantedLevel == 1)
            {
                return 0;
            }
            else if (Player.Investigation.IsActive)
            {
                return 0;
            }
            if (World.TotalWantedLevel == 0)
            {
                return 0;
            }
            else
            {
                return 0;
            }
        }
    }
    private int SpawnedCopVehicleLimit
    {
        get
        {
            if (World.TotalWantedLevel == 6)
            {
                return Settings.SettingsManager.PoliceSettings.VehicleSpawnLimit_Wanted6;//35;//35
            }
            if (World.TotalWantedLevel == 5)
            {
                return Settings.SettingsManager.PoliceSettings.VehicleSpawnLimit_Wanted5;//35;//35
            }
            else if (World.TotalWantedLevel == 4)
            {
                return Settings.SettingsManager.PoliceSettings.VehicleSpawnLimit_Wanted4;//25;//25
            }
            else if (World.TotalWantedLevel == 3)
            {
                return Settings.SettingsManager.PoliceSettings.VehicleSpawnLimit_Wanted3;//18;//18
            }
            else if (World.TotalWantedLevel == 2)
            {
                return Settings.SettingsManager.PoliceSettings.VehicleSpawnLimit_Wanted2;//10;// 12;//10
            }
            else if (World.TotalWantedLevel == 1)
            {
                return Settings.SettingsManager.PoliceSettings.VehicleSpawnLimit_Wanted1;//7;// 10;//7
            }
            else if (Player.Investigation.IsActive)
            {
                return Settings.SettingsManager.PoliceSettings.VehicleSpawnLimit_Investigation;//6;// 9;//6
            }
            if (World.TotalWantedLevel == 0)
            {
                return Settings.SettingsManager.PoliceSettings.VehicleSpawnLimit_Default;//5;// 8;//5
            }
            else
            {
                return Settings.SettingsManager.PoliceSettings.VehicleSpawnLimit_Default;//5;//15
            }
        }
    }
    private int TimeBetweenSpawn
    {
        get
        {
            if (!Player.AnyPoliceRecentlySeenPlayer)
            {
                return Settings.SettingsManager.PoliceSettings.TimeBetweenCopSpawn_Unseen;// 3000;
            }
            else
            {
                return ((6 - World.TotalWantedLevel) * Settings.SettingsManager.PoliceSettings.TimeBetweenCopSpawn_Seen_AdditionalTimeScaler) + Settings.SettingsManager.PoliceSettings.TimeBetweenCopSpawn_Seen_Min;//((5 - World.TotalWantedLevel) * Settings.SettingsManager.PoliceSettings.TimeBetweenCopSpawn_Seen_AdditionalTimeScaler) + Settings.SettingsManager.PoliceSettings.TimeBetweenCopSpawn_Seen_Min;//2000;
            }
        }
    }
    private int TimeBetweenRecall
    {
        get
        {
            if (!Player.AnyPoliceRecentlySeenPlayer)
            {
                return Settings.SettingsManager.PoliceSettings.TimeBetweenCopDespawn_Unseen;// 3000;
            }
            else
            {
                return ((6 - World.TotalWantedLevel) * Settings.SettingsManager.PoliceSettings.TimeBetweenCopDespawn_Seen_AdditionalTimeScaler) + Settings.SettingsManager.PoliceSettings.TimeBetweenCopDespawn_Seen_Min;//((5 - World.TotalWantedLevel) * Settings.SettingsManager.PoliceSettings.TimeBetweenCopSpawn_Seen_AdditionalTimeScaler) + Settings.SettingsManager.PoliceSettings.TimeBetweenCopSpawn_Seen_Min;//2000;
            }
        }
    }

    private int TimeBetweenRoadblocks
    {
        get
        {
            if (!Player.AnyPoliceRecentlySeenPlayer)
            {
                return Settings.SettingsManager.PoliceSettings.TimeBetweenRoadblock_Unseen;//999999;
            }
            else
            {
                return ((6 - World.TotalWantedLevel) * Settings.SettingsManager.PoliceSettings.TimeBetweenRoadblock_Seen_AdditionalTimeScaler) + Settings.SettingsManager.PoliceSettings.TimeBetweenRoadblock_Seen_Min;//90 seconds at level 3?, 70 at level 5? sounds okay?//((5 - World.TotalWantedLevel) * Settings.SettingsManager.PoliceSettings.TimeBetweenRoadblock_Seen_AdditionalTimeScaler) + Settings.SettingsManager.PoliceSettings.TimeBetweenRoadblock_Seen_Min;//90 seconds at level 3?, 70 at level 5? sounds okay?
            }
        }
    }
    public bool Dispatch()
    {
        HasDispatchedThisTick = false;
        TotalIsWanted = World.TotalWantedLevel > 0;

        if (Settings.SettingsManager.PoliceSettings.ManageDispatching && IsTimeToDispatch && HasNeedToDispatch)
        {
            HasDispatchedThisTick = true;//up here for now, might be better down low
            EntryPoint.WriteToConsole($"DISPATCHER: Attempting LE Spawn IsWanted: {TotalIsWanted} WantedLevel:{World.TotalWantedLevel}", 3);
            int timesTried = 0;
            bool isValidSpawn = false;
            SpawnLocation spawnLocation = new SpawnLocation();
            do
            {
                spawnLocation.InitialPosition = GetPositionAroundPlayer();
                spawnLocation.GetClosestStreet();
                spawnLocation.GetClosestSidewalk();
                GameFiber.Yield();
                isValidSpawn = IsValidSpawn(spawnLocation);
                timesTried++;
                GameFiber.Yield();
            }
            while (!spawnLocation.HasSpawns && !isValidSpawn && timesTried < 3);//2//10
            if (spawnLocation.HasSpawns && isValidSpawn)
            {
                Agency agency = GetRandomAgency(spawnLocation, ResponseType.LawEnforcement);
                GameFiber.Yield();
                if (agency != null)
                {
                    EntryPoint.WriteToConsole($"DISPATCHER: Attempting LE Spawn {agency.FullName}", 3);
                    LastAgencySpawned = agency;
                    DispatchableVehicle VehicleType = agency.GetRandomVehicle(World.TotalWantedLevel, World.Vehicles.PoliceHelicoptersCount < SpawnedHeliLimit, World.Vehicles.PoliceBoatsCount < SpawnedBoatLimit, true);//turned off for now as i work on the AI//World.PoliceHelicoptersCount < Settings.SettingsManager.Police.HelicopterLimit, World.PoliceBoatsCount < Settings.SettingsManager.Police.BoatLimit);
                    GameFiber.Yield();
                    bool ShouldSpawnPedestrian = false;
                    if(spawnLocation.HasSidewalk)
                    {
                       bool CanSpawnPedestrian = Jurisdictions.CanSpawnPedestrianAtZone(Zones.GetZoneName(spawnLocation.SidewalkPosition), agency.ID) && World.TotalWantedLevel <= 1;
                        if(CanSpawnPedestrian && RandomItems.RandomPercent(Settings.SettingsManager.PoliceSettings.PedestrianSpawnPercentage))
                        {
                            ShouldSpawnPedestrian = true;
                            VehicleType = null;
                            EntryPoint.WriteToConsole($"DISPATCHER: Attempting LE Spawn PEDESTRIAN", 3);
                        }
                        else
                        {
                            EntryPoint.WriteToConsole($"DISPATCHER: Attempting LE Spawn CanSpawnPedestrian {CanSpawnPedestrian} BUT IS NOT SPAWNING CUZ PERCENTAGES", 3);
                        }
                    }
                    if (VehicleType != null || ShouldSpawnPedestrian)
                    {
                        string RequiredGroup = "";
                        if (VehicleType != null)
                        {
                            RequiredGroup = VehicleType.RequiredPedGroup;
                        }
                        DispatchablePerson OfficerType = agency.GetRandomPed(World.TotalWantedLevel, RequiredGroup);
                        GameFiber.Yield();
                        if (OfficerType != null)
                        {
                            try
                            {
                                SpawnTask spawnTask = new SpawnTask(agency, spawnLocation, VehicleType, OfficerType, Settings.SettingsManager.PoliceSettings.ShowSpawnedBlips, Settings, Weapons, Names,RandomItems.RandomPercent(Settings.SettingsManager.PoliceSettings.AddOptionalPassengerPercentage), World);
                                spawnTask.AttemptSpawn();
                                GameFiber.Yield();
                                spawnTask.CreatedPeople.ForEach(x => World.Pedestrians.AddEntity(x));
                                spawnTask.CreatedVehicles.ForEach(x => World.Vehicles.AddEntity(x, ResponseType.LawEnforcement));
                                HasDispatchedThisTick = true;
                                Player.OnLawEnforcementSpawn(agency, VehicleType, OfficerType);
                            }
                            catch (Exception ex)
                            {
                                EntryPoint.WriteToConsole($"DISPATCHER: SpawnCop ERROR {ex.Message} : {ex.StackTrace}", 0);
                            }
                        }
                    }
                }
            }
            else
            {
                //EntryPoint.WriteToConsole($"DISPATCHER: Attempting to Spawn LE Failed, Has Spawns {spawnLocation.HasSpawns} Is Valid {isValidSpawn}", 3);
            }
            GameTimeAttemptedDispatch = Game.GameTime;
        }
        if (IsTimeToDispatchRoadblock && HasNeedToDispatchRoadblock)
        {
            GameFiber.Yield();
            SpawnRoadblock();
        }
        return HasDispatchedThisTick;
    }
    public void Dispose()
    {
        RemoveRoadblock();
    }
    public void Recall()
    {
        if (Settings.SettingsManager.PoliceSettings.ManageDispatching && IsTimeToRecall)
        {
            GameFiber.Yield();
            foreach (Cop DeleteableCop in DeletableCops)
            {
                if (ShouldCopBeRecalled(DeleteableCop))
                {
                    GameFiber.Yield();
                    Delete(DeleteableCop);
                    
                }
                GameFiber.Yield();
            }
            GameFiber.Yield();
            if (Roadblock != null && Player.Position.DistanceTo2D(Roadblock.CenterPosition) >= 550f)
            {
                Roadblock.Dispose();
                Roadblock = null;
                //EntryPoint.WriteToConsole($"DISPATCHER: Deleted Roadblock", 3);
            }
            GameTimeAttemptedRecall = Game.GameTime;
        }
    }
    private void Delete(PedExt Cop)
    {
        if (Cop != null && Cop.Pedestrian.Exists())
        {
            //EntryPoint.WriteToConsole($"Attempting to Delete {Cop.Pedestrian.Handle}");
            if (Cop.Pedestrian.IsInAnyVehicle(false))
            {
                if (Cop.Pedestrian.CurrentVehicle.HasPassengers)
                {
                    foreach (Ped Passenger in Cop.Pedestrian.CurrentVehicle.Passengers)
                    {
                        RemoveBlip(Passenger);
                        Passenger.Delete();
                        EntryPoint.PersistentPedsDeleted++;
                        GameFiber.Yield();
                    }
                }
                if (Cop.Pedestrian.Exists() && Cop.Pedestrian.CurrentVehicle.Exists() && Cop.Pedestrian.CurrentVehicle != null)
                {
                    Cop.Pedestrian.CurrentVehicle.Delete();
                    EntryPoint.PersistentVehiclesDeleted++;
                    GameFiber.Yield();
                }
            }
            RemoveBlip(Cop.Pedestrian);
            if (Cop.Pedestrian.Exists())
            {
                //EntryPoint.WriteToConsole(string.Format("Delete Cop Handle: {0}, {1}, {2}", Cop.Pedestrian.Handle, Cop.DistanceToPlayer, Cop.AssignedAgency.Initials));
                Cop.Pedestrian.Delete();
                EntryPoint.PersistentPedsDeleted++;
                GameFiber.Yield();
            }
        }
    }
    private void RemoveBlip(Ped MyPed)
    {
        if (!MyPed.Exists())
        {
            return;
        }
        Blip MyBlip = MyPed.GetAttachedBlip();
        if (MyBlip.Exists())
        {
            MyBlip.Delete();
        }
    }
    private List<Agency> GetAgencies(Vector3 Position, int WantedLevel, ResponseType responseType)
    {
        List<Agency> ToReturn = new List<Agency>();
        Street StreetAtPosition = Streets.GetStreet(Position);
        if (StreetAtPosition != null && StreetAtPosition.IsHighway) //Highway Patrol Jurisdiction
        {
            ToReturn.AddRange(Agencies.GetSpawnableHighwayAgencies(WantedLevel, responseType));
        }
        Zone CurrentZone = Zones.GetZone(Position);
        Agency ZoneAgency = Jurisdictions.GetRandomAgency(CurrentZone.InternalGameName, WantedLevel, responseType);
        if (ZoneAgency != null)
        {
            ToReturn.Add(ZoneAgency); //Zone Jurisdiciton Random
        }
        if (!ToReturn.Any() || RandomItems.RandomPercent(LikelyHoodOfAnySpawn))//fall back to anybody
        {
            ToReturn.AddRange(Agencies.GetSpawnableAgencies(WantedLevel, responseType));
        }
        if (!ToReturn.Any() || RandomItems.RandomPercent(LikelyHoodOfCountySpawn))
        {
            Agency CountyAgency = Jurisdictions.GetRandomAgency(CurrentZone.ZoneCounty, WantedLevel, ResponseType.LawEnforcement);
            if (CountyAgency != null)//randomly spawn the county agency
            {
                ToReturn.Add(CountyAgency); //Zone Jurisdiciton Random
            }
        }
        return ToReturn;
    }
    private Vector3 GetPositionAroundPlayer()
    {
        Vector3 Position;
        if (World.TotalWantedLevel > 0 && Player.IsInVehicle)
        {
            Position = Player.Character.GetOffsetPositionFront(250f);//350f
        }
        else if (Player.Investigation.IsActive)
        {
            Position = Player.Investigation.Position;
        }
        else
        {
            Position = Player.Position;
        }
        Position = Position.Around2D(MinDistanceToSpawn, MaxDistanceToSpawn);
        return Position;
    }
    private Agency GetRandomAgency(SpawnLocation spawnLocation, ResponseType responseType)
    {
        Agency agency;
        List<Agency> PossibleAgencies = GetAgencies(spawnLocation.StreetPosition, World.TotalWantedLevel, responseType);
        agency = PossibleAgencies.Where(x=>x.Personnel.Any(y =>y.CanCurrentlySpawn(World.TotalWantedLevel))).PickRandom();
        if (agency == null)
        {
            agency = GetAgencies(spawnLocation.InitialPosition, World.TotalWantedLevel,responseType).Where(x => x.Personnel.Any(y => y.CanCurrentlySpawn(World.TotalWantedLevel))).PickRandom();
        }
        if (agency == null)
        {
            EntryPoint.WriteToConsole("Dispatcher could not find Agency To Spawn");
        }
        return agency;
    }
    private Agency GetRandomAgency(Vector3 spawnLocation, ResponseType responseType)
    {
        Agency agency;
        List<Agency> PossibleAgencies = GetAgencies(spawnLocation, World.TotalWantedLevel, responseType);
        agency = PossibleAgencies.PickRandom();
        if (agency == null)
        {
            agency = GetAgencies(spawnLocation, World.TotalWantedLevel,responseType).PickRandom();
        }
        if (agency == null)
        {
            //EntryPoint.WriteToConsole("Dispatcher could not find Agency To Spawn");
        }
        return agency;
    }
    private bool IsValidSpawn(SpawnLocation spawnLocation)
    {
        if (spawnLocation.StreetPosition.DistanceTo2D(Player.Position) < ClosestPoliceSpawnToSuspectAllowed || World.Pedestrians.AnyCopsNearPosition(spawnLocation.StreetPosition, ClosestPoliceSpawnToOtherPoliceAllowed))
        {
            return false;
        }
        else if (spawnLocation.InitialPosition.DistanceTo2D(Player.Position) < ClosestPoliceSpawnToSuspectAllowed || World.Pedestrians.AnyCopsNearPosition(spawnLocation.InitialPosition, ClosestPoliceSpawnToOtherPoliceAllowed))
        {
            return false;
        }
        return true;
    }
    private bool ShouldCopBeRecalled(Cop cop)
    {
        if (!cop.AssignedAgency.CanSpawn(World.TotalWantedLevel))
        {
            EntryPoint.WriteToConsole($"DISPATCHER: Recalling Cop {cop.Pedestrian.Handle} Reason: Agency Can Not Spawn",5);
            return true;
        }
        else if (cop.IsInVehicle && cop.DistanceToPlayer > DistanceToDelete) //Beyond Caring
        {
            EntryPoint.WriteToConsole($"DISPATCHER: Recalling Cop {cop.Pedestrian.Handle} Reason: Beyond Distance (Vehicle) IS: {cop.DistanceToPlayer} REQ: {DistanceToDelete}",5);
            return true;
        }
        else if (!cop.IsInVehicle && cop.DistanceToPlayer > DistanceToDeleteOnFoot) //Beyond Caring
        {
            EntryPoint.WriteToConsole($"DISPATCHER: Recalling Cop {cop.Pedestrian.Handle} Reason: Beyond Distance (Foot) IS: {cop.DistanceToPlayer} REQ: {DistanceToDeleteOnFoot}",5);
            return true;
        }
        else if (cop.DistanceToPlayer >= 300f && cop.ClosestDistanceToPlayer <= 15f && !cop.IsInHelicopter) //Got Close and Then got away
        {
            EntryPoint.WriteToConsole($"DISPATCHER: Recalling Cop {cop.Pedestrian.Handle} Reason: Was Close IS: {cop.DistanceToPlayer} REQ: {DistanceToDelete}",5);
            return true;
        }

        else if (!cop.IsInHelicopter && cop.DistanceToPlayer >= 150f && cop.ClosestDistanceToPlayer <= 35f && World.Pedestrians.AnyCopsNearCop(cop,3) && !cop.Pedestrian.IsOnScreen)
        {
            EntryPoint.WriteToConsole($"DISPATCHER: Recalling Cop {cop.Pedestrian.Handle} Reason: World.AnyCopsNearCop IS: {cop.DistanceToPlayer} REQ: {DistanceToDelete}", 5);
            return true;
        }

        //else if (World.CountNearbyPolice(cop.Pedestrian) >= 3 && cop.TimeBehindPlayer >= 15000) //Got Close and Then got away
        //{
        //    //EntryPoint.WriteToConsole($"DISPATCHER: Recalling Cop {cop.Pedestrian.Handle} Reason: Behind Player Around Others");
        //    return true;
        //}
        return false;
    }
    public void SpawnRoadblock()//temp public
    {
        Vector3 Position = Player.Character.GetOffsetPositionFront(350f);//400f 400 is mostly far enough to not see it
        Street ForwardStreet = Streets.GetStreet(Position);
        GameFiber.Yield();
        if (ForwardStreet?.Name == Player.CurrentLocation.CurrentStreet?.Name)
        {      
            if (NativeFunction.Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(Position.X, Position.Y, Position.Z, out Vector3 CenterPosition, out float Heading, 0, 3.0f, 0))
            {
                Agency ToSpawn = GetRandomAgency(CenterPosition, ResponseType.LawEnforcement);
                GameFiber.Yield();
                if (ToSpawn != null)
                {

                    DispatchableVehicle VehicleToUse = ToSpawn.GetRandomVehicle(World.TotalWantedLevel, false, false, false);
                    GameFiber.Yield();
                    if (VehicleToUse != null)
                    {
                        string RequiredGroup = "";
                        if (VehicleToUse != null)
                        {
                            RequiredGroup = VehicleToUse.RequiredPedGroup;
                        }
                        DispatchablePerson OfficerType = ToSpawn.GetRandomPed(World.TotalWantedLevel, RequiredGroup);
                        GameFiber.Yield();
                        if (OfficerType != null)
                        {
                            if (Roadblock != null)
                            {
                                Roadblock.Dispose();
                                GameFiber.Yield();
                            }
                            Roadblock = new Roadblock(Player, World, ToSpawn, VehicleToUse, OfficerType, CenterPosition, Settings, Weapons, Names);
                            Roadblock.SpawnRoadblock();
                            GameFiber.Yield();
                            GameTimeLastSpawnedRoadblock = Game.GameTime;
                        }
                    }
                }
            }
        }
    }
    public void RemoveRoadblock()//temp public
    {
        if (Roadblock != null)
        {
            Roadblock.Dispose();
            Roadblock = null;
        }
    }
    public void DebugSpawnCop(string agencyID, bool onFoot)
    {
        EntryPoint.WriteToConsole($"DebugSpawnCop  1 agencyID {agencyID} onFoot {onFoot}");
        int timesTried = 0;
        bool isValidSpawn = false;
        SpawnLocation spawnLocation = new SpawnLocation();
        do
        {
            spawnLocation.InitialPosition = Game.LocalPlayer.Character.GetOffsetPositionFront(10f);//GetPositionAroundPlayer();
            spawnLocation.StreetPosition = spawnLocation.InitialPosition;
            isValidSpawn = true;// IsValidSpawn(spawnLocation);
            timesTried++;
            GameFiber.Yield();
        }
        while (!spawnLocation.HasSpawns && !isValidSpawn && timesTried < 1);//2//10
        if (spawnLocation.HasSpawns && isValidSpawn)
        {
            Agency agency = null;
            if (agencyID == "")
            {
                EntryPoint.WriteToConsole($"DebugSpawnCop  1.5 agencyID {agencyID} onFoot {onFoot}");
                agency = Agencies.GetRandomAgency(ResponseType.LawEnforcement);
            }
            else
            {
                EntryPoint.WriteToConsole($"DebugSpawnCop  1.6 agencyID {agencyID} onFoot {onFoot}");
                agency = Agencies.GetAgency(agencyID);//
            }
            GameFiber.Yield();
            if (agency != null)
            {
                EntryPoint.WriteToConsole($"DebugSpawnCop  2 agency {agency.FullName} onFoot {onFoot}");
                LastAgencySpawned = agency;

                DispatchableVehicle VehicleType = null;
                if (!onFoot)
                {
                    VehicleType = agency.GetRandomVehicle(World.TotalWantedLevel, World.Vehicles.PoliceHelicoptersCount < SpawnedHeliLimit, World.Vehicles.PoliceBoatsCount < SpawnedBoatLimit, true);//turned off for now as i work on the AI//World.PoliceHelicoptersCount < Settings.SettingsManager.Police.HelicopterLimit, World.PoliceBoatsCount < Settings.SettingsManager.Police.BoatLimit);
                }
                GameFiber.Yield();
                if (VehicleType != null || onFoot)
                {
                    EntryPoint.WriteToConsole($"DebugSpawnCop  3 agency {agency.FullName} onFoot {onFoot}");

                    string RequiredGroup = "";
                    if(VehicleType != null)
                    {
                        RequiredGroup = VehicleType.RequiredPedGroup;
                    }
                    DispatchablePerson OfficerType = agency.GetRandomPed(World.TotalWantedLevel, RequiredGroup);
                    GameFiber.Yield();
                    if (OfficerType != null)
                    {
                        try
                        {
                            SpawnTask spawnTask = new SpawnTask(agency, spawnLocation, VehicleType, OfficerType, Settings.SettingsManager.PoliceSettings.ShowSpawnedBlips, Settings, Weapons, Names, true, World);
                            spawnTask.AllowAnySpawn = true;
                            spawnTask.AttemptSpawn();
                            GameFiber.Yield();
                            spawnTask.CreatedPeople.ForEach(x => World.Pedestrians.AddEntity(x));
                            spawnTask.CreatedVehicles.ForEach(x => World.Vehicles.AddEntity(x, ResponseType.LawEnforcement));
                            HasDispatchedThisTick = true;
                            Player.OnLawEnforcementSpawn(agency, VehicleType, OfficerType);
                        }
                        catch (Exception ex)
                        {
                            EntryPoint.WriteToConsole($"DISPATCHER: SpawnCop ERROR {ex.Message} : {ex.StackTrace}", 0);
                        }
                    }
                }
            }
        }
    }
}
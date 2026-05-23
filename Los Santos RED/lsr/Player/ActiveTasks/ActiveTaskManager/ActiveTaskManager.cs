using ExtensionsMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Player.ActiveTasks.ActiveTaskManager
{
    public class ActiveTaskManager
    {
        public GangTaskRegistry GangTaskRegistry { get; private set; }
        public PlayerTasks PlayerTasks { get; set; }
        public ActiveTaskManager(PlayerTasks playerTasks)
        {
            PlayerTasks = playerTasks;
        }
        public void Setup()
        {
            GangTaskRegistry = new GangTaskRegistry(PlayerTasks);
            GangTaskRegistry.Setup();
        }
        public KeyValuePair<string, Action<Gang>> GetGangJob(Gang gang)
        {
            KeyValuePair<string,Action<Gang>> kv = GangTaskRegistry.GetGangJob(gang);
            return kv;
        }
    }
    public class GangTaskRegistry
    {
        private PlayerTasks PlayerTasks;

        public GangTaskRegistry(PlayerTasks playerTasks)
        {
            PlayerTasks = playerTasks;
        }
        Dictionary<string, Action<Gang>> PossibleGangMissions = new Dictionary<string, Action<Gang>>();
        public void Setup()
        {
            PossibleGangMissions.Add("Collect protection money.", StartRacketeering);
            //PossibleGangMissions.Add("I need a cop taken out", StartCopHit);
            //PossibleGangMissions.Add("Need you to hit a gang.", StartGangHit);
            //PossibleGangMissions.Add("Get us a pizza.", StartPizzaMission);
            //PossibleGangMissions.Add("Need a getaway driver.", StartGangWheelman);
        }
        public KeyValuePair<string,Action<Gang>> GetGangJob(Gang gang)
        {
            KeyValuePair<string, Action<Gang>> kv = PossibleGangMissions.PickRandom();
            return kv;
        }
        public void StartRacketeering(Gang gang)
        {
            GangRacketeeringTask newTask = new GangRacketeeringTask(PlayerTasks.Player, PlayerTasks.Gangs, PlayerTasks, PlayerTasks.PlacesOfInterest, PlayerTasks.Settings, PlayerTasks.World, PlayerTasks.Crimes, gang.Contact, PlayerTasks.GangTasks, PlayerTasks.GangTasks.GangTerritories, PlayerTasks.GangTasks.Zones);
            PlayerTasks.GangTasks.GangRacketeeringTasks.Add(newTask);
            newTask.Setup();
            newTask.Start(gang);
        }
        public void StartCopHit(Gang gang)
        {
            Agency targetAgency = PlayerTasks.Agencies.GetRandomAgency(ResponseType.LawEnforcement);
            int killRequirement = RandomItems.GetRandomNumberInt(1, 3);
            GangCopHitTask newTask = new GangCopHitTask(PlayerTasks.Player, PlayerTasks.Time, PlayerTasks.Gangs, PlayerTasks.PlacesOfInterest, PlayerTasks.Settings, PlayerTasks.World, PlayerTasks.Crimes, PlayerTasks.Weapons, PlayerTasks.Names, PlayerTasks.PedGroups, PlayerTasks.ShopMenus, PlayerTasks.ModItems, PlayerTasks, PlayerTasks.GangTasks, gang.Contact, gang, targetAgency, PlayerTasks.Agencies, killRequirement);
            PlayerTasks.GangTasks.AllGenericGangTasks.Add(newTask);
            newTask.Setup();
            newTask.Start();
        }
        public void StartGangHit(Gang gang)
        {
            int killRequirement = RandomItems.GetRandomNumberInt(1, 3);
            Gang targetGang = PlayerTasks.Gangs.GetGang(gang.EnemyGangs.PickRandom());
            RivalGangHitTask newTask = new RivalGangHitTask(PlayerTasks.Player, PlayerTasks.Time, PlayerTasks.Gangs, PlayerTasks, PlayerTasks.PlacesOfInterest, PlayerTasks.ActiveDrops, PlayerTasks.Settings, PlayerTasks.World, PlayerTasks.Crimes, gang.Contact, PlayerTasks.GangTasks, targetGang, killRequirement);
            PlayerTasks.GangTasks.RivalGangHits.Add(newTask);
            newTask.Setup();
            newTask.Start(gang);
        }
        public void StartPizzaMission(Gang gang)
        {
            GangPizzaDeliveryTask newDelivery = new GangPizzaDeliveryTask(PlayerTasks.Player, PlayerTasks.Time, PlayerTasks.Gangs, PlayerTasks, PlayerTasks.PlacesOfInterest, PlayerTasks.ActiveDrops, PlayerTasks.Settings, PlayerTasks.World, PlayerTasks.Crimes, PlayerTasks.ModItems, PlayerTasks.ShopMenus, gang.Contact, PlayerTasks.GangTasks);
            PlayerTasks.GangTasks.GangPizzaDeliveryTasks.Add(newDelivery);
            newDelivery.Setup();
            newDelivery.Start(gang);
        }
        //public void StartGangAmbush(Gang gang, int killRequirement, GangContact gangContact, Gang targetGang)
        //{
        //    RivalGangAmbushTask newTask = new RivalGangAmbushTask(Player, Time, Gangs, PlayerTasks, PlacesOfInterest, ActiveDrops, Settings, World, Crimes, gangContact, this, targetGang, killRequirement, GangTerritories, Zones);
        //    RivalGangAmbush.Add(newTask);
        //    newTask.Setup();
        //    newTask.Start(gang);
        //}
        //public void StartPayoffGang(Gang gang, GangContact gangContact)
        //{
        //    PayoffGangTask newTask = new PayoffGangTask(Player, Time, Gangs, PlayerTasks, PlacesOfInterest, ActiveDrops, Settings, World, Crimes, gangContact, this);
        //    PayoffGangTasks.Add(newTask);
        //    newTask.Setup();
        //    newTask.Start(gang);
        //}
        //public void StartGangVehicleTheft(Gang gang, GangContact gangContact, Gang targetGang, string vehicleModelName, string vehicleDisplayName)
        //{
        //    RivalGangVehicleTheftTask newTask = new RivalGangVehicleTheftTask(Player, Time, Gangs, PlayerTasks, PlacesOfInterest, ActiveDrops, Settings, World, Crimes, gangContact, this, targetGang, vehicleModelName, vehicleDisplayName);
        //    RivalGangTheftTasks.Add(newTask);
        //    newTask.Setup();
        //    newTask.Start(gang);
        //}
        //public void StartGangBribery(Gang gang, GangContact gangContact)
        //{
        //    GangBriberyTask newTask = new GangBriberyTask(Player, Gangs, PlayerTasks, PlacesOfInterest, Settings, World, Crimes, gangContact, this, GangTerritories, Zones);
        //    GangBriberyTasks.Add(newTask);
        //    newTask.Setup();
        //    newTask.Start(gang);
        //}
        //public void StartGangArson(Gang gang, GangContact gangContact)
        //{
        //    GangArsonTask newTask = new GangArsonTask(Player, Time, Gangs, PlayerTasks, PlacesOfInterest, ActiveDrops, Settings, World, Crimes, gangContact, this, GangTerritories, Zones);
        //    GangArsonTasks.Add(newTask);
        //    newTask.Setup();
        //    newTask.Start(gang);
        //}
        //public void StartGangPickup(Gang gang, GangContact gangContact)
        //{
        //    GangPickupTask newTask = new GangPickupTask(Player, Time, Gangs, PlayerTasks, PlacesOfInterest, ActiveDrops, Settings, World, Crimes, gangContact, this);
        //    GangPickupTasks.Add(newTask);
        //    newTask.Setup();
        //    newTask.Start(gang);
        //}
        //public void StartGangDelivery(Gang gang, GangContact gangContact, string modItemName)
        //{
        //    GangDeliveryTask newTask = new GangDeliveryTask(Player, Time, Gangs, PlayerTasks, PlacesOfInterest, ActiveDrops, Settings, World, Crimes, ModItems, ShopMenus, gangContact, this, modItemName);
        //    GangDeliveryTasks.Add(newTask);
        //    newTask.Setup();
        //    newTask.Start(gang);
        //}
        public void StartGangWheelman(Gang gang)
        {
            int robbersToSpawn = RandomItems.GetRandomNumberInt(1, 3);
            string locationType = "Random";
            bool requireAllMembersToFinish = RandomItems.RandomPercent(50);
            GangWheelmanTask newTask = new GangWheelmanTask(PlayerTasks.Player, PlayerTasks.Time, PlayerTasks.Gangs, PlayerTasks, PlayerTasks.PlacesOfInterest, PlayerTasks.ActiveDrops, PlayerTasks.Settings, PlayerTasks.World, PlayerTasks.Crimes, PlayerTasks.Weapons, PlayerTasks.Names, PlayerTasks.PedGroups, PlayerTasks.ShopMenus, PlayerTasks.ModItems, gang.Contact, PlayerTasks.GangTasks, robbersToSpawn, locationType, requireAllMembersToFinish);
            PlayerTasks.GangTasks.GangWheelmanTasks.Add(newTask);
            newTask.Setup();
            newTask.Start(gang);
        }

        //public void StartImpoundTheft(Gang gang, GangContact gangContact)
        //{
        //    GangGetCarOutOfImpoundTask newTask = new GangGetCarOutOfImpoundTask(Player, Time, Gangs, PlayerTasks, PlacesOfInterest, Settings, World, Crimes, Weapons, Names, PedGroups, ShopMenus, ModItems, this, gangContact);
        //    GangGetCarOutOfImpoundTasks.Add(newTask);
        //    newTask.Setup();
        //    newTask.Start(gang);
        //}
        //public void StartGangBodyDisposal(Gang gang)
        //{
        //    GangBodyDisposalTask newTask = new GangBodyDisposalTask(PlayerTasks.Player, PlayerTasks.Time, PlayerTasks.Gangs, PlayerTasks.PlacesOfInterest, PlayerTasks.Settings, PlayerTasks.World, PlayerTasks.Crimes, PlayerTasks.Weapons, PlayerTasks.Names, PlayerTasks.PedGroups, PlayerTasks.ShopMenus, PlayerTasks.ModItems, PlayerTasks, PlayerTasks.GangTasks, gang.Contact, gang);
        //    PlayerTasks.GangTasks.AllGenericGangTasks.Add(newTask);
        //    newTask.Setup();
        //    newTask.Start();
        //}
    }
}

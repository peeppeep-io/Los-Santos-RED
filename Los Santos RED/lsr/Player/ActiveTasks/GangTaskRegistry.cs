using ExtensionsMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Player.ActiveTasks
{
    public class GangTaskRegistry
    {
        private GangTasks GangTasks;

        public GangTaskRegistry(GangTasks gangTasks)
        {
            GangTasks = gangTasks;
        }
        Dictionary<string, Func<Gang, GangJobHelper>> PossibleGangMissions = new Dictionary<string, Func<Gang, GangJobHelper>>();
        public void Setup()
        {
            PossibleGangMissions.Add("Collect protection money.", GangTasks.StartGangRacketeering);
            PossibleGangMissions.Add("I need a cop taken out", GangTasks.StartCopHit);
            PossibleGangMissions.Add("Need you to hit a gang.", GangTasks.StartGangHit);
            PossibleGangMissions.Add("Get us a pizza.", GangTasks.StartGangPizza);
            PossibleGangMissions.Add("Need a getaway driver.", GangTasks.StartGangWheelman);
            PossibleGangMissions.Add("Need a vehicle stolen from the impound.", GangTasks.StartImpoundTheft);
            PossibleGangMissions.Add("Need 'something' disposed of.", GangTasks.StartGangBodyDisposal);
            PossibleGangMissions.Add("Need you to ambush a gang.", GangTasks.StartGangAmbush);
            PossibleGangMissions.Add("Steal a rival's vehicle.", GangTasks.StartGangVehicleTheft);
            PossibleGangMissions.Add("Need you to bribe someone.", GangTasks.StartGangBribery);
            PossibleGangMissions.Add("Need you to light up something for us.", GangTasks.StartGangArson);
            PossibleGangMissions.Add("We need to make a few pick ups.", GangTasks.StartGangPickup);
            PossibleGangMissions.Add("Need you to get something for us.", GangTasks.StartGangDelivery);


        }
        public KeyValuePair<string, Func<Gang, GangJobHelper>> GetGangJob(Gang gang)
        {
            KeyValuePair<string, Func<Gang, GangJobHelper>> kv = PossibleGangMissions.PickRandom();
            return kv;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Plugins
{
    public class LSRPlugin : ILSRPlugin
    {
        public string Author { get; set; }
        public uint Interval { get; set; }
        public string DebugName { get; set; }

        public virtual void EntryPoint(ModController controller)
        {
            throw new NotImplementedException();
        }
    }
}

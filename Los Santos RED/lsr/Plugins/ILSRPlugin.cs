using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Plugins
{
    public interface ILSRPlugin
    {
        void EntryPoint(ModController controller);
        uint Interval { get; set; }
        string DebugName { get; set; }
    }
}

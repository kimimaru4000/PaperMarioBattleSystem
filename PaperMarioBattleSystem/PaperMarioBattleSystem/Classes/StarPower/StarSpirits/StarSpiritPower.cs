using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Star Power granted by the Star Spirits that can be used by Mario.
    /// </summary>
    public sealed class StarSpiritPower : StarPowerBase
    {
        public StarSpiritPower()
        {
            StarPowerType = StarPowerGlobals.StarPowerTypes.StarSpirit;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Thunder Rage item. It deals 5 Electric damage to all enemies.
    /// </summary>
    public sealed class ThunderRage : ThunderBolt
    {
        public ThunderRage()
        {
            Name = "Thunder Rage";
            Description = "Deals 5 HP of damage to all enemies.";

            Icon.SetRect(new Rectangle(403, 136, 25, 25));

            SelectionType = Enumerations.EntitySelectionType.All;
        }
    }
}

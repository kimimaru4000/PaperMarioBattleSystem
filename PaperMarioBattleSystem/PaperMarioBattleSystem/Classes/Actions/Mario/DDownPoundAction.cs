using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The action for D-Down Pound.
    /// </summary>
    public sealed class DDownPoundAction : Hammer
    {
        public DDownPoundAction()
        {
            Name = "D-Down Pound";

            //Set other data
            MoveInfo.Icon.SetRect(new Rectangle(907, 193, 24, 24));
            MoveInfo.Description = "Pierces enemy defense";

            MoveInfo.ResourceCost = 2;

            DamageInfo.Piercing = true;
        }
    }
}

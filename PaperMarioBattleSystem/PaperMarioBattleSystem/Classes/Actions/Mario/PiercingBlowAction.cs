using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The action for Piercing Blow.
    /// </summary>
    public sealed class PiercingBlowAction : HammerAction
    {
        public PiercingBlowAction()
        {
            Name = "Piercing Blow";

            //Set other data
            MoveInfo.Icon.SetRect(new Rectangle(891, 475, 39, 40));
            MoveInfo.Description = "Pierces enemy defense";

            MoveInfo.ResourceCost = 2;

            DamageInfo.Piercing = true;
        }
    }
}

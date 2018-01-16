using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Submenu for Watt's Abilities.
    /// </summary>
    public sealed class WattSubMenu : ActionSubMenu
    {
        public WattSubMenu()
        {
            Position = new Vector2(210, 150);
            BattleActions = new List<MoveAction>() { new ElectroDashAction() };
        }
    }
}

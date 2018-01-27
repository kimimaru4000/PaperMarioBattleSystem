using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Submenu for Kooper's Abilities.
    /// </summary>
    public sealed class KooperSubMenu : ActionSubMenu
    {
        public KooperSubMenu()
        {
            Name = "Abilities";
            Position = new Vector2(210, 150);
            BattleActions = new List<MoveAction>() { new ShellToss(), new ShellShieldAction() };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Submenu for Bow's Abilities.
    /// </summary>
    public sealed class BowSubMenu : ActionSubMenu
    {
        public BowSubMenu()
        {
            Position = new Vector2(210, 150);
            BattleActions = new List<MoveAction>() { new OuttaSight(), new Veil() };
        }
    }
}

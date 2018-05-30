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
        public KooperSubMenu(BattleEntity user) : base(user)
        {
            Name = "Abilities";
            Position = new Vector2(210, 150);
            BattleActions = new List<MoveAction>() { new ShellTossAction(User), new ShellShieldAction(User) };
        }
    }
}

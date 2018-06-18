using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A SubMenu for Bow's Abilities.
    /// </summary>
    public sealed class BowSubMenu : ActionSubMenu
    {
        public BowSubMenu(BattleEntity user) : base(user)
        {
            Name = "Abilities";
            Position = new Vector2(210, 150);
            BattleActions = new List<MoveAction>() { new OuttaSightAction(User), new VeilAction(User) };
        }
    }
}

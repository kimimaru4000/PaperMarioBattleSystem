using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A SubMenu for Goombario's Abilities.
    /// </summary>
    public class GoombarioSubMenu : ActionSubMenu
    {
        public GoombarioSubMenu(BattleEntity user) : base(user)
        {
            Name = "Abilities";
            Position = new Vector2(210, 150);
            BattleActions = new List<MoveAction> { new BonkAction(User), new TattleAction(User, true), new TidalWaveAction(User), new GulpAction(User), new MiniEggAction(User), new BombSquadAction(User, 3, 3), new RallyWinkAction(User), new AirLiftAction(User) };
        }
    }
}

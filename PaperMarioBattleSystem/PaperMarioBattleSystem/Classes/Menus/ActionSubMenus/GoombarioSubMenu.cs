using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    public class GoombarioSubMenu : ActionSubMenu
    {
        public GoombarioSubMenu()
        {
            Name = "Abilities";
            Position = new Vector2(210, 150);
            BattleActions = new List<MoveAction> { new BonkAction(), new TattleAction(true), new TidalWaveAction(), new GulpAction(), new MiniEggAction(), new BombSquadAction(3, 3), new RallyWinkAction(), new AirLiftAction() };
        }
    }
}

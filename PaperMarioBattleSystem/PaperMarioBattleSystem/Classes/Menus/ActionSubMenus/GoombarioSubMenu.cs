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
            Position = new Vector2(210, 150);
            BattleActions = new List<MoveAction> { new Bonk(), new Tattle(true), new TidalWave(), new Gulp(), new MiniEgg() };
        }
    }
}

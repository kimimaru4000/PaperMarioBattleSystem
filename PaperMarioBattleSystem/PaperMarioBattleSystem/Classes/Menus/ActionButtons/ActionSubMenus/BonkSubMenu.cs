using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    public class BonkSubMenu : ActionSubMenu
    {
        public BonkSubMenu()
        {
            Position = new Vector2(210, 150);
            Initialize(new List<BattleAction> { new Bonk(), new TidalWave() });
        }
    }
}

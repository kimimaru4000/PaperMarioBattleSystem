using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Jump submenu for all Jump attacks
    /// </summary>
    public class JumpSubMenu : ActionSubMenu
    {
        public JumpSubMenu()
        {
            Position = new Vector2(230, 150);
            Initialize(new List<BattleAction>() { new Jump(), new PowerBounce(), new Multibounce() });
        }
    }
}

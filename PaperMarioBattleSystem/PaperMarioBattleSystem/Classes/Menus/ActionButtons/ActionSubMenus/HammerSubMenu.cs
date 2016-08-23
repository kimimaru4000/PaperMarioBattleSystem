using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The SubMenu for all Hammer attacks
    /// </summary>
    public class HammerSubMenu : ActionSubMenu
    {
        public HammerSubMenu()
        {
            Position = new Vector2(230, 150);
            Initialize(new List<MoveAction>() { new Hammer() });
        }
    }
}

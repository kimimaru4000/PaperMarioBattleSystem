using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Jump submenu for all Jump attacks
    /// </summary>
    public class JumpSubMenu : ActionSubMenu
    {
        public JumpSubMenu()
        {
            BattleActions.Add(new Jump());
        }
    }
}

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
            AutoSelectSingle = true;

            BattleActions.Add(new Hammer());
            if (BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.IceSmash) > 0)
            {
                BattleActions.Add(new IceSmash());
            }
            if (BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.HeadRattle) > 0)
            {
                BattleActions.Add(new HeadRattle());
            }
        }
    }
}

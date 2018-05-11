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
            Name = "Hammer";
            Position = new Vector2(230, 150);
            AutoSelectSingle = true;

            BattleActions.Add(new HammerAction());

            int powerSmashCount = BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.PowerSmash);
            if (powerSmashCount > 0)
            {
                BattleActions.Add(new PowerSmashAction(powerSmashCount));
            }
            if (BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.MegaSmash) > 0)
            {
                BattleActions.Add(new MegaSmashAction());
            }
            if (BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.PiercingBlow) > 0)
            {
                BattleActions.Add(new PiercingBlowAction());
            }
            if (BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.HeadRattle) > 0)
            {
                BattleActions.Add(new HeadRattleAction());
            }
            if (BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.IceSmash) > 0)
            {
                BattleActions.Add(new IceSmashAction());
            }
            if (BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.DDownPound) > 0)
            {
                BattleActions.Add(new DDownPoundAction());
            }

            int quakeCount = BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.QuakeHammer);
            if (quakeCount > 0)
            {
                BattleActions.Add(new QuakeHammerAction(quakeCount));
            }
        }
    }
}

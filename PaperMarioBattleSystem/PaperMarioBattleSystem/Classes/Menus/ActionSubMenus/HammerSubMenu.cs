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
        public HammerSubMenu(BattleEntity user) : base(user)
        {
            Name = "Hammer";
            Position = new Vector2(230, 150);
            AutoSelectSingle = true;

            BattleActions.Add(new HammerAction(User));

            int powerSmashCount = User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.PowerSmash);
            if (powerSmashCount > 0)
            {
                BattleActions.Add(new PowerSmashAction(User, powerSmashCount));
            }
            if (User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.MegaSmash) > 0)
            {
                BattleActions.Add(new MegaSmashAction(User));
            }
            if (User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.PiercingBlow) > 0)
            {
                BattleActions.Add(new PiercingBlowAction(User));
            }
            if (User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.HeadRattle) > 0)
            {
                BattleActions.Add(new HeadRattleAction(User));
            }
            if (User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.IceSmash) > 0)
            {
                BattleActions.Add(new IceSmashAction(User));
            }
            if (User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.DDownPound) > 0)
            {
                BattleActions.Add(new DDownPoundAction(User));
            }

            int quakeCount = User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.QuakeHammer);
            if (quakeCount > 0)
            {
                BattleActions.Add(new QuakeHammerAction(User, quakeCount));
            }
        }
    }
}

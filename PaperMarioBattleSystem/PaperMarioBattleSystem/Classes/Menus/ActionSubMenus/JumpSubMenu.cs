using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Jump submenu for all Jump attacks.
    /// </summary>
    public class JumpSubMenu : ActionSubMenu
    {
        public JumpSubMenu(BattleEntity user) : base(user)
        {
            Name = "Jump";
            Position = new Vector2(230, 150);
            AutoSelectSingle = true;

            BattleActions.Add(new JumpAction(User));
            if (User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.PowerBounce) > 0)
            {
                BattleActions.Add(new PowerBounceAction(User));
            }
            if (User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.Multibounce) > 0)
            {
                BattleActions.Add(new MultibounceAction(User));
            }
            if (User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.TornadoJump) > 0)
            {
                BattleActions.Add(new TornadoJumpAction(User));
            }
            if (User.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.DDownJump) > 0)
            {
                BattleActions.Add(new DDownJumpAction(User));
            }
        }
    }
}

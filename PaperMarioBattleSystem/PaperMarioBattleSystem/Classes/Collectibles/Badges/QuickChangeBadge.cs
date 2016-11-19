using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Quick Change Badge - Allows Mario or his Partner to switch Partners without using up a turn
    /// </summary>
    public sealed class QuickChangeBadge : Badge
    {
        public QuickChangeBadge()
        {
            Name = "Quick Change";
            Description = "During battle, lets you change your party members... and still use the new member without losing a turn.";

            BPCost = 7;

            BadgeType = BadgeGlobals.BadgeTypes.QuickChange;
            AffectedType = BadgeGlobals.AffectedTypes.Both;
        }

        protected override void OnEquip()
        {
            int count = BattlePlayer.PlayerProperties.GetProperty<int>(BattlePlayerGlobals.PlayerProperties.QuickChangeCount);
            BattlePlayer.PlayerProperties.AddProperty(BattlePlayerGlobals.PlayerProperties.QuickChangeCount, count + 1);
        }

        protected override void OnUnequip()
        {
            int count = BattlePlayer.PlayerProperties.GetProperty<int>(BattlePlayerGlobals.PlayerProperties.QuickChangeCount) - 1;
            BattlePlayer.PlayerProperties.RemoveProperty(BattlePlayerGlobals.PlayerProperties.QuickChangeCount);

            if (count > 0)
            {
                BattlePlayer.PlayerProperties.AddProperty(BattlePlayerGlobals.PlayerProperties.QuickChangeCount, count);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Simplifier Badge - Makes Action Commands easier for Mario and his Partner to perform but lowers the final Action Command value
    /// </summary>
    public sealed class SimplifierBadge : Badge
    {
        public SimplifierBadge()
        {
            Name = "Simplifier";
            Description = "Make action commands easy, but earn less Star Power.";

            BPCost = 1;

            BadgeType = BadgeGlobals.BadgeTypes.Simplifier;
            AffectedType = BadgeGlobals.AffectedTypes.Both;
        }

        protected override void OnEquip()
        {
            //Shouldn't affect Players if equipped on Enemies
            if (EntityEquipped.EntityType != Enumerations.EntityTypes.Player) return;

            int count = BattlePlayer.PlayerProperties.GetProperty<int>(BattlePlayerGlobals.PlayerProperties.SimplifierCount);
            BattlePlayer.PlayerProperties.AddProperty(BattlePlayerGlobals.PlayerProperties.SimplifierCount, count + 1);
        }

        protected override void OnUnequip()
        {
            //Shouldn't affect Players if equipped on Enemies
            if (EntityEquipped.EntityType != Enumerations.EntityTypes.Player) return;

            int count = BattlePlayer.PlayerProperties.GetProperty<int>(BattlePlayerGlobals.PlayerProperties.SimplifierCount) - 1;
            BattlePlayer.PlayerProperties.RemoveProperty(BattlePlayerGlobals.PlayerProperties.SimplifierCount);

            if (count > 0)
            {
                BattlePlayer.PlayerProperties.AddProperty(BattlePlayerGlobals.PlayerProperties.SimplifierCount, count);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Unsimplifier Badge - Makes Action Commands harder for Mario and his Partner to perform but raises the final Action Command value
    /// </summary>
    public sealed class UnsimplifierBadge : Badge
    {
        public UnsimplifierBadge()
        {
            Name = "Unsimplifier";
            Description = "Make action commands hard, but earn more Star Power.";

            BPCost = 1;

            BadgeType = BadgeGlobals.BadgeTypes.Unsimplifier;
            AffectedType = BadgeGlobals.AffectedTypes.Both;
        }

        protected override void OnEquip()
        {
            //Shouldn't affect Players if equipped on Enemies
            if (EntityEquipped.EntityType != Enumerations.EntityTypes.Player) return;

            //int count = BattlePlayer.PlayerProperties.GetProperty<int>(BattlePlayerGlobals.PlayerProperties.UnsimplifierCount);
            //BattlePlayer.PlayerProperties.AddProperty(BattlePlayerGlobals.PlayerProperties.UnsimplifierCount, count + 1);
        }

        protected override void OnUnequip()
        {
            //Shouldn't affect Players if equipped on Enemies
            if (EntityEquipped.EntityType != Enumerations.EntityTypes.Player) return;

            //int count = BattlePlayer.PlayerProperties.GetProperty<int>(BattlePlayerGlobals.PlayerProperties.UnsimplifierCount) - 1;
            //BattlePlayer.PlayerProperties.RemoveProperty(BattlePlayerGlobals.PlayerProperties.UnsimplifierCount);
            //
            //if (count > 0)
            //{
            //    BattlePlayer.PlayerProperties.AddProperty(BattlePlayerGlobals.PlayerProperties.UnsimplifierCount, count);
            //}
        }
    }
}

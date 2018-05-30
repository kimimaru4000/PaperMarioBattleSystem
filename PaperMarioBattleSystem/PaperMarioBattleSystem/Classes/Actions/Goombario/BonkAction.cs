using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Goombario's Bonk action
    /// </summary>
    public sealed class BonkAction : JumpAction
    {
        public BonkAction(BattleEntity user) : base(user)
        {
            Name = "Bonk";
            MoveInfo.Description = "Headbonk an enemy.";

            DamageInfo.Damage = 1;

            //If a partner (Goombario or Goombella) is using this move, the base damage is the Partner rank
            PartnerStats partnerStats = User.BattleStats as PartnerStats;
            if (partnerStats != null) DamageInfo.Damage = (int)partnerStats.PartnerRank;
        }
    }
}

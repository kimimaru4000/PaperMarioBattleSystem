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
    public sealed class Bonk : Jump
    {
        public Bonk()
        {
            Name = "Bonk";
            MoveInfo.Description = "Headbonk an enemy.";

            InteractionParamHolder damageInfo = (InteractionParamHolder)DamageInfo;
            damageInfo.Damage = 1;

            //If a partner (Goombario or Goombella) is using this move, the base damage is the Partner rank
            PartnerStats partnerStats = User.BattleStats as PartnerStats;
            if (partnerStats != null) damageInfo.Damage = (int)partnerStats.PartnerRank;

            DamageInfo = damageInfo;
        }
    }
}

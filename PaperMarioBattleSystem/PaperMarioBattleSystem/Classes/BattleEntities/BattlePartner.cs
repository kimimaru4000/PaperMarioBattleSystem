using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's partners in battle
    /// </summary>
    public abstract class BattlePartner : BattleEntity
    {
        public BattlePartner(Stats stats) : base(stats)
        {
            Name = "Partner";

            EntityType = Enumerations.EntityTypes.Player;
        }

        /// <summary>
        /// Since Mario's partners do not have HP, nothing happens when a partner is somehow healed
        /// </summary>
        /// <param name="hp"></param>
        public override void HealHP(int hp)
        {
            
        }

        /// <summary>
        /// Mario's partners do not have HP; instead, they get afflicted with a status condition that makes them unavailable for
        /// a number of turns based directly on the amount of damaged dealt to them
        /// </summary>
        /// <param name="hp">The number of turns the partner is unavailable</param>
        public override void LoseHP(int hp)
        {
            //Inflict status effect
        }
    }
}

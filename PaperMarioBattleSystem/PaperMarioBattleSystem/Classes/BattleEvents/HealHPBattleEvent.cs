using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public class HealHPBattleEvent : BattleEvent
    {
        /// <summary>
        /// How much HP to heal.
        /// </summary>
        public int HPHeal = 0;

        /// <summary>
        /// The BattleEntity to heal HP for.
        /// </summary>
        protected BattleEntity EntityHealed = null;

        public HealHPBattleEvent(BattleEntity entityHealed)
        {
            EntityHealed = entityHealed;
        }

        protected override void OnUpdate()
        {
            //Heal then end immediately
            EntityHealed.HealHP(HPHeal);

            End();
        }
    }
}

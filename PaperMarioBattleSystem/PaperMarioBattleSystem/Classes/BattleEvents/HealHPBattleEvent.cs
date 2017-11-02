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

        public HealHPBattleEvent(BattleEntity entityHealed, int hpHeal)
        {
            EntityHealed = entityHealed;
            HPHeal = hpHeal;

            IsCombineable = true;
        }

        protected override void OnUpdate()
        {
            //Heal then end immediately
            EntityHealed.HealHP(HPHeal);

            End();
        }

        public override void Combine(BattleEvent other)
        {
            HealHPBattleEvent hpEvent = other as HealHPBattleEvent;
            if (hpEvent != null)
            {
                //Add the hp heal amount onto this one
                HPHeal += hpEvent.HPHeal;
            }
        }
    }
}

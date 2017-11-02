using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public class HealFPBattleEvent : BattleEvent
    {
        /// <summary>
        /// How much FP to heal.
        /// </summary>
        public int FPHeal = 0;

        /// <summary>
        /// The BattleEntity to heal FP for.
        /// </summary>
        protected BattleEntity EntityHealed = null;

        public HealFPBattleEvent(BattleEntity entityHealed, int fpHeal)
        {
            EntityHealed = entityHealed;
            FPHeal = fpHeal;

            IsCombineable = true;
        }

        protected override void OnUpdate()
        {
            //Heal then end immediately
            EntityHealed.HealFP(FPHeal);

            End();
        }

        public override void Combine(BattleEvent other)
        {
            HealFPBattleEvent fpEvent = other as HealFPBattleEvent;
            if (fpEvent != null)
            {
                //Add the fp heal amount onto this one
                FPHeal += fpEvent.FPHeal;
            }
        }
    }
}

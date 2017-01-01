using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Battle Event that waits for an animation to finish.
    /// Do not use this for animations that loop infinitely, as it will never end.
    /// </summary>
    public class WaitForAnimBattleEvent : BattleEvent
    {
        protected Animation Anim = null;

        public WaitForAnimBattleEvent(Animation anim) : base(0)
        {
            Anim = anim;
        }

        protected override void OnUpdate()
        {
            //If the animation doesn't exist, end immediately
            if (Anim == null)
            {
                End();
                return;
            }

            if (Anim.Finished == true)
            {
                End();
            }
        }
    }
}

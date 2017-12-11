using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A SequenceAction that waits for an animation to finish before proceeding.
    /// Do not use this for animations that loop infinitely, as this will never end
    /// </summary>
    public class WaitForAnimationSeqAction : SequenceAction
    {
        protected Animation Anim = null;

        public WaitForAnimationSeqAction(string animName)
        {
            Anim = Entity.AnimManager.GetAnimation(animName);
        }

        public WaitForAnimationSeqAction(Animation anim)
        {
            Anim = anim;
        }

        protected override void OnUpdate()
        {
            //If the animation doesn't exist end immediately
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

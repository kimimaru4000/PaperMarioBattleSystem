using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for the Stone Cap.
    /// </summary>
    public sealed class StoneCapSequence : ItemSequence
    {
        //private double PostWaitDur = 300d;

        public StoneCapSequence(ItemAction itemAction) : base(itemAction)
        {
            
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    EntitiesAffected[0].AnimManager.PlayAnimation(AnimationGlobals.MarioBattleAnimations.StoneCapPutOnName);
                    CurSequenceAction = new WaitForAnimationSeqAction(EntitiesAffected[0].AnimManager.GetAnimation(AnimationGlobals.MarioBattleAnimations.StoneCapPutOnName));
                    break;
                default:
                    base.SequenceMainBranch();
                    break;
            }
        }

        protected override void SequenceEndBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    base.SequenceEndBranch();
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());
                    break;
                default:
                    base.SequenceEndBranch();
                    break;
            }
        }
    }
}

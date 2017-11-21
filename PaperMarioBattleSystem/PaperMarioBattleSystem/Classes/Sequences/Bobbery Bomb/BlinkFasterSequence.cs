using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for the Bobbery Bombs' Blink Faster action.
    /// </summary>
    public sealed class BlinkFasterSequence : Sequence
    {
        /// <summary>
        /// The name of the animation to speed up.
        /// </summary>
        private string AnimName = string.Empty;
        
        /// <summary>
        /// The new speed value of the animation.
        /// </summary>
        private float SpeedValue = 1f;

        /// <summary>
        /// The Animation that is obtained from the animation name.
        /// </summary>
        private Animation Anim = null;

        public BlinkFasterSequence(MoveAction moveAction, string animName, float speedVal) : base(moveAction)
        {
            AnimName = animName;
            SpeedValue = speedVal;

            Anim = User.AnimManager.GetAnimation(AnimName);
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Speed up the animation to the desired value, then end the sequence
                    Anim?.SetSpeed(SpeedValue);

                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceEndBranch()
        {
            PrintInvalidSequence();
        }

        protected override void SequenceMainBranch()
        {
            PrintInvalidSequence();
        }

        protected override void SequenceSuccessBranch()
        {
            PrintInvalidSequence();
        }

        protected override void SequenceFailedBranch()
        {
            PrintInvalidSequence();
        }

        protected override void SequenceMissBranch()
        {
            PrintInvalidSequence();
        }
    }
}

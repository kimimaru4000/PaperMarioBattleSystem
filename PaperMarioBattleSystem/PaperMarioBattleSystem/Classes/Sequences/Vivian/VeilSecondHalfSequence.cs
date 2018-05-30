using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The second half of Vivian's Veil Sequence.
    /// <para>This occurs at the start of Vivian's next turn through an event in Veil.
    /// It should not be assigned to an action added to a menu.</para>
    /// </summary>
    public sealed class VeilSecondHalfSequence : Sequence
    {
        private const double MoveTime = 500d;

        private float ScaleVal = float.Epsilon;

        public VeilSecondHalfSequence(MoveAction moveAction, float scaleVal) : base(moveAction)
        {
            ScaleVal = scaleVal;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Remove evasion and other properties
                    BattleEntity allyAffected = EntitiesAffected[0];

                    User.RemoveEvasionMod(0d);
                    allyAffected.RemoveEvasionMod(0d);

                    //Remove Effects suppression from the Poison, Burn, and Frozen Status Effects
                    User.EntityProperties.UnsuppressStatuses(Enumerations.StatusSuppressionTypes.Effects, Enumerations.StatusTypes.Poison, Enumerations.StatusTypes.Burn, Enumerations.StatusTypes.Frozen);
                    allyAffected.EntityProperties.UnsuppressStatuses(Enumerations.StatusSuppressionTypes.Effects, Enumerations.StatusTypes.Poison, Enumerations.StatusTypes.Burn, Enumerations.StatusTypes.Frozen);

                    //Make them visible
                    User.Scale /= ScaleVal;
                    allyAffected.Scale /= ScaleVal;

                    CurSequenceAction = new WaitSeqAction(0d);

                    ChangeSequenceBranch(SequenceBranch.Main);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Move back up
                    //NOTE: Move the ally as well - this needs to be added in
                    CurSequenceAction = new MoveToSeqAction(User, User.Position - new Vector2(0f, 25f), MoveTime);

                    break;
                case 1:
                    //Make the user move back to its battle position
                    CurSequenceAction = new MoveToSeqAction(User, User.BattlePosition, MoveTime);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceEndBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //End the sequence
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMissBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}

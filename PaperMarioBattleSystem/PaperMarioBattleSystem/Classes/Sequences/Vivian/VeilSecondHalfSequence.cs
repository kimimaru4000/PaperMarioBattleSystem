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

        public VeilSecondHalfSequence(MoveAction moveAction) : base(moveAction)
        {

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

                    //Remove Invincibility
                    User.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.Invincible);
                    allyAffected.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.Invincible);

                    //Make them visible
                    User.TintColor = Color.White;
                    allyAffected.TintColor = Color.White;

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
                    CurSequenceAction = new MoveToSeqAction(User.Position - new Vector2(0f, 25f), MoveTime);

                    break;
                case 1:
                    //Make the user move back to its battle position
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, MoveTime);

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

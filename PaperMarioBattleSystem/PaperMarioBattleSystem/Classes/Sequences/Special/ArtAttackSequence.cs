using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for the Art Attack move.
    /// </summary>
    public sealed class ArtAttackSequence : CrystalStarMoveSequence
    {
        public ArtAttackSequence(SpecialMoveAction specialAction) : base(specialAction)
        {

        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //1. Start Art Attack's Action Command
                    //2. Start drawing
                    //3. For each circle completed, a response will be sent down with the elapsed time and current star position,
                    //then go to the Success branch
                    //4. Go back to this branch and restart the command at the current star position with the elapsed time as the total time

                    StartActionCommandInput();
                    CurSequenceAction = new WaitForCommandSeqAction(0d, actionCommand, CommandEnabled);
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
                case 0:
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}

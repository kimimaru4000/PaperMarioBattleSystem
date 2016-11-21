using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Charge
    /// </summary>
    public sealed class ChargeSequence : Sequence
    {
        /// <summary>
        /// The amount to charge.
        /// </summary>
        private int ChargeAmount = 0;

        public ChargeSequence(MoveAction moveAction, int chargeAmount) : base(moveAction)
        {
            ChargeAmount = chargeAmount;
        }

        protected override void SequenceStartBranch()
        {
            //Have the entity move back and forth quickly for a bit
            switch (SequenceStep)
            {
                case 0:
                    CurSequenceAction = new MoveTo(User.BattlePosition - new Vector2(10, 0), 100d);
                    break;
                case 1:
                    CurSequenceAction = new MoveTo(User.BattlePosition, 100d);
                    break;
                case 2:
                    CurSequenceAction = new MoveTo(User.BattlePosition + new Vector2(10, 0), 100d);
                    break;
                case 3:
                    goto case 1;
                case 4:
                    goto case 0;
                case 5:
                    goto case 1;
                case 6:
                    goto case 2;
                case 7:
                    ChangeSequenceBranch(SequenceBranch.Main);
                    goto case 1;
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
                    User.EntityProperties.AfflictStatus(new ChargedStatus(ChargeAmount));
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceSuccessBranch()
        {
            PrintInvalidSequence();
        }

        protected override void SequenceFailedBranch()
        {
            PrintInvalidSequence();
        }

        protected override void SequenceEndBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMissBranch()
        {
            PrintInvalidSequence();
        }
    }
}

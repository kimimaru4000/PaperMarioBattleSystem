using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    public sealed class ChangePartnerSequence : Sequence
    {
        /// <summary>
        /// The new Partner to switch to.
        /// </summary>
        private BattlePartner NewPartner = null;

        public ChangePartnerSequence(MoveAction moveAction, BattlePartner newPartner) : base(moveAction)
        {
            NewPartner = newPartner;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    CurSequenceAction = new MoveTo(BattleManager.Instance.GetPartner(), BattleManager.Instance.GetMario().BattlePosition, 300d);
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
                    CurSequenceAction = new Wait(300d);
                    break;
                case 1:
                    BattleManager.Instance.SwapPartner(NewPartner);
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
                    CurSequenceAction = new MoveTo(BattleManager.Instance.GetPartner(), BattleManager.Instance.GetPartner().BattlePosition, 300d);
                    break;
                case 1:
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

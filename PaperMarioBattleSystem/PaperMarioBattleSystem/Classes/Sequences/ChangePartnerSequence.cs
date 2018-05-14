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
                    SwapPartnerBattleEvent swapPartnerBattleEvent = new SwapPartnerBattleEvent(BattleManager.Instance.Partner, NewPartner, 300d, 300d);

                    BattleManager.Instance.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.SwapPartner,
                        new BattleManager.BattleState[] { BattleManager.BattleState.Turn },
                        swapPartnerBattleEvent);

                    CurSequenceAction = new WaitForBattleEventSeqAction(swapPartnerBattleEvent);
                    ChangeSequenceBranch(SequenceBranch.End);
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

        //This Sequence can't get interrupted. If it does, print an error
        protected override void OnInterruption(Enumerations.Elements element)
        {
            base.OnInterruption(element);

            Debug.LogError($"Sequence {nameof(ChangePartnerSequence)} CANNOT be interrupted. This is a critical error that should be fixed ASAP");
        }
    }
}

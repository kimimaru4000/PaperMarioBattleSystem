using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for the Defend action.
    /// </summary>
    public sealed class DefendSequence : Sequence
    {
        private const double MessageDur = 2000d;

        /// <summary>
        /// The amount to raise the BattleEntity's Defense by until the start of its next turn.
        /// </summary>
        private int DefenseBoost = 0;

        public DefendSequence(MoveAction moveAction, int defenseBoost) : base(moveAction)
        {
            DefenseBoost = defenseBoost;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Make the entity assume the Guard position and wait for the animation for now (spinning will occur later)
                    User.AnimManager.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.GuardName);
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
                    //Wait a few seconds for now (spinning will occur once rotation is in)
                    //Increase defense by the amount
                    CurSequenceAction = new WaitForAnimationSeqAction(User, AnimationGlobals.PlayerBattleAnimations.GuardName);
                    User.RaiseDefense(DefenseBoost);

                    //Tell the BattleEntity to decrease its Defense at the start of the next turn
                    User.TurnStartEvent -= DecreaseDefenseBoost;
                    User.TurnStartEvent += DecreaseDefenseBoost;

                    //Show the Battle Event
                    User.BManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Message,
                        new BattleGlobals.BattleState[] { BattleGlobals.BattleState.Turn, BattleGlobals.BattleState.TurnEnd },
                        new MessageBattleEvent("Defense will be boosted\nthis turn!", MessageDur));

                    Debug.Log($"Raised Defense for {User.Name} after using the Defend action!");

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
                    //Simply end the sequence
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

        private void DecreaseDefenseBoost()
        {
            Debug.Log($"Lowered Defense for {User.Name} at the start of its turn after the Defend action was used!");

            User.TurnStartEvent -= DecreaseDefenseBoost;

            User.LowerDefense(DefenseBoost);
        }
    }
}

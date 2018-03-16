using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The sequence for the Run Away action.
    /// </summary>
    public sealed class RunAwaySequence : Sequence
    {
        private const double RunDur = 1000d;
        private const double NoCommandRunDur = 2000d;
        private const double FailWaitDur = 1500d;

        private bool Succeeded = false;

        public RunAwaySequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void OnEnd()
        {
            base.OnEnd();

            Succeeded = false;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Turn around and start the running animation
                    for (int i = 0; i < EntitiesAffected.Length; i++)
                    {
                        EntitiesAffected[i].SpriteFlip = !EntitiesAffected[i].SpriteFlip;
                        EntitiesAffected[i].AnimManager.PlayAnimation(AnimationGlobals.RunningName, true);
                    }

                    CurSequenceAction = new WaitSeqAction(0d);
                    ChangeSequenceBranch(SequenceBranch.Main);
                    break;
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
                    //End the battle if we succeeded in running
                    if (Succeeded == true)
                    {
                        BattleManager.Instance.EndBattle();
                    }

                    EndSequence();
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
                    //Start the action command and wait for it
                    StartActionCommandInput();

                    CurSequenceAction = new WaitForCommandSeqAction(NoCommandRunDur, actionCommand, CommandEnabled);
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
                    //On a success, run off the screen and end the battle
                    Succeeded = true;

                    Vector2[] runAmounts = new Vector2[EntitiesAffected.Length];
                    for (int i = 0; i < runAmounts.Length; i++)
                    {
                        runAmounts[i] = new Vector2(-1000, 0);
                    }

                    CurSequenceAction = new MoveAmountMultipleSeqAction(EntitiesAffected, runAmounts, RunDur);
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
                    Succeeded = false;

                    //On a fail, play the trip animation and end
                    for (int i = 0; i < EntitiesAffected.Length; i++)
                    {
                        EntitiesAffected[i].AnimManager.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.RunAwayFailName, true);
                    }

                    CurSequenceAction = new WaitSeqAction(FailWaitDur);
                    break;
                case 1:
                    for (int i = 0; i < EntitiesAffected.Length; i++)
                    {
                        EntitiesAffected[i].AnimManager.PlayAnimation(EntitiesAffected[i].GetIdleAnim());
                    }

                    CurSequenceAction = new WaitSeqAction(FailWaitDur);
                    break;
                case 2:
                    for (int i = 0; i < EntitiesAffected.Length; i++)
                    {
                        EntitiesAffected[i].SpriteFlip = !EntitiesAffected[i].SpriteFlip;
                    }

                    CurSequenceAction = new WaitSeqAction(0d);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //Running away can't miss
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

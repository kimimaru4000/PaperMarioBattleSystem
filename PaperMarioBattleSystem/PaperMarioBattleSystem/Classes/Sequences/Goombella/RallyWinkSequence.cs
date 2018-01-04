using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Goombella's Rally Wink.
    /// </summary>
    public sealed class RallyWinkSequence : Sequence
    {
        public const string SuccessMessage = "Mario's attack turns\nincreased by one!";

        private double WaitDur = 500d;

        public RallyWinkSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    string animName = AnimationGlobals.RunningName;

                    //Make the run animation slower
                    Animation runAnim = User.AnimManager.GetAnimation(animName);
                    runAnim?.SetSpeed(.5f);
                    
                    User.AnimManager.PlayAnimation(animName, true);
                    CurSequenceAction = new WaitSeqAction(WaitDur);

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
                    User.AnimManager.PlayAnimation(User.GetIdleAnim(), true);

                    CurSequenceAction = new WaitSeqAction(0d);
                    break;
                case 1:
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
                    StartActionCommandInput();

                    CurSequenceAction = new WaitForCommandSeqAction(500d, actionCommand, CommandEnabled);
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
                    //Play the wink animation, add the heart (and particles at a later time), and play the sound
                    User.AnimManager.PlayAnimation(AnimationGlobals.GoombellaBattleAnimations.WinkName, true);

                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.GoombellaBattleAnimations.WinkName);
                    break;
                case 1:
                    //Wait a little bit
                    CurSequenceAction = new WaitSeqAction(WaitDur);
                    break;
                case 2:
                    //Subtract the used turn count by one to give another turn
                    //NOTE: Confirm this will work by trying it in TTYD when Mario is immobile in some way
                    EntitiesAffected[0].SetTurnsUsed(EntitiesAffected[0].TurnsUsed - 1);

                    //Show the message
                    BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Message,
                        new BattleManager.BattleState[] { BattleManager.BattleState.Turn },
                        new MessageBattleEvent(SuccessMessage, 2000d));

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
                    //Play the wink animation, add the heart (and particles at a later time), and play the sound
                    User.AnimManager.PlayAnimation(AnimationGlobals.GoombellaBattleAnimations.WinkName, true);

                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.GoombellaBattleAnimations.WinkName);
                    break;
                case 1:
                    //Wait a little bit
                    CurSequenceAction = new WaitSeqAction(WaitDur);
                    break;
                case 2:
                    //Deal 0 damage to show the move wasn't effective
                    AttemptDamage(0, EntitiesAffected[0], Action.DamageProperties, true);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //Rally Wink can't miss
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

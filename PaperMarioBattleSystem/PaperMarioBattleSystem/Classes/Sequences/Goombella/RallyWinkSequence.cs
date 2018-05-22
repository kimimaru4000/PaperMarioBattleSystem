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

        /// <summary>
        /// The heart VFX after successfully performing Rally Wink.
        /// </summary>
        private RallyWinkHeartVFX HeartVFX = null;

        private RallyWinkActionCommandUI RallyWinkUI = null;

        public RallyWinkSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void OnStart()
        {
            base.OnStart();

            HeartVFX = new RallyWinkHeartVFX(EntitiesAffected[0].Position, WaitDur, WaitDur, .6f);

            if (Action.CommandEnabled == true && Action.DrawActionCommandInfo == true)
            {
                RallyWinkUI = new RallyWinkActionCommandUI(actionCommand as RallyWinkCommand);
                BattleUIManager.Instance.AddUIElement(RallyWinkUI);
            }
        }

        protected override void OnEnd()
        {
            if (HeartVFX != null)
            {
                //Ensure the heart will be removed
                if (HeartVFX.ReadyForRemoval == false && HeartVFX.HeartState != RallyWinkHeartVFX.HeartStates.FadeOut)
                {
                    HeartVFX.FadeOut();
                }
            }

            HeartVFX = null;

            if (RallyWinkUI != null)
            {
                BattleUIManager.Instance.RemoveUIElement(RallyWinkUI);
                RallyWinkUI = null;
            }
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
                    //Play the wink animation, add the heart (and particles at a later time), play the sound, and show the Command Rank result
                    User.AnimManager.PlayAnimation(AnimationGlobals.GoombellaBattleAnimations.WinkName, true);

                    if (HighestCommandRank != ActionCommand.CommandRank.None)
                    {
                        ShowCommandRankVFX(HighestCommandRank, EntitiesAffected[0].Position);
                    }

                    BattleObjManager.Instance.AddBattleObject(HeartVFX);

                    if (EntitiesAffected[0].IsImmobile() == false)
                    {
                        EntitiesAffected[0].AnimManager.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.StarSpecialName);
                    }

                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.GoombellaBattleAnimations.WinkName);
                    break;
                case 1:
                    //Wait a little bit
                    CurSequenceAction = new WaitSeqAction(WaitDur);
                    break;
                case 2:
                    //Subtract the used turn count by one to give another turn
                    //This shouldn't work if the entity is immobile in some way (Ex. Stop, Frozen, etc.), so check for it first
                    if (EntitiesAffected[0].MaxTurns > 0)
                    {
                        EntitiesAffected[0].SetTurnsUsed(EntitiesAffected[0].TurnsUsed - 1);
                    }

                    MessageBattleEvent msgEvent = new MessageBattleEvent(SuccessMessage, 2000d);

                    //Show the message
                    BattleManager.Instance.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Message,
                        new BattleManager.BattleState[] { BattleManager.BattleState.Turn }, msgEvent);

                    CurSequenceAction = new WaitForBattleEventSeqAction(msgEvent);
                    break;
                case 3:
                    if (EntitiesAffected[0].IsImmobile() == false)
                    {
                        EntitiesAffected[0].AnimManager.PlayAnimation(EntitiesAffected[0].GetIdleAnim());
                    }
                    HeartVFX.FadeOut();
                    CurSequenceAction = new WaitSeqAction(0d);

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

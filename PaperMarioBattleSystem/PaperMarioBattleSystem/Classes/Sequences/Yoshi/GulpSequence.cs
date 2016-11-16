﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Yoshi's Gulp.
    /// </summary>
    public sealed class GulpSequence : Sequence
    {
        public double WalkDuration = 4000f;

        public GulpSequence()
        {
            //Name = "Gulp";
            //
            //ContactType = Enumerations.ContactTypes.None;
            //
            //FPCost = 4;
            //BaseDamage = 4;
            //Piercing = true;
            //
            //actionCommand = new GulpCommand(this, WalkDuration / 2f, 500f, 1f);
            //SelectionType = TargetSelectionMenu.EntitySelectionType.First;
            //HeightsAffected = new Enumerations.HeightStates[] { Enumerations.HeightStates.Grounded };
        }

        protected override void CommandSuccess()
        {
            ChangeSequenceBranch(SequenceBranch.Success);
        }

        protected override void CommandFailed()
        {
            ChangeSequenceBranch(SequenceBranch.Failed);
        }

        public override void OnCommandResponse(object response)
        {

        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveTo(BattleManager.Instance.GetPositionInFront(BattleManager.Instance.GetFrontPlayer()), WalkDuration / 4f);
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
                    StartActionCommandInput();
                    CurSequenceAction = new MoveTo(BattleManager.Instance.GetPositionInFront(EntitiesAffected[0]), WalkDuration);
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
                    User.PlayAnimation(AnimationGlobals.YoshiBattleAnimations.GulpEat, true);

                    //The entity spit out
                    BattleEntity eatenEntity = EntitiesAffected[0];

                    //Get the entity behind the one spit out
                    //If it can be hit by this move, make that entity take damage as well
                    BattleEntity[] behindEntities = BattleManager.Instance.GetEntitiesBehind(eatenEntity);
                    behindEntities = BattleManager.Instance.FilterEntitiesByHeights(behindEntities, Action.MoveProperties.HeightsAffected);

                    AttemptDamage(BaseDamage, eatenEntity, false);
                    if (behindEntities.Length > 0)
                    {
                        AttemptDamage(BaseDamage, behindEntities[0], false);
                    }

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
                    User.PlayAnimation(AnimationGlobals.YoshiBattleAnimations.GulpEat, true);
                    ChangeSequenceBranch(SequenceBranch.End);
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
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveTo(User.BattlePosition, WalkDuration / 4f);
                    break;
                case 1:
                    User.PlayAnimation(AnimationGlobals.IdleName);
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
